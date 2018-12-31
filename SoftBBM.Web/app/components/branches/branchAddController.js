(function (app) {
    app.controller('branchAddController', branchAddController);

    branchAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state'];

    function branchAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.branch = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.loading = false;
        $scope.addBranch = addBranch;

        function addBranch() {
            $scope.loading = true;
            apiService.post('api/branch/add', $scope.branch,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                    $scope.loading = false;
                    $state.go('branches');
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function authen() {
            apiService.get('api/branch/authenadd', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        $window.document.title = "Thêm mới kho";
        authen();
    }

})(angular.module('softbbm.branches'));