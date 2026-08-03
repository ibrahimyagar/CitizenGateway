FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/MockServices/MockServices.Shared/MockServices.Shared.csproj src/MockServices/MockServices.Shared/
COPY src/MockServices/SporTesisi.Api/SporTesisi.Api.csproj src/MockServices/SporTesisi.Api/
RUN dotnet restore src/MockServices/SporTesisi.Api/SporTesisi.Api.csproj

COPY src/MockServices/MockServices.Shared/ src/MockServices/MockServices.Shared/
COPY src/MockServices/SporTesisi.Api/ src/MockServices/SporTesisi.Api/
RUN dotnet publish src/MockServices/SporTesisi.Api/SporTesisi.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SporTesisi.Api.dll"]
