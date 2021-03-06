﻿(function (app) {
    app.controller('orderDetailController', orderDetailController);

    orderDetailController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$uibModalInstance', '$filter', '$sce'];

    function orderDetailController(apiService, $window, $scope, notificationService, $state, $uibModalInstance, $filter, $sce) {
        $scope.orderDetails = [];
        $scope.totalMoney = 0;
        $scope.isUpdated = false;
        $scope.order = {
            donhang_ct: []
        };
        $scope.newCustomerPhone = '';
        $scope.updating = false;

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
        $scope.editNewCustomerPhone = editNewCustomerPhone;
        $scope.updateNewCustomerPhone = updateNewCustomerPhone;
        $scope.cancelUpdateNewCustomerPhone = cancelUpdateNewCustomerPhone;
        $scope.IsNullOrEmpty = IsNullOrEmpty;

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
                $.each($scope.order.donhang_ct, function (ctIndex, ctItem) {
                    ctItem.variation_name = '';
                    if (!IsNullOrEmpty($scope.order.OrderIdShopeeApi))
                        if ($scope.order.OrderIdShopeeApi.length > 0) {
                            var productTmp = JSON.parse(ctItem.Description);
                            ctItem.variation_name = productTmp.variation_name;
                        }
                    if (!(ctItem.IdPro > 0)) {
                        var productTmp = JSON.parse(ctItem.Description);
                        ctItem.shop_bienthe = {
                            masp: '',
                            tensp: '',
                        };
                        ctItem.shop_bienthe.masp = productTmp.item_sku;
                        ctItem.shop_bienthe.tensp = productTmp.item_name;

                    }
                });

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
        function editNewCustomerPhone() {
            $scope.updating = true;
            $scope.newCustomerPhone = '';
        }
        function updateNewCustomerPhone() {
            if ($scope.newCustomerPhone) {
                $scope.orderUpdate = {
                    Id: $scope.selectedOrder.id,
                    Phone: $scope.newCustomerPhone,
                    UserId: $scope.userId
                }
                apiService.post('/api/order/updatenewcustomerphone', $scope.orderUpdate, function (result) {
                    if (result.data == true) {
                        notificationService.displaySuccess("Cập nhật thành công!");
                        $scope.updating = false;
                        loadOrderDetails();
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            }
        }
        function cancelUpdateNewCustomerPhone() {
            $scope.updating = false;
            $scope.newCustomerPhone = '';
        }
        function IsNullOrEmpty(value) {
            return isNullOrEmpty(value);
        }

        init();
        loadOrderDetails();
    }

})(angular.module('softbbm.orders'));