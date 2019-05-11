(function (app) {
    app.controller('processModalController', processModalController);

    processModalController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', 'FileSaver', '$http'];

    function processModalController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, FileSaver, $http) {
        $scope.loadingModal = true;
        $http({
            method: 'POST',
            url: "api/stock/exportproductsexcel",
            data: $scope.filters,
            responseType: "arraybuffer"
        }).then(function (result, status, headers, config) {
            var blob = new Blob([result.data], {
                type: 'application/vnd.ms-excel;charset=charset=utf-8'
            });
            FileSaver.saveAs(blob, 'Products.xlsx');
            $scope.loadingModal = false;
            $uibModalInstance.dismiss();
        },
        function (result, status, headers, config) {
            notificationService.displayError(result.data);
        });
    }

})(angular.module('softbbm.stocks'));