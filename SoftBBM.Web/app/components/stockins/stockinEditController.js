(function (app) {
    app.controller('stockinEditController', stockinEditController);

    stockinEditController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$stateParams'];

    function stockinEditController(apiService, $window, $scope, notificationService, $state, $stateParams) {
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
        $scope.selectedBranch = {};

        $scope.loadSuppliers = loadSuppliers;
        $scope.loadProductStatus = loadProductStatus;
        $scope.search = search;
        $scope.addBookDetails = addBookDetails;
        $scope.addFilter = addFilter;
        $scope.removeFilter = removeFilter;
        $scope.sumMoney = sumMoney;
        $scope.sumQuantity = sumQuantity;
        $scope.removeBookDetail = removeBookDetail
        $scope.init = init;
        $scope.saveBook = saveBook;
        $scope.loadBranches = loadBranches;
        $scope.loadBookDetail = loadBookDetail;
        $scope.updateBook = updateBook;
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
                }
            }
            else {
                val.Quantity = 1;
                $scope.bookDetails.push(val);
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
        function sumMoney() {
            var total = 0;
            $.each($scope.bookDetails, function (index, item) {
                if (item.PriceNew)
                    total += item.PriceNew * item.Quantity;
            });

            return total;
        }
        function sumQuantity() {
            var total = 0;
            $.each($scope.bookDetails, function (index, item) {
                    total += item.Quantity;
            });

            return total;
        }
        function saveBook() {
            if ($scope.bookDetails.length > 0) {
                $scope.stockin.BranchId = $scope.branchSelectedRoot.Id;
                $scope.stockin.CategoryId = '02';
                $scope.stockin.Total = sumMoney();
                $scope.stockin.SoftStockInDetails = $scope.bookDetails;
                $scope.stockin.Description = $scope.description;
                apiService.post('api/stockin/save', $scope.stockin,
                    function (result) {
                        if (result.data > 0) {
                            notificationService.displayWarning('Đơn đã được lưu tạm.');
                            $state.go('stockins');
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
        function loadBookDetail() {
            $scope.loading = true;
            var config = {
                params: {
                    stockinId: parseInt($stateParams.stockinId),
                    branchId: $scope.branchSelectedRoot.Id
                }
            }
            apiService.get('/api/stockin/detaileditstockin', config, function (result) {
                if (result.data.FromBranchStatusId == '07' || result.data.ToBranchStatusId == '02' || result.data.CategoryId != '02') {
                    notificationService.displayError('Đơn đã được cập nhật.');
                    $state.go('stockins');
                }
                if (result.data.BranchId != $scope.branchSelectedRoot.Id) {
                    notificationService.displayError('404 not found!');
                    $state.go('home');
                }
                $scope.stockin.Id = result.data.Id;
                $window.document.title = "Cập nhật đơn nhập kho - " + $scope.stockin.Id;
                $scope.bookDetails = result.data.SoftStockInDetails;
                $scope.description = result.data.Description;
                $scope.categoryId = result.data.CategoryId;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }        
        function updateBook() {
            if ($scope.bookDetails.length > 0) {
                $scope.stockin.Total = sumMoney();
                $scope.stockin.TotalQuantity = sumQuantity();
                $scope.stockin.SoftStockInDetails = $scope.bookDetails;
                $scope.stockin.Description = $scope.description;

                apiService.post('api/stockin/updatebook', $scope.stockin,
                    function (result) {
                        if (result.data == true) {
                            notificationService.displaySuccess('Đơn đã được cập nhật.');
                            //$window.history.back();
                            $state.go('stockins');
                        }
                    }, function (error) {
                        notificationService.displayError(error.data);
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
        }
        
    }


})(angular.module('softbbm.stockins'));