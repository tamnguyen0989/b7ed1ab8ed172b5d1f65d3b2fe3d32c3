(function (app) {
    app.controller('adjustmentStockAddController', adjustmentStockAddController);

    adjustmentStockAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModal'];

    function adjustmentStockAddController(apiService, $window, $scope, notificationService, $state, $uibModal) {
        $scope.adjustmentStock = {
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

        $scope.loadSuppliers = loadSuppliers;
        $scope.loadProductStatus = loadProductStatus;
        $scope.search = search;
        $scope.addBookDetails = addBookDetails;
        $scope.addFilter = addFilter;
        $scope.removeFilter = removeFilter;
        $scope.removeBookDetail = removeBookDetail
        $scope.init = init;
        $scope.addAjustmentStock = addAjustmentStock;
        $scope.openAddStockFilterModal = openAddStockFilterModal;
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
                if ($scope.filteradjustmentStocks && $scope.filteradjustmentStocks.length && $scope.page == 0) {
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
                    val.Quantity = 0;
                    $scope.bookDetails.push(val);
                }
            }
            else {
                val.Quantity = 0;
                $scope.bookDetails.push(val);
            }
        }
        function addFilter(val, key, type) {
            if (key == 2) {
                var newFilter = {
                    key: key,
                    value: val,
                    name: type + ' ' + val,
                    aliasName: type
                }
            }
            else {
                var newFilter = {
                    key: key,
                    value: val.Id,
                    name: val.Name
                }
            }
            switch (key) {
                case 0:
                    newFilter.alias = 'Nhà cung cấp';
                    break;
                case 1:
                    newFilter.alias = 'Trạng thái';
                    break;
                case 2:
                    newFilter.alias = 'Tồn kho';
                    break;
                case 3:
                    newFilter.alias = 'Nhóm sp';
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
        function addAjustmentStock() {
            if ($scope.bookDetails.length > 0) {
                $scope.adjustmentStock.BranchId = $scope.branchSelectedRoot.Id;
                $scope.adjustmentStock.SoftAdjustmentStockDetails = $scope.bookDetails;
                $scope.adjustmentStock.Description = $scope.description;
                apiService.post('api/adjustmentStock/add', $scope.adjustmentStock,
                    function (result) {
                        if (result.data == true) {
                            notificationService.displaySuccess('Thêm mới phiếu thành công.');
                            $state.go('adjustment_stocks');
                        }

                    }, function (error) {
                        notificationService.displayError(error.data);
                    });
            }
        }
        function openAddStockFilterModal() {
            var modalInstance = $uibModal.open({
                templateUrl: '/app/components/stocks/adjustmentStockAddStockFilterModal.html' + BuildVersion,
                controller: 'adjustmentStockAddStockFilterController',
                scope: $scope,
                size: 'sm',
                backdrop: 'static',
                keyboard: false
            });
            modalInstance.result.then(function (data) {
                addFilter(data.value, 2, data.type);
            }, function () {

            });
        }

        function init() {
            if (localStorage.getItem("userId")) {
                $scope.adjustmentStock.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
        }
        function authen() {
            apiService.get('api/adjustmentstock/authenadd', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }

        else {
            authen();
            loadSuppliers();
            loadProductStatus();
            init();
            $window.document.title = "Thêm mới phiếu điều chỉnh";
        }
    }


})(angular.module('softbbm.stocks'));

//(function (app) {
//    app.controller('adjustmentStockAddController', adjustmentStockAddController);

//    adjustmentStockAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModal','$sce'];

//    function adjustmentStockAddController(apiService, $window, $scope, notificationService, $state, $uibModal,$sce) {
//        $scope.adjustmentStock = {
//            CreatedDate: new Date(),
//            Status: true
//        }
//        $scope.stringSearch = '';
//        $scope.page = 0;
//        $scope.pagesCount = 0;
//        $scope.pageSizeNumber = '5'
//        $scope.searchedProducts = [];
//        $scope.suppliers = [];
//        $scope.productStatus = [];
//        $scope.filters = [];
//        $scope.bookDetails = [];
//        $scope.loading = true;
//        $scope.adding = false;
//        $scope.productCategories = [];
//        $scope.message = {
//            Success: null,
//            Error: null
//        };

//        $scope.loadSuppliers = loadSuppliers;
//        $scope.loadProductStatus = loadProductStatus;
//        $scope.search = search;
//        $scope.addBookDetails = addBookDetails;
//        $scope.addFilter = addFilter;
//        $scope.removeFilter = removeFilter;
//        $scope.removeBookDetail = removeBookDetail
//        $scope.init = init;
//        $scope.addAjustmentStock = addAjustmentStock;
//        $scope.openAddStockFilterModal = openAddStockFilterModal;
//        $scope.loadProductCategories = loadProductCategories;
//        $scope.closeSuccessMessage = closeSuccessMessage;
//        $scope.closeErrorMessage = closeErrorMessage;

//        function loadSuppliers() {
//            $scope.loading = true;
//            var config = {
//                params: {
//                    order: "Name"
//                }

//            }
//            apiService.get('/api/supplier/getall', config, function (result) {
//                $scope.suppliers = result.data;
//                $scope.loading = false;
//            }, function (error) {
//                notificationService.displayError(error);
//            });
//        }
//        function loadProductStatus() {
//            $scope.loading = true;
//            apiService.get('/api/product/getallproductstatus', null, function (result) {
//                $scope.productStatus = result.data;
//                $scope.loading = false;
//            }, function (error) {
//                notificationService.displayError(error);
//            });
//        }
//        function search(page) {
//            page = page || 0;

//            $scope.loading = true;
//            var config = {
//                branchId: $scope.branchSelectedRoot.Id,
//                stringSearch: $scope.stringSearch,
//                page: page,
//                pageSize: parseInt($scope.pageSizeNumber),
//                FilterBookDetail: $scope.filters
//            };
//            apiService.post('/api/product/getallpagingstockfilter/', config, function (result) {
//                $scope.searchedProducts = result.data.Items;
//                $scope.page = result.data.Page;
//                $scope.pagesCount = result.data.TotalPages;
//                $scope.totalCount = result.data.TotalCount;
//                $scope.loading = false;
//                if ($scope.filteradjustmentStocks && $scope.filteradjustmentStocks.length && $scope.page == 0) {
//                    //notificationService.displayInfo($scope.totalCount + ' đơn tìm được');
//                }
//            }, function (response) {
//                notificationService.displayError(response.data);
//            });
//        }
//        function addBookDetails(val) {
//            var exist = 0;
//            if ($scope.bookDetails.length > 0) {
//                $.each($scope.bookDetails, function (index, value) {
//                    if (value.id == val.id) {
//                        exist = 1;
//                        value.Quantity += 1;
//                        return false;
//                    }
//                });
//                if (exist == 0) {
//                    val.Quantity = 0;
//                    $scope.bookDetails.push(val);
//                }
//            }
//            else {
//                val.Quantity = 0;
//                $scope.bookDetails.push(val);
//            }
//        }
//        function addFilter(val, key, type) {
//            if (key == 2) {
//                var newFilter = {
//                    key: key,
//                    value: val,
//                    name: type + ' ' + val,
//                    aliasName: type
//                }
//            }
//            else {
//                var newFilter = {
//                    key: key,
//                    value: val.Id,
//                    name: val.Name
//                }
//            }
//            switch (key) {
//                case 0:
//                    newFilter.alias = 'Nhà cung cấp';
//                    break;
//                case 1:
//                    newFilter.alias = 'Trạng thái';
//                    break;
//                case 2:
//                    newFilter.alias = 'Tồn kho';
//                    break;
//                case 3:
//                    newFilter.alias = 'Nhóm sp';
//                    break;
//            }
//            if ($scope.filters.length > 0) {
//                var exist = 0;
//                $.each($scope.filters, function (index, data) {
//                    if (newFilter.alias == data.alias && newFilter.key == data.key && newFilter.value == data.value) {
//                        exist = 1;
//                        return false;
//                    }
//                });
//                if (exist == 0)
//                    $scope.filters.push(newFilter);
//            }
//            else
//                $scope.filters.push(newFilter);
//            search();
//        }
//        function removeFilter(val) {
//            $scope.filters.splice(val, 1);
//            search();
//        }
//        function removeBookDetail(index) {
//            $scope.bookDetails.splice(index, 1);
//        }
//        function addAjustmentStock() {
//            $scope.adding = true;
//            if ($scope.bookDetails.length > 0) {
//                $scope.adjustmentStock.BranchId = $scope.branchSelectedRoot.Id;
//                $scope.adjustmentStock.SoftAdjustmentStockDetails = $scope.bookDetails;
//                $scope.adjustmentStock.Description = $scope.description;
//                apiService.post('api/adjustmentStock/add', $scope.adjustmentStock,
//                    function (result) {
//                        //if (result.data == true) {
//                        //    notificationService.displaySuccess('Thêm mới phiếu thành công.');
//                        //    $state.go('adjustment_stocks');
//                        //}
//                        $scope.adding = false;
//                        $scope.bookDetails = [];
//                        $scope.description = null;
//                        $scope.message = {
//                            Success: $sce.trustAsHtml(result.data.Success),
//                            Error: $sce.trustAsHtml(result.data.Error)
//                        };

//                    }, function (error) {
//                        notificationService.displayError(error.data);
//                    });
//            }
//        }
//        function init() {
//            if (localStorage.getItem("userId")) {
//                $scope.adjustmentStock.CreatedBy = JSON.parse(localStorage.getItem("userId"));
//            }
//        }
//        function authen() {
//            apiService.get('api/adjustmentstock/authenadd', null, function (result) {

//            }, function (error) {
//                notificationService.displayError(error.data);
//            });
//        }
//        function openAddStockFilterModal() {
//            var modalInstance = $uibModal.open({
//                templateUrl: '/app/components/stocks/adjustmentStockAddStockFilterModal.html' + BuildVersion,
//                controller: 'adjustmentStockAddStockFilterController',
//                scope: $scope,
//                size: 'sm',
//                backdrop: 'static',
//                keyboard: false
//            });
//            modalInstance.result.then(function (data) {
//                addFilter(data.value, 2, data.type);
//            }, function () {

//            });
//        }
//        function loadProductCategories() {
//            $scope.loading = true;
//            apiService.get('/api/productcategory/getall', null, function (result) {
//                $scope.productCategories = result.data;
//                $scope.loading = false;
//            }, function (error) {
//                notificationService.displayError(error);
//            });
//        }
//        function closeSuccessMessage() {
//            $scope.message.Success = null;
//        }
//        function closeErrorMessage() {
//            $scope.message.Error = null;
//        }

//        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
//            notificationService.displayError("Hãy chọn kho");
//            $state.go('home');
//        }

//        else {
//            authen();
//            loadSuppliers();
//            loadProductStatus();
//            loadProductCategories();

//            init();
//            $window.document.title = "Thêm mới phiếu điều chỉnh";
//        }
//    }


//})(angular.module('softbbm.stocks'));