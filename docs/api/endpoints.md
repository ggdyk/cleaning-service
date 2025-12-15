# API Endpoints — Структура REST API

## 📋 Содержание
- [Обзор](#обзор)
- [Общие правила](#общие-правила)
- [Аутентификация и авторизация](#аутентификация-и-авторизация)
- [Identity Context](#identity-context)
- [Catalog Context](#catalog-context)
- [Orders Context](#orders-context)
- [Payment Context](#payment-context)
- [Content Context](#content-context)
- [Notifications Context](#notifications-context)
- [Admin Context](#admin-context)
- [Коды ответов](#коды-ответов)
- [Примеры запросов](#примеры-запросов)

---

## 🎯 Обзор

REST API для платформы клининговых услуг.

**Базовый URL:** `https://api.cleaning-service.com/api`  
**Версия API:** v1 (без явной версии в URL, версионирование через заголовки при необходимости)  
**Формат данных:** JSON  
**Кодировка:** UTF-8

---

## 📐 Общие правила

### 1. Структура URL

```
/api/{resource}
/api/{resource}/{id}
/api/{resource}/{id}/{sub-resource}
```

**Примеры:**
- ✅ `/api/orders` — список заказов
- ✅ `/api/orders/123` — заказ с ID 123
- ✅ `/api/orders/123/extra-services` — доп. услуги заказа 123

---

### 2. HTTP-методы

| Метод | Назначение | Идемпотентность |
|-------|-----------|----------------|
| **GET** | Получение данных | ✅ Да |
| **POST** | Создание ресурса | ❌ Нет |
| **PUT** | Полное обновление | ✅ Да |
| **PATCH** | Частичное обновление | ❌ Нет |
| **DELETE** | Удаление ресурса | ✅ Да |

---

### 3. Пагинация

Все endpoints, возвращающие списки, поддерживают пагинацию:

**Параметры запроса:**
```
?page=1          - Номер страницы (по умолчанию: 1)
&pageSize=20     - Размер страницы (по умолчанию: 20, макс: 100)
```

**Формат ответа:**
```json
{
  "items": [...],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

---

### 4. Фильтрация

Базовая фильтрация через query-параметры:

**Примеры:**
```
GET /api/orders?status=Completed
GET /api/orders?clientId=123&status=New
GET /api/services?categoryId=5&isActive=true
GET /api/orders?from=2024-01-01&to=2024-12-31
```

**Поддерживаемые операторы:**
- Равенство: `?field=value`
- Диапазон дат: `?from=...&to=...`
- Множественный выбор: `?status=New,Assigned` (через запятую)

---

### 5. Сортировка

Базовая сортировка через query-параметры:

**Параметры:**
```
?sortBy=createdAt    - Поле для сортировки
&order=desc          - Направление: asc (по возрастанию) / desc (по убыванию)
```

**Примеры:**
```
GET /api/orders?sortBy=createdAt&order=desc
GET /api/services?sortBy=name&order=asc
GET /api/reviews?sortBy=rating&order=desc
```

**По умолчанию:** `sortBy=id&order=asc`

---

### 6. Мультиязычность

Язык интерфейса передаётся через заголовок:

```
Accept-Language: en
Accept-Language: ru
```

Если заголовок отсутствует — используется русский язык (по умолчанию).

---

### 7. Обработка ошибок

Все ошибки возвращаются в едином формате:

```json
{
  "error": {
    "code": "ORDER_NOT_FOUND",
    "message": "Заказ с ID 123 не найден",
    "details": {
      "orderId": 123
    }
  }
}
```

---

## 🔐 Аутентификация и авторизация

### Механизм

**JWT (JSON Web Token)** — access token + refresh token

**Access token:**
- Срок действия: 15 минут
- Передаётся в заголовке: `Authorization: Bearer {token}`

**Refresh token:**
- Срок действия: 30 дней
- Хранится в БД
- Используется для получения нового access token

---

### Роли пользователей

| Роль | Описание |
|------|----------|
| **Client** | Клиент (заказывает уборку) |
| **Cleaner** | Уборщик (выполняет заказы) |
| **Manager** | Менеджер (управляет заказами, модерирует) |
| **Admin** | Администратор (полный доступ) |

---

## 🔐 Identity Context

### Аутентификация

#### Регистрация
```
POST /api/auth/register
```

**Тело запроса:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "Иван",
  "lastName": "Иванов",
  "phone": "+77001234567",
  "role": "Client"
}
```

**Ответ:** `201 Created`
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "Иван",
  "lastName": "Иванов",
  "role": "Client"
}
```

---

#### Вход в систему
```
POST /api/auth/login
```

**Тело запроса:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Ответ:** `200 OK`
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6...",
  "expiresIn": 900,
  "user": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "Иван",
    "lastName": "Иванов",
    "role": "Client"
  }
}
```

---

#### Обновление токена
```
POST /api/auth/refresh
```

**Тело запроса:**
```json
{
  "refreshToken": "a1b2c3d4e5f6..."
}
```

**Ответ:** `200 OK`
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "g7h8i9j0k1l2...",
  "expiresIn": 900
}
```

---

#### Выход из системы
```
POST /api/auth/logout
Authorization: Bearer {token}
```

**Ответ:** `204 No Content`

---

### Управление профилем

#### Получить свой профиль
```
GET /api/users/me
Authorization: Bearer {token}
```

**Ответ:** `200 OK`
```json
{
  "id": 1,
  "email": "user@example.com",
  "firstName": "Иван",
  "lastName": "Иванов",
  "phone": "+77001234567",
  "role": "Client",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

---

#### Обновить свой профиль
```
PUT /api/users/me
Authorization: Bearer {token}
```

**Тело запроса:**
```json
{
  "firstName": "Иван",
  "lastName": "Петров",
  "phone": "+77007654321"
}
```

**Ответ:** `200 OK` (обновлённый профиль)

---

#### Изменить пароль
```
PUT /api/users/me/password
Authorization: Bearer {token}
```

**Тело запроса:**
```json
{
  "currentPassword": "OldPass123!",
  "newPassword": "NewSecurePass456!"
}
```

**Ответ:** `204 No Content`

---

### Управление пользователями (Admin, Manager)

#### Список пользователей
```
GET /api/users
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Query параметры:**
- `?page=1&pageSize=20` — пагинация
- `?role=Client` — фильтр по роли
- `?search=ivan` — поиск по имени/email
- `?sortBy=createdAt&order=desc` — сортировка

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "email": "user@example.com",
      "firstName": "Иван",
      "lastName": "Иванов",
      "role": "Client",
      "createdAt": "2024-01-15T10:00:00Z"
    }
  ],
  "pagination": { ... }
}
```

---

#### Получить пользователя по ID
```
GET /api/users/{id}
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `200 OK` (профиль пользователя)

---

#### Обновить пользователя
```
PUT /api/users/{id}
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Тело запроса:**
```json
{
  "firstName": "Иван",
  "lastName": "Сидоров",
  "phone": "+77001112233"
}
```

**Ответ:** `200 OK`

---

#### Заблокировать пользователя
```
PUT /api/users/{id}/block
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `204 No Content`

---

#### Разблокировать пользователя
```
PUT /api/users/{id}/unblock
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `204 No Content`

---

#### Изменить роль пользователя
```
PUT /api/users/{id}/role
Authorization: Bearer {token}
Roles: Admin
```

**Тело запроса:**
```json
{
  "role": "Cleaner"
}
```

**Ответ:** `200 OK`

---

#### Удалить пользователя (мягкое удаление)
```
DELETE /api/users/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

## 📦 Catalog Context

### Услуги

#### Список услуг
```
GET /api/services
```

**Query параметры:**
- `?categoryId=1` — фильтр по категории
- `?isActive=true` — только активные
- `?sortBy=name&order=asc` — сортировка

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "categoryId": 1,
      "name": "Уборка квартиры",
      "description": "Профессиональная уборка жилых помещений",
      "isActive": true
    }
  ],
  "pagination": { ... }
}
```

---

#### Получить услугу по ID
```
GET /api/services/{id}
```

**Ответ:** `200 OK`
```json
{
  "id": 1,
  "categoryId": 1,
  "categoryName": "Основные услуги",
  "name": "Уборка квартиры",
  "description": "Профессиональная уборка...",
  "isActive": true,
  "prices": [
    {
      "id": 1,
      "price": 5000.00,
      "areaFrom": 0,
      "areaTo": 50
    },
    {
      "id": 2,
      "price": 7000.00,
      "areaFrom": 51,
      "areaTo": 100
    }
  ]
}
```

---

#### Создать услугу
```
POST /api/services
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Тело запроса:**
```json
{
  "categoryId": 1,
  "nameRu": "Уборка квартиры",
  "nameEn": "Apartment Cleaning",
  "descriptionRu": "Профессиональная уборка жилых помещений",
  "descriptionEn": "Professional apartment cleaning",
  "isActive": true
}
```

**Ответ:** `201 Created`

---

#### Обновить услугу
```
PUT /api/services/{id}
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Тело запроса:** аналогично POST

**Ответ:** `200 OK`

---

#### Удалить услугу
```
DELETE /api/services/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

#### Получить цену услуги
```
GET /api/services/{id}/price?area=75
```

**Ответ:** `200 OK`
```json
{
  "serviceId": 1,
  "area": 75,
  "price": 7000.00
}
```

---

### Категории услуг

#### Список категорий
```
GET /api/categories
```

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "name": "Основные услуги",
      "description": "Базовый клининг"
    }
  ]
}
```

---

#### Получить категорию
```
GET /api/categories/{id}
```

**Ответ:** `200 OK`

---

#### Создать категорию
```
POST /api/categories
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Тело запроса:**
```json
{
  "nameRu": "Основные услуги",
  "nameEn": "Main Services",
  "descriptionRu": "Базовый клининг",
  "descriptionEn": "Basic cleaning"
}
```

**Ответ:** `201 Created`

---

#### Обновить категорию
```
PUT /api/categories/{id}
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `200 OK`

---

#### Удалить категорию
```
DELETE /api/categories/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

### Дополнительные услуги

#### Список дополнительных услуг
```
GET /api/extra-services
```

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "name": "Мытьё окон",
      "description": "Профессиональное мытьё окон",
      "price": 1500.00,
      "isActive": true
    }
  ]
}
```

---

#### Получить дополнительную услугу
```
GET /api/extra-services/{id}
```

**Ответ:** `200 OK`

---

#### Создать дополнительную услугу
```
POST /api/extra-services
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Тело запроса:**
```json
{
  "nameRu": "Мытьё окон",
  "nameEn": "Window Cleaning",
  "descriptionRu": "Профессиональное мытьё окон",
  "descriptionEn": "Professional window cleaning",
  "price": 1500.00,
  "isActive": true
}
```

**Ответ:** `201 Created`

---

#### Обновить дополнительную услугу
```
PUT /api/extra-services/{id}
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `200 OK`

---

#### Удалить дополнительную услугу
```
DELETE /api/extra-services/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

## 🛒 Orders Context ⭐ (Ядро системы)

### Заказы

#### Список заказов
```
GET /api/orders
Authorization: Bearer {token}
```

**Фильтрация по роли:**
- **Client** — видит только свои заказы (где clientId = userId)
- **Cleaner** — видит заказы, назначенные на него (где cleanerId = userId)
- **Manager, Admin** — видят все заказы

**Query параметры:**
- `?status=New,Assigned` — фильтр по статусу
- `?clientId=123` — фильтр по клиенту
- `?cleanerId=456` — фильтр по уборщику
- `?from=2024-01-01&to=2024-12-31` — фильтр по дате
- `?sortBy=scheduledDate&order=desc` — сортировка

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "clientId": 10,
      "clientName": "Иван Иванов",
      "cleanerId": 20,
      "cleanerName": "Пётр Петров",
      "serviceId": 1,
      "serviceName": "Уборка квартиры",
      "scheduledDate": "2024-12-15",
      "timeSlot": "10:00-12:00",
      "status": "Assigned",
      "totalPrice": 7500.00,
      "address": {
        "street": "ул. Абая",
        "building": "10",
        "apartment": "25",
        "city": "Almaty"
      }
    }
  ],
  "pagination": { ... }
}
```

---

#### Мои заказы
```
GET /api/orders/my
Authorization: Bearer {token}
Roles: Client, Cleaner
```

**Ответ:** `200 OK` (список заказов текущего пользователя)

---

#### Получить заказ по ID
```
GET /api/orders/{id}
Authorization: Bearer {token}
```

**Доступ:**
- **Client** — только свои заказы
- **Cleaner** — заказы, назначенные на него
- **Manager, Admin** — все заказы

**Ответ:** `200 OK`
```json
{
  "id": 1,
  "client": {
    "id": 10,
    "firstName": "Иван",
    "lastName": "Иванов",
    "phone": "+77001234567"
  },
  "cleaner": {
    "id": 20,
    "firstName": "Пётр",
    "lastName": "Петров",
    "phone": "+77007654321"
  },
  "service": {
    "id": 1,
    "name": "Уборка квартиры",
    "price": 7000.00
  },
  "extraServices": [
    {
      "id": 1,
      "name": "Мытьё окон",
      "quantity": 2,
      "price": 1500.00
    }
  ],
  "address": {
    "id": 5,
    "street": "ул. Абая",
    "building": "10",
    "apartment": "25",
    "city": "Almaty"
  },
  "timeSlot": {
    "id": 3,
    "startTime": "10:00",
    "endTime": "12:00"
  },
  "scheduledDate": "2024-12-15",
  "status": "Assigned",
  "totalPrice": 10000.00,
  "notes": "Ключи у консьержа",
  "createdAt": "2024-12-10T14:30:00Z"
}
```

---

#### Создать заказ
```
POST /api/orders
Authorization: Bearer {token}
Roles: Client
```

**Тело запроса:**
```json
{
  "serviceId": 1,
  "addressId": 5,
  "timeSlotId": 3,
  "scheduledDate": "2024-12-15",
  "areaSqm": 75,
  "extraServices": [
    {
      "extraServiceId": 1,
      "quantity": 2
    }
  ],
  "notes": "Ключи у консьержа"
}
```

**Ответ:** `201 Created`
```json
{
  "id": 1,
  "status": "New",
  "totalPrice": 10000.00,
  "createdAt": "2024-12-10T14:30:00Z"
}
```

---

#### Обновить заказ
```
PUT /api/orders/{id}
Authorization: Bearer {token}
Roles: Manager
```

**Тело запроса:**
```json
{
  "timeSlotId": 4,
  "scheduledDate": "2024-12-16",
  "notes": "Обновлённые комментарии"
}
```

**Ответ:** `200 OK`

---

#### Отменить заказ
```
DELETE /api/orders/{id}
Authorization: Bearer {token}
Roles: Client (только если status = New), Manager, Admin
```

**Ответ:** `204 No Content`

---

### Управление заказами

#### Изменить статус заказа
```
PUT /api/orders/{id}/status
Authorization: Bearer {token}
Roles: Manager, Cleaner
```

**Тело запроса:**
```json
{
  "status": "InProgress"
}
```

**Возможные переходы:**
- `New` → `Assigned` (Manager)
- `Assigned` → `InProgress` (Cleaner)
- `InProgress` → `Completed` (Cleaner)
- `New/Assigned` → `Cancelled` (Manager)

**Ответ:** `200 OK`

---

#### Назначить уборщика на заказ
```
POST /api/orders/{id}/assign
Authorization: Bearer {token}
Roles: Manager
```

**Тело запроса:**
```json
{
  "cleanerId": 20
}
```

**Ответ:** `200 OK`

---

#### Добавить дополнительные услуги
```
POST /api/orders/{id}/extra-services
Authorization: Bearer {token}
Roles: Client (если status = New), Manager
```

**Тело запроса:**
```json
{
  "extraServiceId": 2,
  "quantity": 1
}
```

**Ответ:** `201 Created`

---

#### Удалить дополнительную услугу из заказа
```
DELETE /api/orders/{id}/extra-services/{extraServiceId}
Authorization: Bearer {token}
Roles: Client (если status = New), Manager
```

**Ответ:** `204 No Content`

---

### Адреса

#### Список адресов клиента
```
GET /api/addresses
Authorization: Bearer {token}
Roles: Client
```

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "street": "ул. Абая",
      "building": "10",
      "apartment": "25",
      "city": "Almaty",
      "isDefault": true
    }
  ]
}
```

---

#### Получить адрес
```
GET /api/addresses/{id}
Authorization: Bearer {token}
Roles: Client
```

**Ответ:** `200 OK`

---

#### Добавить адрес
```
POST /api/addresses
Authorization: Bearer {token}
Roles: Client
```

**Тело запроса:**
```json
{
  "street": "ул. Абая",
  "building": "10",
  "apartment": "25",
  "entrance": "2",
  "floor": 5,
  "city": "Almaty",
  "postalCode": "050000",
  "isDefault": true
}
```

**Ответ:** `201 Created`

---

#### Обновить адрес
```
PUT /api/addresses/{id}
Authorization: Bearer {token}
Roles: Client
```

**Ответ:** `200 OK`

---

#### Удалить адрес
```
DELETE /api/addresses/{id}
Authorization: Bearer {token}
Roles: Client
```

**Ответ:** `204 No Content`

---

### Временные слоты

#### Список временных слотов
```
GET /api/time-slots
```

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "startTime": "08:00",
      "endTime": "10:00"
    },
    {
      "id": 2,
      "startTime": "10:00",
      "endTime": "12:00"
    }
  ]
}
```

---

#### Доступные слоты на дату
```
GET /api/time-slots/available?date=2024-12-15
```

**Ответ:** `200 OK`
```json
{
  "date": "2024-12-15",
  "slots": [
    {
      "id": 1,
      "startTime": "08:00",
      "endTime": "10:00",
      "isAvailable": true
    },
    {
      "id": 2,
      "startTime": "10:00",
      "endTime": "12:00",
      "isAvailable": false
    }
  ]
}
```

---

## 💳 Payment Context

### Платежи

#### Создать платёж (mock)
```
POST /api/payments
Authorization: Bearer {token}
Roles: Internal (вызывается автоматически после создания заказа)
```

**Тело запроса:**
```json
{
  "orderId": 1,
  "amount": 10000.00
}
```

**Ответ:** `201 Created`
```json
{
  "id": 1,
  "orderId": 1,
  "amount": 10000.00,
  "status": "Pending",
  "createdAt": "2024-12-10T14:30:00Z"
}
```

---

#### Получить платёж
```
GET /api/payments/{id}
Authorization: Bearer {token}
Roles: Client (свои заказы), Admin
```

**Ответ:** `200 OK`

---

#### Получить платёж по заказу
```
GET /api/payments/order/{orderId}
Authorization: Bearer {token}
Roles: Client (свои заказы), Admin
```

**Ответ:** `200 OK`

---

#### Обновить статус платежа (mock processor)
```
PUT /api/payments/{id}/status
Authorization: Bearer {token} (специальный токен для mock-процессора)
```

**Тело запроса:**
```json
{
  "status": "Success",
  "transactionId": "TXN123456"
}
```

**Ответ:** `200 OK`

---

## 📝 Content Context

### Страницы

#### Список страниц
```
GET /api/pages
```

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "slug": "about",
      "title": "О нас",
      "isPublished": true
    }
  ]
}
```

---

#### Получить страницу по slug
```
GET /api/pages/{slug}
```

**Ответ:** `200 OK`
```json
{
  "id": 1,
  "slug": "about",
  "title": "О нас",
  "content": "Мы — профессиональная компания...",
  "isPublished": true
}
```

---

#### Создать страницу
```
POST /api/pages
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Тело запроса:**
```json
{
  "slug": "contacts",
  "titleRu": "Контакты",
  "titleEn": "Contacts",
  "contentRu": "Наши контакты...",
  "contentEn": "Our contacts...",
  "isPublished": true
}
```

**Ответ:** `201 Created`

---

#### Обновить страницу
```
PUT /api/pages/{id}
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Ответ:** `200 OK`

---

#### Удалить страницу
```
DELETE /api/pages/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

### FAQ

#### Список FAQ
```
GET /api/faq
```

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "question": "Как заказать уборку?",
      "answer": "Зарегистрируйтесь на сайте...",
      "orderIndex": 1
    }
  ]
}
```

---

#### Получить FAQ
```
GET /api/faq/{id}
```

**Ответ:** `200 OK`

---

#### Создать FAQ
```
POST /api/faq
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Тело запроса:**
```json
{
  "questionRu": "Как заказать уборку?",
  "questionEn": "How to order cleaning?",
  "answerRu": "Зарегистрируйтесь на сайте...",
  "answerEn": "Register on the website...",
  "orderIndex": 1,
  "isPublished": true
}
```

**Ответ:** `201 Created`

---

#### Обновить FAQ
```
PUT /api/faq/{id}
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Ответ:** `200 OK`

---

#### Удалить FAQ
```
DELETE /api/faq/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

### Блог

#### Список статей блога
```
GET /api/blog
```

**Query параметры:**
- `?authorId=5` — фильтр по автору
- `?from=2024-01-01` — дата публикации от
- `?sortBy=publishedAt&order=desc` — сортировка

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "title": "10 советов по уборке",
      "authorName": "Анна Смирнова",
      "publishedAt": "2024-12-01T10:00:00Z"
    }
  ],
  "pagination": { ... }
}
```

---

#### Получить статью блога
```
GET /api/blog/{id}
```

**Ответ:** `200 OK`
```json
{
  "id": 1,
  "title": "10 советов по уборке",
  "content": "Полный текст статьи...",
  "author": {
    "id": 5,
    "firstName": "Анна",
    "lastName": "Смирнова"
  },
  "publishedAt": "2024-12-01T10:00:00Z",
  "createdAt": "2024-11-25T14:00:00Z"
}
```

---

#### Создать статью блога
```
POST /api/blog
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Тело запроса:**
```json
{
  "titleRu": "10 советов по уборке",
  "titleEn": "10 Cleaning Tips",
  "contentRu": "Полный текст статьи...",
  "contentEn": "Full article text...",
  "publishedAt": "2024-12-01T10:00:00Z"
}
```

