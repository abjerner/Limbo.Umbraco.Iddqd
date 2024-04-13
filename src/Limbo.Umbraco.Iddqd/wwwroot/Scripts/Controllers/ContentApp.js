angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.ContentApp.Controller", function ($http, $routeParams, editorState) {

    const vm = this;

    if ($routeParams.id <= 0) return;

    vm.routeParams = $routeParams;
    vm.editorState = editorState.current;

    vm.contentApp = vm.editorState.apps.find(x => x.alias === "iddqd")?.viewModel;

    vm.tabs = vm.contentApp?.tabs ?? [];

    vm.tabs.forEach(function (t, i) {
        if (i === 0) t.active = true;
        t.label = t.name;
    });

    vm.changeTab = function (selectedTab) {
        vm.tabs.forEach(function (tab) {
            tab.active = false;
        });
        selectedTab.active = true;
    };

});