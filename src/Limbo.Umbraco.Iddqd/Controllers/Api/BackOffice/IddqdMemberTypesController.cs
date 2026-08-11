using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Helpers;
using Limbo.Umbraco.Iddqd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice;

[ApiController]
[VersionedApiBackOfficeRoute($"{IddqdApiConstants.Route}/member-types")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Member Types")]
public class IddqdMemberTypesController : Controller {

    private readonly IMemberTypeService _memberTypeService;
    private readonly IddqdRequestHelper _requestHelper;
    private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;

    #region Constructors

    public IddqdMemberTypesController(IMemberTypeService memberTypeService, IddqdRequestHelper requestHelper, IPublishedContentTypeFactory publishedContentTypeFactory) {
        _memberTypeService = memberTypeService;
        _requestHelper = requestHelper;
        _publishedContentTypeFactory = publishedContentTypeFactory;
    }

    #endregion

    #region Public API methods

    [HttpGet("{key}")]
    public async Task<ActionResult<object>> GetMemberType(Guid key) {

        IMemberType? memberType = await _memberTypeService.GetAsync(key);
        if (memberType == null) return NotFound();

        return new {
            memberType.Id,
            memberType.Key,
            memberType.Alias,
            memberType.Icon,
            memberType.Name,
            memberType.CreateDate,
            memberType.UpdateDate,
            propertyGroups = memberType.CompositionPropertyGroups.Select(x => new {
                x.Id,
                x.Key,
                x.Alias,
                x.Name,
                x.SortOrder,
                type = x.Type.ToString(),
                propertyTypes = memberType.PropertyTypes.Select(y => new {
                    y.Id,
                    y.Key,
                    y.Alias,
                    y.Name,
                    y.SortOrder,
                    y.PropertyEditorAlias,
                    dataType = _requestHelper.GetDataType(y.DataTypeKey),
                    clrType = GetClrType(memberType, y)
                })
            })
        };

    }

    #endregion
    private IddqdType GetClrType(IContentTypeComposition ownerContentType, IPropertyType propertyType) {
        IPublishedContentType publishedContentType = _publishedContentTypeFactory.CreateContentType(ownerContentType);
        IPublishedPropertyType publishedPropertyType = _publishedContentTypeFactory.CreatePropertyType(publishedContentType, propertyType);
        return new IddqdType(publishedPropertyType.ClrType ?? typeof(object));
    }

}