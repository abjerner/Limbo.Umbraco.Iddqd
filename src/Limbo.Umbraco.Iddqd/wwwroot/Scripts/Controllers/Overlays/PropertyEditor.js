angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.PropertyEditor.Controller", function ($scope) {

    const vm = this;

    vm.propertyEditor = $scope.model.propertyEditor;
    vm.assembly = $scope.model.propertyEditor.assembly;

});