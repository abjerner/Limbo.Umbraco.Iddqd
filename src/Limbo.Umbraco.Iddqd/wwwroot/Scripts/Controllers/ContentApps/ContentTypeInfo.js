angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.ContentApps.ContentTypeInfo.Controller", function ($scope, $http, editorService, editorState) {

    const vm = this;

    vm.editorState = editorState.current;

    vm.update = function () {

        vm.loading = true;

        const params = {
            key: $scope.model.key
        };

        $http.get("/umbraco/backoffice/Limbo/Iddqd/GetContentType", { params }).then(function (res) {
            vm.contentType = res.data;
        });

    };

    vm.update();

});