**Ответ:** `201 Created`

---

#### Обновить статью блога
```
PUT /api/blog/{id}
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Ответ:** `200 OK`

---

#### Удалить статью блога
```
DELETE /api/blog/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

### Отзывы

#### Список отзывов
```
GET /api/reviews
```

**Query параметры:**
- `?orderId=123` — фильтр по заказу
- `?rating=5` — фильтр по рейтингу
- `?isPublished=true` — только опубликованные

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "orderId": 10,
      "userName": "Иван И.",
      "rating": 5,
      "comment": "Отличная уборка!",
      "createdAt": "2024-12-10T18:00:00Z",
      "isPublished": true
    }
  ],
  "pagination": { ... }
}
```

---

#### Получить отзыв
```
GET /api/reviews/{id}
```

**Ответ:** `200 OK`

---

#### Создать отзыв
```
POST /api/reviews
Authorization: Bearer {token}
Roles: Client
```

**Условие:** Можно оставить отзыв только на завершённый заказ (status = Completed)

**Тело запроса:**
```json
{
  "orderId": 10,
  "rating": 5,
  "commentRu": "Отличная уборка!",
  "commentEn": "Great cleaning!"
}
```

**Ответ:** `201 Created`

---

#### Модерировать отзыв
```
PUT /api/reviews/{id}/moderate
Authorization: Bearer {token}
Roles: Manager
```

**Тело запроса:**
```json
{
  "isPublished": true
}
```

**Ответ:** `200 OK`

---

#### Удалить отзыв
```
DELETE /api/reviews/{id}
Authorization: Bearer {token}
Roles: Admin
```

**Ответ:** `204 No Content`

---

### Запросы обратного звонка

#### Создать запрос обратного звонка
```
POST /api/callback-requests
Roles: Anonymous, Client
```

**Тело запроса:**
```json
{
  "name": "Иван Иванов",
  "phone": "+77001234567",
  "message": "Хочу узнать о ваших услугах"
}
```

**Ответ:** `201 Created`

---

#### Список запросов
```
GET /api/callback-requests
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Query параметры:**
- `?status=New` — фильтр по статусу
- `?from=2024-12-01` — дата создания от

