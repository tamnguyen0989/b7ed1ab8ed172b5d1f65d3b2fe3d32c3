(function (app) {
    app.controller('adjustmentStockDetailController', adjustmentStockDetailController);

    adjustmentStockDetailController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function adjustmentStockDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.book = {
            SoftStockInDetails: []
        };

        $scope.loadStockInDetails = loadStockInDetails;
        $scope.okBook = okBook;

        function okBook() {
            $scope.selectedBook = {};
            $uibModalInstance.dismiss();
        }
        function loadStockInDetails() {
            var config = {
                params: {
                    adjustmentStockId: $scope.selectedBook.Id
                }
            }
            apiService.get('/api/adjustmentstock/detail', config, function (result) {
                $scope.book = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        loadStockInDetails();
    }

})(angular.module('softbbm.stocks'));