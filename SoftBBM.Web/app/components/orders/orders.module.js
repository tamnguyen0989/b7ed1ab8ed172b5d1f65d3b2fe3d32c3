(function () {
    angular.module('softbbm.orders', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('orders', {
            url: '/orders',
            parent:'base',
            templateUrl: '/app/components/orders/orderListView.html' + BuildVersion,
            controller: 'orderListController'
        }).state('add_order', {
            url: '/add_order',
            parent: 'base',
            templateUrl: '/app/components/orders/orderAddView.html' + BuildVersion,
            controller: 'orderAddController'
        }).state('add_offline_order', {
            url: '/add_offline_order',
            parent: 'base',
            templateUrl: '/app/components/orders/orderOfflineAddView.html' + BuildVersion,
            controller: 'orderOfflineAddController'
        }).state('view_offline_order', {
            url: '/view_offline_order',
            //parent: 'base',
            templateUrl: '/app/components/orders/orderOfflineView.html' + BuildVersion,
            controller: 'orderOfflineViewController'
        });
    }
})();