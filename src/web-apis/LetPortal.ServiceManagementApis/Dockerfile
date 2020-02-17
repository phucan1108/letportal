FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
RUN apk add --no-cache tzdata

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["LetPortal.ServiceManagementApis/LetPortal.ServiceManagementApis.csproj", "LetPortal.ServiceManagementApis/"]
COPY ["LetPortal.Core/LetPortal.Core.csproj", "LetPortal.Core/"]
COPY ["LetPortal.ServiceManagement/LetPortal.ServiceManagement.csproj", "LetPortal.ServiceManagement/"]
RUN dotnet restore "LetPortal.ServiceManagementApis/LetPortal.ServiceManagementApis.csproj"
COPY . .
WORKDIR "/src/LetPortal.ServiceManagementApis"
RUN dotnet build "LetPortal.ServiceManagementApis.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LetPortal.ServiceManagementApis.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LetPortal.ServiceManagementApis.dll"]