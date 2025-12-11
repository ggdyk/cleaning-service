# Структура решения (Solution Structure)

Документ описывает структуру Solution для проекта Cleaning Platform, построенного по принципам Clean Architecture.

---

## 🧱 1. Projects Overview

Solution содержит следующие проекты:
/Domain
/Application
/Infrastructure
/Api

Каждый проект имеет строго определенные обязанности и зависимости.

---

## 🧩 2. Описание ответственности проектов

### **Domain**
Ядро системы. Содержит:

- Доменные сущности (Entities)
- Value Objects
- Enums
- Интерфейсы репозиториев
- Доменные события
- Бизнес-правила

**Domain не зависит ни от одного проекта.**

---

### **Application**
Слой бизнес-логики. Содержит:

- Use Cases (команды и запросы)
- DTO
- Интерфейсы сервисов
- FluentValidation
- CQRS

**Application зависит только от Domain.**

---

### **Infrastructure**
Техническая реализация:

- EF Core / DbContext
- Репозитории
- PostgreSQL миграции
- JWT, Email, File Storage
- Конфигурации сущностей

**Infrastructure зависит от Application и Domain.**

---

### **Api**
Веб-слой приложения:

- Controllers / Endpoints
- Авторизация / аутентификация
- Конфигурация сервисов
- Middleware
- Swagger

**Api зависит от Application и Infrastructure.**

---

## 🔗 3. Схема зависимостей
Domain
↑
Application
↑
Infrastructure
↑
Api

Принцип:

- Верхний слой ничего не знает о нижнем
- Бизнес-логика не зависит от технических деталей
- Все внешние реализации находятся в Infrastructure

---

## 📌 4. Критерии готовности

- Архитектура описана
- Структура Solution определена
- Зависимости между проектами понятны
- Документ создан: `docs/architecture/solution-structure.md`

---

## 📁 5. Итоговая структура Solution
Cleaning.sln
/src
/Domain
/Application
/Infrastructure
/Api
/docs
/architecture
solution-structure.md
