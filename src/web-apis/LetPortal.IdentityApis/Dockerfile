FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
RUN apk add --no-cache tzdata

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["LetPortal.IdentityApis/LetPortal.IdentityApis.csproj", "LetPortal.IdentityApis/"]
COPY ["LetPortal.Core/LetPortal.Core.csproj", "LetPortal.Core/"]
COPY ["LetPortal.Identity/LetPortal.Identity.csproj", "LetPortal.Identity/"]
RUN dotnet restore "LetPortal.IdentityApis/LetPortal.IdentityApis.csproj"
COPY . .
WORKDIR "/src/LetPortal.IdentityApis"
RUN dotnet build "LetPortal.IdentityApis.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LetPortal.IdentityApis.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LetPortal.IdentityApis.dll"]