(function (app) {
    app.controller('supplierAddController', supplierAddController);

    supplierAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state'];

    function supplierAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.supplier = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.vatStatuses = [];
        $scope.VAT = null;

        $scope.addSupplier = addSupplier;

        function addSupplier() {
            if ($scope.VAT)
                $scope.supplier.vatId = $scope.VAT.Id;
            apiService.post('api/supplier/add', $scope.supplier,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                    $state.go('suppliers');
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function authen() {
            apiService.get('api/supplier/authenadd', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadVatStatuses() {
            apiService.get('api/supplier/getallvatstatus', null,
                function (result) {
                    $scope.vatStatuses = result.data;
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.supplier.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
        }

        $window.document.title = "Thêm mới nhà cung cấp";
        authen();
        loadVatStatuses();
        init();
    }

})(angular.module('softbbm.suppliers'));