**Ответ:** `200 OK`
```json
{
  "items": [
    {
      "id": 1,
      "name": "Иван Иванов",
      "phone": "+77001234567",
      "message": "Хочу узнать о ваших услугах",
      "status": "New",
      "createdAt": "2024-12-10T14:00:00Z"
    }
  ],
  "pagination": { ... }
}
```

---

#### Получить запрос
```
GET /api/callback-requests/{id}
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Ответ:** `200 OK`

---

#### Обновить статус запроса
```
PUT /api/callback-requests/{id}/status
Authorization: Bearer {token}
Roles: Manager
```

**Тело запроса:**
```json
{
  "status": "InProgress"
}
```

**Статусы:** `New`, `InProgress`, `Completed`

**Ответ:** `200 OK`

---

## 📧 Notifications Context

### Шаблоны уведомлений

#### Список шаблонов
```
GET /api/notifications/templates
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Ответ:** `200 OK`

---

#### Получить шаблон
```
GET /api/notifications/templates/{id}
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Ответ:** `200 OK`

---

#### Обновить шаблон
```
PUT /api/notifications/templates/{id}
Authorization: Bearer {token}
Roles: Manager, Admin
```

**Тело запроса:**
```json
{
  "subjectRu": "Ваш заказ создан",
  "subjectEn": "Your order has been created",
  "bodyRu": "Здравствуйте, {clientName}! Ваш заказ #{orderId}...",
  "bodyEn": "Hello, {clientName}! Your order #{orderId}..."
}
```

**Ответ:** `200 OK`

---

### История уведомлений

#### Список уведомлений
```
GET /api/notifications
Authorization: Bearer {token}
Roles: Admin
```

**Query параметры:**
- `?userId=123` — фильтр по пользователю
- `?status=Sent` — фильтр по статусу
- `?from=2024-12-01` — дата создания от

**Ответ:** `200 OK`

---

#### Отправить уведомление (внутренний endpoint)
```
POST /api/notifications/send
Authorization: Bearer {token}
Roles: Internal
```

**Тело запроса:**
```json
{
  "userId": 10,
  "templateId": 1,
  "variables": {
    "clientName": "Иван",
    "orderId": 123
  }
}
```

**Ответ:** `201 Created`

---

## 👨‍💼 Admin Context

### Статистика

#### Общая статистика
```
GET /api/admin/statistics
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `200 OK`
```json
{
  "totalOrders": 1523,
  "completedOrders": 1234,
  "activeOrders": 45,
  "totalRevenue": 15430000.00,
  "totalUsers": 567,
  "newUsersThisMonth": 23
}
```

