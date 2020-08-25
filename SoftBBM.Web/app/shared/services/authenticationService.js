(function (app) {
    'use strict';
    app.service('authenticationService', ['$http', '$q', '$window', '$injector', 'authData',
    function ($http, $q, $window, $injector, authData) {
        var _accessToken;
        var _userName;
        var _userId;
        var _branches;
        var _selectedBranch;

        this.setTokenInfo = function (accessToken, userName, data) {
            _accessToken = accessToken;
            _userName = userName;
            _userId = data.ApplicationUser.Id;
            _branches = data.SoftBranchs;


            localStorage.setItem("accessToken", JSON.stringify(_accessToken));
            localStorage.setItem("userName", JSON.stringify(_userName));
            localStorage.setItem("branches", JSON.stringify(_branches));
            localStorage.setItem("userId", JSON.stringify(_userId));
        }

        this.removeToken = function () {
            _accessToken = null;
            _userName = null;
            _userId = null;
            _branches = null;
            _selectedBranch = null;
            authData.authenticationData.IsAuthenticated = null;
            authData.authenticationData.userName = null;
            authData.authenticationData.accessToken = null;
            authData.authenticationData.userId = null;
            authData.authenticationData.branches = null;
            //localStorage.removeItem("accessToken");
            //localStorage.removeItem("userName");
            //localStorage.removeItem("branches");
            //localStorage.removeItem("selectedBranch");
            //localStorage.removeItem("selectedChannel");
            localStorage.clear();
        }

        this.init = function () {
            if (localStorage.getItem("accessToken")) {
                _accessToken = JSON.parse(localStorage.getItem("accessToken"));
                _userName = JSON.parse(localStorage.getItem("userName"));
                _branches = JSON.parse(localStorage.getItem("branches"));
                _userId = JSON.parse(localStorage.getItem("userId"));
                
                authData.authenticationData.IsAuthenticated = true;
                authData.authenticationData.userName = _userName;
                authData.authenticationData.accessToken = _accessToken;
                authData.authenticationData.userId = _userId;
                authData.authenticationData.branches = _branches;
                
            }
            if (localStorage.getItem("selectedBranch") && localStorage.getItem("selectedBranch") != "undefined") {
                _selectedBranch = JSON.parse(localStorage.getItem("selectedBranch"));
                authData.authenticationData.selectedBranch = _selectedBranch;
            }
        }

        this.setHeader = function () {
            delete $http.defaults.headers.common['X-Requested-With'];
            if ((_accessToken != undefined) && (_accessToken != null) && (_accessToken != "")) {
                $http.defaults.headers.common['Authorization'] = _accessToken;
                $http.defaults.headers.common['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
            }
            else {
                var stateService = $injector.get('$state');
                stateService.go('login');
            }
        }

        this.validateRequest = function () {
            var url = 'api/home/TestMethod';
            var deferred = $q.defer();
            $http.get(url).then(function () {
                deferred.resolve(null);
            }, function (error) {
                deferred.reject(error);
            });
            return deferred.promise;
        }

        this.getAccessToken = function () {
            return _accessToken;
        }

        this.init();
    }
    ]);
})(angular.module('softbbm.common'));