(function (app) {
    'use strict';
    app.service('loginService', ['$http', '$q', 'authenticationService', 'authData', 'apiService',
    function ($http, $q, authenticationService, authData, apiService) {
        var userInfo;
        var deferred;

        this.login = function (userName, password) {
            deferred = $q.defer();
            var config = {
                params: {
                    userName: userName
                }
            };
            var data = btoa(userName + ':' + password);
            //$http.post('/api/home/testMethod', null, {
            $http.defaults.headers.common['Authorization'] = data;
            $http.defaults.headers.common['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
            $http.get('api/applicationuser/getlistbranch', config)
            .success(function (response) {
                authenticationService.setTokenInfo(data, userName, response);
                authData.authenticationData.IsAuthenticated = true;
                authData.authenticationData.userName = userName;
                authData.authenticationData.branches = response.SoftBranchs;
                
                deferred.resolve(null);
            })
            .error(function (err, status) {
                var result = {
                    error: status,
                    error_description: 'Thông tin đăng nhập không hợp lệ'
                }
                err = result;
                authData.authenticationData.IsAuthenticated = false;
                authData.authenticationData.userName = null;
                authData.authenticationData.branches = null;
                authData.authenticationData.selectedBranch = null;
                deferred.resolve(err);
            });
            return deferred.promise;
        }

        this.logOut = function () {
            authenticationService.removeToken();
            authData.authenticationData.IsAuthenticated = false;
            authData.authenticationData.userName = null;
            authData.authenticationData.branches = null;
            authData.authenticationData.selectedBranch = null;
        }
    }]);
})(angular.module('softbbm.common'));