/// <reference path="/Assets/admin/libs/angular/angular.js" />

(function (app) {
    app.factory('apiService', apiService);

    apiService.$inject = ['$http', 'notificationService', 'authenticationService','$state'];
    //apiService.$inject = ['$http', 'notificationService'];
    function apiService($http, notificationService, authenticationService,$state) {
    //function apiService($http, notificationService) {
        return {
            get: get,
            post: post,
            put: put,
            del: del
        }
        function del(url, data, success, failure) {
            authenticationService.setHeader();
            $http.delete(url, data).then(function (result) {
                success(result);
            }, function (error) {
                console.log(error.status)
                if (error.status === 401) {
                    notificationService.displayError('Chưa được cấp quyền này !');
                    $state.go('home');
                }
                if (error.status === 403) {

                }
                else if (error.status !== 401 && error.status !== 403) {
                    failure(error);
                }


            });
        }
        function post(url, data, success, failure) {
            authenticationService.setHeader();
            $http.post(url, data).then(function (result) {
                success(result);
            }, function (error) {
                console.log(error.status)
                if (error.status === 401) {
                    notificationService.displayError('Chưa được cấp quyền này !');
                    $state.go('home');
                }
                if (error.status === 403) {

                }
                else if (error.status !== 401 && error.status !== 403) {
                    failure(error);
                }


            });
        }
        function put(url, data, success, failure) {
            authenticationService.setHeader();
            $http.put(url, data).then(function (result) {
                success(result);
            }, function (error) {
                console.log(error.status)
                if (error.status === 401) {
                    notificationService.displayError('Chưa được cấp quyền này !');
                    $state.go('home');
                }
                if (error.status === 403) {

                }
                else if (error.status !== 401 && error.status !== 403) {
                    failure(error);
                }


            });
        }
        function get(url, params, success, failure) {
            authenticationService.setHeader();
            $http.get(url, params).then(function (result) {
                success(result);
            }, function (error) {
                console.log(error.status)
                if (error.status === 401) {
                    notificationService.displayError('Chưa được cấp quyền này !');
                    $state.go('home');
                }
                if (error.status === 403) {

                }
                else if (error.status !== 401 && error.status !== 403) {
                    failure(error);
                }

            });
        }
    }
})(angular.module('softbbm.common'));