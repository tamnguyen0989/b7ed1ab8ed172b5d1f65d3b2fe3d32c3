(function (app) {
    app.controller('applicationRoleAddController', applicationRoleAddController);

    applicationRoleAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state'];
    function applicationRoleAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.applicationRole = {
        }
        $scope.loading = false;
        $scope.roleCategories = [];
        $scope.selectedRoleCategory = {};
        $scope.AddApplicationRole = AddApplicationRole;

        function loadApplicationRoleCategories() {
            $scope.loading = true;
            apiService.get('api/applicationrole/getlistallcategoryroleadd', null, function (result) {
                $scope.roleCategories = result.data;
                $scope.selectedRoleCategory = $scope.roleCategories[0];
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function AddApplicationRole() {
            $scope.applicationRole.CategoryId = $scope.selectedRoleCategory.Id;
            apiService.post('api/applicationrole/create', $scope.applicationRole, function (result) {
                notificationService.displaySuccess('Thêm mới ' + result.data.Name + ' thành công');
                $state.go('application_roles');
            }, function () {
                notificationService.displayError('Thêm mới không thành công');
            });
        }
        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        $window.document.title = "Thêm mới Quyền";

        loadApplicationRoleCategories();
    }
})(angular.module('softbbm.application_roles'));