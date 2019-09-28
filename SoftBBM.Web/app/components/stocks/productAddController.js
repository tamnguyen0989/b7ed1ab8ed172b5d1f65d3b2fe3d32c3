(function (app) {
    app.controller('productAddController', productAddController);

    productAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance'];

    function productAddController(apiService, $window, $scope, notificationService, $state, $uibModalInstance) {
        $scope.product = {};
        $scope.productName = '';
        $scope.productCode = '';
        $scope.productCategories = [];
        $scope.selectedProductCategory = null;
        $scope.lastestProductCodeByCategory = null;


        $scope.backButton = backButton;
        $scope.addProduct = addProduct;
        $scope.genNewProductCodeByCategory = genNewProductCodeByCategory;
        $scope.loadSuppliers = loadSuppliers;
        $scope.loadProductStatus = loadProductStatus;

        function backButton() {
            $uibModalInstance.dismiss();
        }
        function addProduct() {
            $scope.product.productName = $scope.productName;
            $scope.product.productCode = $scope.productCode;
            $scope.product.selectedProductCategory = $scope.selectedProductCategory;
            $scope.product.selectedSupplier = $scope.selectedSupplier;
            $scope.product.selectedProductStatus = $scope.selectedProductStatus;
            $scope.product.barCode = $scope.barCode;
            $scope.product.priceRef = $scope.priceRef;
            $scope.product.priceCHA = $scope.priceCHA;
            $scope.product.priceONL = $scope.priceONL;
            $scope.product.userId = $scope.userId;
            apiService.post('/api/product/add/', $scope.product, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Thêm mới thành công!");
                    $uibModalInstance.dismiss();
                }
            }, function (result) {
                notificationService.displayError(result.data);
            });
        }
        function loadProductCategories() {
            apiService.get('api/productcategory/getall', null, function (result) {
                $scope.productCategories = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function genNewProductCodeByCategory() {
            if ($scope.selectedProductCategory) {
                $scope.lastestProductCodeByCategory = null;
                $scope.productCode = null;
                var config = {
                    params: {
                        categoryId: $scope.selectedProductCategory.Id
                    }
                }
                apiService.get('api/product/gennewproductcodebycategory', config, function (result) {
                    //$scope.lastestProductCodeByCategory = result.data;
                    $scope.lastestProductCodeByCategory = result.data.lastest;
                    $scope.productCode = result.data.brandnew;
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            }
        }
        function loadSuppliers() {
            $scope.loading = true;
            var config = {
                params: {
                    order: "Name"
                }

            }
            apiService.get('/api/supplier/getallbookadd', config, function (result) {
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

        loadProductCategories();
    }

})(angular.module('softbbm.stocks'));