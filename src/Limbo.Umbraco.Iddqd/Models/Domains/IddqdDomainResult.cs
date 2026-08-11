using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.Iddqd.Models.Domains;

public class IddqdDomainResult {

    public int Id { get; }

    public string Name { get; }

    public string NameAscii { get; }

    public int? RootContentId { get; }

    public IddqdDomainRootContent? RootContent { get; }

    public string? LanguageIsoCode { get; }

    public string? LanguageName { get; }

    public IddqdDomainResult(IDomain domain, IddqdDomainRootContent? content, ILanguage? language) {
        Id = domain.Id;
        Name = domain.DomainName;
        RootContentId = domain.RootContentId;
        RootContent = content;
        LanguageIsoCode = domain.LanguageIsoCode;
        LanguageName = language?.CultureName;
        if (Name.Contains("xn--")) {
            NameAscii = Name;
            Name = IddqdUtils.FromPunycode(Name);
        } else {
            NameAscii = Name;
        }
    }

}

public class IddqdDomainRootContent {

    public int Id { get; }

    public Guid Key { get; }

    public string Name { get; }

    public string EditUrl => $"/umbraco/section/content/workspace/document/edit/{Key}";

    public IddqdDomainRootContentType? ContentType { get; }

    public IddqdDomainRootContent(IPublishedContent content, IddqdDomainRootContentType? contentType) {
        ContentType = contentType;
        Id = content.Id;
        Key = content.Key;
        Name = content.Name;
    }

}

public class IddqdDomainRootContentType {

    public int Id { get; }

    public Guid Key { get; }

    public string Alias { get; }

    public string Name { get; }

    public string? Icon { get; }

    public IddqdDomainRootContentType(IContentType contentType) {
        Id = contentType.Id;
        Key = contentType.Key;
        Alias = contentType.Alias;
        Name = contentType.Name ?? string.Empty;
        Icon = contentType.Icon;
    }

}