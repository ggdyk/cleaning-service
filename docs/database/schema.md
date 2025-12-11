# Database Schema (PostgreSQL)

Документ описывает структуру базы данных проекта **Cleaning Platform**, построенной на принципах Clean Architecture и ориентированной на масштабирование.

---

# 1. Таблица users (Пользователи)

### Назначение
Хранение клиентов и администраторов платформы.

| Поле           | Тип             | Ограничения                      | Описание |
|----------------|-----------------|----------------------------------|----------|
| id             | UUID            | PRIMARY KEY                      | Идентификатор пользователя |
| email          | TEXT            | NOT NULL, UNIQUE                 | Email (логин) |
| password_hash  | TEXT            | NOT NULL                         | Хэш пароля |
| first_name     | TEXT            | NOT NULL                         | Имя |
| last_name      | TEXT            | NOT NULL                         | Фамилия |
| phone          | TEXT            | NOT NULL                         | Телефон |
| role           | TEXT            | NOT NULL                         | client / admin |
| registered_at  | TIMESTAMPTZ     | NOT NULL DEFAULT now()           | Дата регистрации |
| is_active      | BOOLEAN         | NOT NULL DEFAULT true            | Признак активности |
| city           | TEXT            | NULL                             | Город |

**Индексы:**

```sql
CREATE UNIQUE INDEX idx_users_email ON users(email);


⸻

2. Таблица cities (Города)

Назначение

Справочник городов присутствия сервиса.

Поле	Тип	Ограничения	Описание
id	UUID	PRIMARY KEY	Идентификатор
name	JSONB	NOT NULL	Мультиязычное название
is_active	BOOLEAN	NOT NULL DEFAULT true	Активен ли город
sort_order	INT	NOT NULL DEFAULT 0	Порядок сортировки

Индексы:

CREATE INDEX idx_cities_active ON cities(is_active);


⸻

3. Таблица categories (Категории услуг)

Назначение

Категории клининговых услуг: уборка квартир, офисов и т. д.

Поле	Тип	Ограничения	Описание
id	UUID	PRIMARY KEY	Идентификатор
name	JSONB	NOT NULL	Мультиязычное название
description	JSONB	NOT NULL	Мультиязычное описание
icon_url	TEXT	NOT NULL	Иконка
sort_order	INT	NOT NULL DEFAULT 0	Порядок
is_active	BOOLEAN	NOT NULL DEFAULT true	Активность

Индексы:

CREATE INDEX idx_categories_active ON categories(is_active);


⸻

4. Таблица services (Основные услуги)

Назначение

Услуги, назначаемые пользователю — генеральная уборка, после ремонта и т. д.

Поле	Тип	Ограничения	Описание
id	UUID	PRIMARY KEY	Идентификатор
category_id	UUID	NOT NULL REFERENCES categories(id)	Категория
name	JSONB	NOT NULL	Мультиязычное название
description	JSONB	NOT NULL	Описание
base_price	NUMERIC(10,2)	NOT NULL	Цена
unit	TEXT	NOT NULL	Единица измерения
min_area	DOUBLE PRECISION	NULL	Минимальная площадь
duration_minutes	INT	NULL	Примерная длительность
sort_order	INT	NOT NULL DEFAULT 0	Порядок
is_active	BOOLEAN	NOT NULL DEFAULT true	Активность

Индексы:

CREATE INDEX idx_services_category ON services(category_id);
CREATE INDEX idx_services_active ON services(is_active);


⸻

5. Таблица extra_services (Дополнительные услуги)

Назначение

Дополнительные услуги: мойка окон, мебели, ковров.

Поле	Тип	Ограничения	Описание
id	UUID	PRIMARY KEY	Идентификатор
name	JSONB	NOT NULL	Мультиязычное название
description	JSONB	NOT NULL	Описание
price	NUMERIC(10,2)	NOT NULL	Цена
unit	TEXT	NOT NULL	Единица измерения
is_active	BOOLEAN	NOT NULL DEFAULT true	Активность


⸻

6. Таблица time_slots (Временные слоты)

Назначение

Возможные интервалы времени для записи на уборку.

Поле	Тип	Ограничения	Описание
id	UUID	PRIMARY KEY	Идентификатор
date	DATE	NOT NULL	Дата
start	TIME	NOT NULL	Начало
end	TIME	NOT NULL	Конец
city_id	UUID	NOT NULL REFERENCES cities(id)	Город
max_orders	INT	NOT NULL	Максимум заказов
current_orders	INT	NOT NULL DEFAULT 0	Текущее число
is_available	BOOLEAN	NOT NULL DEFAULT true	Доступен ли слот

Индексы:

CREATE INDEX idx_time_slots_city_date ON time_slots(city_id, date);
CREATE INDEX idx_time_slots_available ON time_slots(is_available);


⸻

7. Таблица orders (Заказы)

Назначение

Основная сущность — заказ уборки.

Поле	Тип	Ограничения	Описание
id	UUID	PRIMARY KEY	Идентификатор
order_number	TEXT	NOT NULL UNIQUE	Человекочитаемый номер
user_id	UUID	NOT NULL REFERENCES users(id)	Клиент
city_id	UUID	NOT NULL REFERENCES cities(id)	Город
time_slot_id	UUID	NOT NULL REFERENCES time_slots(id)	Временной слот

Адрес

Поле	Тип	Ограничения	Описание
street	TEXT	NOT NULL	Улица
house	TEXT	NOT NULL	Дом
apartment	TEXT	NULL	Квартира
entrance	TEXT	NULL	Подъезд
floor	TEXT	NULL	Этаж
door_code	TEXT	NULL	Домофон

Параметры заказа

Поле	Тип	Ограничения	Описание
area	DOUBLE PRECISION	NOT NULL	Площадь
bathrooms	INT	NOT NULL	Кол-во санузлов
comment	TEXT	NULL	Комментарий
status	TEXT	NOT NULL DEFAULT ‘new’	Статус заказа
total_price	NUMERIC(10,2)	NOT NULL	Итоговая цена
created_at	TIMESTAMPTZ	NOT NULL DEFAULT now()	Создан
updated_at	TIMESTAMPTZ	NOT NULL DEFAULT now()	Обновлён

Индексы:

CREATE UNIQUE INDEX idx_orders_number ON orders(order_number);
CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_orders_city ON orders(city_id);
CREATE INDEX idx_orders_timeslot ON orders(time_slot_id);


⸻

8. Таблица order_services (Услуги внутри заказа)

Назначение

Подчинённая сущность агрегата Order.

Поле	Тип	Ограничения	Описание
id	UUID	PRIMARY KEY	Идентификатор
order_id	UUID	NOT NULL REFERENCES orders(id) ON DELETE CASCADE	Заказ
service_id	UUID	NOT NULL	Идентификатор услуги
service_name	TEXT	NOT NULL	Название услуги копией
price	NUMERIC(10,2)	NOT NULL	Цена
quantity	DOUBLE PRECISION	NOT NULL	Количество

Индексы:

CREATE INDEX idx_order_services_order ON order_services(order_id);


⸻

ER-диаграмма

erDiagram
    USERS ||--o{ ORDERS : places
    CITIES ||--o{ TIME_SLOTS : has
    CITIES ||--o{ ORDERS : in
    CATEGORIES ||--o{ SERVICES : includes
    SERVICES ||--o{ ORDER_SERVICES : applied
    ORDERS ||--o{ ORDER_SERVICES : contains
