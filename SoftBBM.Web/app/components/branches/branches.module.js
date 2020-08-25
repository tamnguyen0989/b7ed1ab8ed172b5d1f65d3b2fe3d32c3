(function () {
    angular.module('softbbm.branches', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('branches', {
            url: '/branches',
            parent: 'base',
            templateUrl: '/app/components/branches/branchListView.html' + BuildVersion,
            controller: 'branchListController'
        }).state('add_branch', {
            url: '/add_branch',
            parent: 'base',
            templateUrl: '/app/components/branches/branchAddView.html' + BuildVersion,
            controller: 'branchAddController'
        });
    }
})();