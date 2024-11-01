angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.ContentApps.ContentTypeInfo.Controller", function ($scope, $http, editorService, editorState) {

    const vm = this;

    vm.editorState = editorState.current;

    if ($scope.model.createDate) $scope.model.createDateDiff = moment(new Date($scope.model.createDate)).fromNow();
    if ($scope.model.updateDate) $scope.model.updateDateDiff = moment(new Date($scope.model.updateDate)).fromNow();

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