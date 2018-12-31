(function (app) {
    app.controller('applicationRoleEditController', applicationRoleEditController);

    applicationRoleEditController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$stateParams'];
    function applicationRoleEditController(apiService, $window, $scope, notificationService, $state, $stateParams) {
        $scope.applicationRole = {

        }
        $scope.loading = false;
        $scope.roleCategories = [];
        $scope.selectedRoleCategory = {};
        $scope.UpdateApplicationRole = UpdateApplicationRole;

        function loadApplicationRoleCategories() {
            $scope.loading = true;
            apiService.get('api/applicationrole/getlistallcategoryroleedit', null, function (result) {
                $scope.roleCategories = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function LoadApplicationRoleDetail() {
            apiService.get('api/applicationRole/detail/' + $stateParams.id, null, function (result) {
                $scope.applicationRole = result.data;
                $scope.selectedRoleCategory = result.data.ApplicationRoleCategory;
                $window.document.title = "Cập nhật quyền - " + $scope.applicationRole.Id;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function UpdateApplicationRole() {
            $scope.applicationRole.CategoryId = $scope.selectedRoleCategory.Id;
            apiService.post('api/applicationRole/update', $scope.applicationRole, function (result) {
                notificationService.displaySuccess('Cập nhật ' + result.data.Name + ' thành công');
                $state.go('application_roles');
            }, function () {
                notificationService.displayError('Cập nhật không thành công');
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}

        loadApplicationRoleCategories();
        LoadApplicationRoleDetail();
    }
})(angular.module('softbbm.application_roles'));