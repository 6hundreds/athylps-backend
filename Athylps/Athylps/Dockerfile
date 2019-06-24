FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Athylps/Athylps.csproj", "Athylps/"]
RUN dotnet restore "Athylps/Athylps.csproj"
COPY . .
WORKDIR "/src/Athylps"
RUN dotnet build "Athylps.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Athylps.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Athylps.dll"]