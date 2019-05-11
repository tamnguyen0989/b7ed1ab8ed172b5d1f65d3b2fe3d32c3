(function (app) {
    app.controller('bookListController', bookListController);

    bookListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state']
    function bookListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state) {
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10'
        $scope.animate = false;
        $scope.filters = {
            selectedBookStatusFilters: [],
            selectedBranchFilters: [],
            selectedSupplierFilters: [],
            selectedStockinCategoryFilters: [],
            selectedPaymentStatusFilters: []
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
        $scope.categoryId = '00';
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
        $scope.loadBookStatuses = loadBookStatuses;
        $scope.loadSuppliers = loadSuppliers;
        $scope.init = init;
        $scope.loadFilter = loadFilter;
        $scope.updateCancel = updateCancel;
        $scope.resetTimeFilter = resetTimeFilter;
        $scope.getBookDetailToPrint = getBookDetailToPrint;
        $scope.exportExcel = exportExcel;
        function search(page) {
            page = page || 0;
            $scope.loading = true;

            $scope.filters.branchId = $scope.branchSelectedRoot.Id;
            $scope.filters.page = page;
            $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
            $scope.filters.filter = $scope.filterBooks;
            $scope.filters.categoryId = $scope.categoryId;
            $scope.filters.startDateFilter = $scope.startDateFilter;
            $scope.filters.endDateFilter = $scope.endDateFilter;
            $scope.filters.isListBook = 1;
            apiService.post('/api/stockin/searchbook', $scope.filters, function (result) {
                $scope.books = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterBooks && $scope.filterBooks.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' đơn tìm được');
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
            $scope.filters.startDateFilter = null;
            $scope.filters.endDateFilter = null;
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            $scope.sortTotal = false;
            $scope.CreatedDate = false;
            search();
        }
        function bookDetail(selectedBook) {
            $scope.selectedBook = selectedBook;
            $uibModal.open({
                templateUrl: '/app/components/books/bookDetailModal.html',
                controller: 'bookDetailController',
                scope: $scope,
                windowClass: 'app-modal-window-medium'
            }).result.finally(function () {

            });
        }
        function loadChannels() {
            $scope.loading = true;
            apiService.get('/api/channel/getall', null, function (result) {
                $scope.channels = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
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
        function loadBookStatuses() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallbookstatus', null, function (result) {
                var filterData = result.data;
                $.each(filterData, function (index, value) {
                    if (value.Id == '00') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $.each(filterData, function (index, value) {
                    if (value.Id == '01') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $.each(filterData, function (index, value) {
                    if (value.Id == '04') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $.each(filterData, function (index, value) {
                    if (value.Id == '05') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $.each(filterData, function (index, value) {
                    if (value.Id == '06') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $.each(filterData, function (index, value) {
                    if (value.Id == '07') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $scope.bookStatusFilters = filterData;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
            if (localStorage.getItem("userName")) {
                $scope.userName = JSON.parse(localStorage.getItem("userName"));
            }
        }
        function loadFilter() {
            $scope.animate = !$scope.animate;
        }
        function updateCancel(val) {
            $ngBootbox.confirm('Bạn có chắc chắn muốn huỷ?').then(function () {
                var config = {
                    params: {
                        stockinId: val.Id,
                        userId: $scope.userId,
                        type: "fromsupplier"
                    }
                }
                apiService.get('/api/stockin/updatecancelbook', config, function (result) {
                    if (result.data.Id > 0) {
                        notificationService.displaySuccess("Cập nhật trạng thái huỷ thành công!")
                        search($scope.page);
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            });
        }
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }
        function getBookDetailToPrint(book) {
            var config = {
                params: {
                    stockinId: book.Id
                }
            }
            $scope.loading = true;
            apiService.get('/api/stockin/getbookdetailtoprint', config, function (result) {
                $scope.supplierPrint = result.data.Supplier;
                $scope.bookDetails = result.data.BookDetails;

            }, function (error) {
                notificationService.displayError(error);
            });

            setTimeout(function () {
                var innerContents = document.getElementById("printDiv").innerHTML;
                var popupWinindow = window.open();
            popupWinindow.window.focus();
            popupWinindow.document.open();
            popupWinindow.document.write('<!DOCTYPE html><html><head>'
                                + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
                                + '</head><body onload="window.print(); window.close();"><div>'
                                + innerContents + '</div></html>');
            popupWinindow.document.close();
            }, 500);
        }
        function exportExcel(item) {
            $scope.selectedBook = item;
            $uibModal.open({
                templateUrl: '/app/components/books/processBookModal.html',
                controller: 'processBookModalController',
                scope: $scope,
                backdrop: 'static',
                keyboard: false,
                size: 'sm'
            }).result.finally(function () {
                $scope.selectedBook = null;
            });
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadSuppliers();
            loadBookStatuses();
            init();
            search();
            $window.document.title = "DS đơn ĐH Nhà cung cấp";
        }
        
    }
})(angular.module('softbbm.books'));