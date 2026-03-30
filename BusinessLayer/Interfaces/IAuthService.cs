using ModelLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAuthService
    {
        AuthResponseDTO Register(RegisterDTO registerDto);
        AuthResponseDTO Login(LoginDTO loginDto);
        Task<AuthResponseDTO> GoogleLoginAsync(GoogleLoginDTO googleLoginDto);
    }
}
