(function (app) {
    app.controller('soldProductsByDateController', soldProductsByDateController);

    soldProductsByDateController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance'];

    function soldProductsByDateController(apiService, $window, $scope, notificationService, $state, $uibModalInstance) {
        $scope.loadingSoldProduct = false;
        $scope.picker = {};
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            showWeeks: false
        };
        $scope.config = {};
        $scope.startDateFilter = {};
        $scope.endDateFilter = {};

        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.backButton = backButton;
        $scope.loadSoldProductsByDate = loadSoldProductsByDate

        function openStartDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.startDateFilter = true;

        }
        function openEndDateFilter($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.endDateFilter = true;

        }
        function backButton() {
            $uibModalInstance.dismiss();
        }
        function loadSoldProductsByDate() {
            $scope.loadingSoldProduct = true;
            $scope.config.branchId = $scope.selectedBranch.Id;
            $scope.config.startDateFilter = $scope.startDateFilter;
            $scope.config.endDateFilter = $scope.endDateFilter;

            apiService.post('/api/stockin/loadsoldproductsbydate', $scope.config, function (result) {
                $scope.loadingSoldProduct = false;
                $uibModalInstance.close(result.data);
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
    }

})(angular.module('softbbm.stockouts'));