using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Conditions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.EntryPoints;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Localization;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Workspaces;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd;

public class IddqdPackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string alias = IddqdPackage.Alias;
        string cacheBuster = IddqdPackage.InformationalVersion.ToMd5Hash();

        List<PackageManifest> temp = [
            new() {
                Id = IddqdPackage.Alias,
                Name = IddqdPackage.Name,
                AllowTelemetry = true,
                Version = IddqdPackage.InformationalVersion,
                Extensions = [
                    new BackofficeEntryPointExtension {
                        Name = "limbo.iddqd.entrypoint",
                        Alias = "Limbo.Umbraco.Redirects.EntryPoint",
                        Js = $"/App_Plugins/{alias}/EntryPoint.js?v={cacheBuster}"
                    },
                    //new DashboardExtension {
                    //    Name = "Iddqd",
                    //    Alias = "Limbo.Umbraco.Iddqd.Dashboard",
                    //    ElementName = "limbo-iddqd-dashboard",
                    //    Js = $"/App_Plugins/{alias}/Elements/Dashboard.js?v={cacheBuster}",
                    //    Weight = -10,
                    //    Meta = new DashboardExtensionMeta {
                    //        Label = "Iddqd",
                    //        PathName = "iddqd",
                    //    }
                    //},
                    CreateContentApp("Document", alias, cacheBuster, WorkspaceCondition.Document, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/Content.js?v={cacheBuster}"),
                    CreateContentApp("DocumentType", alias, cacheBuster, WorkspaceCondition.DocumentType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/ContentType.js?v={cacheBuster}"),
                    CreateContentApp("DataType", alias, cacheBuster, WorkspaceCondition.DataType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/DataType.js?v={cacheBuster}"),
                    CreateContentApp("Media", alias, cacheBuster, new WorkspaceCondition("Umb.Workspace.Media"), js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/Media.js?v={cacheBuster}"),
                    CreateContentApp("MediaType", alias, cacheBuster, WorkspaceCondition.MediaType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/MediaType.js?v={cacheBuster}"),
                    CreateContentApp("Member", alias, cacheBuster, WorkspaceCondition.Member, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/Member.js?v={cacheBuster}"),
                    CreateContentApp("MemberType", alias, cacheBuster, WorkspaceCondition.MemberType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/MemberType.js?v={cacheBuster}"),
                    CreateContentApp("User", alias, cacheBuster, WorkspaceCondition.User, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/User.js?v={cacheBuster}"),
                    new EntityUserPermissionExtension {
	                    Alias = "My.UserPermission.Document.Iddqd",
	                    Name = "IDDQD document permission",
	                    ForEntityTypes = ["limbo"],
	                    Meta = new EntityUserPermissionExtensionMeta {
                            Verbs = ["My.Document.Iddqd"],
		                    Label = "Workspace View / Content App",
		                    Description = "Allows access to the IDDQD workspace view / content app.",
		                    Group = "limboIddqd",
	                    },
                    },
                    new LocalizationExtension {
                        Alias = $"{alias}.Localize.EnUS",
                        Name = "English",
                        Meta = new LocalizationExtensionMeta {
                            Culture = "en",
                            Localizations = LocalizationExtensionLocalizations
                                .Create()
                                .Set("user", "permissionsEntityGroup_limbo", "Limbo Packages")
                                .Set("actionCategories", "limboIddqd", "Iddqd")
                        }
                    },
                    //new {
                    //    type = "repository",
                    //    alias = "MyCustomTree.Repository",
                    //    name =  "My Custom Tree Repository",
                    //    api = $"/App_Plugins/{alias}/Tree/Repository.js?v={cacheBuster}"
                    //},
                    //new {
                    //    type = "tree",
                    //    alias = "MyCustomTree.Tree",
                    //    name = "My Custom Tree",
                    //    meta = new {
                    //        repositoryAlias = "MyCustomTree.Repository"
                    //    }
                    //}
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        {"@limbo/iddqd/auth", $"/App_Plugins/{alias}/IddqdAuth.js?v={cacheBuster}"},
                        {"@limbo/iddqd/package", $"/App_Plugins/{alias}/IddqdPackage.js?{cacheBuster}"},
                        {"@limbo/iddqd/service", $"/App_Plugins/{alias}/IddqdService.js?v={cacheBuster}"},
                        {"@limbo/iddqd/http", $"/App_Plugins/{alias}/IddqdHttpClient.js?v={cacheBuster}"},
                        {"@limbo/iddqd/services/content", $"/App_Plugins/{alias}/Services/Content.js?v={cacheBuster}"},
                        {"@limbo/iddqd/services/media", $"/App_Plugins/{alias}/Services/Media.js?v={cacheBuster}"},
                        {"@limbo/iddqd/services/media-types", $"/App_Plugins/{alias}/Services/MediaTypes.js?v={cacheBuster}"},
                        {"@limbo/iddqd/services/members", $"/App_Plugins/{alias}/Services/Members.js?v={cacheBuster}"},
                        {"@limbo/iddqd/elements/workspace-views/base", $"/App_Plugins/{alias}/Elements/WorkspaceViews/Base.js?v={cacheBuster}"}
                    }
                }
            }

        ];

        return await Task.FromResult(temp);

    }

    private WorkspaceViewExtension CreateContentApp(string type, string alias, string cacheBuster, WorkspaceCondition condition, string? js = null) {
        return new WorkspaceViewExtension {
            Alias = $"Limbo.Umbraco.Iddqd.{type}ContentApp",
            Name = "Iddqd",
            ElementName = "limbo-iddqd-content-app",
            Js = js ?? $"/App_Plugins/{alias}/Elements/ContentApp.js?v={cacheBuster}",
            Meta = new WorkspaceViewExtensionMeta {
                Label = "Iddqd",
                PathName = "iddqd",
                Icon = "icon-lab"
            },
            Conditions = [ condition/*, UserGroupCondition.Admin*/ ]
        };
    }

}