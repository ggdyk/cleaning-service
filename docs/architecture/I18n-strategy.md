# Стратегия мультиязычности (i18n) — Internationalization Strategy

## 📋 Содержание
- [Обзор](#обзор)
- [Требования](#требования)
- [Анализ подходов](#анализ-подходов)
- [Выбранное решение](#выбранное-решение)
- [Применение к сущностям](#применение-к-сущностям)
- [Примеры реализации](#примеры-реализации)
- [Миграция и развёртывание](#миграция-и-развёртывание)

---

## 🎯 Обзор

Система поддерживает **два языка**:
- 🇷🇺 **Русский (ru)** — основной язык
- 🇬🇧 **Английский (en)** — дополнительный язык

**Цель:** Обеспечить отображение контента на выбранном пользователем языке.

---

## 📋 Требования

### Функциональные требования:
1. Пользователь может выбрать язык интерфейса (RU/EN)
2. Весь контент отображается на выбранном языке
3. Если перевод отсутствует — показывать русский текст (fallback)
4. Язык сохраняется в сессии пользователя

### Нефункциональные требования:
1. **Производительность:** Быстрый доступ к переводам (без лишних JOIN'ов)
2. **Простота:** Легко писать запросы и тесты
3. **Поддерживаемость:** Понятная структура для новых разработчиков
4. **Масштабируемость:** Не планируется добавление новых языков

---

## 🔍 Анализ подходов

### Подход 1: Колонки с суффиксами (_ru / _en)

**Структура:**
```sql
CREATE TABLE Service (
  id SERIAL PRIMARY KEY,
  name_ru VARCHAR(200) NOT NULL,
  name_en VARCHAR(200) NOT NULL,
  description_ru TEXT,
  description_en TEXT
);
```

**Плюсы:**
- ✅ Простая структура БД
- ✅ Высокая производительность (нет JOIN'ов)
- ✅ Легко писать запросы
- ✅ Легко создавать индексы для поиска
- ✅ Типобезопасность в коде (строгие типы полей)
- ✅ Меньше строк в БД (компактное хранение)

**Минусы:**
- ❌ Добавление нового языка требует изменения схемы БД
- ❌ Дублирование логики выбора языка в коде
- ❌ Сложнее версионировать изменения переводов

---

### Подход 2: Отдельная таблица переводов

**Структура:**
```sql
CREATE TABLE Service (
  id SERIAL PRIMARY KEY
  -- только технические поля
);

CREATE TABLE Translation (
  id SERIAL PRIMARY KEY,
  entity_type VARCHAR(50) NOT NULL,
  entity_id INT NOT NULL,
  language VARCHAR(5) NOT NULL,
  field_name VARCHAR(50) NOT NULL,
  value TEXT NOT NULL,
  UNIQUE(entity_type, entity_id, language, field_name)
);
```

**Плюсы:**
- ✅ Легко добавлять новые языки
- ✅ Централизованное управление переводами
- ✅ Возможность версионирования переводов
- ✅ Удобно для админ-панели (CRUD переводов)

**Минусы:**
- ❌ Сложные запросы (много JOIN'ов)
- ❌ Медленнее (больше обращений к БД)
- ❌ Больше строк в БД (1 строка = 1 перевод 1 поля)
- ❌ Сложнее писать и тестировать код
- ❌ Нет типобезопасности (всё TEXT)

---

### Подход 3: JSON-поле

**Структура:**
```sql
CREATE TABLE Service (
  id SERIAL PRIMARY KEY,
  name JSONB NOT NULL,  -- {"ru": "Уборка", "en": "Cleaning"}
  description JSONB
);
```

**Плюсы:**
- ✅ Гибкость (легко добавлять языки)
- ✅ Компактное хранение

**Минусы:**
- ❌ Сложнее индексировать для поиска
- ❌ Нет типобезопасности
- ❌ Сложнее писать миграции
- ❌ Менее понятно для новых разработчиков

---

## ✅ Выбранное решение: Колонки _ru/_en

### 🎯 Обоснование выбора

**Контекст проекта:**
- Учебный проект для изучения Clean Architecture и DDD
- Только 2 языка (RU/EN), добавление других не планируется
- Переводы редактируются редко (через миграции БД)
- Редактируют только разработчики
- Поиск по переводам не требуется

**Почему этот подход лучше:**
1. **Простота** — главный приоритет для учебного проекта
2. **Производительность** — нет JOIN'ов, прямой доступ к полям
3. **Понятность** — любой разработчик сразу поймёт структуру
4. **Достаточность** — 2 языка покрываются без усложнения

---

### 📊 Сравнение с альтернативами

| Критерий | Колонки _ru/_en | Translation | JSON |
|----------|----------------|-------------|------|
| Простота кода | ✅ Отлично | ⚠️ Сложно | ⚠️ Средне |
| Производительность | ✅ Отлично | ❌ Медленно | ✅ Хорошо |
| Добавление языков | ❌ Сложно | ✅ Легко | ✅ Легко |
| Типобезопасность | ✅ Да | ❌ Нет | ❌ Нет |
| Индексы/Поиск | ✅ Легко | ✅ Возможно | ⚠️ Сложно |
| Для учебного проекта | ✅✅✅ | ⚠️ | ⚠️ |

**Вывод:** Колонки _ru/_en — оптимальный выбор для данного проекта.

---

## 📦 Применение к сущностям

### Сущности, требующие перевода:

#### 1. Catalog Context

**Service:**
```sql
CREATE TABLE Service (
  id SERIAL PRIMARY KEY,
  category_id INT NOT NULL,
  name_ru VARCHAR(200) NOT NULL,
  name_en VARCHAR(200) NOT NULL,
  description_ru TEXT,
  description_en TEXT,
  is_active BOOLEAN DEFAULT TRUE
);
```

**ServiceCategory:**
```sql
CREATE TABLE ServiceCategory (
  id SERIAL PRIMARY KEY,
  name_ru VARCHAR(100) NOT NULL,
  name_en VARCHAR(100) NOT NULL,
  description_ru TEXT,
  description_en TEXT
);
```

**ExtraService:**
```sql
CREATE TABLE ExtraService (
  id SERIAL PRIMARY KEY,
  name_ru VARCHAR(200) NOT NULL,
  name_en VARCHAR(200) NOT NULL,
  description_ru TEXT,
  description_en TEXT,
  price DECIMAL(10,2) NOT NULL
);
```

---

#### 2. Content Context

**Page:**
```sql
CREATE TABLE Page (
  id SERIAL PRIMARY KEY,
  slug VARCHAR(100) UNIQUE NOT NULL,
  title_ru VARCHAR(200) NOT NULL,
  title_en VARCHAR(200) NOT NULL,
  content_ru TEXT,
  content_en TEXT,
  is_published BOOLEAN DEFAULT TRUE
);
```

**FAQ:**
```sql
CREATE TABLE FAQ (
  id SERIAL PRIMARY KEY,
  question_ru VARCHAR(500) NOT NULL,
  question_en VARCHAR(500) NOT NULL,
  answer_ru TEXT NOT NULL,
  answer_en TEXT NOT NULL,
  order_index INT DEFAULT 0
);
```

**BlogPost:**
```sql
CREATE TABLE BlogPost (
  id SERIAL PRIMARY KEY,
  title_ru VARCHAR(200) NOT NULL,
  title_en VARCHAR(200) NOT NULL,
  content_ru TEXT NOT NULL,
  content_en TEXT NOT NULL,
  author_id INT NOT NULL,
  published_at TIMESTAMP
);
```

---

#### 3. Notifications Context

**NotificationTemplate:**
```sql
CREATE TABLE NotificationTemplate (
  id SERIAL PRIMARY KEY,
  name VARCHAR(100) UNIQUE NOT NULL,
  subject_ru VARCHAR(200) NOT NULL,
  subject_en VARCHAR(200) NOT NULL,
  body_ru TEXT NOT NULL,
  body_en TEXT NOT NULL,
  template_type VARCHAR(50) NOT NULL
);
```

---

### Сущности, НЕ требующие перевода:

#### Identity Context
- ❌ **User** — имена людей не переводятся
- ❌ **Role** — системные роли (Client, Cleaner, Manager, Admin)

#### Orders Context
- ❌ **Order** — технические данные (цены, даты, статусы)
- ❌ **Address** — конкретные адреса
- ❌ **TimeSlot** — время (универсально)

#### Payment Context
- ❌ **Payment** — цифры и статусы (универсальны)

#### Content Context
- ❌ **Review** — отзывы пользователей на их родном языке (не переводятся)
- ❌ **CallbackRequest** — запросы от пользователей

---

## 💻 Примеры реализации

### 1. Entity в Domain Layer

```csharp
namespace CleaningService.Domain.Entities.Catalog
{
    public class Service : Entity
    {
        public int Id { get; private set; }
        public int CategoryId { get; private set; }
        
        // Мультиязычные поля
        public string NameRu { get; private set; }
        public string NameEn { get; private set; }
        public string DescriptionRu { get; private set; }
        public string DescriptionEn { get; private set; }
        
        public bool IsActive { get; private set; }
        
        // Навигационные свойства
        public ServiceCategory Category { get; private set; }
        
        // Методы для работы с переводами
        public string GetName(string language)
        {
            return language?.ToLower() == "en" ? NameEn : NameRu;
        }
        
        public string GetDescription(string language)
        {
            return language?.ToLower() == "en" ? DescriptionEn : DescriptionRu;
        }
    }
}
```

---

### 2. DTO для API

```csharp
namespace CleaningService.Application.DTOs.Catalog
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }        // Перевод по выбранному языку
        public string Description { get; set; }  // Перевод по выбранному языку
        public bool IsActive { get; set; }
    }
}
```

---

### 3. Mapper в Application Layer

```csharp
public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.Name, 
                opt => opt.MapFrom((src, dest, destMember, context) => 
                {
                    var language = context.Items["Language"] as string ?? "ru";
                    return src.GetName(language);
                }))
            .ForMember(dest => dest.Description, 
                opt => opt.MapFrom((src, dest, destMember, context) => 
                {
                    var language = context.Items["Language"] as string ?? "ru";
                    return src.GetDescription(language);
                }));
    }
}
```

---

### 4. Controller (API)

```csharp
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpGet]
    public async Task<ActionResult<List<ServiceDto>>> GetServices(
        [FromHeader(Name = "Accept-Language")] string language = "ru")
    {
        var query = new GetServicesQuery { Language = language };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
```

---

### 5. Query Handler

```csharp
public class GetServicesQueryHandler 
    : IRequestHandler<GetServicesQuery, List<ServiceDto>>
{
    private readonly IServiceRepository _repository;
    private readonly IMapper _mapper;
    
    public async Task<List<ServiceDto>> Handle(
        GetServicesQuery request, 
        CancellationToken cancellationToken)
    {
        var services = await _repository.GetAllActiveAsync();
        
        // Передаём язык в контекст маппера
        var mappingContext = new ResolutionContext(
            new MappingOperationOptions(), 
            _mapper.ConfigurationProvider);
        mappingContext.Items["Language"] = request.Language ?? "ru";
        
        return services
            .Select(s => _mapper.Map<ServiceDto>(s, opts => 
                opts.Items["Language"] = request.Language ?? "ru"))
            .ToList();
    }
}
```

---

### 6. EF Core Configuration

```csharp
public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.NameRu)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(s => s.NameEn)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(s => s.DescriptionRu)
            .HasColumnType("TEXT");
            
        builder.Property(s => s.DescriptionEn)
            .HasColumnType("TEXT");
            
        // Индексы для поиска (опционально)
        builder.HasIndex(s => s.NameRu);
        builder.HasIndex(s => s.NameEn);
    }
}
```

---

## 🔄 Миграция и развёртывание

### 1. Создание миграции

```bash
dotnet ef migrations add AddMultilingualSupport --project src/Infrastructure
```

### 2. Пример миграции

```csharp
public partial class AddMultilingualSupport : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Для существующих таблиц: переименовываем колонки
        migrationBuilder.RenameColumn(
            name: "Name",
            table: "Services",
            newName: "NameRu");
            
        migrationBuilder.RenameColumn(
            name: "Description",
            table: "Services",
            newName: "DescriptionRu");
        
        // Добавляем английские колонки
        migrationBuilder.AddColumn<string>(
            name: "NameEn",
            table: "Services",
            maxLength: 200,
            nullable: false,
            defaultValue: "");
            
        migrationBuilder.AddColumn<string>(
            name: "DescriptionEn",
            table: "Services",
            type: "TEXT",
            nullable: true);
            
        // Заполняем начальными значениями (копируем из русского)
        migrationBuilder.Sql(
            "UPDATE Services SET NameEn = NameRu, DescriptionEn = DescriptionRu");
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "NameEn", table: "Services");
        migrationBuilder.DropColumn(name: "DescriptionEn", table: "Services");
        
        migrationBuilder.RenameColumn(
            name: "NameRu",
            table: "Services",
            newName: "Name");
            
        migrationBuilder.RenameColumn(
            name: "DescriptionRu",
            table: "Services",
            newName: "Description");
    }
}
```

---

### 3. Заполнение переводов

**Опция А:** Через SQL-скрипт

```sql
-- Обновление переводов услуг
UPDATE Services 
SET 
  NameEn = 'Apartment Cleaning',
  DescriptionEn = 'Professional apartment cleaning...'
WHERE Id = 1;

UPDATE Services 
SET 
  NameEn = 'Post-Renovation Cleaning',
  DescriptionEn = 'Deep cleaning after renovation...'
WHERE Id = 2;
```

**Опция Б:** Через Seed данных

```csharp
public class ServiceSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>().HasData(
            new Service 
            { 
                Id = 1,
                CategoryId = 1,
                NameRu = "Уборка квартиры",
                NameEn = "Apartment Cleaning",
                DescriptionRu = "Профессиональная уборка квартиры...",
                DescriptionEn = "Professional apartment cleaning...",
                IsActive = true
            },
            // ... другие услуги
        );
    }
}
```

---

## 📝 Соглашения и рекомендации

### Именование полей:

✅ **Правильно:**
```
name_ru, name_en
description_ru, description_en
```

❌ **Неправильно:**
```
nameRU, nameEN  (неконсистентно с нижним регистром)
name_russian, name_english  (слишком длинно)
name_1, name_2  (непонятно, что это за языки)
```

---

### Fallback на русский язык:

Если английский перевод отсутствует — показывать русский:

```csharp
public string GetName(string language)
{
    if (language?.ToLower() == "en" && !string.IsNullOrEmpty(NameEn))
        return NameEn;
    
    return NameRu;  // Fallback на русский
}
```

---

### Валидация при создании/обновлении:

```csharp
public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Название на русском обязательно")
            .MaximumLength(200);
            
        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("Название на английском обязательно")
            .MaximumLength(200);
    }
}
```

---

## 🚀 Будущие улучшения (опционально)

### 1. Resource Manager (.resx файлы)

Для UI-строк (кнопки, сообщения об ошибках) можно использовать стандартный .NET механизм:

```csharp
// Resources/Messages.ru.resx
// Resources/Messages.en.resx

public string GetErrorMessage(string language)
{
    var culture = new CultureInfo(language);
    return Messages.ResourceManager.GetString("ErrorMessage", culture);
}
```

---

### 2. Middleware для определения языка

```csharp
public class LanguageMiddleware
{
    private readonly RequestDelegate _next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var language = context.Request.Headers["Accept-Language"].FirstOrDefault()
                       ?? context.Request.Query["lang"]
                       ?? "ru";
        
        context.Items["Language"] = language.StartsWith("en") ? "en" : "ru";
        
        await _next(context);
    }
}
```

---

### 3. Кэширование переводов

Для часто запрашиваемых сущностей:

```csharp
public class CachedServiceRepository : IServiceRepository
{
    private readonly IServiceRepository _repository;
    private readonly IMemoryCache _cache;
    
    public async Task<Service> GetByIdAsync(int id)
    {
        return await _cache.GetOrCreateAsync(
            $"service_{id}",
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                return await _repository.GetByIdAsync(id);
            });
    }
}
```

---

## 📚 Ссылки и ресурсы

- [Microsoft: Globalization and localization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization)
- [PostgreSQL: JSONB vs Columns](https://www.postgresql.org/docs/current/datatype-json.html)
- [Clean Architecture: Domain Entities](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)

---

## ✅ Критерии готовности

- [x] Подход выбран и обоснован
- [x] Описаны плюсы и минусы
- [x] Определены сущности, требующие перевода
- [x] Приведены примеры реализации
- [x] Документ создан и понятен обоим разработчикам

---

**Версия документа:** 1.0  
**Дата создания:** 2024-12-11  
**Авторы:** Developer 1, Developer 2  
**Статус:** Approved