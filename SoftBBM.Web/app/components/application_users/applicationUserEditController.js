(function (app) {
    app.controller('applicationUserEditController', applicationUserEditController);

    applicationUserEditController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$stateParams', '$filter'];
    function applicationUserEditController(apiService, $window, $scope, notificationService, $state, $stateParams, $filter) {
        $scope.applicationUser = {
            Groups: [],
            Branches: []
        }
        $scope.applicationGroups = [];
        $scope.branches = [];
        $scope.$watch('mydateOfBirth', function (newValue) {
            $scope.applicationUser.BirthDay = $filter('date')(newValue, 'dd/MMM/yyyy');
        });

        $scope.$watch('applicationUser.BirthDay', function (newValue) {
            $scope.mydateOfBirth = $filter('date')(newValue, 'dd/MMM/yyyy');
        });
        $scope.UpdateApplicationUser = UpdateApplicationUser;

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
        function LoadApplicationUserDetail() {
            apiService.get('api/applicationuser/detail/' + $stateParams.id, null, function (result) {
                $scope.applicationUser = result.data;
                $window.document.title = "Cập nhật User - " + $scope.applicationUser.Id;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function UpdateApplicationUser() {
            debugger
            apiService.post('api/applicationuser/update', $scope.applicationUser, function (result) {
                notificationService.displaySuccess('Cập nhật ' + result.data + ' thành công');
                $state.go('application_users');
            }, function (error) {
                notificationService.displayError('Cập nhật không thành công');
            });
        }
        function authen() {
            apiService.get('api/applicationuser/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}

        authen();
        LoadApplicationGroups();
        LoadApplicationBranches();
        LoadApplicationUserDetail();
    }
})(angular.module('softbbm.application_users'));