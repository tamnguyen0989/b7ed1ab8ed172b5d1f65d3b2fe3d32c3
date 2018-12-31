(function () {
    angular.module('softbbm.stockins', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('stockins', {
            url: '/stockins',
            parent: 'base',
            templateUrl: '/app/components/stockins/stockinListView.html',
            controller: 'stockinListController'
        }).state('edit_stockin', {
            url: '/edit_stockin/:stockinId',
            parent: 'base',
            templateUrl: '/app/components/stockins/stockinEditView.html',
            controller: 'stockinEditController'
        }).state('add_stockin', {
            url: '/add_stockin',
            parent: 'base',
            templateUrl: '/app/components/stockins/stockinAddView.html',
            controller: 'stockinAddController'
        });
    }
})();