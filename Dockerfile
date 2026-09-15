# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BibliotecaApi/BibliotecaApi.csproj BibliotecaApi/
RUN dotnet restore BibliotecaApi/BibliotecaApi.csproj

COPY BibliotecaApi/ BibliotecaApi/
RUN dotnet publish BibliotecaApi/BibliotecaApi.csproj -c Release -o /app/publish --no-restore

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BibliotecaApi.dll"]
