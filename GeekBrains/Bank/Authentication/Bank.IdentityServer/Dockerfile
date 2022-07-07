#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Authentication/Bank.IdentityServer/Bank.IdentityServer.csproj", "Authentication/Bank.IdentityServer/"]
COPY ["Authentication/Bank.AuthenticationRules/Bank.AuthenticationRules.csproj", "Authentication/Bank.AuthenticationRules/"]
COPY ["Authentication/Bank.Authentication.Data/Bank.Authentication.Data.csproj", "Authentication/Bank.Authentication.Data/"]
RUN dotnet restore "Authentication/Bank.IdentityServer/Bank.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Authentication/Bank.IdentityServer"
RUN dotnet build "Bank.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bank.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bank.IdentityServer.dll"]