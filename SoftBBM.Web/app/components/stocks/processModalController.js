(function (app) {
    app.controller('processModalController', processModalController);

    processModalController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter','FileSaver'];

    function processModalController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, FileSaver) {
        $scope.loadingModal = true;
        var config = {
            params: {
                branchId: $scope.branchSelectedRoot.Id,
                filter: $scope.filterStocks
            },
            responseType: "arraybuffer"
        };
        apiService.get('/api/stock/exportProductsExcel/', config, function (result) {
            var blob = new Blob([result.data], {
                type: 'application/vnd.ms-excel;charset=charset=utf-8'
            });
            FileSaver.saveAs(blob, 'Products.xlsx');
            $scope.loadingModal = false;
            $uibModalInstance.dismiss();
        }, function (response) {
            notificationService.displayError(response.data);
        });
    }

})(angular.module('softbbm.stocks'));