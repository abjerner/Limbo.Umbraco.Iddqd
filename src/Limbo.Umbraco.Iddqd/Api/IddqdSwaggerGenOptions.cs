using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Skybrud.Essentials.Time;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Limbo.Umbraco.Iddqd.Api;

#pragma warning disable CS1591

public class IddqdSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {

    public void Configure(SwaggerGenOptions options) {

        options.SwaggerDoc(IddqdApiConstants.Alias, new OpenApiInfo {
            Title = IddqdApiConstants.Name,
            Version = "1.0"
        });

        options.OperationFilter<IddqdSecurityFilter>();

        //options.MapType<EssentialsTime>(() => new OpenApiSchema {
        //    Type = JsonSchemaType.String,
        //    Format = "date-time"
        //});

    }

}