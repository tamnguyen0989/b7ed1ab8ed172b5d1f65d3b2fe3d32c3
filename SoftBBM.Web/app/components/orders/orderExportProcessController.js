(function (app) {
    app.controller('orderExportProcessController', orderExportProcessController);

    orderExportProcessController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', 'FileSaver', '$http'];

    function orderExportProcessController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, FileSaver, $http) {
        $scope.loadingModal = true;
        $http({
            method: 'POST',
            url: "api/order/exportordersexcel",
            data: $scope.filters,
            responseType: "arraybuffer"
        }).then(function (result, status, headers, config) {
            var blob = new Blob([result.data], {
                type: 'application/vnd.ms-excel;charset=charset=utf-8'
            });
            var d = new Date();
            var day = d.getDate();
            if (day.length = 1)
                day = "0" + day;
            var m = d.getMonth() + 1;
            if (m.length = 1)
                m = "0" + m;
            var daySuf = day + m + d.getFullYear();
            FileSaver.saveAs(blob, 'Orders'+daySuf+'.xlsx');
            $scope.loadingModal = false;
            $uibModalInstance.dismiss();
        },
        function (result, status, headers, config) {
            notificationService.displayError(result.data);
        });
    }

})(angular.module('softbbm.orders'));