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
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите JWT токен в формате: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
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

// Перенаправление HTTP → HTTPS
app.UseHttpsRedirection();

// Подключаем аутентификацию
// ⚠️ ДОЛЖНО БЫТЬ ДО UseAuthorization
app.UseAuthentication();

// Подключаем авторизацию
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Маппинг контроллеров
app.MapControllers();

// Запуск приложения
app.Run();