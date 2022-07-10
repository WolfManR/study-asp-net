#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Catalog/Catalog.csproj", "Catalog/"]
COPY ["Catalog.FullTextSearch/Catalog.FullTextSearch.csproj", "Catalog.FullTextSearch/"]
COPY ["Catalog.Books/Catalog.Books.csproj", "Catalog.Books/"]
COPY ["Catalog.Books.Data/Catalog.Books.Data.csproj", "Catalog.Books.Data/"]
COPY ["Neo4j.Core/Neo4j.Core.csproj", "Neo4j.Core/"]
RUN dotnet restore "Catalog/Catalog.csproj"
COPY . .
WORKDIR "/src/Catalog"
RUN dotnet build "Catalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.dll"]