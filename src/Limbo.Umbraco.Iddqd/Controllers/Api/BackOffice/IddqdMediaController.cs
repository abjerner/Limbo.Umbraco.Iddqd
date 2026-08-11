using System;
using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Examine;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Models.Examine;
using Limbo.Umbraco.Iddqd.Models.Media;
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
[VersionedApiBackOfficeRoute($"{IddqdApiConstants.Route}/media")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Media")]
public class IddqdMediaController : Controller {

    private readonly IMediaService _mediaService;
    private readonly IExamineManager _examineManager;
    private readonly IddqdService _iddqdService;

    #region Constructors

    public IddqdMediaController(IMediaService mediaService, IExamineManager examineManager, IddqdService iddqdService) {
        _mediaService = mediaService;
        _examineManager = examineManager;
        _iddqdService = iddqdService;
    }

    #endregion

    #region Public API methods

    [HttpGet("{key}")]
    public ActionResult<IddqdMediaResult> GetMedia(Guid key) {

        // Get the media item - if it doesn't exist, return 404
        IMedia? media = _mediaService.GetById(key);
        if (media == null) return NotFound();

        // Initialize a new media result
        IddqdMediaResult result =  new() {
            Id = media.Id,
            Key = media.Key,
            Name = media.Name ?? string.Empty,
            CreateDate = media.CreateDate,
            UpdateDate = media.UpdateDate
        };

        // Return the media item
        return Ok(result);

    }

    [HttpGet("{key}/examine")]
    public ActionResult<IReadOnlyList<IddqdIndexResult>> GetMediaExamine(Guid key) {

        // Get the media item - if it doesn't exist, return 404
        IMedia? media = _mediaService.GetById(key);
        if (media == null) return NotFound($"Media with key '{key}' not found.");

        // Get the index names for the media item
        IReadOnlyList<string> indexNames = _iddqdService.GetIndexNames(media);

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
    [HttpPost("{key}/examine/indexes/{indexName}/reindex")]
    public ActionResult<IReadOnlyList<IddqdIndexResult>> ReIndexMedia(Guid key, string indexName) {

        // Get the media item - if it doesn't exist, return 404
        IMedia? media = _mediaService.GetById(key);
        if (media == null) return NotFound($"Media with key '{key}' not found.");

        // Get the index - if it doesn't exist, return 404
        if (!_examineManager.TryGetIndex(indexName, out IIndex index)) return NotFound($"Index with name '{indexName}' not found.");

        // Add or update the media item in the index
        _iddqdService.IndexMedia(index, media);

        // Return the examine result for the media item after re-indexing
        return GetMediaExamine(key);

    }

    #endregion

}