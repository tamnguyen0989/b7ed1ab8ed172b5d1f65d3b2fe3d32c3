(function (app) {
    app.controller('priceExportProcessController', priceExportProcessController);

    priceExportProcessController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', 'FileSaver','$http'];

    function priceExportProcessController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, FileSaver, $http) {
        $scope.loadingModal = true;
        //var config = {
        //    params: {
        //        branchId: $scope.branchSelectedRoot.Id,
        //        filter: $scope.filterStocks
        //    },
        //    responseType: "arraybuffer"
        //};
        //apiService.get('/api/stock/exportProductsExcel/', config, function (result) {
        //    var blob = new Blob([result.data], {
        //        type: 'application/vnd.ms-excel;charset=charset=utf-8'
        //    });
        //    FileSaver.saveAs(blob, 'Products.xlsx');
        //    $scope.loadingModal = false;
        //    $uibModalInstance.dismiss();
        //}, function (response) {
        //    notificationService.displayError(response.data);
        //});


        //var params = {
        //    exportPriceParamsDetails: $scope.bookDetails
        //}
        //apiService.post('api/product/exportprice', params,
        //    function (result) {
        //        var blob = new Blob([result.data], {
        //            type: 'application/vnd.ms-excel;charset=charset=utf-8'
        //        });
        //        FileSaver.saveAs(blob, 'Products.xlsx');
        //        $scope.loadingModal = false;
        //        $uibModalInstance.dismiss();
        //    }, function (error) {
        //        notificationService.displayError(error.data);
        //    });

        $http({
            method: 'POST',
            url: "api/product/exportprice",
            data: {
                exportPriceParamsDetails: $scope.bookDetails
            },
            responseType: "arraybuffer"
        }).then(function (result, status, headers, config) {
            var blob = new Blob([result.data], {
                type: 'application/vnd.ms-excel;charset=charset=utf-8'
            });
            FileSaver.saveAs(blob, 'Price-Products.xlsx');
            $scope.loadingModal = false;
            $uibModalInstance.dismiss();
        },
        function (result, status, headers, config) {
            notificationService.displayError(result.data);
        });
    }

})(angular.module('softbbm.exports'));