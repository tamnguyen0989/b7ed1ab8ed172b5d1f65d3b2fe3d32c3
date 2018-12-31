(function () {
    angular.module('softbbm.channels', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('channels', {
            url: '/channels',
            parent: 'base',
            templateUrl: '/app/components/channels/channelListView.html',
            controller: 'channelListController'
        }).state('add_channel', {
            url: '/add_channel',
            parent: 'base',
            templateUrl: '/app/components/channels/channelAddView.html',
            controller: 'channelAddController'
        });
    }
})();