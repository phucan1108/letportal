#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LetPortal.Saturn/LetPortal.Saturn.csproj", "LetPortal.Saturn/"]
RUN dotnet restore "LetPortal.Saturn/LetPortal.Saturn.csproj"
COPY . .
WORKDIR "/src/LetPortal.Saturn"
RUN dotnet build "LetPortal.Saturn.csproj" -c Release -o /app/build

RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump

FROM build AS publish
RUN dotnet publish "LetPortal.Saturn.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update && apt-get install -y curl
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /tools /tools
ENTRYPOINT ["dotnet", "LetPortal.Saturn.dll"]