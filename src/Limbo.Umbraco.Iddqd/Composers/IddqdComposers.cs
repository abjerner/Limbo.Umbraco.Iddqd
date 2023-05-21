using Limbo.Umbraco.Iddqd.ContentApps;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.Iddqd.Composers {

    /// <inheritdoc />
    public class IddqdComposer : IComposer {

        /// <inheritdoc />
        public void Compose(IUmbracoBuilder builder) {

            builder.ManifestFilters().Append<IddqdManifestFilter>();

            builder.ContentApps().Append<IddqdInfoContentAppFactory>();
            builder.ContentApps().Append<IddqdExamineContentAppFactory>();

        }

    }

}