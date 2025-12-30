namespace Domain.ValueObjects;

public class LocalizedString
{
    public string Ru { get; private set; }
    public string En { get; private set; }

    private LocalizedString()
    {
        Ru = null!;
        En = null!;
    }

    private LocalizedString(string ru, string en)
    {
        Ru = ru;
        En = en;
    }

    public static LocalizedString Create(string ru, string en)
    {
        if (string.IsNullOrWhiteSpace(ru))
            throw new ArgumentException("Russian value cannot be empty", nameof(ru));

        if (string.IsNullOrWhiteSpace(en))
            throw new ArgumentException("English value cannot be empty", nameof(en));

        return new LocalizedString(ru.Trim(), en.Trim());
    }

    public string Get(string language)
        => language switch
        {
            "ru" => Ru,
            "en" => En,
            _ => En
        };
}