(function (app) {
    app.controller('orderOfflineViewController', orderOfflineViewController);

    orderOfflineViewController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$rootScope', '$timeout', '$uibModal', '$sce', '$http', '$q'];

    function orderOfflineViewController(apiService, $window, $scope, notificationService, $state, $rootScope, $timeout, $uibModal, $sce, $http, $q) {
        $scope.userId = JSON.parse(localStorage.getItem("userId"));
        $scope.order = {
            Status: true
        }

        $scope.IsNullOrEmpty = IsNullOrEmpty

        function getOfflineOrderWindow() {
            var config = {
                params: {
                    userId: $scope.userId
                }
            }
            apiService.get('api/order/getofflineorderwindow', config, function (result) {
                if (!isNullOrEmpty(result.data)) {
                    $scope.order = JSON.parse(result.data);
                    if ($scope.order.OrderDetails.length > 0) {
                        let thanhtien = 0;
                        $.each($scope.order.OrderDetails, function (i, v) {
                            thanhtien += v.Quantity * v.Price
                        });
                        $scope.order.thanhtien = thanhtien;
                    }
                    console.log($scope.order)
                }
            }, function (response) {
                //notificationService.displayError(response.data);
                console.log(response.data)
            });
        }
        function IsNullOrEmpty(value) {
            return value == 0 ? true : isNullOrEmpty(value);
        }

        function init() {

        }

        $scope.chatHub = null;
        $scope.chatHub = $.connection.chatHub; // initializes hub
        $.connection.hub.start(); // starts hub
        $scope.chatHub.client.broadcastMessage = function (type, userId) {
            if (type == "update_offline_window" && $scope.userId == userId) {
                $scope.order = {
                    Status: true
                }
                getOfflineOrderWindow();
            }
        };

        init();
        $window.document.title = "Đơn hàng hiện tại";

    }

})(angular.module('softbbm.orders'));