FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Athylps.IdentityServer/Athylps.IdentityServer.csproj", "Athylps.IdentityServer/"]
RUN dotnet restore "Athylps.IdentityServer/Athylps.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Athylps.IdentityServer"
RUN dotnet build "Athylps.IdentityServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Athylps.IdentityServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Athylps.IdentityServer.dll"]