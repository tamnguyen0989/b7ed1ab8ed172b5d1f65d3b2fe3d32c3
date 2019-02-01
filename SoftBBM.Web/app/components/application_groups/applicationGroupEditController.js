(function (app) {
    app.controller('applicationGroupEditController', applicationGroupEditController);

    applicationGroupEditController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$stateParams'];
    function applicationGroupEditController(apiService, $window, $scope, notificationService, $state, $stateParams) {
        $scope.applicationGroup = {

        }
        $scope.applicationRoles = [];
        $scope.UpdateApplicationGroup = UpdateApplicationGroup;

        function loadApplicationGroupDetail() {
            apiService.get('api/applicationgroup/getbyid/' + $stateParams.id, null, function (result) {
                $scope.applicationGroup = result.data;
                $window.document.title = "Cập nhật nhóm - " + $scope.applicationGroup.Id;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function UpdateApplicationGroup() {
            $scope.applicationGroup.Roles = [];
            $.each($scope.applicationGroup.OrderRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.StockInRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.StockOutRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.BookRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.BookBranchRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.ProductRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.AdjustmentStockRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.StampRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.BranchRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.SupplierRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.ChannelRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.UserRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.OwnRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.GroupRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.CustomerRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.ProductCategoryRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.ReturnSupplierRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            $.each($scope.applicationGroup.ReportRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            apiService.post('api/applicationgroup/update', $scope.applicationGroup, function (result) {
                notificationService.displaySuccess('Cập nhật ' + result.data.Name + ' thành công');
                $state.go('application_groups');
            }, function () {
                notificationService.displayError('Cập nhật không thành công');
            });
        }
        function LoadApplicationRoles() {
            apiService.get('api/applicationRole/getlistall', null, function (result) {
                $scope.applicationRoles = result.data;
            }, function (error) {
                notificationService.displayError('Load danh sách quyền không thành công');
            });
        }
        function loadApplicationRoleByCategory() {
            apiService.get('api/applicationRole/getlistallbycategorygroupedit', null, function (result) {
                $scope.roleCategory = result.data;
            }, function (error) {
                notificationService.displayError('Load danh sách quyền không thành công');
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        //LoadApplicationRoles();
        loadApplicationRoleByCategory();
        loadApplicationGroupDetail();
    }
})(angular.module('softbbm.application_groups'));
