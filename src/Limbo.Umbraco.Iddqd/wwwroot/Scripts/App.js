angular.module("umbraco.services").config(function ($httpProvider) {

    $httpProvider.interceptors.push(function ($q) {
        return {
            "request": function (request) {

                const url = request.url.split("?");

                if (url[0].toLowerCase().indexOf("/app_plugins/limboiddqd/backoffice/iddqd/") === 0) {
                    request.url = `/App_Plugins/Limbo.Umbraco.Iddqd/Views/Trees/${request.url.substring(41)}`;
                }

                return request || $q.when(request);

            }
        };
    });

});