angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.Packages.Controller", function ($http, $q, $timeout, editorService) {

    const cacheBuster = Umbraco.Sys.ServerVariables.limbo.iddqd.cacheBuster;

    const vm = this;

    let wait = null;

    vm.loading = true;
    vm.groups = [];

    vm.title = "Packages";

    vm.options = [
        {
            alias: "text",
            type: "text",
            placeholder: "Start typing to search..."
        },
        {
            alias: "type",
            type: "dropdown",
            name: "Type",
            items: [
                { alias: "", name: "All", selected: true },
                { alias: "manifestFilter", name: "C# Manifest Filter" },
                { alias: "packageManifest", name: "package.manifest", order: "ascending" }
            ]
        },
        {
            alias: "groupBy",
            type: "dropdown",
            name: "Group by",
            items: [
                { alias: "", name: "None" },
                //{ alias: "assembly", name: "Assembly" },
                { alias: "company", name: "Company", order: "ascending", selected: true },
                { alias: "product", name: "Product", order: "ascending" },
                { alias: "type", name: "Type", order: "ascending" }
            ]
        },
        {
            alias: "sortField",
            type: "dropdown",
            name: "Sort by",
            items: [
                { alias: "alias", name: "Alias", order: "ascending" },
                { alias: "name", name: "Name", order: "ascending", selected: true },
                { alias: "assembly", name: "Assembly", order: "ascending" },
                //{ alias: "valueType", name: "Value Type", order: "ascending" },
                //{ alias: "dataTypes", name: "Data Types", order: "descending" }
            ]
        },
        {
            alias: "sortOrder",
            type: "dropdown",
            name: "Order by",
            items: [
                { alias: "ascending", name: "Ascending" },
                { alias: "descending", name: "Descending" }
            ]
        }
    ];

    vm.textChanged = function () {

        if (wait) $timeout.cancel(wait);

        // Add a small delay so we dont call the API on each keystroke
        wait = $timeout(function () {
            updateList();
        }, 300);

    };

    vm.showPackage = function (package) {

        const o = {
            title: package.packageName,
            package: package,
            size: "large",
            view: "/App_Plugins/Limbo.Umbraco.Iddqd/Views/Overlays/Package.html?v=" + cacheBuster,
            close: function () {
                editorService.close();
            }
        };

        editorService.open(o);

    };

    function updateList() {

        vm.loading = true;

        const params = {};

        vm.options.forEach(function (option) {
            if (option.type === "text") {
                if (option.value) params[option.alias] = option.value;
            } else if (option.type === "dropdown") {
                params[option.alias] = option.value.alias;
            } else {
                params[option.alias] = option.value;
            }
        });

        const http = $http.get("/umbraco/backoffice/Limbo/Iddqd/GetPackages", { params });

        // Probably shouldn't sleep for so long, but loader looks nice 😎
        const timeout = $timeout(function () { }, 1000);

        $q.all([http, timeout]).then(function (array) {

            const res = array[0];

            vm.groupBy = res.data.groupBy;
            vm.sortField = res.data.sortField;
            vm.sortOrder = res.data.sortOrder;
            vm.type = res.data.type;

            vm.groups = res.data.groups;

            vm.groups.forEach(function (g) {
                g.items.forEach(function (p) {
                    if (p.version) {
                        const v = p.version.split('+');
                        if (v.length > 1) {
                            p.version = v[0];
                            p.versionHash = v[1];
                        }
                    }

                    p.links = [];

                    if (p.assembly?.packageProjectUrl) {
                        p.links.push({
                            icon: "icon-globe-alt",
                            name: "Website",
                            url: p.assembly.packageProjectUrl,
                            list: true
                        });
                    }

                    if (p.assembly?.documentationUrl) {
                        p.links.push({
                            icon: "fa fa-book",
                            name: "Documentation",
                            url: p.assembly.documentationUrl,
                            list: false
                        });
                    }

                    if (p.assembly?.repositoryUrl) {
                        let icon = p.assembly.repositoryUrl.indexOf("github.com") ? "fa fa-github" : null;
                        let name = p.assembly.repositoryUrl.indexOf("github.com") ? "GitHub" : "Repository";
                        p.links.push({
                            icon,
                            name,
                            url: p.assembly.repositoryUrl,
                            list: true
                        });
                    }

                    if (p.assembly?.marketplaceUrl) {
                        p.links.push({
                            icon: "icon-umbraco",
                            name: "Marketplace",
                            url: p.assembly.marketplaceUrl,
                            list: true
                        });
                    }

                    if (p.assembly?.nuGetUrl) {
                        p.links.push({
                            icon: "icon-code",
                            name: "NuGet",
                            url: p.assembly.nuGetUrl,
                            list: true
                        });
                    }

                });
            });

            vm.loading = false;

        });

    }

    function init() {

        vm.options.forEach(function (option) {
            if (option.type === "dropdown") {
                option.value = option.items.find(x => x.selected) ?? option.items[0];
                option.select = function (item) {
                    option.value = item;
                    option.close();
                    updateList();
                };
                option.toggle = function () {
                    option.dropdownOpen = !option.dropdownOpen;
                    if (option.dropdownOpen) {
                        vm.options.forEach(function (o) {
                            if (option != o) {
                                o.dropdownOpen = false;
                            }
                        });
                    }
                };
                option.close = function () {
                    option.dropdownOpen = false;
                };
            }
        });

        updateList();

    }

    init();

});