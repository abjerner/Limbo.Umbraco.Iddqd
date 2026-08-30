using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Conditions;
using Skybrud.Essentials.Umbraco.Manifests.Conditions.Users;
using Skybrud.Essentials.Umbraco.Manifests.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.EntryPoints;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Icons;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Localization;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Menus;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Modals;
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
                        Alias = $"{Alias}.EntryPoint",
                        Name = $"{Name}: Entry Point",
                        Js = $"/App_Plugins/{alias}/EntryPoint.js?v={CacheBuster}"
                    },
                    ..GetSettingsTreeExtensions(),
                    CreateWorkspaceView("Document", WorkspaceAliasCondition.Document),
                    CreateWorkspaceView("DocumentType", WorkspaceAliasCondition.DocumentType),
                    CreateWorkspaceView("DataType", WorkspaceAliasCondition.DataType),
                    CreateWorkspaceView("Media", WorkspaceAliasCondition.Media),
                    CreateWorkspaceView("MediaType", WorkspaceAliasCondition.MediaType),
                    CreateWorkspaceView("Member", WorkspaceAliasCondition.Member),
                    CreateWorkspaceView("MemberType", WorkspaceAliasCondition.MemberType),
                    CreateWorkspaceView("User", WorkspaceAliasCondition.User),
                    new EntityUserPermissionExtension {
	                    Alias = "My.UserPermission.Document.Iddqd",
	                    Name = $"{Name}: Document permission",
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
                        Name = $"{Name}: English",
                        Meta = new LocalizationMeta {
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

    private static WorkspaceViewExtension CreateWorkspaceView(string type, WorkspaceAliasCondition condition) {
        return new WorkspaceViewExtension {
            Alias = $"{Alias}.{type}WorkspaceView",
            Name = $"{Name}: {type} Workspace View",
            ElementName = "limbo-iddqd-content-app",
            Js = $"/App_Plugins/{Alias}/Elements/WorkspaceViews/{type}.js?v={CacheBuster}",
            Meta = new WorkspaceViewMeta {
                Label = "Iddqd",
                PathName = "iddqd",
                Icon = "icon-lab"
            },
            Conditions = [ condition, UserGroupIdCondition.Admin ]
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
            Meta = new SectionSidebarAppMeta {
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
            ..GetDataTypeExtensions(menu),
            ..GetDomainExtensions(menu),
            ..GetExtensionExtensions(menu),
            ..GetPackageExtensions(menu)
        ];

    }

    private static IEnumerable<IExtension> GetDataTypeExtensions(MenuExtension menu) {

        MenuItemExtension menuItem = new() {
            Alias = $"{Alias}.DataTypes.TreeRoot",
            Name = $"{Name}: Data Types Tree",
            Weight = 100,
            Meta = new MenuItemMeta {
                Label = "Data Types",
                Icon = "icon-box",
                EntityType = "iddqd-data-types",
                Menus = [menu.Alias]
            }
        };

        WorkspaceExtension workspace = new() {
            Alias = $"{Alias}.DataTypes.Workspace",
            Name = $"{Name}: Data Types Workspace",
            Meta = new WorkspaceMeta {
                EntityType = "iddqd-data-types"
            }
        };

        WorkspaceViewExtension workspaceView = new() {
            Alias = $"{Alias}.DataTypes.WorkspaceView",
            Name = $"{Name}: Data Types Workspace View",
            Js = $"/App_Plugins/{Alias}/Elements/WorkspaceViews/DataTypes.js?v={CacheBuster}",
            Weight = 100,
            Meta = new WorkspaceViewMeta {
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

    private static IEnumerable<IExtension> GetDomainExtensions(MenuExtension menu) {

        MenuItemExtension menuItem = new() {
            Alias = $"{Alias}.Domains.TreeRoot",
            Name = $"{Name}: Domains Tree",
            Weight = 100,
            Meta = new MenuItemMeta {
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
            Meta = new WorkspaceMeta {
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
            Meta = new WorkspaceViewMeta {
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

    private static IEnumerable<IExtension> GetExtensionExtensions(MenuExtension menu) {

        return [];

        MenuItemExtension menuItem = new() {
            Alias = $"{Alias}.Extensions.TreeRoot",
            Name = $"{Name}: Extensions Tree",
            Weight = 100,
            Meta = new MenuItemMeta {
                Label = "Extensions",
                Icon = "icon-box",
                EntityType = "iddqd-extensions",
                Menus = [menu.Alias]
            }
        };

        WorkspaceExtension workspace = new() {
            Kind = "default",
            Alias = $"{Alias}.Extensions.Workspace",
            Name = $"{Name}: Extensions Workspace",
            Meta = new WorkspaceMeta {
                EntityType = "iddqd-extensions"
            }
        };

        WorkspaceViewExtension workspaceView = new() {
            Kind = "default",
            Alias = $"{Alias}.Extensions.WorkspaceView",
            Name = $"{Name}: Extensions Workspace View",
            Js = $"/App_Plugins/{Alias}/Elements/WorkspaceViews/Extensions.js?v={CacheBuster}",
            ElementName = "iddqd-extensions-workspace-view",
            Weight = 100,
            Meta = new WorkspaceViewMeta {
                Label = "Overview",
                PathName = "overview",
                Icon = "icon-info"
            },
            Conditions = [
                new WorkspaceAliasCondition(workspace.Alias)
            ]
        };

        //ModalExtension modal = new() {
        //    Alias = $"{Alias}.Extensions.Modal",
        //    Name = $"{Name}: Extensions Modal",
        //    Element = $"/App_Plugins/{Alias}/Elements/Modals/Extensions.js?v={CacheBuster}"
        //};

        return [menuItem, workspace, workspaceView];

    }

    private static IEnumerable<IExtension> GetPackageExtensions(MenuExtension menu) {

        MenuItemExtension menuItem = new() {
            Alias = $"{Alias}.Packages.TreeRoot",
            Name = $"{Name}: Packages Tree",
            Weight = 100,
            Meta = new MenuItemMeta {
                Label = "Packages",
                Icon = "icon-box",
                EntityType = "iddqd-packages",
                Menus = [menu.Alias]
            }
        };

        WorkspaceExtension workspace = new() {
            Alias = $"{Alias}.Packages.Workspace",
            Name = $"{Name}: Packages Workspace",
            Meta = new WorkspaceMeta {
                EntityType = "iddqd-packages"
            }
        };

        WorkspaceViewExtension workspaceView = new() {
            Alias = $"{Alias}.Packages.WorkspaceView",
            Name = $"{Name}: Packages Workspace View",
            Js = $"/App_Plugins/{Alias}/Elements/WorkspaceViews/Packages.js?v={CacheBuster}",
            Weight = 100,
            Meta = new WorkspaceViewMeta {
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

}