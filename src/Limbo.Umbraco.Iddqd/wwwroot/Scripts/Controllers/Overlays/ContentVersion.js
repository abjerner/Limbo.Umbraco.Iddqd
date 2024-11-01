angular.module("umbraco").controller("Limbo.Umbraco.Iddqd.Overlays.ContentVersion.Controller", function ($scope) {

    const vm = this;

    vm.properties = Utilities.copy($scope.model.version.properties);

	vm.properties.forEach(function (p) {
		if (p.editor?.valueType === "json") {
			p.values.forEach(function (v) {
				v.sourceValue = v.value;
				v.value = JSON.parse(v.value);
			});
		} else if (p.editor?.valueType === "datetime") {
			p.values.forEach(function (v) {
				v.sourceValue = v.value;
				v.value = new Date(v.value);
				v.diff = moment(v.value).fromNow();
			});
		}
	});

});