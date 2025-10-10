FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY src/DotNet-portfolio/*.csproj ./src/DotNet-portfolio/
COPY tests/DotNet-portfolio.Tests/*.csproj ./tests/DotNet-portfolio.Tests/
RUN dotnet restore

COPY . .
WORKDIR /app/src/DotNet-portfolio
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DotNet-portfolio.dll"]