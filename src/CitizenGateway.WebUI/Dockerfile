FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/CitizenGateway.WebUI/CitizenGateway.WebUI.csproj src/CitizenGateway.WebUI/
RUN dotnet restore src/CitizenGateway.WebUI/CitizenGateway.WebUI.csproj

COPY src/CitizenGateway.WebUI/ src/CitizenGateway.WebUI/
RUN dotnet publish src/CitizenGateway.WebUI/CitizenGateway.WebUI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CitizenGateway.WebUI.dll"]