---

#### Статистика заказов
```
GET /api/admin/statistics/orders
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Query параметры:**
- `?from=2024-01-01&to=2024-12-31` — период

**Ответ:** `200 OK`
```json
{
  "period": {
    "from": "2024-01-01",
    "to": "2024-12-31"
  },
  "totalOrders": 1523,
  "byStatus": {
    "New": 12,
    "Assigned": 33,
    "InProgress": 15,
    "Completed": 1234,
    "Cancelled": 229
  },
  "byMonth": [
    { "month": "2024-01", "count": 120 },
    { "month": "2024-02", "count": 135 }
  ]
}
```

---

#### Статистика выручки
```
GET /api/admin/statistics/revenue
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `200 OK`
```json
{
  "totalRevenue": 15430000.00,
  "byMonth": [
    { "month": "2024-01", "revenue": 1200000.00 },
    { "month": "2024-02", "revenue": 1350000.00 }
  ],
  "byService": [
    { "serviceId": 1, "serviceName": "Уборка квартиры", "revenue": 8500000.00 },
    { "serviceId": 2, "serviceName": "Генеральная уборка", "revenue": 4200000.00 }
  ]
}
```

---

#### Статистика пользователей
```
GET /api/admin/statistics/users
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Ответ:** `200 OK`
```json
{
  "totalUsers": 567,
  "byRole": {
    "Client": 450,
    "Cleaner": 100,
    "Manager": 15,
    "Admin": 2
  },
  "newUsersThisMonth": 23,
  "activeUsersThisMonth": 312
}
```

---

### Отчёты

#### Отчёт по заказам
```
GET /api/admin/reports/orders
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Query параметры:**
- `?from=2024-01-01&to=2024-12-31` — период
- `?format=json` или `?format=csv` — формат отчёта

