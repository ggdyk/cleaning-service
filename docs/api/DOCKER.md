# Запуск через Docker Compose

## Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) 24+
- Свободный порт `8080` (API) и `5432` (PostgreSQL)

---

## Быстрый старт

```bash
# 1. Скопировать переменные окружения
cp .env.example .env

# 2. Поднять весь стек (первый запуск скомпилирует образ ~1-2 минуты)
docker compose up --build

# API доступно по адресу:
# http://localhost:8080/swagger
```

При последующих запусках без изменений в коде образ пересобирается из кэша (~5 секунд):

```bash
docker compose up
```

---

## Состав стека

| Сервис | Образ | Порт | Описание |
|--------|-------|------|----------|
| `api` | `cleaning-api:local` (собирается из Dockerfile) | 8080 | ASP.NET Core 9.0 API |
| `postgres` | `postgres:16-alpine` | 5432 | База данных |

---

## Переменные окружения

Все настройки хранятся в файле `.env` (создаётся из `.env.example`).
Файл `.env` **не коммитится в git** — он содержит секреты.

| Переменная | Описание | Дефолт |
|-----------|----------|--------|
| `POSTGRES_DB` | Имя базы данных | `cleaning_platform_dev` |
| `POSTGRES_USER` | Пользователь PostgreSQL | `postgres` |
| `POSTGRES_PASSWORD` | Пароль PostgreSQL | `devpassword123` |
| `Jwt__Key` | Секрет для подписи JWT (мин. 32 символа) | см. `.env.example` |
| `Jwt__Issuer` | Издатель JWT | `CleaningServiceApi` |
| `Jwt__Audience` | Аудитория JWT | `CleaningServiceClient` |
| `API_PORT` | Внешний порт API | `8080` |
| `ASPNETCORE_ENVIRONMENT` | Окружение ASP.NET Core | `Production` |

---

## Миграции базы данных

Миграции **не применяются автоматически** при старте контейнера.
После первого `docker compose up` нужно применить их вручную.

### Способ 1 — через dotnet ef (рекомендуется для разработки)

```bash
# PostgreSQL должен быть запущен (можно только его)
docker compose up postgres -d

# Применить все миграции
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

### Способ 2 — SQL-скрипт

```bash
# Сгенерировать SQL из всех миграций
dotnet ef migrations script --idempotent \
  --project src/Infrastructure \
  --startup-project src/Api \
  --output migration.sql

# Применить через psql в контейнере
docker compose up postgres -d
docker exec -i cleaning_platform_db psql -U postgres -d cleaning_platform_dev < migration.sql
```

---

## Полезные команды

```bash
# Запустить только PostgreSQL (для локальной разработки API)
docker compose up postgres -d

# Посмотреть логи API в реальном времени
docker compose logs -f api

# Перезапустить только API (после изменений в коде)
docker compose up api --build

# Остановить все сервисы (данные БД сохраняются)
docker compose down

# Остановить и удалить данные БД (полный сброс)
docker compose down -v

# Зайти в контейнер PostgreSQL
docker exec -it cleaning_platform_db psql -U postgres -d cleaning_platform_dev

# Посмотреть размер итогового Docker-образа
docker image ls cleaning-api:local
```

---

## Архитектура сети

Внутри Docker Compose все сервисы находятся в одной сети.
API подключается к postgres по имени сервиса `postgres` (не `localhost`).

```
[Host]  localhost:8080  →  [api container]:8080
[Host]  localhost:5432  →  [postgres container]:5432
[api]   postgres:5432   →  [postgres container]:5432  (internal)
```

---

## Данные между перезапусками

Данные PostgreSQL хранятся в Docker volume `postgres_data`.
Volume **сохраняется** при `docker compose down` и **удаляется** только при `docker compose down -v`.

```bash
# Посмотреть все volumes
docker volume ls | grep cleaning

# Проверить содержимое volume
docker volume inspect cleaning-service_postgres_data
```

---

## Troubleshooting

**API не стартует, ошибка подключения к БД:**
Healthcheck postgres ожидается до запуска api. Если всё равно падает — проверь:
```bash
docker compose logs postgres
```

**Порт 8080 занят:**
Измени `API_PORT=8081` в `.env`.

**Порт 5432 занят (локальный PostgreSQL):**
Останови локальный postgres или измени маппинг в `docker-compose.yml`:
```yaml
ports:
  - "5433:5432"
```
И обнови строку подключения в `.env` для локальной разработки.
