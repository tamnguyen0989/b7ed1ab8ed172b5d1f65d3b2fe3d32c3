(function (app) {
    app.controller('channelEditController', channelEditController);

    channelEditController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$stateParams'];
    function channelEditController(apiService, $window, $scope, notificationService, $state, $stateParams) {
        $scope.channel = {

        }
        $scope.loading = false;
        $scope.update = update;

        function loadChannel() {
            apiService.get('api/channel/detail/' + $stateParams.id, null, function (result) {
                debugger
                $scope.channel = result.data;
                $window.document.title = "Cập nhật kênh - " + $scope.channel.Name;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function update() {
            apiService.post('api/channel/update', $scope.channel, function (result) {
                notificationService.displaySuccess(result.data);
                $state.go('channels');
            }, function (result) {
                notificationService.displayError(result.data);
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}

        loadChannel();
        
    }
})(angular.module('softbbm.channels'));