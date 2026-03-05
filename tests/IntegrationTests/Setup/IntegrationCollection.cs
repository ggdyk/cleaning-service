namespace IntegrationTests.Setup;

/// <summary>
/// Определяет xUnit-коллекцию "Integration".
/// Все тест-классы с атрибутом [Collection("Integration")] разделяют один экземпляр
/// IntegrationWebApplicationFactory (и, следовательно, одну SQLite in-memory БД).
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<IntegrationWebApplicationFactory>
{
    // Этот класс — только маркер. Код сюда не добавляется.
}
