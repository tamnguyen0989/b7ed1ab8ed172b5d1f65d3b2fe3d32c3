(function (app) {
    app.controller('stockinThenOutController', stockinThenOutController);

    stockinThenOutController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', '$window'];

    function stockinThenOutController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, $window) {
        $scope.stockouts = [];

        $scope.loadStockouts = loadStockouts;
        $scope.okBook = okBook;
        $scope.thenOut = thenOut;

        function okBook() {
            $uibModalInstance.dismiss();
        }
        function loadStockouts() {
            $scope.stockin = {};
            $scope.stockin.SoftStockInDetails = $scope.bookDetails;
            apiService.post('/api/stockin/thenout', $scope.stockin, function (result) {
                $scope.stockouts = result.data;
                $.each($scope.stockouts, function (index, value) {
                    value.outed = false;
                });
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function thenOut(stockout,index) {
            $scope.stockouts[index].outed = true;
            localStorage.setItem('thenOut', JSON.stringify(stockout));
            var url = $state.href('add_stockout');
            $window.open(url, '_blank');
        }
        loadStockouts();
    }

})(angular.module('softbbm.stockins'));