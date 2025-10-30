FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj .

# Restore dependencies for the project
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .

EXPOSE 5253

ENTRYPOINT ["dotnet", "backend.dll"]
