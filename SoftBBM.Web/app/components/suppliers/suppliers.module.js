(function () {
    angular.module('softbbm.suppliers', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('suppliers', {
            url: '/suppliers',
            parent: 'base',
            templateUrl: '/app/components/suppliers/supplierListView.html' + BuildVersion,
            controller: 'supplierListController'
        }).state('add_supplier', {
            url: '/add_supplier',
            parent: 'base',
            templateUrl: '/app/components/suppliers/supplierAddView.html' + BuildVersion,
            controller: 'supplierAddController'
        });
        //    .state('edit_supplier', {
        //    url: '/edit_supplier',
        //    templateUrl: '/app/components/suppliers/supplierEditView.html' + BuildVersion,
        //    controller: 'supplierEditController'
        //});
    }
})();