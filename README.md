# API для портфолио на .NET

Это бэкенд-проект, созданный на ASP.NET Core Web API, который предоставляет RESTful API для управления списком проектов в портфолио. Он использует Entity Framework Core для взаимодействия с базой данных PostgreSQL.

## 🚀 Основные возможности

- **CRUD-операции:** Полный набор операций для создания, чтения, обновления и удаления (CRUD) проектов.

- **База данных PostgreSQL:** Использует Npgsql для эффективной работы с базой данных PostgreSQL.

- **Entity Framework Core:** Применяет подход Code-First для управления схемой базы данных с помощью миграций.

- **Документация API:** Автоматически генерируемая документация API с помощью Swagger (OpenAPI), доступная в среде разработки.

- **Обработка CORS:** Настроена политика CORS для разрешения запросов с определённых доменов (localhost и развёрнутое фронтенд-приложение).

- **Массовое создание:** Возможность добавления нескольких проектов одним запросом.

## 🛠️ Используемые технологии

- **.NET 9.0**

- **ASP.NET Core Web API**

- **Entity Framework Core 9.0.9**

- **Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4** (Драйвер для PostgreSQL)

- **Swagger/Swashbuckle 9.0.4** (Для документации API)

## API эндпоинты

## Ниже приведены основные эндпоинты, доступные в этом API:| Метод | URL | Описание |

| `GET` | `/portfolio` | Возвращает приветственное сообщение от бэкенда. |

| `GET` | `/api/Projects` | Получает список всех проектов. |

| `GET` | `/api/Projects/{id}` | Получает конкретный проект по его ID. |

| `POST` | `/api/Projects` | Создаёт новый проект. |

| `POST` | `/api/Projects/bulk` | Создаёт несколько проектов одним запросом. |

| `PUT` | `/api/Projects/{id}` | Обновляет существующий проект по его ID. |

| `DELETE` | `/api/Projects/{id}` | Удаляет проект по его ID. |

## ⚙️ Начало работы

### Требования

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0 "null")

- [PostgreSQL](https://www.postgresql.org/download/ "null")

### Установка

1.  **Клонируйте репозиторий:**

    git clone https://github.com/Musin-Mihail/DotNet-portfolio.git
    cd <имя-папки-репозитория>

2.  **Настройте строку подключения:** Откройте файл `appsettings.json` и измените `DefaultConnection` для подключения к вашему экземпляру PostgreSQL.

    "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=your_db;Username=your_user;Password=your_password"
    }

3.  **Примените миграции:** Выполните команду в терминале в корневой папке проекта, чтобы создать таблицы в базе данных.

    dotnet ef database update

4.  **Запустите проект:**

        dotnet run

    Приложение запустится на `http://localhost:5160` и `https://localhost:7174`.

5.  **Просмотр документации API:** В среде разработки откройте браузер и перейдите по адресу `http://localhost:5160` (или `https://localhost:7174`), чтобы увидеть UI Swagger и протестировать эндпоинты.
