using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Вопрос и ответ из раздела "Часто задаваемые вопросы".
/// Поддерживает три языка: русский, казахский, английский.
/// </summary>
public class FAQ : BaseEntity
{
    /// <summary>Текст вопроса на русском языке.</summary>
    public string QuestionRu { get; private set; } = default!;

    /// <summary>Текст вопроса на казахском языке.</summary>
    public string QuestionKk { get; private set; } = default!;

    /// <summary>Текст вопроса на английском языке.</summary>
    public string QuestionEn { get; private set; } = default!;

    /// <summary>Текст ответа на русском языке.</summary>
    public string AnswerRu { get; private set; } = default!;

    /// <summary>Текст ответа на казахском языке.</summary>
    public string AnswerKk { get; private set; } = default!;

    /// <summary>Текст ответа на английском языке.</summary>
    public string AnswerEn { get; private set; } = default!;

    /// <summary>Порядок отображения. Меньшее значение — выше в списке.</summary>
    public int SortOrder { get; private set; }

    /// <summary>Признак активности. Неактивные записи не показываются на сайте.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Дата и время создания записи (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Дата и время последнего обновления (UTC). Null — если не обновлялась.</summary>
    public DateTime? UpdatedAt { get; private set; }

    // Конструктор для EF Core
    private FAQ() { }

    /// <summary>
    /// Создать новую запись FAQ.
    /// </summary>
    public static FAQ Create(
        string questionRu,
        string questionKk,
        string questionEn,
        string answerRu,
        string answerKk,
        string answerEn,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(questionRu))
            throw new ArgumentException("Вопрос на русском не может быть пустым.", nameof(questionRu));

        if (string.IsNullOrWhiteSpace(questionKk))
            throw new ArgumentException("Вопрос на казахском не может быть пустым.", nameof(questionKk));

        if (string.IsNullOrWhiteSpace(questionEn))
            throw new ArgumentException("Вопрос на английском не может быть пустым.", nameof(questionEn));

        if (string.IsNullOrWhiteSpace(answerRu))
            throw new ArgumentException("Ответ на русском не может быть пустым.", nameof(answerRu));

        if (string.IsNullOrWhiteSpace(answerKk))
            throw new ArgumentException("Ответ на казахском не может быть пустым.", nameof(answerKk));

        if (string.IsNullOrWhiteSpace(answerEn))
            throw new ArgumentException("Ответ на английском не может быть пустым.", nameof(answerEn));

        return new FAQ
        {
            QuestionRu = questionRu.Trim(),
            QuestionKk = questionKk.Trim(),
            QuestionEn = questionEn.Trim(),
            AnswerRu = answerRu.Trim(),
            AnswerKk = answerKk.Trim(),
            AnswerEn = answerEn.Trim(),
            SortOrder = sortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Обновить тексты вопроса и ответа.
    /// </summary>
    public void Update(
        string questionRu,
        string questionKk,
        string questionEn,
        string answerRu,
        string answerKk,
        string answerEn,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(questionRu))
            throw new ArgumentException("Вопрос на русском не может быть пустым.", nameof(questionRu));

        if (string.IsNullOrWhiteSpace(questionKk))
            throw new ArgumentException("Вопрос на казахском не может быть пустым.", nameof(questionKk));

        if (string.IsNullOrWhiteSpace(questionEn))
            throw new ArgumentException("Вопрос на английском не может быть пустым.", nameof(questionEn));

        if (string.IsNullOrWhiteSpace(answerRu))
            throw new ArgumentException("Ответ на русском не может быть пустым.", nameof(answerRu));

        if (string.IsNullOrWhiteSpace(answerKk))
            throw new ArgumentException("Ответ на казахском не может быть пустым.", nameof(answerKk));

        if (string.IsNullOrWhiteSpace(answerEn))
            throw new ArgumentException("Ответ на английском не может быть пустым.", nameof(answerEn));

        QuestionRu = questionRu.Trim();
        QuestionKk = questionKk.Trim();
        QuestionEn = questionEn.Trim();
        AnswerRu = answerRu.Trim();
        AnswerKk = answerKk.Trim();
        AnswerEn = answerEn.Trim();
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Деактивировать запись — она перестанет отображаться на сайте.</summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Активировать запись.</summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
