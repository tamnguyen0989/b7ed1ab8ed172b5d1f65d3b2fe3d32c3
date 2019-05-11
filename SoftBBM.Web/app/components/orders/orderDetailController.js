(function (app) {
    app.controller('orderDetailController', orderDetailController);

    orderDetailController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', '$sce'];

    function orderDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, $sce) {
        $scope.orderDetails = [];
        $scope.totalMoney = 0;
        $scope.isUpdated = false;
        $scope.order = {
            donhang_ct: []
        };

        $scope.loadOrderDetails = loadOrderDetails;
        $scope.updateOrderDetails = updateOrderDetails;
        $scope.processOrder = processOrder;
        $scope.shippedOrder = shippedOrder;
        $scope.refundOrder = refundOrder;
        $scope.doneOrder = doneOrder;
        $scope.cancelOrder = cancelOrder;
        $scope.shipCancelOrder = shipCancelOrder;
        $scope.sumMoney = sumMoney;
        $scope.back = back;
        $scope.authenShippedOrder = authenShippedOrder;

        var config = {
            params: {
                orderId: $scope.selectedOrder.id
            }
        }

        function okOrderDetails() {
            $scope.selectedOrder = {};
            $uibModalInstance.dismiss();
        }
        function processOrder() {
            var idShipper;
            if ($scope.selectedShipper)
                idShipper = $scope.selectedShipper.Id;
            else
                idShipper = null;
            $scope.orderUpdate = {
                id: $scope.selectedOrder.id,
                ghichu: $scope.order.ghichu,
                ShipperId: idShipper,
                UserId: $scope.userId
            }
            apiService.post('/api/order/updateprocess', $scope.orderUpdate, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Cập nhật thành công");
                    $uibModalInstance.dismiss();
                    $scope.selectedOrder = {};
                    $scope.page = $scope.page || 0;
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function shippedOrder() {
            var idShipper;
            if ($scope.selectedShipper)
                idShipper = $scope.selectedShipper.Id;
            else
                idShipper = null;
            $scope.orderUpdate = {
                id: $scope.selectedOrder.id,
                ghichu: $scope.order.ghichu,
                ShipperId: idShipper,
                UserId: $scope.userId
            }
            apiService.post('/api/order/updateshipped', $scope.orderUpdate, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Cập nhật thành công");
                    $uibModalInstance.dismiss();
                    $scope.selectedOrder = {};
                    $scope.page = $scope.page || 0;
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadOrderDetails() {
            apiService.get('/api/order/detail', config, function (result) {
                $scope.order = result.data;
                $scope.statusPrint = $sce.trustAsHtml($scope.order.StatusPrint);
                if ($scope.order.Status == 3 || $scope.order.Status == 4 || $scope.order.Status == 6)
                    $scope.isUpdated = true;
                $.each($scope.userFilters, function (i, item) {
                    if (item.Id == $scope.order.ShipperId) {
                        $scope.selectedShipper = item;
                        return false;
                    }
                });
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function refundOrder() {
            var idShipper;
            if ($scope.selectedShipper)
                idShipper = $scope.selectedShipper.Id;
            else
                idShipper = null;
            $scope.orderUpdate = {
                id: $scope.selectedOrder.id,
                ghichu: $scope.order.ghichu,
                ShipperId: idShipper,
                UserId: $scope.userId
            }
            apiService.post('/api/order/updaterefund', $scope.orderUpdate, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Cập nhật thành công");
                    $uibModalInstance.dismiss();
                    $scope.selectedOrder = {};
                    $scope.page = $scope.page || 0;
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function doneOrder() {
            var idShipper;
            if ($scope.selectedShipper)
                idShipper = $scope.selectedShipper.Id;
            else
                idShipper = null;
            $scope.orderUpdate = {
                id: $scope.selectedOrder.id,
                ghichu: $scope.order.ghichu,
                ShipperId: idShipper,
                UserId: $scope.userId
            }
            apiService.post('/api/order/updatedone', $scope.orderUpdate, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Cập nhật thành công");
                    $uibModalInstance.dismiss();
                    $scope.selectedOrder = {};
                    $scope.page = $scope.page || 0;
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function cancelOrder() {
            var idShipper;
            if ($scope.selectedShipper)
                idShipper = $scope.selectedShipper.Id;
            else
                idShipper = null;
            $scope.orderUpdate = {
                id: $scope.selectedOrder.id,
                ghichu: $scope.order.ghichu,
                ShipperId: idShipper,
                UserId: $scope.userId
            }
            apiService.post('/api/order/updatecancel', $scope.orderUpdate, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Cập nhật thành công");
                    $uibModalInstance.dismiss();
                    $scope.selectedOrder = {};
                    $scope.page = $scope.page || 0;
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function shipCancelOrder() {
            var idShipper;
            if ($scope.selectedShipper)
                idShipper = $scope.selectedShipper.Id;
            else
                idShipper = null;
            $scope.orderUpdate = {
                id: $scope.selectedOrder.id,
                ghichu: $scope.order.ghichu,
                ShipperId: idShipper,
                UserId: $scope.userId
            }
            apiService.post('/api/order/updateshipcancel', $scope.orderUpdate, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Cập nhật thành công");
                    $uibModalInstance.dismiss();
                    $scope.selectedOrder = {};
                    $scope.page = $scope.page || 0;
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function updateOrderDetails() {
            var idShipper;
            if ($scope.selectedShipper)
                idShipper = $scope.selectedShipper.Id;
            else
                idShipper = null;
            $scope.orderUpdate = {
                id: $scope.selectedOrder.id,
                ghichu: $scope.order.ghichu,
                ShipperId: idShipper,
                UserId: $scope.userId
            }
            apiService.post('/api/order/update', $scope.orderUpdate, function (result) {
                if (result.data == true) {
                    notificationService.displaySuccess("Cập nhật thành công");
                    $uibModalInstance.dismiss();
                    $scope.selectedOrder = {};
                    $scope.page = $scope.page || 0;
                    $scope.search($scope.page);
                }
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function sumMoney() {
            var total = 0;
            $.each($scope.order.donhang_ct, function (index, item) {
                if (item.Dongia)
                    total += item.Dongia * item.Soluong;
            });
            return total;
        }
        function back() {
            $uibModalInstance.dismiss();
            $scope.selectedOrder = {};
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
        }
        function authenShippedOrder() {
            var config = {
                params: {
                    userId: $scope.userId,
                    orderId: $scope.selectedOrder.id,
                }
            };
            apiService.get('api/order/authenupdate', config, function (result) {
                var idShipper;
                if ($scope.selectedShipper)
                    idShipper = $scope.selectedShipper.Id;
                else
                    idShipper = null;
                $scope.orderUpdate = {
                    id: $scope.selectedOrder.id,
                    ghichu: $scope.order.ghichu,
                    ShipperId: idShipper,
                    UserId: $scope.userId
                }
                apiService.post('/api/order/updateshipped', $scope.orderUpdate, function (result) {
                    if (result.data == true) {
                        notificationService.displaySuccess("Cập nhật thành công");
                        $uibModalInstance.dismiss();
                        $scope.selectedOrder = {};
                        $scope.page = $scope.page || 0;
                        $scope.search($scope.page);
                    }
                }, function (error) {
                    notificationService.displayError(error);
                });
            }, function (error) {
                notificationService.displayError(error.data);

            });
        }

        init();
        loadOrderDetails();
    }

})(angular.module('softbbm.orders'));