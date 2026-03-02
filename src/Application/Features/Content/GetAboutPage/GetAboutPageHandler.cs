using Application.Common;
using Application.DTOs.Content;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Content.GetAboutPage;

public class GetAboutPageHandler : IRequestHandler<GetAboutPageQuery, PageResponse>
{
    private const string AboutSlug = "about";

    private readonly IPageRepository _pageRepository;
    private readonly ILanguageContext _languageContext;

    public GetAboutPageHandler(IPageRepository pageRepository, ILanguageContext languageContext)
    {
        _pageRepository = pageRepository;
        _languageContext = languageContext;
    }

    public async Task<PageResponse> Handle(GetAboutPageQuery request, CancellationToken cancellationToken)
    {
        var page = await _pageRepository.GetBySlugAsync(AboutSlug, cancellationToken);

        if (page is null)
            throw new NotFoundException("Page", AboutSlug);

        var lang = _languageContext.Language;

        return new PageResponse
        {
            Slug = page.Slug,
            Title = LocalizationHelper.Pick(page.TitleRu, page.TitleKk, page.TitleEn, lang),
            Content = LocalizationHelper.Pick(page.ContentRu, page.ContentKk, page.ContentEn, lang),
            UpdatedAt = page.UpdatedAt
        };
    }
}
