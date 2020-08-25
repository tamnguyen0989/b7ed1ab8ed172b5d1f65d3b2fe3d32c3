(function () {
    angular.module('softbbm.stockouts', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('stockouts', {
            url: '/stockouts',
            parent: 'base',
            templateUrl: '/app/components/stockouts/stockoutListView.html' + BuildVersion,
            controller: 'stockoutListController'
        }).state('add_stockout', {
            url: '/add_stockout',
            parent: 'base',
            templateUrl: '/app/components/stockouts/stockoutAddView.html' + BuildVersion,
            controller: 'stockoutAddController'
        }).state('edit_stockout', {
            url: '/edit_stockout/:stockinId',
            parent: 'base',
            templateUrl: '/app/components/stockouts/stockoutEditView.html' + BuildVersion,
            controller: 'stockoutEditController'
        })
        ;
    }
})();