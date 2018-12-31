(function (app) {
    app.controller('inputstockinEditController', inputstockinEditController);

    inputstockinEditController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$stateParams'];

    function inputstockinEditController(apiService, $window, $scope, notificationService, $state, $stateParams) {
        $scope.stockin = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.suppliers = [];
        $scope.selectedSupplier = {};
        $scope.orders = [];
        $scope.addedProductIds = [];
        $scope.product = {};
        $scope.loading = true;
        $scope.detailStockIns = [];
        $scope.isAdding = false;

        $scope.updateStockIn = updateStockIn;
        $scope.loadSuppliers = loadSuppliers;
        $scope.loadordersBySupplier = loadordersBySupplier;
        $scope.loadProduct = loadProduct;
        $scope.addToList = addToList;
        $scope.sumMoney = sumMoney;
        $scope.removeFromList = removeFromList
        $scope.loadStockIn = loadStockIn;
        $scope.deleteStockIn = deleteStockIn;

        function updateStockIn() {
            //get current branch
            //$scope.stockin.BranchId = 1;
            $scope.stockin.BranchId = $scope.branchSelectedRoot.Id;
            $scope.stockin.SupplierId = $scope.selectedSupplier.Id;
            $scope.stockin.Total = sumMoney();
            $scope.stockin.StatusId = '00';
            $scope.stockin.SoftStockInDetails = $scope.detailStockIns;
            apiService.post('api/stockin/update', $scope.stockin,
                function (result) {
                    if (result.data == true) {
                        notificationService.displaySuccess('Đơn đã được nhập kho.');
                        $scope.selectedSupplier = {};
                        $scope.selectedProduct = {};
                        $scope.product = {};
                        $scope.orders = [];
                        $scope.detailStockIns = [];
                        $scope.isAdding = false;
                    }
                    $state.go('input_stockins');
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function loadSuppliers() {
            $scope.loading = true;
            apiService.get('/api/supplier/getall', null, function (result) {
                $scope.suppliers = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadordersBySupplier(supplierId) {
            if (supplierId) {
                $scope.loading = true;
                var config = {
                    params: {
                        supplierId: supplierId
                    }
                }
                apiService.get('/api/product/getallbysupplier', config, function (result) {
                    $scope.orders = result.data;
                    $scope.loading = false;
                }, function (error) {
                    notificationService.displayError(error);
                });
            }

        }
        function loadProduct(productId) {
            if (productId) {
                $scope.loading = true;
                var config = {
                    params: {
                        productId: productId
                    }
                }
                apiService.get('/api/stockin/getsoftstockinproduct', config, function (result) {
                    $scope.product = result.data;

                    $scope.loading = false;
                }, function (error) {
                    notificationService.displayError(error);
                });
            }

        }
        function addToList() {
            $scope.isAdding = true;
            if ($scope.addedProductIds.indexOf($scope.product.id) == -1) {
                $scope.detailStockIns.push($scope.product);
                $scope.addedProductIds.push($scope.product.id);
            }
            else
                notificationService.displayError("Đã có sản phẩm này trong danh sách");
            $scope.product = {};
        }
        function removeFromList(index) {
            $scope.detailStockIns.splice(index, 1);
            $scope.addedProductIds.splice(index, 1);
        }
        function sumMoney() {
            var total = 0;
            $.each($scope.detailStockIns, function (index, item) {
                total += item.PriceNew * item.Quantity;
            });
            return total;
        }
        function loadStockIn() {
            $scope.loading = true;
            apiService.get('/api/stockin/getbyid/' + $stateParams.id, null, function (result) {
                $scope.stockin.Id = $stateParams.id;
                $window.document.title = "Cập nhật đơn hàng - " + $scope.stockin.Id;
                $scope.selectedSupplier = result.data.SoftSupplier;
                loadordersBySupplier(result.data.SupplierId);
                $scope.detailStockIns = result.data.SoftStockInDetails;
                $.each(result.data.SoftStockInDetails, function (index, item) {
                    $scope.addedProductIds.push(item.id);
                });
                sumMoney();
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function deleteStockIn() {
            var config = {
                params: {
                    stockInId: $scope.stockin.Id
                }
            }

            apiService.del('/api/stockin/delete', config,
                function (result) {
                    if (result.data == true) {
                        notificationService.displaySuccess('Đơn đã huỷ.');
                        $scope.selectedSupplier = {};
                        $scope.selectedProduct = {};
                        $scope.product = {};
                        $scope.orders = [];
                        $scope.detailStockIns = [];
                        $scope.isAdding = false;
                    }
                    $state.go('supplier_stockins');
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadSuppliers();
            loadStockIn();
        }


        $scope.$watch('selectedSupplier', function (n, o) {
            if (n) {
                $scope.selectedSupplier = n;
                $scope.product = {};
                loadordersBySupplier(n.Id);
            }
        });
        $scope.$watch('selectedProduct', function (n, o) {
            if (n) {
                loadProduct(n.id);
            }
        });

    }

})(angular.module('softbbm.stockins'));