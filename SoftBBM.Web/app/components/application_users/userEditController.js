(function (app) {
    app.controller('userEditController', userEditController);

    userEditController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$stateParams', '$filter'];
    function userEditController(apiService, $window, $scope, notificationService, $state, $stateParams, $filter) {
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
            if ($scope.userId == $stateParams.id) {
                apiService.get('api/applicationuser/detailuser/' + $stateParams.id, null, function (result) {
                    $scope.applicationUser = result.data;
                    $window.document.title = "Cập nhật User - " + $scope.applicationUser.Id;
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            }
            else {
                notificationService.displayError('404 not found !');
                $state.go('home');
            }
        }
        function UpdateApplicationUser() {
            $scope.applicationUser.userId = $scope.userId;
            apiService.post('api/applicationuser/updateinfo', $scope.applicationUser, function (result) {
                notificationService.displaySuccess('Cập nhật ' + result.data + ' thành công');
                $state.go('home');
            }, function (error) {
                notificationService.displayError('Cập nhật không thành công');
            });
        }
        function authen() {
            apiService.get('api/applicationuser/autheneditinfo', null, function (result) {

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