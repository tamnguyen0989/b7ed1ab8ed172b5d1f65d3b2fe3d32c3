(function () {
    angular.module('softbbm.exports', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('export_price', {
            url: '/export_price',
            parent: 'base',
            templateUrl: '/app/components/exports/priceExportView.html',
            controller: 'priceExportController'
        })
        ;
    }
})();