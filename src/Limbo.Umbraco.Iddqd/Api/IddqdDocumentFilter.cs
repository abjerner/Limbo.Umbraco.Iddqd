using Microsoft.OpenApi;
using Skybrud.Essentials.Time;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Limbo.Umbraco.Iddqd.Api;

/// <summary>
/// This document filter is used to remove the <see cref="EssentialsTime"/> schema from the OpenAPI document for the
/// Iddqd API. This is necessary because the <see cref="EssentialsTime"/> type is represented as a string with a
/// date-time format, and we want to avoid including unnecessary object-like schema information in the generated
/// OpenAPI documentation. The filter only applies to the Iddqd API document.
/// </summary>
internal class IddqdDocumentFilter : IDocumentFilter {

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
        if (context.DocumentName != IddqdApiConstants.Alias) return;
        const string schemaKey = nameof(EssentialsTime);
        swaggerDoc.Components?.Schemas?.Remove(schemaKey);
    }

}