**Ответ:** `200 OK` (JSON или CSV файл)

---

#### Отчёт по уборщикам
```
GET /api/admin/reports/cleaners
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Query параметры:**
- `?from=2024-01-01&to=2024-12-31` — период

**Ответ:** `200 OK`
```json
{
  "period": {
    "from": "2024-01-01",
    "to": "2024-12-31"
  },
  "cleaners": [
    {
      "cleanerId": 20,
      "cleanerName": "Пётр Петров",
      "totalOrders": 234,
      "completedOrders": 220,
      "averageRating": 4.8
    }
  ]
}
```

---

#### Отчёт по выручке
```
GET /api/admin/reports/revenue
Authorization: Bearer {token}
Roles: Admin, Manager
```

**Query параметры:**
- `?from=2024-01-01&to=2024-12-31` — период
- `?groupBy=month` или `?groupBy=service` — группировка

**Ответ:** `200 OK`

---

## 📊 Коды ответов

| Код | Название | Описание |
|-----|----------|----------|
| **200** | OK | Успешный запрос |
| **201** | Created | Ресурс создан |
| **204** | No Content | Успешно, нет контента |
| **400** | Bad Request | Ошибка валидации |
| **401** | Unauthorized | Не авторизован |
| **403** | Forbidden | Нет прав доступа |
| **404** | Not Found | Ресурс не найден |
| **409** | Conflict | Конфликт (например, email уже существует) |
| **422** | Unprocessable Entity | Ошибка бизнес-логики |
| **500** | Internal Server Error | Внутренняя ошибка сервера |

---

## 📝 Примеры запросов

### Пример 1: Создание заказа (полный flow)

#### Шаг 1: Регистрация
```bash
curl -X POST https://api.cleaning-service.com/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "client@example.com",
    "password": "SecurePass123!",
    "firstName": "Иван",
    "lastName": "Иванов",
    "phone": "+77001234567",
    "role": "Client"
  }'
