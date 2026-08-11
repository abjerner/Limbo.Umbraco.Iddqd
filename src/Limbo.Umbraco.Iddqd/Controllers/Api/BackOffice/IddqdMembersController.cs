using System;
using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Examine;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Models.Examine;
using Limbo.Umbraco.Iddqd.Models.Members;
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
[VersionedApiBackOfficeRoute($"{IddqdApiConstants.Route}/members")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Members")]
public class IddqdMembersController : Controller {

    private readonly IMemberService _memberService;
    private readonly IddqdService _iddqdService;
    private readonly IExamineManager _examineManager;

    #region Constructors

    public IddqdMembersController(IMemberService memberService, IddqdService iddqdService, IExamineManager examineManager) {
        _memberService = memberService;
        _iddqdService = iddqdService;
        _examineManager = examineManager;
    }

    #endregion

    #region Public API methods

    [HttpGet("{key}")]
    public ActionResult<IddqdMember> GetMember(Guid key) {

        IMember? member = _memberService.GetById(key);
        if (member == null) return NotFound();

        return new IddqdMember {
            Id = member.Id,
            Key = member.Key,
            Name = member.Name,
            Username = member.Username,
            Email = member.Email,
            IsApproved = member.IsApproved,
            IsLockedOut = member.IsLockedOut,
            CreateDate = member.CreateDate,
            UpdateDate = member.UpdateDate
        };

    }

    [HttpGet("{key}/examine")]
    public ActionResult<IReadOnlyList<IddqdIndexResult>> GetExamine(Guid key) {

        // Get the member - if it doesn't exist, return 404
        IMember? member = _memberService.GetById(key);
        if (member == null) return NotFound($"Member with key '{key}' not found.");

        // Get the index names for the member
        IReadOnlyList<string> indexNames = _iddqdService.GetIndexNames(member);

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
    [HttpPost("{key}/examine/indexes/{indexName}/index")]
    [HttpPost("{key}/examine/indexes/{indexName}/reindex")]
    public ActionResult<IReadOnlyList<IddqdIndexResult>> IndexMedia(Guid key, string indexName) {

        // Get the member - if it doesn't exist, return 404
        IMember? member = _memberService.GetById(key);
        if (member == null) return NotFound($"Member with key '{key}' not found.");

        // Get the index - if it doesn't exist, return 404
        if (!_examineManager.TryGetIndex(indexName, out IIndex index)) return NotFound($"Index with name '{indexName}' not found.");

        // Add or update the member in the index
        _iddqdService.IndexMember(index, member);

        // Return the examine result for the member after re-indexing
        return GetExamine(key);

    }

    #endregion


}