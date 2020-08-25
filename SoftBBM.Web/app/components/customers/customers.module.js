(function () {
    angular.module('softbbm.customers', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('customers', {
            url: '/customers',
            parent: 'base',
            templateUrl: '/app/components/customers/customerListView.html' + BuildVersion,
            controller: 'customerListController'
        }).state('add_customer', {
            url: '/add_customer',
            parent: 'base',
            templateUrl: '/app/components/customers/customerAddView.html' + BuildVersion,
            controller: 'customerAddController'
        });
    }
})();