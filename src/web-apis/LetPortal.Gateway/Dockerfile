FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.10 AS base
WORKDIR /app
EXPOSE 80
RUN apk add --no-cache tzdata

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.10 AS build
WORKDIR /src
COPY ["LetPortal.Gateway/LetPortal.Gateway.csproj", "LetPortal.Gateway/"]
COPY ["LetPortal.Core/LetPortal.Core.csproj", "LetPortal.Core/"]
RUN dotnet restore "LetPortal.Gateway/LetPortal.Gateway.csproj"
COPY . .
WORKDIR "/src/LetPortal.Gateway"
RUN dotnet build "LetPortal.Gateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LetPortal.Gateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LetPortal.Gateway.dll"]