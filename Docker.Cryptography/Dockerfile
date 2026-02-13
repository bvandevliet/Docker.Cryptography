FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
EXPOSE 80

RUN apk add -U --no-cache tzdata

WORKDIR /app
COPY bin/Release/net10.0/publish .

RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Docker.Cryptography.dll"]