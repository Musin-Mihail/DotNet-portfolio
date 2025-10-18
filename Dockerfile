FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DotNet-portfolio.sln", "./"]
COPY ["src/DotNet-portfolio/DotNet-portfolio.csproj", "src/DotNet-portfolio/"]
COPY ["tests/DotNet-portfolio.Tests/DotNet-portfolio.Tests.csproj", "tests/DotNet-portfolio.Tests/"]
COPY ["tests/ApiTest/ApiTest.csproj", "tests/ApiTest/"]

RUN dotnet restore "DotNet-portfolio.sln"

COPY . .

RUN dotnet test "DotNet-portfolio.sln" --no-restore

WORKDIR "/src/src/DotNet-portfolio"
RUN dotnet publish "DotNet-portfolio.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DotNet-portfolio.dll"]
