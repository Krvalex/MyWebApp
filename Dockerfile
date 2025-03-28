
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MyWebApp/MyWebApp.csproj", "MyWebApp/"]
RUN dotnet restore "MyWebApp/MyWebApp.csproj"

COPY . .
WORKDIR "/src/MyWebApp"
RUN dotnet build "MyWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyWebApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MyWebApp.dll"]