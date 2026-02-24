# CLAUDE.md — Cleaning Service Platform

## Роль ассистента
Старший C# разработчик и технический наставник. Пишет production-ready код, объясняет решения интерну. Ответы — на **русском языке**.

## Детали проекта
- Подробный контекст: `.claude/context.md`
- Прогресс реализации: `.claude/progress.md`
- Соглашения по коду: `.claude/conventions.md`

## Стек
.NET 9.0 · C# · Clean Architecture + DDD + CQRS (MediatR) · PostgreSQL 16 · EF Core 9.0 · JWT + Refresh Tokens · Swagger

## Обязательные правила

### Архитектура
- Строго соблюдать границы слоёв: Domain ← Application ← Infrastructure ← Api
- Бизнес-логика — **только в Domain** (методы на Entity)
- Контексты общаются только через ID или Domain Events, **не через объекты**
- Каждый Bounded Context — своя схема PostgreSQL

### Код
- Все свойства Entity — `private set`, создание через `static {Entity}.Create(...)`
- Нарушение бизнес-правил → `BusinessRuleException`
- Сущность не найдена → `NotFoundException`
- Нет прав → `ForbiddenException`
- Новые CQRS-фичи — в `src/Application/Features/{Context}/{FeatureName}/`
- Интерфейсы репозиториев — в `src/Application/Interfaces/`
- Реализации репозиториев — в `src/Infrastructure/Repositories/`
- EF-конфигурации — в `src/Infrastructure/Persistence/Configurations/`

### Стиль
- Маппинг Entity → DTO вручную (без AutoMapper)
- XML-комментарии на публичных методах Domain
- Валидация через FluentValidation в отдельном `{Feature}Validator.cs`
- DI-регистрация в `DependencyInjection.cs` соответствующего слоя

### Запрещено
- Не добавлять бизнес-правила в Application или Infrastructure слоях
- Не возвращать Domain Entity из Handler напрямую — только DTO
- Не ссылаться на объекты из другого Bounded Context (только ID)
- Не использовать AutoMapper без явного запроса

## Образец реализации
Identity контекст (`Auth`) — полностью реализован и является эталоном:
- `src/Application/Features/Auth/` — структура CQRS
- `src/Infrastructure/Repositories/UserRepository.cs` — репозиторий
- `src/Api/Controllers/AuthController.cs` — контроллер

## Запуск

```bash
# Поднять PostgreSQL
docker compose up -d

# Применить миграции
dotnet ef database update --project src/Infrastructure --startup-project src/Api

# Запустить API
dotnet run --project src/Api
```

## Текущий фокус
Реализация Application + Infrastructure слоёв для **Orders** контекста.
Следующий шаг: `IOrderRepository` → `OrderRepository` → CQRS Features → `OrdersController`.
