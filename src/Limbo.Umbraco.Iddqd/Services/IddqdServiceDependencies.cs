using Examine;
using Umbraco.Cms.Core.Services;

namespace Limbo.Umbraco.Iddqd.Services;

#pragma warning disable CS1591
public class IddqdServiceDependencies {

    public IContentService ContentService { get; }

    public IContentTypeService ContentTypeService { get; }

    public IDataTypeService DataTypeService { get; }

    public IMediaService MediaService { get; }

    public IMediaTypeService MediaTypeService { get; }

    public IExamineManager ExamineManager { get; }

    public IddqdServiceDependencies(IContentService contentService, IContentTypeService contentTypeService, IDataTypeService dataTypeService, IMediaService mediaService, IMediaTypeService mediaTypeService, IExamineManager examineManager) {
        ContentService = contentService;
        ContentTypeService = contentTypeService;
        DataTypeService = dataTypeService;
        MediaService = mediaService;
        MediaTypeService = mediaTypeService;
        ExamineManager = examineManager;
    }

}