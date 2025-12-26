using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!)
        ),

        // Убираем стандартный допуск времени (по умолчанию 5 минут)
        // Это делает проверку срока действия более строгой
        ClockSkew = TimeSpan.Zero
    };
});


// Добавляем поддержку авторизации (политики, роли и т.д.)
builder.Services.AddAuthorization();


//
// 3️⃣ СЕРВИСЫ API
//

// Регистрируем MVC контроллеры
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

// Маппинг контроллеров
app.MapControllers();

// Запуск приложения
app.Run();