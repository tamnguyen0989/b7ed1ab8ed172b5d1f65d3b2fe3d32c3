(function (app) {

	app.directive('csDateToIso', csDateToIso);

	function csDateToIso() {
		var linkFunction = function (scope, element, attrs, ngModelCtrl) {
			ngModelCtrl.$parsers.push(function (datepickerValue) {
                return moment(datepickerValue).format("MM/DD/YYYY");
			});
		};

		return {
			restrict: "A",
			require: "ngModel",
			link: linkFunction
		};
	}
})(angular.module('softbbm.common'));