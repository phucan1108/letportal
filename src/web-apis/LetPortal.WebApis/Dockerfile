FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
RUN apk add --no-cache tzdata

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["LetPortal.WebApis/LetPortal.PortalApis.csproj", "LetPortal.WebApis/"]
COPY ["LetPortal.Core/LetPortal.Core.csproj", "LetPortal.Core/"]
COPY ["LetPortal.Portal/LetPortal.Portal.csproj", "LetPortal.Portal/"]
RUN dotnet restore "LetPortal.WebApis/LetPortal.PortalApis.csproj"
COPY . .
WORKDIR "/src/LetPortal.WebApis"
RUN dotnet build "LetPortal.PortalApis.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LetPortal.PortalApis.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LetPortal.PortalApis.dll"]