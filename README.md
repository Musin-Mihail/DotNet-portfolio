# API для портфолио на .NET

Это бэкенд-проект, созданный на ASP.NET Core Web API, который предоставляет RESTful API для управления списком проектов в портфолио. Он использует Entity Framework Core для взаимодействия с базой данных PostgreSQL.

## 🚀 Основные возможности

- **CRUD-операции:** Полный набор операций для создания, чтения, обновления и удаления (CRUD) проектов.
- **Массовая загрузка:** Эндпоинт для одновременного создания нескольких проектов.
- **База данных PostgreSQL:** Использует Npgsql для эффективной работы с PostgreSQL.
- **Entity Framework Core:** Применяет подход Code-First для управления схемой базы данных с помощью миграций.
- **Документация API:** Автоматически генерируемая документация с помощью Swagger (OpenAPI), доступная в среде разработки.
- **Обработка CORS:** Настроена политика CORS для разрешения запросов с определённых доменов.

## 🛠️ Используемые технологии

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core 8**
- **Npgsql** (Драйвер для PostgreSQL)
- **Swagger/Swashbuckle** (Для документации API)

## API Эндпоинты

Ниже приведены основные эндпоинты, доступные в этом API:
| Метод | URL | Описание |
| -------- | -------------------- | ------------------------------------------- |
| `GET` | `/portfolio` | Возвращает приветственное сообщение от API. |
| `GET` | `/api/Projects` | Получает список всех проектов. |
| `GET` | `/api/Projects/{id}` | Получает конкретный проект по его ID. |
| `POST` | `/api/Projects` | Создаёт новый проект. |
| `POST` | `/api/Projects/bulk` | Создаёт несколько проектов одним запросом. |
| `PUT` | `/api/Projects/{id}` | Обновляет существующий проект по его ID. |
| `DELETE` | `/api/Projects/{id}` | Удаляет проект по его ID. |

## ⚙️ Начало работы

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0 "null")
- [PostgreSQL](https://www.postgresql.org/download/ "null")

### Установка

1.  **Клонируйте репозиторий:**
    git clone https://github.com/Musin-Mihail/DotNet-portfolio.git
    cd DotNet-portfolio

2.  **Настройте строку подключения:** Откройте файл `appsettings.json` и измените `DefaultConnection` для подключения к вашему экземпляру PostgreSQL.
    "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=your_db;Username=your_user;Password=your_password"
    }

3.  **Примените миграции:** Выполните команду в терминале в корневой папке проекта, чтобы создать таблицы в базе данных.
    dotnet ef database update

4.  **Запустите проект:**
    dotnet run

    Приложение запустится на `http://localhost:5160` и `https://localhost:7174`.

5.  **Просмотр документации API:** В среде разработки откройте браузер и перейдите по адресу `http://localhost:5160`, чтобы увидеть UI Swagger и протестировать эндпоинты.
