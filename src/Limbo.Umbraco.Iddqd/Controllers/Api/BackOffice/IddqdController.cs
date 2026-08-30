using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Helpers;
using Limbo.Umbraco.Iddqd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice;

[ApiController]
[VersionedApiBackOfficeRoute(IddqdApiConstants.Route)]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "General")]
public class IddqdController : Controller {

    private readonly IContentTypeService _contentTypeService;
    private readonly IMediaTypeService _mediaTypeService;
    private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
    private readonly IddqdRequestHelper _requestHelper;
    private readonly IUserService _userService;

    public IddqdController(IContentTypeService contentTypeService, IMediaTypeService mediaTypeService,
        IPublishedContentTypeFactory publishedContentTypeFactory, IddqdRequestHelper requestHelper, IUserService userService) {
        _contentTypeService = contentTypeService;
        _mediaTypeService = mediaTypeService;
        _publishedContentTypeFactory = publishedContentTypeFactory;
        _requestHelper = requestHelper;
        _userService = userService;
    }

    #region Public API methods

    [HttpGet]
    [Route("serverVariables")]
    public object GetServerVariables() {
        return new {
            version = IddqdPackage.InformationalVersion,
            cacheBuster = IddqdPackage.InformationalVersion.ToMd5Hash()
        };
    }

    [HttpGet("content-types/{key}")]
    public async Task<ActionResult<object>> GetContentType(Guid key) {

        IContentType? contentType = await _contentTypeService.GetAsync(key);
        if (contentType == null) return NotFound();

        return new {
            contentType.Id,
            contentType.Key,
            contentType.Alias,
            contentType.Icon,
            contentType.Name,
            contentType.CreateDate,
            contentType.UpdateDate,
            propertyGroups = contentType.CompositionPropertyGroups.Select(x => new {
                x.Id,
                x.Key,
                x.Alias,
                x.Name,
                x.SortOrder,
                type = x.Type.ToString(),
                propertyTypes = contentType.PropertyTypes.Select(y => new {
                    y.Id,
                    y.Key,
                    y.Alias,
                    y.Name,
                    y.SortOrder,
                    y.PropertyEditorAlias,
                    dataType = _requestHelper.GetDataType(y.DataTypeKey),
                    clrType = GetClrType(contentType, y)
                })
            })
        };

    }

    [HttpGet("media-types/{key}")]
    public async Task<ActionResult<object>> GetMediaType(Guid key) {

        IMediaType? mediaType = await _mediaTypeService.GetAsync(key);
        if (mediaType == null) return NotFound();

        return new {
            mediaType.Id,
            mediaType.Key,
            mediaType.Alias,
            mediaType.Icon,
            mediaType.Name,
            mediaType.CreateDate,
            mediaType.UpdateDate,
            propertyGroups = mediaType.CompositionPropertyGroups.Select(x => new {
                x.Id,
                x.Key,
                x.Alias,
                x.Name,
                x.SortOrder,
                type = x.Type.ToString(),
                propertyTypes = mediaType.PropertyTypes.Select(y => new {
                    y.Id,
                    y.Key,
                    y.Alias,
                    y.Name,
                    y.SortOrder,
                    y.PropertyEditorAlias,
                    dataType = _requestHelper.GetDataType(y.DataTypeKey),
                    clrType = GetClrType(mediaType, y)
                })
            })
        };

    }

    [HttpGet("users/{key}")]
    public async Task<ActionResult<object>> GetUser(Guid key) {

        IUser? user = await _userService.GetAsync(key);
        if (user == null) return NotFound();

        return new {
            user.Id,
            user.Key,
            user.Username,
            user.Email,
            user.Name,
            user.CreateDate,
            user.UpdateDate,
            userState = user.UserState.ToString(),
            user.Language,
            user.Groups
        };

    }

    #endregion

    private IddqdType GetClrType(IContentTypeComposition ownerContentType, IPropertyType propertyType) {
        IPublishedContentType publishedContentType = _publishedContentTypeFactory.CreateContentType(ownerContentType);
        IPublishedPropertyType publishedPropertyType = _publishedContentTypeFactory.CreatePropertyType(publishedContentType, propertyType);
        return new IddqdType(publishedPropertyType.ClrType ?? typeof(object));
    }

}