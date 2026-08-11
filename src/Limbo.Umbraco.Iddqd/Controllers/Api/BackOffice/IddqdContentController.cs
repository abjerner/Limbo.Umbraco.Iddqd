using System;
using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Examine;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Models;
using Limbo.Umbraco.Iddqd.Models.Examine;
using Limbo.Umbraco.Iddqd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice;

[ApiController]
[VersionedApiBackOfficeRoute($"{IddqdApiConstants.Route}/content")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Content")]
public class IddqdContentController : Controller {

    private readonly IContentService _contentService;
    private readonly IddqdService _iddqdService;
    private readonly IExamineManager _examineManager;

    #region Constructors

    public IddqdContentController(IContentService contentService, IddqdService iddqdService, IExamineManager examineManager) {
        _contentService = contentService;
        _iddqdService = iddqdService;
        _examineManager = examineManager;
    }

    #endregion

    #region Public API methods

    [HttpGet("{key}")]
    public ActionResult<IddqdContent> GetContent(Guid key) {

        // Get the content - if it doesn't exist, return 404
        IContent? content = _contentService.GetById(key);
        if (content == null) return NotFound($"Content with key '{key}' not found.");

        return new IddqdContent {
            Id = content.Id,
            Key = content.Key,
            Name = content.Name,
            CreateDate = content.CreateDate,
            UpdateDate = content.UpdateDate
        };

    }

    [HttpGet("{key}/examine")]
    public ActionResult<IReadOnlyList<IddqdIndexResult>> GetExamine(Guid key) {

        // Get the content - if it doesn't exist, return 404
        IContent? content = _contentService.GetById(key);
        if (content == null) return NotFound($"Content with key '{key}' not found.");

        // Get the index names for the content
        IReadOnlyList<string> indexNames = _iddqdService.GetIndexNames(content);

        // Attempt to get the examine result for the media item from each index
        List<IddqdIndexResult> temp = [];
        foreach (string indexName in indexNames) {
            _examineManager.TryGetIndex(indexName, out IIndex? index);
            ISearchResult? result = index?.Searcher.CreateQuery().NativeQuery($"__Key:'{key}'").Execute().FirstOrDefault();
            temp.Add(new IddqdIndexResult(indexName, result));
        }

        // Return the examine results for the media item
        return Ok(temp);

    }

    [HttpPost("{key}/examine/indexes/{indexName}")]
    public ActionResult<IReadOnlyList<IddqdIndexResult>> UpdateExamine(Guid key, string indexName) {

        // Get the content - if it doesn't exist, return 404
        IContent? content = _contentService.GetById(key);
        if (content == null) return NotFound($"Content with key '{key}' not found.");

        // Get the index - if it doesn't exist, return 404
        if (!_examineManager.TryGetIndex(indexName, out IIndex index)) return NotFound($"Index with name '{indexName}' not found.");

        // Add or update the content in the index
        _iddqdService.IndexContent(index, content);

        // Return the examine result for the content after re-indexing
        return GetExamine(key);

    }

    #endregion

}