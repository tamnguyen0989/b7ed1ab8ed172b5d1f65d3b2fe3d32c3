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

        function backButton() {
            $uibModalInstance.dismiss();
        }
        function addProduct() {
            $scope.product.tensp = $scope.productName;
            $scope.product.masp = $scope.productCode;
            $scope.product.userId = $scope.userId;
            if ($scope.selectedProductCategory)
                $scope.product.categoryId = $scope.selectedProductCategory.Id;
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
                    $scope.productCode = result.data.brandnew;
                    $scope.lastestProductCodeByCategory = result.data.lastest;
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            }
        }

        loadProductCategories();
    }

})(angular.module('softbbm.stocks'));