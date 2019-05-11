(function (app) {
    app.controller('exportPriceWholesaleController', exportPriceWholesaleController);

    exportPriceWholesaleController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$http', 'authenticationService', 'FileSaver'];

    function exportPriceWholesaleController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $http, authenticationService, FileSaver) {
        $scope.loadingModal = true;
        $http({
            method: 'POST',
            url: "api/stock/exportpricewholesalesexcel",
            data: $scope.filters,
            responseType: "arraybuffer"
        }).then(function (result, status, headers, config) {
            var blob = new Blob([result.data], {
                type: 'application/vnd.ms-excel;charset=charset=utf-8'
            });
            FileSaver.saveAs(blob, 'PriceWholesale-Products.xlsx');
            $scope.loadingModal = false;
            $uibModalInstance.dismiss();
        },
        function (result, status, headers, config) {
            notificationService.displayError(result.data);
        });
    }

})(angular.module('softbbm.stocks'));