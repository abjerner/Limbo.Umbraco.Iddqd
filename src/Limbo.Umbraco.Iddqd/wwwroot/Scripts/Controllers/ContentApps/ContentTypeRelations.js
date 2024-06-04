angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.ContentApps.ContentTypeRelations.Controller", function ($scope, $http, editorService, editorState) {

    const vm = this;

    vm.editorState = editorState.current;

    vm.openContentType = function (contentType) {

        editorService.documentTypeEditor({
            id: contentType.id,
            submit: function () {
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        });

    };

    vm.openDataType = function (dataType) {

        var dataTypeSettings = {
            view: "views/common/infiniteeditors/datatypesettings/datatypesettings.html",
            size: "medium",
            id: dataType.id,
            submit: function () {
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(dataTypeSettings);

    };

    vm.update = function () {

        vm.loading = true;

        const params = {
            key: $scope.model.key
        };

        $http.get("/umbraco/backoffice/Limbo/Iddqd/GetContentTypeRelations", { params }).then(function (res) {
            vm.relations = res.data;
        });

    };

    vm.update();

});