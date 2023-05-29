angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.PropertyEditors.Controller", function ($http, $q, $timeout) {

    const vm = this;

    vm.loading = true;
    vm.groups = [];
    vm.sortField = null;
    vm.sortOrder = null;
    vm.groupBy = null;

    vm.title = "Property Editors";

    vm.options = [
        {
            alias: "groupBy",
            type: "dropdown",
            name: "Group by",
            items: [
                { alias: "", name: "None" },
                { alias: "group", name: "Group", selected: true },
                { alias: "valueType", name: "Value Type", order: "ascending" },
                { alias: "assembly", name: "Assembly" },
                { alias: "company", name: "Company", order: "ascending" }
            ]
        },
        {
            alias: "sortField",
            type: "dropdown",
            name: "Sort by",
            items: [
                { alias: "alias", name: "Alias", order: "ascending" },
                { alias: "name", name: "Name", order: "ascending" },
                { alias: "assembly", name: "Assembly", order: "ascending" },
                //{ alias: "valueType", name: "Value Type", order: "ascending" },
                { alias: "dataTypes", name: "Data Types", order: "descending" }
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

    function updateList() {

        vm.loading = true;

        const params = {};

        vm.options.forEach(function (option) {
            if (option.type === "dropdown") {
                params[option.alias] = option.value.alias;
            } else {
                params[option.alias] = option.value;
            }
        });

        const http = $http.get("/umbraco/backoffice/Limbo/Iddqd/GetPropertyEditors", { params });

        // Probably shouldn't sleep for so long, but loader looks nice 😎
        const timeout = $timeout(function () { }, 1000);

        $q.all([http, timeout]).then(function (array) {
            const res = array[0];
            vm.sortField = res.data.sortField;
            vm.sortOrder = res.data.sortOrder;
            vm.groupBy = res.data.groupBy;
            vm.groups = res.data.groups;
            vm.loading = false;

            vm.groups.forEach(function (g) {
                if (vm.groupBy === "assembly") {
                    g.assembly = g.editors[0].assembly;
                }
            });

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