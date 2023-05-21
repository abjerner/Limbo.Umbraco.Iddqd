angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.PropertyEditors.Controller", function ($http) {

    const vm = this;

    vm.loading = true;
    vm.propertyEditors = [];

    vm.title = "Property Editors";

    $http.get("/umbraco/backoffice/Limbo/Iddqd/GetPropertyEditors").then(function (res) {
        vm.propertyEditors = res.data;
        vm.loading = false;
    });

});