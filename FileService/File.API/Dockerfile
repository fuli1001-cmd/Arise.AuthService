#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FileService/File.API/File.API.csproj", "FileService/File.API/"]
COPY ["FileService/File.Infrastructure/File.Infrastructure.csproj", "FileService/File.Infrastructure/"]
COPY ["FileService/File.Domain/File.Domain.csproj", "FileService/File.Domain/"]
RUN dotnet restore "FileService/File.API/File.API.csproj"
COPY . .
WORKDIR "/src/FileService/File.API"
RUN dotnet build "File.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "File.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "File.API.dll"]