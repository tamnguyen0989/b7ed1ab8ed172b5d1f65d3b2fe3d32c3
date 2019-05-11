(function (app) {
    app.controller('stockoutAddController', stockoutAddController);

    stockoutAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$timeout', '$uibModal'];

    function stockoutAddController(apiService, $window, $scope, notificationService, $state, $timeout, $uibModal) {
        $scope.stockin = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.stringSearch = '';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '5'
        $scope.searchedProducts = [];
        $scope.suppliers = [];
        $scope.productStatus = [];
        $scope.filters = [];
        $scope.bookDetails = [];
        $scope.loading = true;
        $scope.branches = [];
        $scope.selectedBranch = null;
        $scope.thenOut = null;
        $scope.stockoutReturn = {
        }
        $scope.totalQuantity = 0;
        $scope.totalMoneyPrint = 0;
        $scope.adding = false;

        //$scope.chatHub = null;
        //$scope.chatHub = $.connection.chatHub;
        //$.connection.hub.start();

        $scope.loadSuppliers = loadSuppliers;
        $scope.loadProductStatus = loadProductStatus;
        $scope.search = search;
        $scope.addBookDetails = addBookDetails;
        $scope.addFilter = addFilter;
        $scope.removeFilter = removeFilter;
        $scope.sumQuantity = sumQuantity;
        $scope.sumMoney = sumMoney;
        $scope.removeBookDetail = removeBookDetail
        $scope.init = init;
        $scope.saveBook = saveBook;
        $scope.addBook = addBook;
        $scope.loadBranches = loadBranches;
        $scope.updateSelectedStock = updateSelectedStock;
        $scope.getSoldProductsByDate = getSoldProductsByDate;
        $scope.clearBookDetails = clearBookDetails;

        function loadSuppliers() {
            $scope.loading = true;
            var config = {
                params: {
                    order: "Name"
                }

            }
            apiService.get('/api/supplier/getallstockoutadd', config, function (result) {
                $scope.suppliers = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadProductStatus() {
            $scope.loading = true;
            apiService.get('/api/product/getallproductstatus', null, function (result) {
                $scope.productStatus = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function search(page) {
            page = page || 0;

            $scope.loading = true;
            var config = {
                branchId: $scope.branchSelectedRoot.Id,
                stringSearch: $scope.stringSearch,
                page: page,
                pageSize: parseInt($scope.pageSizeNumber),
                FilterBookDetail: $scope.filters
            };
            apiService.post('/api/product/getallpaging/', config, function (result) {
                $scope.searchedProducts = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterStockIns && $scope.filterStockIns.length && $scope.page == 0) {
                    //notificationService.displayInfo($scope.totalCount + ' đơn tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function addBookDetails(val) {
            var exist = 0;
            if ($scope.bookDetails.length > 0) {
                $.each($scope.bookDetails, function (index, value) {
                    if (value.id == val.id) {
                        exist = 1;
                        value.Quantity += 1;
                        return false;
                    }
                });
                if (exist == 0) {
                    val.Quantity = 1;
                    $scope.bookDetails.push(val);
                    updateSelectedStock($scope.selectedBranch);
                }
            }
            else {
                val.Quantity = 1;
                $scope.bookDetails.push(val);
                updateSelectedStock($scope.selectedBranch);
            }
        }
        function addFilter(val, key) {
            var newFilter = {
                key: key,
                value: val.Id,
                name: val.Name
            }
            switch (key) {
                case 0:
                    newFilter.alias = 'Nhà cung cấp';
                    break;
                case 1:
                    newFilter.alias = 'Trạng thái';
                    break;
            }
            if ($scope.filters.length > 0) {
                var exist = 0;
                $.each($scope.filters, function (index, data) {
                    if (newFilter.alias == data.alias && newFilter.key == data.key && newFilter.value == data.value) {
                        exist = 1;
                        return false;
                    }
                });
                if (exist == 0)
                    $scope.filters.push(newFilter);
            }
            else
                $scope.filters.push(newFilter);
            search();
        }
        function removeFilter(val) {
            $scope.filters.splice(val, 1);
            search();
        }
        function removeBookDetail(index) {
            $scope.bookDetails.splice(index, 1);
        }
        function sumQuantity() {
            var total = 0;
            $.each($scope.bookDetails, function (index, item) {
                total += item.Quantity;
            });
            return total;
        }
        function sumMoney() {
            var total = 0;
            $.each($scope.bookDetails, function (index, item) {
                total += item.Quantity * item.PriceBase;
            });
            return total;
        }
        function saveBook() {
            if ($scope.bookDetails.length > 0) {
                $scope.adding = true;
                $scope.stockin.BranchId = $scope.branchSelectedRoot.Id;
                $scope.stockin.CategoryId = '03';
                $scope.stockin.Total = sumMoney();
                $scope.stockin.TotalQuantity = sumQuantity();
                $scope.stockin.FromBranchId = $scope.branchSelectedRoot.Id;
                $scope.stockin.ToBranchId = $scope.selectedBranch.Id;
                $scope.stockin.SoftStockInDetails = $scope.bookDetails;
                $scope.stockin.Description = $scope.description;
                apiService.post('api/stockin/savestockout', $scope.stockin,
                    function (result) {
                        $scope.adding = false;
                        notificationService.displaySuccess('Đơn đã được lưu tạm.');
                        $state.go('stockouts');
                    }, function (error) {
                        notificationService.displayError(error.data);
                    });
            }
        }
        function addBook() {
            if ($scope.bookDetails.length > 0) {
                $scope.adding = true;
                $scope.stockin.BranchId = $scope.branchSelectedRoot.Id;
                $scope.stockin.CategoryId = '03';
                $scope.stockin.Total = sumMoney();
                $scope.stockin.TotalQuantity = sumQuantity();
                $scope.stockin.FromBranchId = $scope.branchSelectedRoot.Id;
                $scope.stockin.ToBranchId = $scope.selectedBranch.Id;
                $scope.stockin.SoftStockInDetails = $scope.bookDetails;
                $scope.stockin.Description = $scope.description;
                apiService.post('api/stockin/addstockout', $scope.stockin,
                    function (result) {
                        if (result.data.Id > 0) {
                            $scope.adding = false;
                            $scope.stockoutReturn = {};
                            $scope.totalQuantity = 0;
                            $scope.totalMoneyPrint = 0;
                            $scope.stockoutReturn = result.data;
                            //$scope.chatHub.server.send(result.data.ToBranchId);
                            notificationService.displaySuccess('Đơn đã được xuất kho thành công.');
                            $scope.thenOut = null;
                            localStorage.removeItem("thenOut");
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
                            refeshPage();
                            //$state.go('stockouts');
                        }

                    }, function (error) {
                        notificationService.displayError(error.data);
                    });
            }
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.stockin.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
            if (localStorage.getItem("thenOut")) {
                $scope.thenOut = JSON.parse(localStorage.getItem("thenOut"));
                $scope.selectedBranch = $scope.thenOut.ToBranch;
                $scope.bookDetails = $scope.thenOut.Products;
                updateSelectedStock($scope.selectedBranch);
            }
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
                $scope.branches = filterData;
                $scope.selectedBranch = $scope.branches[0];
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function updateSelectedStock(val) {
            if (val) {
                var fromBranchId = val.Id;
                $.each($scope.bookDetails, function (index, value) {
                    var stockTmp = 0;
                    $.each(value.SoftBranchProductStocks, function (indexStock, valStock) {
                        if (valStock.BranchId == fromBranchId) {
                            stockTmp = valStock.StockTotal;
                            return false;
                        }
                    });
                    value.SelectedStockTotal = stockTmp;
                });
            }
            else {
                $.each($scope.bookDetails, function (index, value) {
                    value.SelectedStockTotal = null;
                });
            }
        }
        function getSoldProductsByDate() {
            var modalInstance = $uibModal.open({
                templateUrl: '/app/components/stockouts/soldProductsByDateModal.html',
                controller: 'soldProductsByDateController',
                scope: $scope
            });
            modalInstance.result.then(function (data) {
                $scope.bookDetails = data;
                updateSelectedStock($scope.selectedBranch);
            }, function () {

            });
        }
        function clearBookDetails() {
            $scope.bookDetails = [];
            $scope.thenOut = null;
            localStorage.removeItem("thenOut");
        }
        function refeshPage() {
            $scope.searchedProducts = [];
            $scope.stringSearch = '';
            $scope.page = 0;
            $scope.pagesCount = 0;
            $scope.pageSizeNumber = '5';
            $scope.filters = [];
            $scope.bookDetails = [];
            $scope.thenOut = null;
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadSuppliers();
            loadProductStatus();
            loadBranches();
            init();
            $window.document.title = "Thêm mới đơn xuất kho";
        }
    }

})(angular.module('softbbm.stockouts'));