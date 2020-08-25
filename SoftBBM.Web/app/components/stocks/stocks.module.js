(function () {
    angular.module('softbbm.stocks', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('stocks', {
            url: '/stocks',
            parent: 'base',
            templateUrl: '/app/components/stocks/stockListView.html' + BuildVersion,
            controller: 'stockListController'
        }).state('add_product', {
            url: '/add_product',
            parent: 'base',
            templateUrl: '/app/components/stocks/productAddView.html' + BuildVersion,
            controller: 'productAddController'
        }).state('add_adjustment_stock', {
            url: '/add_adjustment_stock',
            parent: 'base',
            templateUrl: '/app/components/stocks/adjustmentStockAddView.html' + BuildVersion,
            controller: 'adjustmentStockAddController'
        }).state('adjustment_stocks', {
            url: '/adjustment_stocks',
            parent: 'base',
            templateUrl: '/app/components/stocks/adjustmentStockListView.html' + BuildVersion,
            controller: 'adjustmentStockListController'
        })
        ;
    }
})();