# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.22 AS build
WORKDIR /src
COPY LifeDbApi.csproj .
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build as publish
WORKDIR /src
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.22
ENV ASPNETCORE_URLS=http://0.0.0.0:5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "LifeDbApi.dll"]
