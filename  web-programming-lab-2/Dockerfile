FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY [" web-programming-lab-2/web-programming-lab-2.csproj", " web-programming-lab-2/"]
RUN dotnet restore " web-programming-lab-2/web-programming-lab-2.csproj"
COPY . .
WORKDIR "/src/ web-programming-lab-2"
RUN dotnet build "./web-programming-lab-2.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./web-programming-lab-2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web-programming-lab-2.dll"]
