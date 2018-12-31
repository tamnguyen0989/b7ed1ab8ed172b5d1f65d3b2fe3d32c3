(function (app) {
    app.controller('processBookModalController', processBookModalController);

    processBookModalController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', 'FileSaver'];

    function processBookModalController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, FileSaver) {
        $scope.loadingModal = true;
        var config = {
            params: {
                stockinId: $scope.selectedBook.Id
            },
            responseType: "arraybuffer"
        };
        apiService.get('/api/stockin/exportexcel/', config, function (result) {
            var blob = new Blob([result.data], {
                type: 'application/vnd.ms-excel;charset=charset=utf-8'
            });
            FileSaver.saveAs(blob, 'Book' + $scope.selectedBook.Id + '.xlsx');
            $scope.loadingModal = false;
            $uibModalInstance.dismiss();
        }, function (response) {
            notificationService.displayError(response.data);
        });
    }

})(angular.module('softbbm.books'));