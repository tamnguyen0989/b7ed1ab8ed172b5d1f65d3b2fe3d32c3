(function (app) {
    app.controller('adjustmentStockListController', adjustmentStockListController);

    adjustmentStockListController.$inject = ['$scope', 'apiService', '$window',  'notificationService', '$ngBootbox', '$uibModal', '$state']
    function adjustmentStockListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state) {
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
        $scope.addBook = addBook;
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

            apiService.post('/api/adjustmentstock/search', $scope.filters, function (result) {
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
            $scope.filters.selectedBookStatusFilters = [];
            $scope.filters.selectedSupplierFilters = [];
            $scope.filters.sortBy = '';
            $scope.filters.startDateFilter = {};
            $scope.filters.endDateFilter = {};
            $scope.startDateFilter = {};
            $scope.endDateFilter = {};
            $scope.sortTotal = false;
            $scope.CreatedDate = false;
            $scope.animate = false;
            search();
        }
        function bookDetail(selectedBook) {
            $scope.selectedBook = selectedBook;
            $uibModal.open({
                templateUrl: '/app/components/stocks/adjustmentStockDetailModal.html' + BuildVersion,
                controller: 'adjustmentStockDetailController',
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
        function addBook(val) {
            var config = {
                params: {
                    adjustmentStockId: val.Id,
                    userId: $scope.userId
                }
            }
            apiService.get('api/adjustmentStock/addexist', config,
                    function (result) {
                        if (result.data == true) {
                            notificationService.displaySuccess('Đơn đã được nhập kho.');
                            search($scope.page);
                        }

                    }, function (error) {
                        notificationService.displayError(error.data);
                    });

        }
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            init();
            search();
            $window.document.title = "DS phiếu điều chỉnh tồn kho";
        }
    }
})(angular.module('softbbm.stocks'));