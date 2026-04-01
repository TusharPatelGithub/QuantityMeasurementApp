using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessLayer.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTOs;
using ModelLayer.Models;
using RepositoryLayer.Interfaces;

namespace BusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public AuthResponseDTO Register(RegisterDTO registerDto)
        {
            var existingUser = _userRepository.GetUserByEmail(registerDto.Email);
            if (existingUser != null)
            {
                throw new ApplicationException("User with this email already exists.");
            }

            var user = new AppUser
            {
                FullName     = registerDto.FullName,
                Email        = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                MobileNumber = registerDto.MobileNumber
            };

            var userId = _userRepository.CreateUser(user);
            if (userId <= 0)
            {
                throw new ApplicationException("Failed to register user.");
            }

            return new AuthResponseDTO
            {
                Email = user.Email,
                Message = "Registration successful. Please login."
            };
        }

        public AuthResponseDTO Login(LoginDTO loginDto)
        {
            var user = _userRepository.GetUserByEmail(loginDto.Email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new ApplicationException("Invalid email or password.");
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isValidPassword)
            {
                throw new ApplicationException("Invalid email or password.");
            }

            var token = GenerateJwtToken(user);
            return new AuthResponseDTO
            {
                Email = user.Email,
                Token = token,
                Message = "Login successful."
            };
        }

        public async Task<AuthResponseDTO> GoogleLoginAsync(GoogleLoginDTO googleLoginDto)
        {
            var clientId = _config["Google:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ApplicationException("Google Client ID is not configured.");
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new ApplicationException("Invalid Google ID token.");
            }

            var user = _userRepository.GetUserByGoogleId(payload.Subject) ?? _userRepository.GetUserByEmail(payload.Email);

            if (user == null)
            {
                // Register new user via Google
                user = new AppUser
                {
                    Email = payload.Email,
                    PasswordHash = string.Empty,
                    GoogleId = payload.Subject
                };
                user.Id = _userRepository.CreateUser(user);
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                // Link Google ID to existing user? For this UC, we can just login.
                // In a real app we'd trigger an update to save the GoogleId.
            }

            var token = GenerateJwtToken(user);
            return new AuthResponseDTO
            {
                Email = user.Email,
                Token = token,
                Message = "Google Login successful."
            };
        }

        private string GenerateJwtToken(AppUser user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = jwtSettings["Key"] ?? throw new ApplicationException("JWT Key not configured.");
            var issuer = jwtSettings["Issuer"] ?? throw new ApplicationException("JWT Issuer not configured.");
            var audience = jwtSettings["Audience"] ?? throw new ApplicationException("JWT Audience not configured.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
