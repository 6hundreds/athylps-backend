FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Athylps.Management/Athylps.Management.csproj", "Athylps.Management/"]
RUN dotnet restore "Athylps.Management/Athylps.Management.csproj"
COPY . .
WORKDIR "/src/Athylps.Management"
RUN dotnet build "Athylps.Management.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Athylps.Management.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Athylps.Management.dll"]