(function (app) {
    app.factory('notificationService', notificationService);

    function notificationService() {
        toastr.options = {
            "debug": false,
            "positionClass": "toast-top-right",
            "onclick": null,
            "fadeIn": 300,
            "fadeOut": 1000,
            "timeOut": 3000,
            "extendedTimeOut": 1000
        };

        function displaySuccess(message) {
            toastr.success(message);
        }

        function displayWarning(message) {
            toastr.warning(message);
        }

        function displayInfo(message) {
            toastr.info(message);
        }

        function displayError(error) {
            if (Array.isArray(error)) {
                each(error, function (item) {
                    toastr.error(item);
                })
            }
            else
                toastr.error(error);
        }

        return {
            displaySuccess: displaySuccess,
            displayWarning: displayWarning,
            displayInfo: displayInfo,
            displayError: displayError
        }
    }
})(angular.module('softbbm.common'));