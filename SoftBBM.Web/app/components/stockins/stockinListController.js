(function (app) {
    app.controller('stockinListController', stockinListController);

    stockinListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state', '$rootScope']
    function stockinListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state, $rootScope) {
        $rootScope.arrayPrint = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10'
        $scope.animate = false;
        $scope.filters = {
            selectedStockinCategoryFilters: [],
            selectedBranchFilters: [],
            selectedSupplierFilters: [],
            selectedBookStatusFilters: [],
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
        $scope.supplierFilters = [];
        $scope.categoryId = '00';
        $scope.startDateFilter = null;
        $scope.endDateFilter = null;
        $scope.startStockinDateFilter = null;
        $scope.endStockinDateFilter = null;
        $scope.bookDetails = [];
        $scope.dynamicPopover = {
            content: 'Hello, World!',
            templateUrl: 'myPopoverTemplate.html',
            title: 'Title'
        };
        $scope.enabledPopover = [];
        $scope.selectedPaymentPopover = null;
        $scope.paymentStatusFilters = [
            { Id: 1, Name: 'Đã trả' },
            { Id: 2, Name: 'Chưa trả' }
        ]
        $scope.adding = false;

        $scope.search = search;
        $scope.refeshPage = refeshPage;
        $scope.bookDetail = bookDetail;
        $scope.updateFilter = updateFilter;
        $scope.sortBy = sortBy;
        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.openStartStockinDateFilter = openStartStockinDateFilter;
        $scope.openEndStockinDateFilter = openEndStockinDateFilter;
        $scope.updateDateFilter = updateDateFilter;
        $scope.setMinDate = setMinDate;
        $scope.loadBookStatuses = loadBookStatuses;
        $scope.loadSuppliers = loadSuppliers;
        $scope.init = init;
        $scope.loadFilter = loadFilter;
        $scope.updateCancel = updateCancel;
        $scope.addBook = addBook;
        $scope.loadStockinCategories = loadStockinCategories;
        $scope.resetTimeFilter = resetTimeFilter;
        $scope.resetStockinTimeFilter = resetStockinTimeFilter;
        $scope.addStamp = addStamp;
        $scope.getDetailStockinToPrint = getDetailStockinToPrint;
        $scope.openPaymentPopover = openPaymentPopover;
        $scope.closePaymentPopover = closePaymentPopover;
        $scope.updatePaymentPopover = updatePaymentPopover;
        $scope.updateSelectedPaymentMethod = updateSelectedPaymentMethod;
        $scope.clearStamp = clearStamp;
        $scope.authenExport = authenExport;

        function search(page) {
            page = page || 0;
            $scope.loading = true;
            $scope.filters.page = page;
            $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
            $scope.filters.filter = $scope.filterBooks;
            //$scope.filters.categoryId = $scope.categoryId;
            $scope.filters.toBranchId = $scope.branchSelectedRoot.Id;
            $scope.filters.startDateFilter = $scope.startDateFilter;
            $scope.filters.endDateFilter = $scope.endDateFilter;
            $scope.filters.startStockinDateFilter = $scope.startStockinDateFilter;
            $scope.filters.endStockinDateFilter = $scope.endStockinDateFilter;
            $scope.filters.isListStockin = 1;
            apiService.post('/api/stockin/searchstockin', $scope.filters, function (result) {
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
            $scope.filters.selectedStockinCategoryFilters = [];
            $scope.filters.selectedBranchFilters = [];
            $scope.filters.selectedSupplierFilters = [];
            $scope.filters.selectedBookStatusFilters = [];
            $scope.filters.selectedPaymentStatusFilters = [];
            $scope.filters.sortBy = '';
            $scope.filters.startDateFilter = null;
            $scope.filters.endDateFilter = null;
            $scope.filters.startStockinDateFilter = null;
            $scope.filters.endStockinDateFilter = null;
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            $scope.startStockinDateFilter = null;
            $scope.endStockinDateFilter = null;

            $scope.sortTotal = false;
            $scope.CreatedDate = false;
            search();
        }
        function bookDetail(selectedBook) {
            $scope.selectedBook = selectedBook;
            var modalInstance = $uibModal.open({
                templateUrl: '/app/components/stockins/stockinDetailModal.html' + BuildVersion,
                controller: 'stockinDetailController',
                scope: $scope,
                windowClass: 'app-modal-window-medium'
            });
            modalInstance.result.then(function (data) {
                if (data == 1) {
                    search($scope.page);
                }
            }, function () {

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
        function openStartStockinDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.startStockinDateFilter = true;

        }
        function openEndStockinDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.endStockinDateFilter = true;

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
                var type = 'stockin';
                if (val.CategoryId == '00') {
                    type = 'stockinbook';
                }
                var config = {
                    params: {
                        stockinId: val.Id,
                        userId: $scope.userId,
                        type: type
                    }
                }
                apiService.get('/api/stockin/updatecancelstockin', config, function (result) {
                    if (result.data.Id > 0) {
                        notificationService.displaySuccess("Cập nhật trạng thái huỷ thành công!");
                        search($scope.page);
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            });
        }
        function addBook(val) {
            $scope.adding = true;
            $scope.bookDetails = [];
            $scope.selectedStockin = val;
            $ngBootbox.confirm('Bạn có chắc chắn muốn nhập kho đơn ' + val.Id + ' ?').then(function () {
                if (val.CategoryId == '00') {
                    $uibModal.open({
                        templateUrl: '/app/components/stockins/stockinPaymentModal.html' + BuildVersion,
                        controller: 'stockinPaymentController',
                        scope: $scope,
                        backdrop: 'static',
                        keyboard: false
                    }).result.finally(function () {
                        $scope.selectedStockin = null;
                        var config = {
                            params: {
                                stockinId: val.Id,
                                userId: $scope.userId
                            }
                        }
                        apiService.get('api/stockin/addexist', config,
                            function (result) {
                                $scope.adding = false;
                                notificationService.displaySuccess('Đơn đã được nhập kho thành công.');
                                $scope.bookDetails = result.data.SoftStockInDetails;
                                if (result.data.stockoutAble == true) {
                                    $ngBootbox.confirm('Bạn có muốn xuất đến kho đang thiếu các sản phẩm này?').then(function () {

                                        $uibModal.open({
                                            templateUrl: '/app/components/stockins/stockinThenOutModal.html' + BuildVersion,
                                            controller: 'stockinThenOutController',
                                            scope: $scope,
                                            backdrop: 'static',
                                            keyboard: false
                                        }).result.finally(function () {
                                            if (result.data.updatedPrice == true) {
                                                openStockinThenUpdatePrice();
                                            }
                                            else
                                                search($scope.page);
                                        });
                                    }, function () {
                                        if (result.data.updatedPrice == true) {
                                            openStockinThenUpdatePrice();
                                        }
                                        else
                                            search($scope.page);
                                    });
                                }
                                else if (result.data.updatedPrice == true) {
                                    if (result.data.updatedPrice == true) {
                                        openStockinThenUpdatePrice();
                                    }
                                    else
                                        search($scope.page);
                                }
                                else
                                    search($scope.page);
                            }, function (error) {
                                notificationService.displayError(error.data);
                            });
                    });
                }
                else {
                    $scope.selectedStockin = null;
                    var config = {
                        params: {
                            stockinId: val.Id,
                            userId: $scope.userId
                        }
                    }
                    apiService.get('api/stockin/addexist', config,
                        function (result) {
                            $scope.adding = false;
                            notificationService.displaySuccess('Đơn đã được nhập kho thành công.');
                            $scope.bookDetails = result.data.SoftStockInDetails;
                            if (result.data.stockoutAble == true) {
                                $ngBootbox.confirm('Bạn có muốn xuất đến kho đang thiếu các sản phẩm này?').then(function () {

                                    $uibModal.open({
                                        templateUrl: '/app/components/stockins/stockinThenOutModal.html' + BuildVersion,
                                        controller: 'stockinThenOutController',
                                        scope: $scope,
                                        backdrop: 'static',
                                        keyboard: false
                                    }).result.finally(function () {
                                        if (result.data.updatedPrice == true) {
                                            openStockinThenUpdatePrice();
                                        }
                                        else
                                            search($scope.page);
                                    });
                                }, function () {
                                    if (result.data.updatedPrice == true) {
                                        openStockinThenUpdatePrice();
                                    }
                                    else
                                        search($scope.page);
                                });
                            }
                            else if (result.data.updatedPrice == true) {
                                if (result.data.updatedPrice == true) {
                                    openStockinThenUpdatePrice();
                                }
                                else
                                    search($scope.page);
                            }
                            else
                                search($scope.page);
                        }, function (error) {
                            notificationService.displayError(error.data);
                        });
                }
            }, function () {
                $scope.adding = false;
            });
        }
        function loadStockinCategories() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallcategories', null, function (result) {
                $scope.stockinCategoryFilters = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
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
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }
        function resetStockinTimeFilter() {
            $scope.startStockinDateFilter = null;
            $scope.endStockinDateFilter = null;
            search();
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
        function getDetailStockinToPrint(stockin) {
            $scope.stockinReturn = {};
            var config = {
                params: {
                    stockinId: stockin.Id
                }
            }
            apiService.get('/api/stockin/detailtoprint', config, function (result) {
                $scope.stockinReturn = result.data;
                var sum = 0;
                var sumQuan = 0;
                $.each($scope.stockinReturn.SoftStockInDetails, function (index, value) {
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
        function openStockinThenUpdatePrice() {
            $uibModal.open({
                templateUrl: '/app/components/stockins/stockinThenUpdatePriceModal.html' + BuildVersion,
                controller: 'stockinThenUpdatePriceController',
                scope: $scope,
                backdrop: 'static',
                keyboard: false
            }).result.finally(function () {
                search($scope.page);
            });
        }
        function openPaymentPopover(stockin) {
            if (stockin.CategoryId == '00' || stockin.CategoryId == '02') {
                if ($scope.selectedStockin)
                    closePaymentPopover();
                $scope.selectedStockin = stockin;
                $scope.selectedPaymentMethod = stockin.SoftStockInPaymentMethod;
                $scope.enabledPopover[stockin.Id] = true;
                //$scope.selectedPaymentPopover = {
                //    priceChannel: $scope.selectedStockin.PriceChannel,
                //    priceDiscount: $scope.selectedStockin.PriceDiscount,
                //    startDateDiscount: $scope.selectedStockin.StartDateDiscount,
                //    endDateDiscount: $scope.selectedStockin.EndDateDiscount,
                //    productId: $scope.selectedStockin.shop_sanpham.id,
                //    channelId: $scope.selectedStockin.Id
                //}
            }
        }
        function closePaymentPopover() {
            if ($scope.selectedStockin)
                $scope.enabledPopover[$scope.selectedStockin.Id] = false;
            $scope.selectedStockin = null;
            $scope.selectedPaymentPopover = null;
        }
        function loadPaymentMethods() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallpaymentmethod', null, function (result) {
                $scope.paymentMethods = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        };
        function updatePaymentPopover() {
            var config = {
                params: {
                    stockinId: $scope.selectedStockin.Id,
                    paymentMethodId: $scope.selectedPaymentMethod.Id,
                    updateBy: $scope.userId
                }
            }
            apiService.get('/api/stockin/updatepaymentpopover', config, function (result) {
                closePaymentPopover();
                notificationService.displaySuccess('Cập nhật phương thức TT thành công!');
                search($scope.page);
            }, function (error) {
                notificationService.displayError(error);
            });

        };
        function updateSelectedPaymentMethod(item) {
            $scope.selectedPaymentMethod = item;
        }
        function clearStamp() {
            $scope.arrayPrint = [];
            sessionStorage.removeItem('arrayPrint');
        }
        function authenExport() {
            apiService.get('api/order/authenexport', null, function (result) {
                confirmStockinsExcel();
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function confirmStockinsExcel() {
            if ($scope.filters.selectedStockinCategoryFilters.length == 0
                && $scope.filters.selectedSupplierFilters.length == 0
                && $scope.filters.selectedBookStatusFilters.length == 0
                && $scope.filters.selectedPaymentStatusFilters.length == 0
                && $scope.filters.startDateFilter == null
                && $scope.filters.endDateFilter == null
                && $scope.filters.startStockinDateFilter == null
                && $scope.filters.endStockinDateFilter == null) {
                if (confirm("Bạn có muốn xuất tất cả đơn nhập kho !")) {
                    exportStockin();
                }
            }
            else {
                exportStockin();
            }
        }
        function exportStockin() {
            $scope.waiting = true;
            $http({
                method: 'POST',
                url: "api/stockin/exportstockinsexcel",
                data: $scope.filters,
                responseType: "arraybuffer"
            }).then(function (result, status, headers, config) {
                var blob = new Blob([result.data], {
                    type: 'application/vnd.ms-excel;charset=charset=utf-8'
                });
                var d = new Date();
                var day = d.getDate();
                if (day.length = 1)
                    day = "0" + day;
                var m = d.getMonth() + 1;
                if (m.length = 1)
                    m = "0" + m;
                var daySuf = day + m + d.getFullYear();
                FileSaver.saveAs(blob, 'NhapKho' + daySuf + '.xlsx');
                $scope.waiting = false;
            });
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadSuppliers();
            loadBookStatuses();
            loadStockinCategories();
            loadBranches();
            loadPaymentMethods();
            init();
            search();
            $window.document.title = "DS đơn nhập kho";
        }

    }
})(angular.module('softbbm.stockins'));