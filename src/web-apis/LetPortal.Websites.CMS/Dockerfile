#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LetPortal.Websites.CMS/LetPortal.Websites.CMS.csproj", "LetPortal.Websites.CMS/"]
RUN dotnet restore "LetPortal.Websites.CMS/LetPortal.Websites.CMS.csproj"
COPY . .
WORKDIR "/src/LetPortal.Websites.CMS"
RUN dotnet build "LetPortal.Websites.CMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetPortal.Websites.CMS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LetPortal.Websites.CMS.dll"]