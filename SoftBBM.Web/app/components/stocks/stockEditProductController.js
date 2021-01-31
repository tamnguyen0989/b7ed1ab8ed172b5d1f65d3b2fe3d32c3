(function (app) {
    app.controller('stockEditProductController', stockEditProductController);

    stockEditProductController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function stockEditProductController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.product = null;

        $scope.loadProduct = loadProduct;
        $scope.update = update
        $scope.back = back;
        $scope.copyPriceRefToRestPrice = copyPriceRefToRestPrice;

        function back() {
            $scope.selectedStock = null;
            $scope.product = null;
            $uibModalInstance.dismiss();
        }
        function loadProduct() {
            var config = {
                params: {
                    productId: $scope.selectedStock.shop_sanpham.id
                }
            }
            apiService.get('/api/stock/detailproduct', config, function (result) {
                $scope.product = result.data;
                $scope.selectedProductCategory = $scope.product.productCategory;
                $scope.selectedSupplier = $scope.product.supplier;
                $scope.selectedProductStatus = $scope.product.productStatus;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function update() {
            var catId = null;
            var supId = null;
            var statusId = null;
            if ($scope.selectedProductCategory)
                catId = $scope.selectedProductCategory.Id
            if ($scope.selectedSupplier)
                supId = $scope.selectedSupplier.Id
            if ($scope.selectedProductStatus)
                statusId = $scope.selectedProductStatus.Id
            $scope.modelUpdate = {
                id: $scope.selectedStock.shop_sanpham.id,
                productCode: $scope.product.productCode,
                barCode: $scope.product.barCode,
                name: $scope.product.name,
                priceBase: $scope.product.priceBase,
                priceBaseOld: $scope.product.priceBaseOld,
                priceAvg: $scope.product.priceAvg,
                priceRef: $scope.product.priceRef,
                categoryId: catId,
                supplierId: supId,
                statusId: statusId,
                userId: $scope.userId,
                shopeeId: $scope.product.shopeeId,
                softChannelProductPrices: $scope.product.SoftChannelProductPrices
            }
            apiService.post('/api/stock/updatedetailproduct', $scope.modelUpdate, function (result) {
                notificationService.displaySuccess('Cập nhật thành công !');
                $scope.selectedStock = null;
                $scope.product = null;
                $uibModalInstance.dismiss();
                $scope.search();
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function copyPriceRefToRestPrice() {
            $.each($scope.product.SoftChannelProductPrices, function (i, v) {
                if (v.ChannelId != 7)
                    v.Price = $scope.product.priceRef;
            });
        }

        if (localStorage.getItem("userId")) {
            $scope.userId = JSON.parse(localStorage.getItem("userId"));
        }
        loadProduct();
    }

})(angular.module('softbbm.stocks'));