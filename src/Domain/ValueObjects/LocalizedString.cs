namespace Domain.ValueObjects;

/// <summary>
/// Value Object для хранения локализованного текста на трёх языках: RU, KK, EN.
/// </summary>
public class LocalizedString
{
    public string Ru { get; private set; }
    public string Kk { get; private set; }
    public string En { get; private set; }

    private LocalizedString()
    {
        Ru = null!;
        Kk = null!;
        En = null!;
    }

    private LocalizedString(string ru, string kk, string en)
    {
        Ru = ru;
        Kk = kk;
        En = en;
    }

    /// <summary>Создать локализованную строку с текстом на двух языках (KK = RU по умолчанию).</summary>
    public static LocalizedString Create(string ru, string en)
        => Create(ru, ru, en);

    /// <summary>Создать локализованную строку с текстом на трёх языках.</summary>
    public static LocalizedString Create(string ru, string kk, string en)
    {
        if (string.IsNullOrWhiteSpace(ru))
            throw new ArgumentException("Русский текст не может быть пустым.", nameof(ru));

        if (string.IsNullOrWhiteSpace(kk))
            throw new ArgumentException("Казахский текст не может быть пустым.", nameof(kk));

        if (string.IsNullOrWhiteSpace(en))
            throw new ArgumentException("Английский текст не может быть пустым.", nameof(en));

        return new LocalizedString(ru.Trim(), kk.Trim(), en.Trim());
    }

    /// <summary>Получить текст на указанном языке. Если язык не найден — возвращает русский.</summary>
    public string Get(string language)
        => language switch
        {
            "ru" => Ru,
            "kk" => Kk,
            "en" => En,
            _ => Ru
        };
}
