using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Iddqd.Controllers.Trees {

    [PluginController("LimboIddqd")]
    [Tree("settings", "iddqd", TreeTitle = "Iddqd", SortOrder = 5)]
    public class IddqdTreeController : TreeController {

        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;

        public IddqdTreeController(IMenuItemCollectionFactory menuItemCollectionFactory, ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IEventAggregator eventAggregator) : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator) {
            _menuItemCollectionFactory = menuItemCollectionFactory;
        }

        protected override ActionResult<TreeNode?> CreateRootNode(FormCollection queryStrings) {

            var node = base.CreateRootNode(queryStrings);

            if (node.Value is not null) node.Value.Icon = "icon-lab";

            return node;

        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings) {
            var nodes = new TreeNodeCollection();

            var node = CreateTreeNode("property-editors", "-1", queryStrings, "Property Editors", "icon-presentation", false);
            node.RoutePath = "settings/iddqd/property-editors";
            nodes.Add(node);

            return nodes;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings) {
            return _menuItemCollectionFactory.Create();
        }

    }

}