angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.Package.Controller", function ($scope) {

    const vm = this;

    vm.package = $scope.model.package;
    vm.assembly = $scope.model.package.assembly;

});