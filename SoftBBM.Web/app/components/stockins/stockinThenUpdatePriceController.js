(function (app) {
    app.controller('stockinThenUpdatePriceController', stockinThenUpdatePriceController);

    stockinThenUpdatePriceController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', '$window', '$uibModal'];

    function stockinThenUpdatePriceController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, $window, $uibModal) {
        $scope.updatedProducts = [];

        $scope.loadUpdatedProducts = loadUpdatedProducts;
        $scope.okBook = okBook;
        $scope.thenUpdatePrice = thenUpdatePrice;

        function okBook() {
            $uibModalInstance.dismiss();
        }
        function loadUpdatedProducts() {
            $scope.stockin = {};
            $scope.stockin.SoftStockInDetails = $scope.bookDetails;
            apiService.post('/api/stockin/getupdatedproducts', $scope.stockin, function (result) {
                $scope.updatedProducts = result.data;
                $.each($scope.updatedProducts, function (index, value) {
                    value.updatedChannelPrice = false;
                });
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function thenUpdatePrice(updatedProduct, index) {
            $scope.updatedProducts[index].updatedChannelPrice = true;
            $scope.selectedStock = {
                id: $scope.updatedProducts[index].id
            };
            $uibModal.open({
                templateUrl: '/app/components/stocks/stockChannelPricesModal.html',
                controller: 'stockChannelPricesController',
                scope: $scope,
                backdrop: 'static',
                keyboard: false,
                windowClass: 'app-modal-window'
            }).result.finally(function () {
                //search($scope.page);
            });
        }
        loadUpdatedProducts();
    }

})(angular.module('softbbm.stockins'));