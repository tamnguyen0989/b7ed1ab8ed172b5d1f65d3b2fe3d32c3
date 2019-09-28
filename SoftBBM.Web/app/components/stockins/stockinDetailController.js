(function (app) {
    app.controller('stockinDetailController', stockinDetailController);

    stockinDetailController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function stockinDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.book = {
            SoftStockInDetails: []
        };

        $scope.loadStockInDetails = loadStockInDetails;
        $scope.okBook = okBook;
        $scope.sumMoney = sumMoney;
        $scope.updateCancel = updateCancel;
        $scope.updateDescription = updateDescription;

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
                console.log($scope.book.FromSuppliers);
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
        function updateDescription() {
            $scope.loading = true;
            var config = {
                stockinId: $scope.selectedBook.Id,
                description: $scope.book.Description,
                userId: $scope.userId
            }
            apiService.post('/api/stockin/updatestockindescriptionfromsupplier', config, function (result) {
                $scope.loading = false;
                notificationService.displaySuccess("Cập nhật ghi chú thành công");
                $scope.selectedBook = {};
                //$uibModalInstance.dismiss();
                $uibModalInstance.close(1);
            }, function (error) {
                notificationService.displayError(error);
            });
        }

        loadStockInDetails();
    }

})(angular.module('softbbm.stockins'));