angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.Examine.Controller", function ($http, $q, $routeParams, $timeout, editorState, editorService, localizationService) {

    const vm = this;

    if ($routeParams.id <= 0) return;

    vm.routeParams = $routeParams;
    vm.editorState = editorState.current;

    vm.loading = true;

    const params = {
        id: $routeParams.id,
        contentTypeAlias: editorState.current.contentTypeAlias,
        section: $routeParams.section
    };

    vm.refresh = function () {

        vm.loading = true;

        // Refreshing is generally super fast, in which case the load indicator will flash very quickly. Adding a small
        // delay should ensure that the load indicator doesn't appear to flash but still isn't shown too long for the
        // user to notice (hopefully)
        const promises = [
            $timeout(function () { }, 250),
            $http.get("/umbraco/backoffice/Limbo/Iddqd/GetExamineResultForContent", { params }).then(function (res) {
                vm.results = res.data;
            })
        ];

        $q.all(promises).then(function () {
            vm.loading = false;
        });

    };

    vm.showSearchResultDialog = function (values) {
        localizationService.localize("examineManagement_fieldValues").then(function (value) {
            editorService.open({
                title: value,
                searchResultValues: values,
                size: "large",
                view: "views/dashboard/settings/examinemanagementresults.html",
                close: function () {
                    editorService.close();
                }
            });
        });
    };

    vm.reIndex = function (result) {

        vm.loading = true;

        const params = {
            id: result.id,
            contentTypeAlias: result.nodeTypeAlias,
            section: result.section,
            indexName: result.indexName,
            indexType: result.indexType,
            section: result.editSection
        };

        $http.get("/umbraco/backoffice/Limbo/Iddqd/ReIndexNode", { params }).then(function (res) {
            vm.results = res.data;
            vm.loading = false;
        });

    }

    vm.refresh();

});