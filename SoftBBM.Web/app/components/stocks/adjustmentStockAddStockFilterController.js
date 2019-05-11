(function (app) {
    app.controller('adjustmentStockAddStockFilterController', adjustmentStockAddStockFilterController);

    adjustmentStockAddStockFilterController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$http', 'authenticationService'];

    function adjustmentStockAddStockFilterController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $http, authenticationService) {
        $scope.loadingModal = true;
        $scope.selectedStockFilter = null;
        $scope.selectedStockFilterValue = 0;

        $scope.addStockFilter = addStockFilter
        $scope.backButton = backButton;

        function addStockFilter() {
            var rbselectedStockFilter = '';
            switch ($scope.selectedStockFilter) {
                case '1':
                    rbselectedStockFilter = '>';
                    break;
                case '2':
                    rbselectedStockFilter = '<';
                    break;
                case '3':
                    rbselectedStockFilter = '=';
                    break;
            }
            var data = {
                value: $scope.selectedStockFilterValue,
                type: rbselectedStockFilter
            }
            $uibModalInstance.close(data);
        }
        function backButton() {
            $uibModalInstance.dismiss();
        }
        //authenticationService.setHeader();

    }

})(angular.module('softbbm.stocks'));