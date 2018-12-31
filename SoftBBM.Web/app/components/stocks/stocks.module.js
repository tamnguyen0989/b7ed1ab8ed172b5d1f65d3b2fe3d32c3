(function () {
    angular.module('softbbm.stocks', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('stocks', {
            url: '/stocks',
            parent: 'base',
            templateUrl: '/app/components/stocks/stockListView.html',
            controller: 'stockListController'
        }).state('add_product', {
            url: '/add_product',
            parent: 'base',
            templateUrl: '/app/components/stocks/productAddView.html',
            controller: 'productAddController'
        }).state('add_adjustment_stock', {
            url: '/add_adjustment_stock',
            parent: 'base',
            templateUrl: '/app/components/stocks/adjustmentStockAddView.html',
            controller: 'adjustmentStockAddController'
        }).state('adjustment_stocks', {
            url: '/adjustment_stocks',
            parent: 'base',
            templateUrl: '/app/components/stocks/adjustmentStockListView.html',
            controller: 'adjustmentStockListController'
        })
        ;
    }
})();