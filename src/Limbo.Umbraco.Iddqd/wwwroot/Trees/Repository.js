import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";

class MyTreeRepository extends UmbControllerBase {

    async requestTreeRoot() {
        return {
            data: {
                unique: "root",
                entityType: "my-custom-root",
                name: "My Custom Tree",
                icon: "icon-folder",
                hasChildren: true
            }
        };
    }

    async requestTreeRootItems() {
        return {
            data: [
                {
                    unique: "item-1",
                    entityType: "my-custom-item",
                    name: "Item 1",
                    icon: "icon-document",
                    hasChildren: true
                },
                {
                    unique: "item-2",
                    entityType: "my-custom-item",
                    name: "Item 2",
                    icon: "icon-document",
                    hasChildren: false
                }
            ]
        };
    }

    async requestTreeItemsOf(parentUnique) {
        if (parentUnique === "item-1") {
            return {
                data: [
                    {
                        unique: "item-1-1",
                        entityType: "my-custom-item",
                        name: "Child Item 1.1",
                        icon: "icon-document",
                        hasChildren: false
                    }
                ]
            };
        }

        return { data: [] };
    }

    async requestTreeItemAncestors(unique) {
        if (unique === "item-1-1") {
            return {
                data: [
                    {
                        unique: "root",
                        entityType: "my-custom-root",
                        name: "My Custom Tree",
                        icon: "icon-folder",
                        hasChildren: true
                    },
                    {
                        unique: "item-1",
                        entityType: "my-custom-item",
                        name: "Item 1",
                        icon: "icon-document",
                        hasChildren: true
                    }
                ]
            };
        }

        return { data: [] };
    }
}

export { MyTreeRepository as api };