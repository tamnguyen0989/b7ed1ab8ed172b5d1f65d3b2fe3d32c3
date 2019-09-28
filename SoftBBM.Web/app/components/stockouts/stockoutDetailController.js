(function (app) {
    app.controller('stockoutDetailController', stockoutDetailController);

    stockoutDetailController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function stockoutDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.loading = false;
        $scope.book = {
            SoftStockInDetails: []
        };

        $scope.loadStockInDetails = loadStockInDetails;
        $scope.okBook = okBook;

        $scope.sumMoney = sumMoney;
        $scope.sumQuantity = sumQuantity;
        $scope.updateCancel = updateCancel;

        function okBook() {
            $scope.selectedBook = {};
            $uibModalInstance.dismiss();
        }
        function loadStockInDetails() {
            $scope.loading = true;
            var config = {
                params: {
                    stockinId: $scope.selectedBook.Id,
                    branchId: $scope.branchSelectedRoot.Id
                }
            }
            apiService.get('/api/stockin/detailstockout', config, function (result) {
                $scope.loading = false;
                $scope.book = result.data;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function sumQuantity() {
            var total = 0;
            $.each($scope.book.SoftStockInDetails, function (index, item) {
                total += item.Quantity;
            });
            return total;
        }
        function sumMoney() {
            var total = 0;
            $.each($scope.book.SoftStockInDetails, function (index, item) {
                total += item.Quantity * item.PriceBase;
            });
            return total;
        }
        function updateCancel() {
            var config = {
                params: {
                    stockinId: $scope.selectedBook.Id,
                    userId: $scope.userId
                }
            }
            apiService.get('/api/stockin/updatecancel', config, function (result) {
                if (result.data.Id > 0) {
                    notificationService.displaySuccess("Cập nhật trạng thái huỷ thành công!")
                    $scope.selectedBook = {};
                    $uibModalInstance.dismiss();
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        loadStockInDetails();
    }

})(angular.module('softbbm.stockouts'));