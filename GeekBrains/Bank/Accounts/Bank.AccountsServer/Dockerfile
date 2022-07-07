#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Accounts/Bank.AccountsServer/Bank.AccountsServer.csproj", "Accounts/Bank.AccountsServer/"]
COPY ["Accounts/Bank.Accounts.Data.Dapper/Bank.Accounts.Data.Dapper.csproj", "Accounts/Bank.Accounts.Data.Dapper/"]
COPY ["Accounts/Bank.Accounts.Data/Bank.Accounts.Data.csproj", "Accounts/Bank.Accounts.Data/"]
COPY ["Accounts/Bank.Accounts.Data.EF/Bank.Accounts.Data.EF.csproj", "Accounts/Bank.Accounts.Data.EF/"]
RUN dotnet restore "Accounts/Bank.AccountsServer/Bank.AccountsServer.csproj"
COPY . .
WORKDIR "/src/Accounts/Bank.AccountsServer"
RUN dotnet build "Bank.AccountsServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bank.AccountsServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bank.AccountsServer.dll"]