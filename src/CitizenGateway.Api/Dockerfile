# Gateway API — solution kökünden build edilir (project reference'lar için).
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CitizenGateway.sln ./
COPY src/CitizenGateway.Domain/CitizenGateway.Domain.csproj src/CitizenGateway.Domain/
COPY src/CitizenGateway.Application/CitizenGateway.Application.csproj src/CitizenGateway.Application/
COPY src/CitizenGateway.Infrastructure/CitizenGateway.Infrastructure.csproj src/CitizenGateway.Infrastructure/
COPY src/CitizenGateway.Api/CitizenGateway.Api.csproj src/CitizenGateway.Api/
COPY src/MockServices/MockServices.Shared/MockServices.Shared.csproj src/MockServices/MockServices.Shared/

RUN dotnet restore src/CitizenGateway.Api/CitizenGateway.Api.csproj

COPY src/CitizenGateway.Domain/ src/CitizenGateway.Domain/
COPY src/CitizenGateway.Application/ src/CitizenGateway.Application/
COPY src/CitizenGateway.Infrastructure/ src/CitizenGateway.Infrastructure/
COPY src/CitizenGateway.Api/ src/CitizenGateway.Api/
COPY src/MockServices/MockServices.Shared/ src/MockServices/MockServices.Shared/

RUN dotnet publish src/CitizenGateway.Api/CitizenGateway.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CitizenGateway.Api.dll"]
