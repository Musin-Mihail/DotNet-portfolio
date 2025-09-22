# Этап 1: Сборка проекта с использованием .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Копируем .csproj и восстанавливаем зависимости
COPY *.csproj .
RUN dotnet restore

# Копируем остальные файлы и собираем приложение
COPY . .
RUN dotnet publish -c Release -o out

# Этап 2: Запуск приложения на .NET 9 ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "DotNet-portfolio.dll"]