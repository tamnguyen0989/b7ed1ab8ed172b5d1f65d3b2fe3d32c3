(function (app) {
    app.controller('stockinDetailController', stockinDetailController);

    stockinDetailController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function stockinDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.book = {
            SoftStockInDetails: []
        };

        $scope.loadStockInDetails = loadStockInDetails;
        $scope.okBook = okBook;
        $scope.sumMoney = sumMoney;
        $scope.updateCancel = updateCancel;

        function okBook() {
            $scope.selectedBook = {};
            $uibModalInstance.dismiss();
        }
        function loadStockInDetails() {
            var config = {
                params: {
                    stockinId: $scope.selectedBook.Id,
                    branchId: $scope.branchSelectedRoot.Id
                }
            }
            apiService.get('/api/stockin/detailstockin', config, function (result) {
                $scope.book = result.data;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function sumMoney() {
            var total = 0;
            $.each($scope.book.SoftStockInDetails, function (index, item) {
                if ($scope.book.CategoryId == '00' || $scope.book.CategoryId == '02')
                    total += item.PriceNew * item.Quantity;
                else total += item.Quantity;
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

})(angular.module('softbbm.stockins'));