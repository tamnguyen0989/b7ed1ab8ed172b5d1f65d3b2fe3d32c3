(function () {
    angular.module('softbbm.suppliers', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('suppliers', {
            url: '/suppliers',
            parent: 'base',
            templateUrl: '/app/components/suppliers/supplierListView.html',
            controller: 'supplierListController'
        }).state('add_supplier', {
            url: '/add_supplier',
            parent: 'base',
            templateUrl: '/app/components/suppliers/supplierAddView.html',
            controller: 'supplierAddController'
        });
        //    .state('edit_supplier', {
        //    url: '/edit_supplier',
        //    templateUrl: '/app/components/suppliers/supplierEditView.html',
        //    controller: 'supplierEditController'
        //});
    }
})();