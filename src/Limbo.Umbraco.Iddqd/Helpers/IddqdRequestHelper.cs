using System;
using System.Collections.Generic;
using Limbo.Umbraco.Iddqd.Models.DataTypes;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Limbo.Umbraco.Iddqd.Helpers;

public class IddqdRequestHelper {

    private readonly IDataTypeService _dataTypeService;

    private readonly Dictionary<Guid, IContentType?> _contentTypes = new();
    private readonly Dictionary<Guid, IddqdDataType?> _dataTypes = new();

    public IddqdRequestHelper(IDataTypeService dataTypeService) {
        _dataTypeService = dataTypeService;
    }

    public IddqdDataType? GetDataType(Guid key) {

        if (_dataTypes.TryGetValue(key, out var dataType)) {
            return dataType;
        }

        IDataType? dt = _dataTypeService.GetAsync(key).Result;
        _dataTypes[key] = dataType = dt is null ? null : new IddqdDataType(dt, []);

        return dataType;

    }

}