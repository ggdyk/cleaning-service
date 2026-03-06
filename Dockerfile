# =============================================================================
# Stage 1: BUILD
# Используем полный SDK-образ для компиляции и публикации
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем файлы проектов отдельно — это позволяет Docker кэшировать
# слой с восстановлением зависимостей (dotnet restore).
# Пересборка кэша произойдёт только при изменении .csproj-файлов.
COPY cleaning-service.sln ./
COPY src/Api/Api.csproj                         src/Api/
COPY src/Application/Application.csproj         src/Application/
COPY src/Domain/Domain.csproj                   src/Domain/
COPY src/Infrastructure/Infrastructure.csproj   src/Infrastructure/

RUN dotnet restore cleaning-service.sln

# Копируем весь исходный код и публикуем
COPY src/ src/

RUN dotnet publish src/Api/Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# =============================================================================
# Stage 2: RUNTIME
# Минимальный образ — только ASP.NET Runtime, без SDK (~100 MB vs ~800 MB)
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Создаём непривилегированного пользователя для запуска приложения
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

COPY --from=build /app/publish .

# Порт, на котором слушает ASP.NET Core внутри контейнера
EXPOSE 8080

# Переменные окружения по умолчанию (переопределяются при запуске контейнера)
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "Api.dll"]
