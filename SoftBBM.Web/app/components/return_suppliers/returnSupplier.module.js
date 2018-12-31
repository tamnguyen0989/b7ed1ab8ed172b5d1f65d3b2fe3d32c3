(function () {
    angular.module('softbbm.return_suppliers', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('return_suppliers', {
            url: '/return_suppliers',
            parent: 'base',
            templateUrl: '/app/components/return_suppliers/returnSupplierListView.html',
            controller: 'returnSupplierListController'
        }).state('add_return_supplier', {
            url: '/add_return_supplier',
            parent: 'base',
            templateUrl: '/app/components/return_suppliers/returnSupplierAddView.html',
            controller: 'returnSupplierAddController'
        })
    }
})();