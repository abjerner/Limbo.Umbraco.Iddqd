using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Models.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skybrud.Essentials.Threading;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice;

[ApiController]
[VersionedApiBackOfficeRoute($"{IddqdApiConstants.Route}/domains")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Domains")]
public class IddqdDomainsController : Controller {

    private readonly IDomainService _domainService;
    private readonly IContentTypeService _contentTypeService;
    private readonly ILanguageService _languageService;
    private readonly IPublishedContentQuery _publishedContentQuery;

    #region Constructors

    public IddqdDomainsController(IDomainService domainService, IContentTypeService contentTypeService, ILanguageService languageService, IPublishedContentQuery publishedContentQuery ) {
        _domainService = domainService;
        _contentTypeService = contentTypeService;
        _languageService = languageService;
        _publishedContentQuery = publishedContentQuery;
    }

    #endregion

    #region Public API methods

    [HttpGet]
    public async Task<ActionResult<object>> Index() {

        Dictionary<Guid, IContentType?> contentTypes = [];
        Dictionary<string, ILanguage?> languages = [];

        var items = await _domainService
            .GetAllAsync(true)
            .Then(x => x.Select(y => CreateDomainResult(y, contentTypes, languages)))
            .Then(x => x.ToList());

        return Ok(items);

    }

    #endregion

    private IddqdDomainResult CreateDomainResult(IDomain domain, Dictionary<Guid, IContentType?> contentTypes, Dictionary<string, ILanguage?> languages) {

        IPublishedContent? content = domain.RootContentId is null ? null : _publishedContentQuery.Content(domain.RootContentId.Value);

        if (content is null) return new IddqdDomainResult(domain, null, null);

        if (!contentTypes.TryGetValue(content.ContentType.Key, out IContentType? contentType)) {
            contentType = _contentTypeService.Get(content.ContentType.Key);
            contentTypes[content.ContentType.Key] = contentType;
        }

        ILanguage? language = null;
        if (!string.IsNullOrWhiteSpace(domain.LanguageIsoCode)) {
            if (!languages.TryGetValue(domain.LanguageIsoCode, out language)) {
                language = _languageService.GetAsync(domain.LanguageIsoCode).Result;
                languages[domain.LanguageIsoCode] = language;
            }
        }

        IddqdDomainRootContent rc = new(content, contentType is null ? null : new IddqdDomainRootContentType(contentType));

        return new IddqdDomainResult(domain, rc, language);

    }

}