# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.sln .
COPY QuantityMeasurementApp/*.csproj ./QuantityMeasurementApp/
COPY BusinessLayer/*.csproj ./BusinessLayer/
COPY ModelLayer/*.csproj ./ModelLayer/
COPY RepositoryLayer/*.csproj ./RepositoryLayer/
RUN dotnet restore

COPY . .
RUN dotnet publish QuantityMeasurementApp -c Release -o out

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "QuantityMeasurementApp.dll"]
