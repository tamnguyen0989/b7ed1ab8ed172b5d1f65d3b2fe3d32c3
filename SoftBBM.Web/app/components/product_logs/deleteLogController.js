(function (app) {
    app.controller('deleteLogController', deleteLogController);

    deleteLogController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', 'FileSaver'];

    function deleteLogController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, FileSaver) {
        $scope.loadingModal = true;
        $scope.config = {
            startDateDeleteFilter: $scope.startDateDeleteFilter,
            endDateDeleteFilter: $scope.endDateDeleteFilter
        };
        apiService.post('/api/productlog/deleteall/', $scope.config, function (result) {            
            $scope.loadingModal = false;
            $uibModalInstance.dismiss();
        }, function (response) {
            notificationService.displayError(response.data);
        });
    }

})(angular.module('softbbm.stocks'));