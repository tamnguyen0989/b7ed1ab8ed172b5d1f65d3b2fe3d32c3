(function (app) {
    app.controller('stockinPaymentController', stockinPaymentController);

    stockinPaymentController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter'];

    function stockinPaymentController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter) {

        $scope.loadPaymentStatuses = loadPaymentStatuses;
        $scope.loadPaymentMethods = loadPaymentMethods;
        $scope.updatePaymentStatus = updatePaymentStatus;
        $scope.validPaymentMethod = validPaymentMethod;
        $scope.updatePayment = updatePayment;
        $scope.back = back;

        function loadPaymentTypes() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallpaymenttype', null, function (result) {
                $scope.paymentTypes = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        };
        function loadPaymentMethods() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallpaymentmethod', null, function (result) {
                $scope.paymentMethods = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        };
        function loadPaymentStatuses() {
            $scope.loading = true;
            apiService.get('/api/stockin/getallpaymentstatus', null, function (result) {
                $scope.paymentStatuses = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        };
        function validPaymentMethod() {
            if ($scope.selectedPaymentStatus) {
                if ($scope.selectedPaymentStatus.Id == 1)
                    return true
                else
                    return false
            }
            else
                return false
        }
        function updatePayment() {
            $scope.stockinUpdate = {};
            $scope.stockinUpdate.stockinId = $scope.selectedStockin.Id;
            var paymentStatusId = null;
            if ($scope.selectedPaymentStatus) {
                paymentStatusId = $scope.selectedPaymentStatus.Id;
            }
            $scope.stockinUpdate.paymentStatusId = paymentStatusId;
            var paymentMethodId = null;
            if ($scope.selectedPaymentMethod)
                paymentMethodId = $scope.selectedPaymentMethod.Id;
            $scope.stockinUpdate.paymentMethodId = paymentMethodId;
            $scope.stockinUpdate.updateBy = $scope.userId;
            apiService.post('/api/stockin/updatepayment/', $scope.stockinUpdate, function (result) {
                $scope.stockinUpdate = {};
                $uibModalInstance.dismiss();
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function updatePaymentStatus() {
            $scope.selectedPaymentMethod = null;
        }
        function back() {
            $uibModalInstance.dismiss();
        }

        loadPaymentStatuses();
        loadPaymentMethods();
    }

})(angular.module('softbbm.stockins'));