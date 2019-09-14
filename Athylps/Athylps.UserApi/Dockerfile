FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Athylps.UserApi/Athylps.UserApi.csproj", "Athylps.UserApi/"]
RUN dotnet restore "Athylps.UserApi/Athylps.UserApi.csproj"
COPY . .
WORKDIR "/src/Athylps.UserApi"
RUN dotnet build "Athylps.UserApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Athylps.UserApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Athylps.UserApi.dll"]