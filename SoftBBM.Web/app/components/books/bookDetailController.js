(function (app) {
    app.controller('bookDetailController', bookDetailController);

    bookDetailController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function bookDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
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
            apiService.get('/api/stockin/detailbook', config, function (result) {
                $scope.book = result.data;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function sumMoney() {
            var total = 0;
            $.each($scope.book.SoftStockInDetails, function (index, item) {
                total += item.PriceBase * item.Quantity;
            });
            return total;
        }
        function updateCancel() {
            var config = {
                params: {
                    stockinId: $scope.selectedBook.Id,
                    userId: $scope.userId,
                    type: "fromsupplier"
                }
            }
            apiService.get('/api/stockin/updatecancel', config, function (result) {
                if (result.data.Id >0) {
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

})(angular.module('softbbm.books'));