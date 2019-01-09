(function () {
    angular.module('softbbm.imports', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('imports', {
            url: '/imports',
            parent: 'base',
            templateUrl: '/app/components/imports/importView.html',
            controller: 'importController'
        })
        ;
    }
})();