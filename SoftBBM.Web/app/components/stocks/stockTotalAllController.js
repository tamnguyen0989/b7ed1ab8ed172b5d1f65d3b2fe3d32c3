(function (app) {
    app.controller('stockTotalAllController', stockTotalAllController);

    stockTotalAllController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function stockTotalAllController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.branches = [];

        $scope.loadstockTotalAllModal = loadstockTotalAllModal;
        $scope.okstockTotalAllModal = okstockTotalAllModal;

        var config = {
            params: {
                productId: $scope.selectedStock.shop_sanpham.id
            }
        }

        function okstockTotalAllModal() {
            $scope.selectedStock = null;
            $uibModalInstance.dismiss();
        }
        function loadstockTotalAllModal() {
            apiService.get('/api/stock/getallbyproductid', config, function (result) {
                $scope.branches = result.data;
            }, function (error) {
                notificationService.displayError(error);
            });
        }

        loadstockTotalAllModal();
    }

})(angular.module('softbbm.stocks'));