```

---

#### Шаг 2: Вход
```bash
curl -X POST https://api.cleaning-service.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "client@example.com",
    "password": "SecurePass123!"
  }'

# Ответ:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6...",
  "expiresIn": 900
}
```

---

#### Шаг 3: Добавить адрес
```bash
curl -X POST https://api.cleaning-service.com/api/addresses \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "street": "ул. Абая",
    "building": "10",
    "apartment": "25",
    "city": "Almaty"
  }'

# Ответ:
{
  "id": 5
}
```

---

#### Шаг 4: Получить список услуг
```bash
curl -X GET https://api.cleaning-service.com/api/services \
  -H "Accept-Language: ru"
```

---

#### Шаг 5: Создать заказ
```bash
curl -X POST https://api.cleaning-service.com/api/orders \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "serviceId": 1,
    "addressId": 5,
    "timeSlotId": 3,
    "scheduledDate": "2024-12-15",
    "areaSqm": 75
  }'

# Ответ:
{
  "id": 123,
  "status": "New",
  "totalPrice": 7000.00
}
```

---

### Пример 2: Получение заказов с фильтрацией

```bash
curl -X GET "https://api.cleaning-service.com/api/orders?status=Completed&from=2024-01-01&sortBy=scheduledDate&order=desc&page=1&pageSize=20" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

---

### Пример 3: Обновление статуса заказа

```bash
curl -X PUT https://api.cleaning-service.com/api/orders/123/status \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "status": "InProgress"
  }'
```

---

## ✅ Критерии готовности

- [x] Все endpoints перечислены по контекстам
- [x] Определены HTTP-методы
- [x] Структура URL логичная и RESTful
- [x] Указаны роли доступа
- [x] Описаны query-параметры (пагинация, фильтрация, сортировка)
- [x] Приведены примеры запросов и ответов
- [x] Документ структурирован и понятен

---

**Версия документа:** 1.0  
**Дата создания:** 2024-12-11  
**Авторы:** Developer 1, Developer 2  
**Статус:** Draft → Требует review
