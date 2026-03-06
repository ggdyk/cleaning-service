# Cleaning Service Platform

REST API платформы для онлайн-заказа клининговых услуг. Клиенты создают заказы, уборщики выполняют, менеджеры управляют.

## Стек

- **Runtime:** .NET 9.0, C#
- **Архитектура:** Clean Architecture + DDD + CQRS (MediatR)
- **БД:** PostgreSQL 16 + Entity Framework Core 9.0
- **Аутентификация:** JWT + Refresh Tokens (BCrypt)
- **Документация API:** Swagger / OpenAPI

---

## Быстрый старт (Docker)

Самый простой способ — поднять всё через Docker Compose.

**Требования:** Docker Desktop 24+

```bash
# 1. Клонировать репозиторий
git clone <repo-url>
cd cleaning-service

# 2. Создать файл с переменными окружения
cp .env.example .env

# 3. Поднять стек (первый раз ~2 минуты на сборку)
docker compose up --build

# 4. Применить миграции БД (один раз)
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

**API доступно:** http://localhost:8080/swagger

---

## Локальный запуск (без Docker)

**Требования:**
- .NET 9 SDK (`dotnet --version` → `9.0.x`)
- PostgreSQL 16 (локально или через Docker)

```bash
# Только БД через Docker
docker compose up postgres -d

# Восстановить зависимости
dotnet restore

# Применить миграции
dotnet ef database update --project src/Infrastructure --startup-project src/Api

# Запустить API
dotnet run --project src/Api
```

**API доступно:** https://localhost:5001/swagger

---

## Структура проекта

```
cleaning-service/
├── src/
│   ├── Api/                  # Controllers, Middleware, Program.cs
│   ├── Application/          # CQRS (Commands/Queries/Handlers), DTOs, Validators
│   ├── Domain/               # Entities, Value Objects, Exceptions, Enums
│   └── Infrastructure/       # EF Core, Repositories, JWT, PasswordHasher
├── tests/
│   ├── UnitTests/            # Тесты Domain и Application слоёв
│   └── IntegrationTests/     # Тесты API endpoints
├── docs/
│   └── api/DOCKER.md         # Подробная инструкция по Docker
├── Dockerfile                # Multi-stage build (52 MB итоговый образ)
├── docker-compose.yml        # Полный стек: API + PostgreSQL
└── .env.example              # Шаблон переменных окружения
```

### Bounded Contexts

| Контекст | Схема БД | Описание |
|----------|----------|----------|
| **Identity** | `identity` | Пользователи, роли, JWT/Refresh токены |
| **Orders** | `orders` | Заказы, статусы, история изменений |
| **Catalog** | `catalog` | Услуги, доп. услуги, категории, города |
| **Content** | `content` | FAQ, отзывы, страницы, callback-заявки |
| **Payment** | `payment` | Платежи (в разработке) |

---

## API Endpoints

Полная документация доступна в Swagger после запуска.

### Аутентификация
| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/auth/register` | Регистрация |
| POST | `/api/auth/login` | Вход, получение JWT |
| POST | `/api/auth/refresh` | Обновление access token |
| POST | `/api/auth/logout` | Выход, отзыв токенов |

### Заказы
| Метод | URL | Доступ |
|-------|-----|--------|
| POST | `/api/orders` | Client |
| GET | `/api/orders` | Client (свои заказы) |
| GET | `/api/orders/{id}` | Client (владелец) |

### Каталог
| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/calculator` | Расчёт стоимости уборки |
| GET | `/api/services` | Список услуг |
| GET | `/api/extra-services` | Список доп. услуг |
| GET | `/api/categories` | Категории услуг |

### Контент
| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/faq` | Список активных FAQ |
| GET | `/api/reviews` | Одобренные отзывы |
| POST | `/api/reviews` | Оставить отзыв |
| POST | `/api/callbacks` | Заявка на обратный звонок |

### Администрирование
| Метод | URL | Доступ |
|-------|-----|--------|
| GET | `/api/admin/reviews` | Admin, Manager |
| GET | `/api/admin/reviews/pending` | Admin, Manager |
| PUT | `/api/admin/reviews/{id}/approve` | Admin, Manager |
| PUT | `/api/admin/reviews/{id}/reject` | Admin, Manager |
| CRUD | `/api/faq/admin/*` | Admin, Manager |
| CRUD | `/api/extra-services` | Admin |

---

## Роли пользователей

| Роль | Возможности |
|------|-------------|
| `Client` | Создаёт заказы, оставляет отзывы, отменяет свои New-заказы |
| `Cleaner` | Видит назначенные заказы, меняет статус InProgress → Completed |
| `Manager` | Назначает уборщиков, управляет контентом, модерирует отзывы |
| `Admin` | Полный доступ ко всем операциям |

При регистрации через `/api/auth/register` роль устанавливается `Client`.
Другие роли назначаются через БД или Admin API.

---

## Переменные окружения

Скопируй `.env.example` в `.env` и измени под своё окружение.

| Переменная | Описание | Дефолт |
|-----------|----------|--------|
| `POSTGRES_PASSWORD` | Пароль PostgreSQL | `devpassword123` |
| `Jwt__Key` | Секрет JWT (мин. 32 символа) | см. `.env.example` |
| `API_PORT` | Внешний порт API | `8080` |
| `ASPNETCORE_ENVIRONMENT` | Окружение (`Development`/`Production`) | `Production` |

> В `Development` Swagger доступен в браузере и ошибки содержат stack trace.

---

## Тесты

```bash
# Все тесты
dotnet test

# Только unit-тесты
dotnet test tests/UnitTests

# Только интеграционные
dotnet test tests/IntegrationTests
```

---

## Команда

| Разработчик | Зона ответственности |
|-------------|---------------------|
| Разработчик 1 | Domain, Application (CQRS), архитектура |
| Разработчик 2 | Infrastructure, API, DevOps (Docker) |
