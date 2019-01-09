(function () {
    angular.module('softbbm', [
        'softbbm.application_users',
        'softbbm.application_groups',
        'softbbm.application_roles',
        'softbbm.suppliers',
        'softbbm.branches',
        'softbbm.channels',
        'softbbm.stocks',
        'softbbm.stockins',
        'softbbm.stockouts',
        'softbbm.books',
        'softbbm.orders',
        'softbbm.stamps',
        'softbbm.customers',
        'softbbm.product_categories',
        'softbbm.product_logs',
        'softbbm.return_suppliers',
        'softbbm.imports',
        'softbbm.common'
    ]).config(config)
      .config(configAuthentication);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('base', {
            url: '',
            templateUrl: '/app/shared/views/baseView.html',
            controller: 'baseController',
            abstract: true
        }).state('home', {
            url: '/home',
            parent: 'base',
            templateUrl: '/app/components/home/homeView.html',
            controller: 'homeController'
        }).state('login', {
            url: '/login',
            templateUrl: '/app/components/login/loginView.html',
            controller: 'loginController'
        });
        $urlRouterProvider.otherwise('/login');
    }
    function configAuthentication($httpProvider) {
        //$httpProvider.interceptors.push(function ($q, $location) {
        //    return {
        //        request: function (config) {

        //            return config;
        //        },
        //        requestError: function (rejection) {

        //            return $q.reject(rejection);
        //        },
        //        response: function (response) {
        //            if (response.status == "401") {
        //                //toastr.error('401 (Unauthorized)');
        //                toastr.error('Chưa được cấp quyền này');
        //                $location.path('/admin');
        //                debugger
        //            }
        //            //the same response/modified/or a new one need to be returned.
        //            return response;
        //        },
        //        responseError: function (rejection) {

        //            if (rejection.status == "401") {
        //                //toastr.error('401 (Unauthorized)');
        //                toastr.error('Chưa được cấp quyền này');
        //                $location.path('/admin');
        //                debugger
        //            }
        //            return $q.reject(rejection);
        //        }
        //    };
        //});
    }
})();