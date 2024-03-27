FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Docker.Cryptography/Docker.Cryptography.csproj"

WORKDIR /src/Docker.Cryptography
ARG MODE=Release
RUN dotnet build "Docker.Cryptography.csproj" -c ${MODE} -o /app/build

FROM build AS publish
ARG MODE=Release
RUN dotnet publish "Docker.Cryptography.csproj" -c ${MODE} -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apk add -U --no-cache tzdata
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker.Cryptography.dll"]