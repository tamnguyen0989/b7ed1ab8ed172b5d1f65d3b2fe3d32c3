(function (app) {
    app.controller('productLogListController', productLogListController);

    productLogListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state', '$ngBootbox']
    function productLogListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state, $ngBootbox) {
        $scope.productLogs = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10';
        $scope.filters = {

        }
        $scope.animate = false;
        $scope.startDateDeleteFilter = null;
        $scope.endDateDeleteFilter = null;
        $scope.startDateFilter = null;
        $scope.endDateFilter = null;
        $scope.picker = {};
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            showWeeks: false
        };
        $scope.isDelete = false;
        $scope.isSearch = false;

        $scope.search = search;
        $scope.refeshPage = refeshPage;
        $scope.loadDelete = loadDelete;
        $scope.loadFilter = loadFilter;
        $scope.openStartDateDeleteFilter = openStartDateDeleteFilter;
        $scope.openEndDateDeleteFilter = openEndDateDeleteFilter;
        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.resetTimeFilter = resetTimeFilter;
        $scope.resetTimeDeleteFilter = resetTimeDeleteFilter;
        $scope.deleteLog = deleteLog;

        function search(page) {
            page = page || 0;
            $scope.loading = true;
            $scope.filters.page = page;
            $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
            $scope.filters.filter = $scope.filterProductLog;
            $scope.filters.startDateDeleteFilter = $scope.startDateDeleteFilter;
            $scope.filters.endDateDeleteFilter = $scope.endDateDeleteFilter;
            $scope.filters.startDateFilter = $scope.startDateFilter;
            $scope.filters.endDateFilter = $scope.endDateFilter;
            $scope.filters.branchId = $scope.branchSelectedRoot.Id;
            apiService.post('/api/productlog/search/', $scope.filters, function (result) {
                $scope.productLogs = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterProductLog && $scope.filterProductLog.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' log tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterProductLog = '';
            $scope.pagesCount = 0;
            $scope.pageSizeNumber = '10';
            $scope.page = 0;
            $scope.startDateDeleteFilter = null;
            $scope.endDateDeleteFilter = null;
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }
        function authenDelete() {
            apiService.get('api/productlog/authendelete', null, function (result) {
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadDelete() {
            authenDelete();
            $scope.isDelete = true;
            $scope.isSearch = false;
            $scope.animate = !$scope.animate;
        }
        function loadFilter() {
            $scope.isDelete = false;
            $scope.isSearch = true;
            $scope.animate = !$scope.animate;
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
        }
        function openStartDateDeleteFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.startDateDeleteFilter = true;

        }
        function openEndDateDeleteFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.endDateDeleteFilter = true;

        }
        function openStartDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.startDateFilter = true;

        }
        function openEndDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.endDateFilter = true;

        }
        function resetTimeDeleteFilter() {
            $scope.startDateDeleteFilter = null;
            $scope.endDateDeleteFilter = null;
            search();
        }
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }
        function deleteLog() {
            if (!$scope.startDateDeleteFilter || !$scope.endDateDeleteFilter)
                notificationService.displayError("Hãy chọn thời gian !")
            else
                $ngBootbox.confirm('Bạn có chắc chắn muốn xoá?').then(function () {
                    $uibModal.open({
                        templateUrl: '/app/components/product_logs/deleteLogModal.html',
                        controller: 'deleteLogController',
                        scope: $scope,
                        backdrop: 'static',
                        keyboard: false,
                        size: 'sm'
                    }).result.finally(function () {
                        refeshPage();
                    });
                });
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        $window.document.title = "Lịch sử sản phẩm";
        $scope.search();
        init();
    }
})(angular.module('softbbm.product_logs'));