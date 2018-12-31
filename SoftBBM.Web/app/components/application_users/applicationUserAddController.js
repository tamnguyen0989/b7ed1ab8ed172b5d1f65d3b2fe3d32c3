(function (app) {
    app.controller('applicationUserAddController', applicationUserAddController);

    applicationUserAddController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state'];
    function applicationUserAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.applicationUser = {
            Groups:[]
        }
        $scope.applicationGroups = [];
        $scope.branches = [];

        $scope.AddApplicationUser = AddApplicationUser;
        $scope.LoadApplicationGroups = LoadApplicationGroups;
        //$scope.$watch('mydateOfBirth', function (newValue) {
        //    $scope.applicationUser.BirthDay = $filter('date')(newValue, 'yyyy/MM/dd');
        //});

        //$scope.$watch('applicationUser.BirthDay', function (newValue) {
        //    $scope.mydateOfBirth = $filter('date')(newValue, 'yyyy/MM/dd');
        //});
        function AddApplicationUser() {
            apiService.post('api/applicationUser/create', $scope.applicationUser, function (result) {
                notificationService.displaySuccess('Thêm mới ' + result.data + ' thành công');
                $state.go('application_users');
            }, function (error) {
                notificationService.displayError(error.data.Message);
            });
        }
        function LoadApplicationGroups() {
            apiService.get('api/applicationGroup/getlistall', null, function (result) {
                $scope.applicationGroups = result.data;
            }, function (error) {
                notificationService.displayError('Load danh sách group không thành công');
            });
        }
        function LoadApplicationBranches() {
            apiService.get('api/branch/getall', null, function (result) {
                $scope.branches = result.data;
            }, function (error) {
                notificationService.displayError('Load danh sách kho không thành công');
            });
        }
        function authen() {
            apiService.get('api/applicationuser/authenadd', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }


        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        authen();
        $scope.LoadApplicationGroups();
        LoadApplicationBranches();
        $window.document.title = "Thêm mới User";
        debugger
    }
})(angular.module('softbbm.application_users'));