FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
RUN apk add --no-cache tzdata

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["LetPortal.ChatApis/LetPortal.ChatApis.csproj", "LetPortal.ChatApis/"]
COPY ["LetPortal.Core/LetPortal.Core.csproj", "LetPortal.Core/"]
COPY ["LetPortal.Chat/LetPortal.Chat.csproj", "LetPortal.Chat/"]
RUN dotnet restore "LetPortal.ChatApis/LetPortal.ChatApis.csproj"
COPY . .
WORKDIR "/src/LetPortal.ChatApis"
RUN dotnet build "LetPortal.ChatApis.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LetPortal.ChatApis.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LetPortal.ChatApis.dll"]