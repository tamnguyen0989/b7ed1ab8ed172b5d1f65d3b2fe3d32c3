(function (app) {
    app.controller('stockinAddController', stockinAddController);

    stockinAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$rootScope', '$ngBootbox', '$uibModal', '$q'];

    function stockinAddController(apiService, $window, $scope, notificationService, $state, $rootScope, $ngBootbox, $uibModal, $q) {
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
        $scope.adding = false;

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
        $scope.addBook = addBook;
        $scope.updatePaymentStatus = updatePaymentStatus;
        $scope.validPaymentMethod = validPaymentMethod;

        function loadSuppliers() {
            $scope.loading = true;
            var config = {
                params: {
                    order: "Name"
                }

            }
            apiService.get('/api/supplier/getallstockinadd', config, function (result) {
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
                $scope.stockin.TotalQuantity = sumQuantity();
                $scope.stockin.ToBranchId = $scope.branchSelectedRoot.Id;
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
        function addBook() {
            if ($scope.bookDetails.length > 0) {
                $scope.adding = true;
                $scope.stockin.BranchId = $scope.branchSelectedRoot.Id;
                $scope.stockin.CategoryId = '02';
                $scope.stockin.Total = sumMoney();
                $scope.stockin.TotalQuantity = sumQuantity();
                $scope.stockin.ToBranchId = $scope.branchSelectedRoot.Id;
                $scope.stockin.SoftStockInDetails = $scope.bookDetails;
                $scope.stockin.Description = $scope.description;

                var paymentStatusId = null;
                if ($scope.selectedPaymentStatus) {
                    paymentStatusId = $scope.selectedPaymentStatus.Id;
                }
                $scope.stockin.PaymentStatusId = paymentStatusId;

                var paymentMethodId = null;
                if ($scope.selectedPaymentMethod)
                    paymentMethodId = $scope.selectedPaymentMethod.Id;
                $scope.stockin.PaymentMethodId = paymentMethodId;
                apiService.post('api/stockin/addstockin', $scope.stockin,
                    function (result) {
                        $scope.adding = false;
                        notificationService.displaySuccess('Đơn đã được nhập kho thành công.');
                        if (result.data.stockoutAble == true) {
                            $ngBootbox.confirm('Bạn có muốn xuất đến kho đang thiếu các sản phẩm này?').then(function () {
                                $uibModal.open({
                                    templateUrl: '/app/components/stockins/stockinThenOutModal.html',
                                    controller: 'stockinThenOutController',
                                    scope: $scope,
                                    backdrop: 'static',
                                    keyboard: false
                                }).result.finally(function () {
                                    if (result.data.updatedPrice == true) {
                                        openStockinThenUpdatePrice();
                                    }
                                    else
                                        $state.go('stockins');
                                });
                            }, function () {
                                if (result.data.updatedPrice == true) {
                                    openStockinThenUpdatePrice();
                                }
                                else
                                    $state.go('stockins');
                            });
                        }
                        else if (result.data.updatedPrice == true) {
                            if (result.data.updatedPrice == true) {
                                openStockinThenUpdatePrice();
                            }
                            else
                                $state.go('stockins');
                        }
                        else
                            $state.go('stockins');
                    }, function (error) {
                        notificationService.displayError(error.data);
                    });
            }
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.stockin.CreatedBy = JSON.parse(localStorage.getItem("userId"));
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
        }
        function openStockinThenUpdatePrice() {
            $uibModal.open({
                templateUrl: '/app/components/stockins/stockinThenUpdatePriceModal.html',
                controller: 'stockinThenUpdatePriceController',
                scope: $scope,
                backdrop: 'static',
                keyboard: false
            }).result.finally(function () {
                $state.go('stockins');
            });
        }
        function loadPaymentStatuses() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallpaymentstatus', null, function (result) {
                $scope.paymentStatuses = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        };
        function loadPaymentMethods() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallpaymentmethod', null, function (result) {
                $scope.paymentMethods = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        };
        function updatePaymentStatus() {
            $scope.selectedPaymentMethod = null;
        }
        function validPaymentMethod() {
            if ($scope.selectedPaymentStatus) {
                if ($scope.selectedPaymentStatus.Id == 1)
                    return true
                else
                    return false
            }
            else
                return false
        }


        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            $window.document.title = "Thêm mới đơn nhập kho";
            loadSuppliers();
            loadProductStatus();
            loadPaymentStatuses();
            loadPaymentMethods();
            init();
        }

    }


})(angular.module('softbbm.stockins'));