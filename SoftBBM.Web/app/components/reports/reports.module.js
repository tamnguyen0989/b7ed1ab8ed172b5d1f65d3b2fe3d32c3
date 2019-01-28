(function () {
    angular.module('softbbm.reports', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('channel_sales_report', {
            url: '/channel_sales_report',
            parent: 'base',
            templateUrl: '/app/components/reports/channelSalesReportView.html',
            controller: 'channelSalesReportController'
        }).state('sales_report', {
            url: '/sales_report',
            parent: 'base',
            templateUrl: '/app/components/reports/salesReportView.html',
            controller: 'salesReportController'
        })
        ;
    }
})();