#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1809 AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1809 AS build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FileUploadService/FileUploadService.csproj", "FileUploadService/"]
RUN dotnet restore "FileUploadService/FileUploadService.csproj"
COPY . .
WORKDIR "/src/FileUploadService"
RUN dotnet build "FileUploadService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileUploadService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#RUN sed -i "s|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g" /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "FileUploadService.dll"]