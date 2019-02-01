(function (app) {
    app.controller('priceExportController', priceExportController);

    priceExportController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$timeout', '$uibModal'];

    function priceExportController(apiService, $window, $scope, notificationService, $state, $timeout, $uibModal) {
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

        $scope.loadSuppliers = loadSuppliers;
        $scope.loadProductStatus = loadProductStatus;
        $scope.search = search;
        $scope.addBookDetails = addBookDetails;
        $scope.addFilter = addFilter;
        $scope.removeFilter = removeFilter;
        $scope.removeBookDetail = removeBookDetail
        $scope.init = init;
        $scope.exportPrice = exportPrice;
        $scope.exportPriceExcel = exportPriceExcel;

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
            apiService.post('/api/product/getallpagingexport/', config, function (result) {
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
                        return false;
                    }
                });
                if (exist == 0) {
                    $scope.bookDetails.push(val);
                }
            }
            else {
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
        function exportPrice() {
            if ($scope.bookDetails.length > 0) {
                var params = {
                    exportPriceParamsDetails: $scope.bookDetails
                }
                apiService.post('api/product/exportprice', params,
                    function (result) {
                        //notificationService.displaySuccess('Đơn đã được lưu tạm.');
                        //$state.go('stockouts');
                    }, function (error) {
                        notificationService.displayError(error.data);
                    });
            }
        }
        function init() {
        
        }
        function refeshPage() {
            $scope.searchedProducts = [];
            $scope.stringSearch = '';
            $scope.page = 0;
            $scope.pagesCount = 0;
            $scope.pageSizeNumber = '5';
            $scope.filters = [];
            $scope.bookDetails = [];
        }
        function exportPriceExcel() {
            if ($scope.bookDetails.length > 0) {
                $uibModal.open({
                    templateUrl: '/app/components/exports/priceExportProcessModal.html',
                    controller: 'priceExportProcessController',
                    scope: $scope,
                    backdrop: 'static',
                    keyboard: false,
                    size: 'sm'
                }).result.finally(function ($scope) {

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
            init();
            $window.document.title = "Export giá sỉ";
        }
    }

})(angular.module('softbbm.exports'));