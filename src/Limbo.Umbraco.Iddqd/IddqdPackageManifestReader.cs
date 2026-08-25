using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Conditions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.EntryPoints;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Localization;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Menus;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Sections;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Users;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Workspaces;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd;

public class IddqdPackageManifestReader : IPackageManifestReader {

    public static string CacheBuster = IddqdPackage.InformationalVersion.ToMd5Hash();

    public const string Alias = IddqdPackage.Alias;

    public const string Name = IddqdPackage.Name;

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string alias = IddqdPackage.Alias;

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
                        Js = $"/App_Plugins/{alias}/EntryPoint.js?v={CacheBuster}"
                    },
                    ..GetSettingsTreeExtensions(),
                    CreateContentApp("Document", alias, CacheBuster, WorkspaceAliasCondition.Document, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/Content.js?v={CacheBuster}"),
                    CreateContentApp("DocumentType", alias, CacheBuster, WorkspaceAliasCondition.DocumentType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/ContentType.js?v={CacheBuster}"),
                    CreateContentApp("DataType", alias, CacheBuster, WorkspaceAliasCondition.DataType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/DataType.js?v={CacheBuster}"),
                    CreateContentApp("Media", alias, CacheBuster, new WorkspaceAliasCondition("Umb.Workspace.Media"), js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/Media.js?v={CacheBuster}"),
                    CreateContentApp("MediaType", alias, CacheBuster, WorkspaceAliasCondition.MediaType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/MediaType.js?v={CacheBuster}"),
                    CreateContentApp("Member", alias, CacheBuster, WorkspaceAliasCondition.Member, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/Member.js?v={CacheBuster}"),
                    CreateContentApp("MemberType", alias, CacheBuster, WorkspaceAliasCondition.MemberType, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/MemberType.js?v={CacheBuster}"),
                    CreateContentApp("User", alias, CacheBuster, WorkspaceAliasCondition.User, js: $"/App_Plugins/{alias}/Elements/WorkspaceViews/User.js?v={CacheBuster}"),
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
                            Localizations = LocalizationDictionary
                                .Create()
                                .Set("user", "permissionsEntityGroup_limbo", "Limbo Packages")
                                .Set("actionCategories", "limboIddqd", "Iddqd")
                        }
                    },
                    new IconsExtension {
                        Alias = $"{alias}.Icons",
                        Name = $"{Name}: Icons",
                        Js = $"/App_Plugins/{alias}/Icons/Icons.js?v={CacheBuster}"
                    }
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        {"@limbo/iddqd/auth", $"/App_Plugins/{alias}/IddqdAuth.js?v={CacheBuster}"},
                        {"@limbo/iddqd/package", $"/App_Plugins/{alias}/IddqdPackage.js?v={CacheBuster}"},
                        {"@limbo/iddqd/service", $"/App_Plugins/{alias}/IddqdService.js?v={CacheBuster}"},
                        {"@limbo/iddqd/http", $"/App_Plugins/{alias}/IddqdHttpClient.js?v={CacheBuster}"},
                        {"@limbo/iddqd/services/content", $"/App_Plugins/{alias}/Services/Content.js?v={CacheBuster}"},
                        {"@limbo/iddqd/services/media", $"/App_Plugins/{alias}/Services/Media.js?v={CacheBuster}"},
                        {"@limbo/iddqd/services/media-types", $"/App_Plugins/{alias}/Services/MediaTypes.js?v={CacheBuster}"},
                        {"@limbo/iddqd/services/members", $"/App_Plugins/{alias}/Services/Members.js?v={CacheBuster}"},
                        {"@limbo/iddqd/elements/workspace-views/base", $"/App_Plugins/{alias}/Elements/WorkspaceViews/Base.js?v={CacheBuster}"}
                    }
                }
            }

        ];

        return await Task.FromResult(temp);

    }

    private WorkspaceViewExtension CreateContentApp(string type, string alias, string cacheBuster, WorkspaceAliasCondition condition, string? js = null) {
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
            Conditions = [ condition, UserGroupCondition.Admin ]
        };
    }

    private static IEnumerable<IExtension> GetSettingsTreeExtensions() {

        MenuExtension menu = new() {
            Alias = $"{Alias}.Menu.SettingsGroup",
            Name = $"{Name}: Tree Menu"
        };

        SectionSidebarAppExtension sidebarApp = new() {
            Kind = "menu",
            Alias = $"{Alias}.SidebarApp.SettingsGroup",
            Name = $"{Name}: Sidebar App",
            Meta = new SectionSidebarAppExtensionMeta {
                Label = "Iddqd",
                Menu = menu.Alias
            },
            Conditions = [
                SectionAliasCondition.Settings
            ]
        };

        return [
            menu,
            sidebarApp,
            ..GetDomainExtensions(menu),
            ..GetPackageExtensions(menu)
        ];

    }

    private static IEnumerable<IExtension> GetDomainExtensions(MenuExtension menu) {

        MenuItemExtension menuItem = new() {
            Alias = $"{Alias}.Domains.TreeRoot",
            Name = $"{Name}: Domains Tree",
            Weight = 100,
            Meta = new MenuItemExtensionMeta {
                Label = "Domains",
                Icon = "icon-globe",
                EntityType = "iddqd-domains",
                Menus = [menu.Alias]
            }
        };

        WorkspaceExtension workspace = new() {
            Kind = "default",
            Alias = $"{Alias}.Domains.Workspace",
            Name = $"{Name}: Domains Workspace",
            Meta = new WorkspaceExtensionMeta {
                EntityType = "iddqd-domains"
            }
        };

        WorkspaceViewExtension workspaceView = new() {
            Kind = "default",
            Alias = $"{Alias}.Domains.WorkspaceView",
            Name = $"{Name}: Domains Workspace View",
            Js = $"/App_Plugins/{Alias}/Elements/WorkspaceViews/Domains.js?v={CacheBuster}",
            ElementName = "iddqd-domains-workspace-view",
            Weight = 100,
            Meta = new WorkspaceViewExtensionMeta {
                Label = "Iddqd",
                PathName = "overview",
                Icon = "icon-info"
            },
            Conditions = [
                new WorkspaceAliasCondition(workspace.Alias)
            ]
        };

        return [menuItem, workspace, workspaceView];

    }

    private static IEnumerable<IExtension> GetPackageExtensions(MenuExtension menu) {

        MenuItemExtension menuItem = new() {
            Alias = $"{Alias}.Packages.TreeRoot",
            Name = $"{Name}: Packages Tree",
            Weight = 100,
            Meta = new MenuItemExtensionMeta {
                Label = "Packages",
                Icon = "icon-box",
                EntityType = "iddqd-packages",
                Menus = [menu.Alias]
            }
        };

        WorkspaceExtension workspace = new() {
            Alias = $"{Alias}.Packages.Workspace",
            Name = $"{Name}: Packages Workspace",
            Meta = new WorkspaceExtensionMeta {
                EntityType = "iddqd-packages"
            }
        };

        WorkspaceViewExtension workspaceView = new() {
            Alias = $"{Alias}.Packages.WorkspaceView",
            Name = $"{Name}: Packages Workspace View",
            Js = $"/App_Plugins/{Alias}/Elements/WorkspaceViews/Packages.js?v={CacheBuster}",
            Weight = 100,
            Meta = new WorkspaceViewExtensionMeta {
                Label = "Overview",
                PathName = "overview",
                Icon = "icon-info"
            },
            Conditions = [
                new WorkspaceAliasCondition(workspace.Alias)
            ]
        };

        return [menuItem, workspace, workspaceView];

    }

    internal class IconsExtension : IExtension {

        public string Type => "icons";

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public required string Alias { get; init; }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public required string Name { get; init; }

        public required string Js { get; init; }

    }

}