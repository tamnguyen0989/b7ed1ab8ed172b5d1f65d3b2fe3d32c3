(function (app) {
    app.controller('returnSupplierListController', returnSupplierListController);

    returnSupplierListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state']
    function returnSupplierListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state) {
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10'
        $scope.animate = false;
        $scope.filters = {
            selectedBookStatusFilters: [],
            selectedBranchFilters: [],
            selectedSupplierFilters: []
        }
        $scope.filters.sortBy = '';
        $scope.sortTotal = false;
        $scope.CreatedDate = false;
        $scope.picker = {};
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            showWeeks: false
        };
        $scope.supplierFilters = [];
        $scope.startDateFilter = null;
        $scope.endDateFilter = null;

        $scope.search = search;
        $scope.refeshPage = refeshPage;
        $scope.bookDetail = bookDetail;
        $scope.updateFilter = updateFilter;
        $scope.sortBy = sortBy;
        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.updateDateFilter = updateDateFilter;
        $scope.setMinDate = setMinDate;
        $scope.init = init;
        $scope.loadFilter = loadFilter;
        $scope.resetTimeFilter = resetTimeFilter;
        function search(page) {
            page = page || 0;
            $scope.loading = true;
            $scope.filters.page = page;
            $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
            $scope.filters.filter = $scope.filterBooks;
            $scope.filters.branchId = $scope.branchSelectedRoot.Id;
            $scope.filters.startDateFilter = $scope.startDateFilter;
            $scope.filters.endDateFilter = $scope.endDateFilter;

            apiService.post('/api/returnsupplier/search', $scope.filters, function (result) {
                $scope.books = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterBooks && $scope.filterBooks.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' phiếu tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterBooks = '';
            $scope.filters.selectedSupplierFilters = [];
            $scope.filters.sortBy = '';
            $scope.filters.startDateFilter = {};
            $scope.filters.endDateFilter = {};
            $scope.startDateFilter = {};
            $scope.endDateFilter = {};
            $scope.sortTotal = false;
            $scope.CreatedDate = false;
            search();
        }
        function bookDetail(selectedBook) {
            $scope.selectedBook = selectedBook;
            $uibModal.open({
                templateUrl: '/app/components/return_suppliers/returnSupplierDetailModal.html' + BuildVersion,
                controller: 'returnSupplierDetailController',
                scope: $scope,
                windowClass: 'app-modal-window-medium'
            }).result.finally(function () {

            });
        }
        function updateFilter() {
            $scope.page = $scope.page || 0;
            search($scope.page);
        }
        function sortBy(value) {
            switch (value) {
                case 'Total':
                    $scope.sortTotal = !$scope.sortTotal;
                    $scope.filters.sortBy = $scope.sortTotal == true ? 'Total_des' : 'Total_asc';
                    search();
                    break;
                case 'CreatedDate':
                    $scope.CreatedDate = !$scope.CreatedDate;
                    $scope.filters.sortBy = $scope.CreatedDate == true ? 'CreatedDate_des' : 'CreatedDate_asc';
                    search();
                    break;
            }
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
        function updateDateFilter() {
            if ($scope.startDateFilter && $scope.endDateFilter) {
                if ($scope.startDateFilter <= $scope.endDateFilter) {
                    $scope.page = $scope.page || 0;
                    search($scope.page);
                }
                else {
                    notificationService.displayError("Ngày lọc không đúng")
                }
            }
        }
        function setMinDate(val) {
            if (val) {
                $scope.dateOptions.minDate = $scope.startDateFilter;
            }
            else
                $scope.dateOptions.minDate = null;
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
        }
        function loadFilter() {
            $scope.animate = !$scope.animate;
        }
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }
        function loadSuppliers() {
            $scope.loading = true;
            var config = {
                params: {
                    order: "Name"
                }

            }
            apiService.get('/api/supplier/getall', config, function (result) {
                $scope.supplierFilters = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        loadSuppliers();
        init();
        search();
        $window.document.title = "DS phiếu trả hàng";
    }
})(angular.module('softbbm.return_suppliers'));