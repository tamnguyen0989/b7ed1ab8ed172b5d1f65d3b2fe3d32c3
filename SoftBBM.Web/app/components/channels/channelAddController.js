(function (app) {
    app.controller('channelAddController', channelAddController);

    channelAddController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state'];

    function channelAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.channel = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.loading = false;
        $scope.addChannel = addChannel;

        function addChannel() {
            $scope.loading = true;
            apiService.post('api/channel/add', $scope.channel,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                    $scope.loading = false;
                    $state.go('channels');
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function authen() {
            apiService.get('api/channel/authenadd', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        $window.document.title = "Thêm mới kênh";
        authen();
    }

})(angular.module('softbbm.channels'));