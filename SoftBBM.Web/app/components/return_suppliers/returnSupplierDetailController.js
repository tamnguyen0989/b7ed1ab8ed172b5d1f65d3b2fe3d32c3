(function (app) {
    app.controller('returnSupplierDetailController', returnSupplierDetailController);

    returnSupplierDetailController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function returnSupplierDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
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
                    returnSupplierId: $scope.selectedBook.Id
                }
            }
            apiService.get('/api/returnsupplier/detail', config, function (result) {
                $scope.book = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        loadStockInDetails();
    }

})(angular.module('softbbm.return_suppliers'));