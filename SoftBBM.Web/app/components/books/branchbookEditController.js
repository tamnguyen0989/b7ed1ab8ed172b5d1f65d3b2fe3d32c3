(function (app) {
    app.controller('branchbookEditController', branchbookEditController);

    branchbookEditController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$stateParams', '$window'];

    function branchbookEditController(apiService, $window, $scope, notificationService, $state, $stateParams, $window) {
        $scope.stockin = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.stringSearch = '';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '5'
        $scope.searchedProducts = [];
        $scope.productStatus = [];
        $scope.filters = [];
        $scope.bookDetails = [];
        $scope.loading = true;
        $scope.description = '';
        $scope.branches = [];
        $scope.selectedBranch = null;

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
        $scope.updateBook = updateBook;
        $scope.loadBookDetail = loadBookDetail;
        $scope.loadBranches = loadBranches;
        $scope.updateSelectedStock = updateSelectedStock;
        function loadSuppliers() {
            $scope.loading = true;
            var config = {
                params: {
                    order: "Name"
                }

            }
            apiService.get('/api/supplier/getall', config, function (result) {
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
                val.SelectedStockTotal = null;
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
        function updateBook() {
            if ($scope.bookDetails.length > 0) {
                $scope.stockin.Total = sumMoney();
                $scope.stockin.TotalQuantity = sumQuantity();
                $scope.stockin.SoftStockInDetails = $scope.bookDetails;
                $scope.stockin.Description = $scope.description;
                $scope.stockin.FromBranchId = $scope.selectedBranch.Id;
                apiService.post('api/stockin/updatebook', $scope.stockin,
                    function (result) {
                        if (result.data == true) {
                            //$scope.chatHub.server.send($scope.selectedBranch.Id);
                            notificationService.displaySuccess('Đơn đã được cập nhật.');
                            $state.go('branch_books');
                        }
                    }, function (error) {
                        notificationService.displayError(error.data);
                    });
            }
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.stockin.UpdatedBy = JSON.parse(localStorage.getItem("userId"));
            }
        }
        function loadBookDetail() {
            $scope.loading = true;
            var config = {
                params: {
                    stockinId: parseInt($stateParams.stockinId),
                    branchId: $scope.branchSelectedRoot.Id
                }
            }
            apiService.get('/api/stockin/detaileditbranchbook', config, function (result) {
                if (result.data.SupplierStatusId == '08' || result.data.SupplierStatusId == '02' || result.data.CategoryId != '01') {
                    notificationService.displayError('Đơn đã được cập nhật.');
                    $state.go('branch_books');
                }
                if (result.data.BranchId != $scope.branchSelectedRoot.Id) {
                    notificationService.displayError('404 not found!');
                    $state.go('home');
                }
                $scope.stockin.Id = result.data.Id;
                $scope.bookDetails = result.data.SoftStockInDetails;
                $scope.description = result.data.Description;
                $scope.categoryId = result.data.CategoryId;
                $.each($scope.branches, function (index, value) {
                    if (value.Id == result.data.FromBranchId) {
                        $scope.selectedBranch = value;
                        var fromBranchId = value.Id;
                        $.each($scope.bookDetails, function (indexBookDetails, valueBookDetails) {
                            var stockTmp = 0;
                            var stringTmps = valueBookDetails.shop_sanpham.SoftBranchProductStocks;
                            valueBookDetails.SoftBranchProductStocks = stringTmps;
                            $.each(stringTmps, function (indexStock, valStock) {

                                if (valStock.BranchId == fromBranchId) {
                                    stockTmp = valStock.StockTotal;
                                    return false;
                                }
                            });
                            valueBookDetails.SelectedStockTotal = stockTmp;
                        });
                        return false;
                    }
                });
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
                $scope.branches = filterData;
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

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadSuppliers();
            loadProductStatus();
            loadBranches();
            init();
            loadBookDetail();
            $window.document.title = "Cập nhật đơn ĐH Kho nội bộ";
        }
    }

})(angular.module('softbbm.books'));