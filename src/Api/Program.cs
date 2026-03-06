using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Api.Middleware;
using Api.Authorization;

var builder = WebApplication.CreateBuilder(args);

//
// 1️⃣ РЕГИСТРАЦИЯ СЛОЁВ ПРИЛОЖЕНИЯ
//

// Регистрация Application layer
// Здесь регистрируются:
// - use cases
// - application services
// - интерфейсы, необходимые API
builder.Services.AddApplication();

// Регистрация Infrastructure layer
// Здесь подключаются:
// - DbContext (PostgreSQL + EF Core)
// - реализации репозиториев
// - JWT, хеширование паролей и т.д.
builder.Services.AddInfrastructure(builder.Configuration);


//
// 2️⃣ НАСТРОЙКА JWT АУТЕНТИФИКАЦИИ
//

// Добавляем механизм аутентификации в ASP.NET Core
builder.Services.AddAuthentication(options =>
{
    // Указываем, что по умолчанию используем JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Настраиваем JWT Bearer
.AddJwtBearer(options =>
{
    // Читаем секцию Jwt из appsettings.json
    var jwtSection = builder.Configuration.GetSection("Jwt");

    // Параметры валидации JWT токена
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Проверять издателя токена (Issuer)
        ValidateIssuer = true,

        // Проверять аудиторию (Audience)
        ValidateAudience = true,

        // Проверять срок действия токена
        ValidateLifetime = true,

        // Проверять подпись токена
        ValidateIssuerSigningKey = true,

        // Ожидаемый Issuer (кто выпустил токен)
        ValidIssuer = jwtSection["Issuer"],

        // Ожидаемая Audience (для кого токен)
        ValidAudience = jwtSection["Audience"],

        // Ключ для проверки подписи токена
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSection["Key"]!)
        ),

        // Убираем стандартный допуск времени (по умолчанию 5 минут)
        // Это делает проверку срока действия более строгой
        ClockSkew = TimeSpan.Zero
    };
});


// Добавляем поддержку авторизации с именованными политиками.
// Политики читают ClaimTypes.Role из JWT — значение приходит из UserRole enum.
builder.Services.AddAuthorization(options =>
{
    // Только Admin
    options.AddPolicy(Policies.AdminOnly, policy =>
        policy.RequireRole("Admin"));

    // Admin или Manager — управление контентом, модерация отзывов/FAQ
    options.AddPolicy(Policies.AdminOrManager, policy =>
        policy.RequireRole("Admin", "Manager"));

    // Только Cleaner — для endpoints уборщика
    options.AddPolicy(Policies.CleanerOnly, policy =>
        policy.RequireRole("Cleaner"));

    // Операционный персонал: Manager, Admin, Cleaner
    options.AddPolicy(Policies.Staff, policy =>
        policy.RequireRole("Admin", "Manager", "Cleaner"));
});


//
// 3️⃣ СЕРВИСЫ API
//

// Регистрируем MVC контроллеры
// JsonStringEnumConverter: enum-поля в JSON передаются строками ("Cancelled"), а не числами (4)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cleaning Service API",
        Version = "v1",
        Description = """
            REST API платформы для заказа клининговых услуг.

            **Аутентификация**: Bearer JWT.
            Получите токен через `POST /api/auth/login`, затем нажмите кнопку **Authorize** и введите `Bearer <ваш_токен>`.

            **Роли пользователей**:
            - `Client` — создаёт заказы, оставляет отзывы
            - `Cleaner` — видит назначенные заказы
            - `Manager` — управляет контентом, модерирует
            - `Admin` — полный доступ
            """
    });

    // Подключаем XML-комментарии из слоя Api и Application
    foreach (var xmlFile in new[] { "Api.xml", "Application.xml" })
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен в формате: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//
// 4️⃣ СБОРКА ПРИЛОЖЕНИЯ
//

var app = builder.Build();


//
// 5️⃣ HTTP PIPELINE (ПОСЛЕДОВАТЕЛЬНОСТЬ MIDDLEWARE)
//

// Swagger включаем только не в Production
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Перенаправление HTTP → HTTPS (отключено в тестовом окружении)
if (!app.Environment.IsEnvironment("Testing"))
    app.UseHttpsRedirection();

// Подключаем аутентификацию
// ⚠️ ДОЛЖНО БЫТЬ ДО UseAuthorization
// ⚠️ ExceptionHandlingMiddleware должен быть ДО Authentication/Authorization,
// чтобы перехватывать исключения из любого места пайплайна
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LanguageMiddleware>();

app.UseAuthentication();

// Подключаем авторизацию
app.UseAuthorization();

// Маппинг контроллеров
app.MapControllers();

// Запуск приложения
app.Run();

// Делаем класс Program доступным для интеграционных тестов через WebApplicationFactory
public partial class Program { }