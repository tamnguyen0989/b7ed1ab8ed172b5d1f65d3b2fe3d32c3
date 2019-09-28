(function (app) {
    app.controller('productKgUpdateController', productKgUpdateController);

    productKgUpdateController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance'];

    function productKgUpdateController(apiService, $window, $scope, notificationService, $state, $uibModalInstance) {
        $scope.kg = 0;
        $scope.chieucao = 0;
        $scope.chieudai = 0;
        $scope.chieurong = 0;
        $scope.masp = '';
        $scope.tensp = '';

        $scope.updateKg = updateKg;
        $scope.back = back;

        function updateKg() {
            var config = {
                params: {
                    productId : $scope.selectedProduct.id,
                    kg: $scope.kg,
                    chieudai: $scope.chieudai,
                    chieurong: $scope.chieurong,
                    chieucao: $scope.chieucao
                }
            }
            apiService.get('/api/product/updatekg/', config, function (result) {
                $uibModalInstance.close(result.data);
            }, function (result) {
                notificationService.displayError(result.data);
            });
        }
        function loadProductInfo() {
            var config = {
                params: {
                    productId: $scope.selectedProduct.id
                }
            }
            apiService.get('/api/product/getinfo/', config, function (result) {
                $scope.kg = result.data.kg;
                $scope.chieucao = result.data.chieucao;
                $scope.chieudai = result.data.chieudai;
                $scope.chieurong = result.data.chieurong;
                $scope.masp = result.data.masp;
                $scope.tensp = result.data.tensp;
            }, function (result) {
                notificationService.displayError(result.data);
            });
        }
        function back() {
            $uibModalInstance.close(null);
        }

        loadProductInfo();
    }

})(angular.module('softbbm.orders'));