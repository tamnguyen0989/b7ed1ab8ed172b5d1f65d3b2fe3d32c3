(function () {
    angular.module('softbbm.product_logs', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('product_logs', {
            url: '/product_logs',
            parent: 'base',
            templateUrl: '/app/components/product_logs/productLogListView.html' + BuildVersion,
            controller: 'productLogListController'
        })
    }
})();