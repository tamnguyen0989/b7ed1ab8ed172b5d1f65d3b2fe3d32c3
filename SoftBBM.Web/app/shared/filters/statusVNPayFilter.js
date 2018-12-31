(function (app) {
    app.filter('statusVNPayFilter', function () {
        return function (input) {
            if (input == "00")
                return 'Thành công';
            else
                return 'Thất bại';
        }
    });
})(angular.module('softbbm.common'));