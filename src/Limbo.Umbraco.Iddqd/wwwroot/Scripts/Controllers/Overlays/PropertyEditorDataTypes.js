angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.PropertyEditorDataTypes.Controller", function (editorService) {

    const vm = this;

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

});