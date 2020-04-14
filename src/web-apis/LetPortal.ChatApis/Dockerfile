#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["LetPortal.ChatApis/LetPortal.ChatApis.csproj", "LetPortal.ChatApis/"]
RUN dotnet restore "LetPortal.ChatApis/LetPortal.ChatApis.csproj"
COPY . .
WORKDIR "/src/LetPortal.ChatApis"
RUN dotnet build "LetPortal.ChatApis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetPortal.ChatApis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LetPortal.ChatApis.dll"]