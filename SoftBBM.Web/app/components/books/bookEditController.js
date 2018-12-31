(function (app) {
    app.controller('bookEditController', bookEditController);

    bookEditController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$stateParams', '$window'];

    function bookEditController(apiService, $window, $scope, notificationService, $state, $stateParams, $window) {
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
        $scope.description = '';

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
        $scope.updateBook = updateBook;
        $scope.loadBookDetail = loadBookDetail;
        $scope.back = back;
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
                            $window.history.back();
                            //$state.go('books');
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
            apiService.get('/api/stockin/detaileditbook', config, function (result) {
                if (result.data.SupplierStatusId == '08' || result.data.SupplierStatusId == '02' || result.data.CategoryId != '00') {
                    notificationService.displayError('Đơn đã được cập nhật.');
                    $state.go('books');
                }
                if (result.data.BranchId != $scope.branchSelectedRoot.Id) {
                    notificationService.displayError('404 not found!');
                    $state.go('home');
                }
                $scope.stockin.Id = result.data.Id;
                $window.document.title = "Cập nhật đơn đặt hàng NCC - " + $scope.stockin.Id;
                $scope.bookDetails = result.data.SoftStockInDetails;
                $scope.description = result.data.Description;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function back() {
            $window.history.back();
        }


        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadSuppliers();
            loadProductStatus();
            init();
            loadBookDetail();
            $window.document.title = "Cập nhật đơn ĐH nhà cung cấp";
        }
        
    }

})(angular.module('softbbm.books'));