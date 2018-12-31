(function (app) {
    app.controller('applicationGroupAddController', applicationGroupAddController);

    applicationGroupAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state'];
    function applicationGroupAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.applicationGroup = {
            Roles: [],
            OrderRoles: [],
            StockInRoles: [],
            StockOutRoles: [],
            BookRoles: [],
            BookBranchRoles: [],
            ProductRoles: [],
            AdjustmentStockRoles: [],
            StampRoles: [],
            BranchRoles: [],
            SupplierRoles: [],
            ChannelRoles: [],
            UserRoles: [],
            OwnRoles: [],
            GroupRoles: [],
            CustomerRoles: [],
            ProductCategoryRoles: []
        }
        $scope.applicationRoles = [];
        $scope.roleCategory = {};
        $scope.AddApplicationGroup = AddApplicationGroup;

        function AddApplicationGroup() {
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
            $.each($scope.applicationGroup.ReturnSuppliersRoles, function (index, value) {
                $scope.applicationGroup.Roles.push(value);
            });
            apiService.post('api/applicationgroup/create', $scope.applicationGroup, function (result) {
                notificationService.displaySuccess('Thêm mới ' + result.data.Name + ' thành công');
                $state.go('application_groups');
            }, function (error) {
                notificationService.displayError('Thêm mới không thành công');
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
            apiService.get('api/applicationRole/getlistallbycategorygroupadd', null, function (result) {
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
        $window.document.title = "Thêm mới Nhóm";
    }
})(angular.module('softbbm.application_groups'));
