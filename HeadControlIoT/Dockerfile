FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HeadControlIoT.csproj", "./"]
RUN dotnet restore "HeadControlIoT.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "HeadControlIoT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HeadControlIoT.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HeadControlIoT.dll"]
