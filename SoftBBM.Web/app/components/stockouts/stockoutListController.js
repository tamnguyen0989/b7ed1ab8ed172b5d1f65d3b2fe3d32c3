(function (app) {
    app.controller('stockoutListController', stockoutListController);

    stockoutListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state']
    function stockoutListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state) {
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
        $scope.totalMoney = 0;
        $scope.branchFilters = [];
        //$scope.categoryId = '01';
        $scope.startDateFilter = null;
        $scope.endDateFilter = null;
        $scope.startStockoutDateFilter = null;
        $scope.endStockoutDateFilter = null;
        $scope.adding = false;

        //$scope.chatHub = null;
        //$scope.chatHub = $.connection.chatHub;
        //$.connection.hub.start();

        $scope.search = search;
        $scope.refeshPage = refeshPage;
        $scope.bookDetail = bookDetail;
        $scope.updateFilter = updateFilter;
        $scope.sortBy = sortBy;
        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.openStartStockoutDateFilter = openStartStockoutDateFilter;
        $scope.openEndStockoutDateFilter = openEndStockoutDateFilter;
        $scope.updateDateFilter = updateDateFilter;
        //$scope.setMinDate = setMinDate;
        $scope.loadBookStatuses = loadBookStatuses;
        $scope.loadSuppliers = loadSuppliers;
        $scope.init = init;
        $scope.loadFilter = loadFilter;
        $scope.updateCancel = updateCancel;
        $scope.loadBranches = loadBranches;
        $scope.outBook = outBook;
        $scope.updateOutStock = updateOutStock;
        $scope.loadStockinCategories = loadStockinCategories;
        $scope.resetTimeFilter = resetTimeFilter;
        $scope.resetStockoutTimeFilter = resetStockoutTimeFilter;
        $scope.getDetailStockoutToPrint = getDetailStockoutToPrint;
        $scope.addStamp = addStamp;
        $scope.clearStamp = clearStamp;

        function search(page) {
            page = page || 0;
            $scope.loading = true;

            $scope.filters.page = page;
            $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
            $scope.filters.filter = $scope.filterBooks;
            $scope.filters.startDateFilter = $scope.startDateFilter;
            $scope.filters.endDateFilter = $scope.endDateFilter;
            $scope.filters.startStockoutDateFilter = $scope.startStockoutDateFilter;
            $scope.filters.endStockoutDateFilter = $scope.endStockoutDateFilter;
            $scope.filters.fromBranchId = $scope.branchSelectedRoot.Id;
            $scope.filters.isListStockout = 1;
            apiService.post('/api/stockin/searchstockout', $scope.filters, function (result) {
                $scope.books = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.totalMoney = result.data.TotalMoney;
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
            $scope.filters.selectedBranchFilters = [];
            $scope.filters.selectedStockinCategoryFilters = [];
            $scope.filters.sortBy = '';
            $scope.filters.startDateFilter = null;
            $scope.filters.endDateFilter = null;
            $scope.filters.startStockoutDateFilter = null;
            $scope.filters.endStockoutDateFilter = null;
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            $scope.startStockoutDateFilter = null;
            $scope.endStockoutDateFilter = null;
            $scope.startDateFilter = {};
            $scope.endDateFilter = {};
            $scope.sortTotal = false;
            $scope.CreatedDate = false;
            //$scope.animate = false;
            search();
        }
        function bookDetail(selectedBook) {
            $scope.selectedBook = selectedBook;
            $uibModal.open({
                templateUrl: '/app/components/stockouts/stockoutDetailModal.html',
                controller: 'stockoutDetailController',
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
        function openStartStockoutDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.startStockoutDateFilter = true;

        }
        function openEndStockoutDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.endStockoutDateFilter = true;

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
        //function setMinDate(val) {
        //    if (val) {
        //        $scope.dateOptions.minDate = $scope.startDateFilter;
        //    }
        //    else
        //        $scope.dateOptions.minDate = null;
        //}
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
                    if (value.Id == '07') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $.each(filterData, function (index, value) {
                    if (value.Id == '08') {
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
                        type: "stockout"
                    }
                }
                apiService.get('/api/stockin/updatecancelstockout', config, function (result) {
                    if (result.data.Id > 0) {
                        notificationService.displaySuccess("Cập nhật trạng thái huỷ thành công!")
                        search($scope.page);
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            });
        }
        function loadBranches() {
            $scope.loading = true;
            apiService.get('/api/branch/getall', null, function (result) {
                var filterData = result.data;
                $.each(filterData, function (index, value) {
                    if (value.Id == $scope.branchSelectedRoot.Id) {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $scope.branchFilters = filterData;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function outBook(val) {
            $scope.adding = true;
            $ngBootbox.confirm('Bạn có chắc chắn muốn xuất kho đơn ' + val.Id + ' ?').then(function () {
                var config = {
                    params: {
                        stockinId: val.Id,
                        userId: $scope.userId
                    }
                }
                apiService.get('api/stockin/outexist', config,
                        function (result) {
                            if (result.data > 0) {
                                $scope.adding = false;
                                //$scope.chatHub.server.send(result.data);
                                notificationService.displaySuccess('Đơn đã được xuất kho thành công.');
                                search($scope.page);
                            }

                        }, function (error) {
                            notificationService.displayError(error.data);
                        });
            }, function () {
                $scope.adding = false;
            });
        }
        function updateOutStock(val) {
            var config = {
                params: {
                    stockinId: val.Id,
                    userId: $scope.userId
                }
            }
            apiService.get('/api/stockin/updateoutstock', config, function (result) {
                if (result.data > 0) {
                    //$scope.chatHub.server.send(result.data);
                    notificationService.displaySuccess("Cập nhật trạng thái tồn không đủ xuất thành công!")
                    search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadStockinCategories() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallcategories', null, function (result) {
                var filterData = result.data;
                $.each(filterData, function (index, value) {
                    if (value.Id == '00') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $.each(filterData, function (index, value) {
                    if (value.Id == '02') {
                        filterData.splice(index, 1);
                        return false;
                    }
                });
                $scope.stockinCategoryFilters = filterData;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }
        function resetStockoutTimeFilter() {
            $scope.startStockoutDateFilter = null;
            $scope.endStockoutDateFilter = null;
            search();
        }
        function getDetailStockoutToPrint(stockout) {
            $scope.stockoutReturn = {};
            var config = {
                params: {
                    stockoutId: stockout.Id
                }
            }
            apiService.get('/api/stockin/detailstockouttoprint', config, function (result) {
                $scope.stockoutReturn = result.data;
                var sum = 0;
                var sumQuan = 0;
                $.each($scope.stockoutReturn.SoftStockInDetails, function (index, value) {
                    sum += value.Quantity * value.PriceNew;
                    sumQuan += value.Quantity;
                });
                $scope.totalMoneyPrint = sum;
                $scope.totalQuantity = sumQuan;
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
                search();
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function addStamp(item) {
            $scope.loading = true;
            var config = {
                params: {
                    stockinId: item.Id
                }
            }
            apiService.get('/api/stockin/detailstockintoaddstamp', config, function (result) {
                if (sessionStorage.getItem("arrayPrint")) {
                    var arrayPrint = JSON.parse(sessionStorage.getItem("arrayPrint"));
                    if (arrayPrint.length > 0) {
                        $.each(result.data, function (index, value) {
                            var exist = false;
                            $.each(arrayPrint, function (indexRoot, valueRoot) {
                                if (valueRoot.id == value.id) {
                                    valueRoot.Quantity += value.Quantity;
                                    exist = true;
                                    return false;
                                }
                            });
                            if (exist == false)
                                arrayPrint.push(value);
                        });
                        sessionStorage.setItem("arrayPrint", JSON.stringify(arrayPrint));
                    }
                    else
                        sessionStorage.setItem("arrayPrint", JSON.stringify(result.data));
                }
                else
                    sessionStorage.setItem("arrayPrint", JSON.stringify(result.data));
                $scope.loading = false;
                //$state.go('add_stamp');
                var url = $state.href('add_stamp');
                window.open(url, '_blank');
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function clearStamp() {
            $scope.arrayPrint = [];
            sessionStorage.removeItem('arrayPrint');
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadBranches();
            loadBookStatuses();
            loadStockinCategories();
            init();
            search();
            $window.document.title = "DS đơn xuất kho";
        }
    }
})(angular.module('softbbm.stockouts'));