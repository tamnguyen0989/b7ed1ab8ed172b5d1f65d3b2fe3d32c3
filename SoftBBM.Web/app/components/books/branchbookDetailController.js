(function (app) {
    app.controller('branchbookDetailController', branchbookDetailController);

    branchbookDetailController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function branchbookDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.book = {
            SoftStockInDetails: []
        };

        $scope.loadStockInDetails = loadStockInDetails;
        $scope.okBook = okBook;
        $scope.sumTotal = sumTotal;
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
            apiService.get('/api/stockin/detailbranchbook', config, function (result) {
                $scope.book = result.data;
                var fromBranchId = result.data.FromBranchId;
                $.each($scope.book.SoftStockInDetails, function (indexBookDetails, valueBookDetails) {
                    var stockTmp = 0;
                    var stringTmps = valueBookDetails.shop_sanpham.SoftBranchProductStocks;
                    //valueBookDetails.SoftBranchProductStocks = stringTmps;
                    $.each(stringTmps, function (indexStock, valStock) {

                        if (valStock.BranchId == fromBranchId) {
                            stockTmp = valStock.StockTotal;
                            return false;
                        }
                    });
                    valueBookDetails.SelectedStockTotal = stockTmp;
                });                
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function sumTotal() {
            var total = 0;
            $.each($scope.book.SoftStockInDetails, function (index, item) {
                total += item.Quantity;
            });
            return total;
        }
        function updateCancel() {
            var config = {
                params: {
                    stockinId: $scope.selectedBook.Id,
                    userId: $scope.userId,
                    type: "branch"
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

})(angular.module('softbbm.books'));