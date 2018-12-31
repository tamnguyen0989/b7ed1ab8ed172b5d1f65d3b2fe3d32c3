(function (app) {
    app.controller('stockChannelPricesController', stockChannelPricesController);

    stockChannelPricesController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function stockChannelPricesController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {
        $scope.stockChannelPrices = {};
        $scope.picker = {};

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 0,
            showWeeks: false
        };

        $scope.loadChannelPricesModal = loadChannelPricesModal;
        $scope.okChannelPricesModal = okChannelPricesModal;
        $scope.updateChannelPricesModal = updateChannelPricesModal;
        $scope.cancelChannelPricesModal = cancelChannelPricesModal;
        $scope.openStartDate = openStartDate;
        $scope.openEndDate = openEndDate;

        var config = {
            params: {
                productId: $scope.selectedStock.id
            }
        }

        function okChannelPricesModal() {
            var id = $scope.selectedStock;
            $uibModalInstance.dismiss();
        }
        function cancelChannelPricesModal() {
            $uibModalInstance.dismiss();
        }
        function updateChannelPricesModal() {
            $scope.stockChannelPrices.UpdateBy = $scope.userId;
            apiService.post('/api/channelproductprice/updatechannelprice', $scope.stockChannelPrices,
                function (result) {
                    notificationService.displaySuccess('Cập nhật thành công');
                    $uibModalInstance.dismiss();
                    $scope.search($scope.page);
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function loadChannelPricesModal() {
            apiService.get('/api/channelproductprice/getallpricechannelproduct', config, function (result) {
                $scope.stockChannelPrices = result.data;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function openStartDate($event, Id) {
            var idOpen = "startDate"+Id;
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker[idOpen] = true;
        };
        function openEndDate($event,Id) {
            var idEnd = "endDate"+Id;
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker[idEnd] = true;
        };

        loadChannelPricesModal();
    }

})(angular.module('softbbm.stocks'));