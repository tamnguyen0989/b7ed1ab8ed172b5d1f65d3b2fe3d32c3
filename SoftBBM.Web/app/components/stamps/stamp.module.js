(function () {
    angular.module('softbbm.stamps', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('add_stamp', {
            url: '/add_stamp',
            parent: 'base',
            templateUrl: '/app/components/stamps/stampAddView.html' + BuildVersion,
            controller: 'stampAddController'
        })
        ;
    }
})();