angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.ContentApps.ContentVersions.Controller", function ($scope, $http, $timeout, $q, editorService, editorState) {

    const cacheBuster = Umbraco.Sys.ServerVariables.limbo.iddqd.cacheBuster;

    const vm = this;

    vm.loading = true;

    vm.editorState = editorState.current;

    vm.openVersion = function (version) {

        const o = {
            title: version.name,
            version: version,
            size: "large",
            view: "/App_Plugins/Limbo.Umbraco.Iddqd/Views/Overlays/ContentVersion.html?v=" + cacheBuster,
            close: function () {
                editorService.close();
            }
        };

        editorService.open(o);

    };

    vm.update = function () {

        vm.loading = true;

        const params = {
            key: vm.editorState.key
        };

        // Refreshing is generally super fast, in which case the load indicator will flash very quickly. Adding a small
        // delay should ensure that the load indicator doesn't appear to flash but still isn't shown too long for the
        // user to notice (hopefully)
        const promises = [
            $timeout(function () { }, 250),
            $http.get("/umbraco/backoffice/Limbo/Iddqd/GetContentVersions", { params }).then(function (res) {
                vm.versions = res.data;
                vm.versions.forEach(function (v) {
                    v.updateDateDiff = moment(new Date(v.updateDate)).fromNow();
                });
            })
        ];

        $q.all(promises).then(function () {
            vm.loading = false;
        });

    };

    vm.update();

});