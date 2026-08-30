using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Limbo.Umbraco.Iddqd.Api;
using Limbo.Umbraco.Iddqd.Factories;
using Limbo.Umbraco.Iddqd.Helpers;
using Limbo.Umbraco.Iddqd.Models.DataTypes;
using Limbo.Umbraco.Iddqd.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skybrud.Essentials.Threading;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;

namespace Limbo.Umbraco.Iddqd.Controllers.Api.BackOffice;

[ApiController]
[VersionedApiBackOfficeRoute(IddqdApiConstants.Routes.DataTypes)]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(IddqdApiConstants.Alias)]
[ApiVersion(IddqdApiConstants.Version)]
[ApiExplorerSettings(GroupName = IddqdApiConstants.GroupNames.DataTypes)]
public class IddqdDataTypesController : Controller {

    private readonly IDataTypeService _dataTypeService;
    private readonly IddqdHelper _helper;
    private readonly IddqdModelFactory _modelFactory;

    public IddqdDataTypesController(IDataTypeService dataTypeService, IddqdHelper helper, IddqdModelFactory modelFactory) {
        _dataTypeService = dataTypeService;
        _helper = helper;
        _modelFactory = modelFactory;
    }

    #region Public API methods

    [HttpGet("")]
    public async Task<ActionResult<IReadOnlyList<IddqdDataType>>> GetDataTypes() {
        return await _dataTypeService
            .GetAllAsync()
            .Then(x => x.Select(y => _modelFactory.CreateDataType(y, [])))
            .Then(x => x.ToList());
    }


    [HttpGet("{key}")]
    public async Task<ActionResult<IddqdDataType>> GetDataType(Guid key) {
        IDataType? dataType = await _dataTypeService.GetAsync(key);
        if (dataType == null) return NotFound();
        return _modelFactory.CreateDataType(dataType, []);
    }

    [HttpGet("{key}/relations")]
    public async Task<ActionResult<object>> GetDataTypeRelations(Guid key) {

        IDataType? dataType = await _dataTypeService.GetAsync(key);
        if (dataType == null) return NotFound();

        List<IddqdPropertyTypeDto> contentTypes = [];
        List<IddqdPropertyTypeDto> mediaTypes = [];
        List<IddqdPropertyTypeDto> memberTypes = [];

        foreach (IddqdPropertyTypeDto dto in _helper.GetPropertyTypes(dataType)) {
            switch (dto.NodeObjectType.ToString().ToUpper()) {
                case global::Umbraco.Cms.Core.Constants.ObjectTypes.Strings.DocumentType: contentTypes.Add(dto); break;
                case global::Umbraco.Cms.Core.Constants.ObjectTypes.Strings.MediaType: mediaTypes.Add(dto); break;
                case global::Umbraco.Cms.Core.Constants.ObjectTypes.Strings.MemberType: memberTypes.Add(dto); break;
            }
        }

        return new { contentTypes, mediaTypes, memberTypes };

    }

    #endregion

}