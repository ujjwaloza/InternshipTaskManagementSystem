FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# ?? ADD THIS LINE (IMPORTANT)
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InternshipTaskManagementSystem.dll"]