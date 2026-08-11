using System;
using System.Collections.Generic;
using Examine;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Umbraco.Examine;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Examine;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Services;

public class IddqdService {

    private readonly IServiceProvider _serviceProvider;
    private readonly IddqdServiceDependencies _dependencies;

    #region Properties

    protected IExamineManager ExamineManager => _dependencies.ExamineManager;

    #endregion

    #region Constructors

    public IddqdService(IServiceProvider serviceProvider, IddqdServiceDependencies dependencies) {
        _serviceProvider = serviceProvider;
        _dependencies = dependencies;
    }

    #endregion

    #region member methods

    public virtual IReadOnlyList<string> GetIndexNames(IContent content) {
        return [
            global::Umbraco.Cms.Core.Constants.UmbracoIndexes.ExternalIndexName,
            global::Umbraco.Cms.Core.Constants.UmbracoIndexes.InternalIndexName
        ];
    }

    public virtual IReadOnlyList<string> GetIndexNames(IMedia media) {

        List<string> indexNames = [
            global::Umbraco.Cms.Core.Constants.UmbracoIndexes.ExternalIndexName,
            global::Umbraco.Cms.Core.Constants.UmbracoIndexes.InternalIndexName
        ];

        if (ExamineManager.TryGetIndex(ExamineIndexes.PdfIndex, out _)) indexNames.Add(ExamineIndexes.PdfIndex);

        return indexNames;

    }

    public virtual IReadOnlyList<string> GetIndexNames(IMember member) {
        return [ global::Umbraco.Cms.Core.Constants.UmbracoIndexes.MembersIndexName ];
    }

    public void IndexContent(IIndex index, IContent content) {

        switch (index.Name) {

            case ExamineIndexes.ExternalIndex:
            case ExamineIndexes.InternalIndex:
                IEnumerable<ValueSet> valueSets = _serviceProvider.GetRequiredService<IValueSetBuilder<IContent>>().GetValueSets(content);
                index.IndexItems(valueSets);
                break;

            default:
                throw new Exception($"Unsupported index '{index.Name}'");

        }

    }

    public void IndexMedia(IIndex index, IMedia media) {

        switch (index.Name) {

            case ExamineIndexes.ExternalIndex:
            case ExamineIndexes.InternalIndex:
                IEnumerable<ValueSet> valueSets = _serviceProvider.GetRequiredService<IValueSetBuilder<IMedia>>().GetValueSets(media);
                index.IndexItems(valueSets);
                break;

            default:
                throw new Exception($"Unsupported index '{index.Name}'");

        }

    }

    public void IndexMember(IIndex index, IMember member) {

        switch (index.Name) {

            case ExamineIndexes.MembersIndex:
                IEnumerable<ValueSet> valueSets = _serviceProvider.GetRequiredService<IValueSetBuilder<IMember>>().GetValueSets(member);
                index.IndexItems(valueSets);
                break;

            default:
                throw new Exception($"Unsupported index '{index.Name}'");

        }

    }

    #endregion

}

public class TableFlipException : Exception {

    public TableFlipException() : base("(╯°□°）╯︵ ┻━┻") { }

    public TableFlipException(string message) : base("(╯°□°）╯︵ ┻━┻\r\n\r\n" + message) { }

    public TableFlipException(Exception? innerException) : base("(╯°□°）╯︵ ┻━┻", innerException) { }

    public TableFlipException(string message, Exception? innerException) : base("(╯°□°）╯︵ ┻━┻\r\n\r\n" + message, innerException) { }

}