(function (app) {
    app.controller('loginController', ['$scope', 'loginService', '$injector', 'notificationService','$window',
        function ($scope, loginService, $injector, notificationService, $window) {
            $scope.loading = false;
            $scope.loginData = {
                userName: "",
                password: ""
            };

            $scope.loginSubmit = function () {
                $scope.loading = true;
                localStorage.clear();
                loginService.login($scope.loginData.userName, $scope.loginData.password).then(function (response) {
                    $scope.loading = false;
                    if (response != null && response.error != undefined) {
                        notificationService.displayError(response.error_description);
                    }
                    else {
                        var stateService = $injector.get('$state');
                        stateService.go('home');                        
                    }
                });
            }
            $window.document.title = "Quản lý kho BBM";
        }]);
})(angular.module('softbbm'));