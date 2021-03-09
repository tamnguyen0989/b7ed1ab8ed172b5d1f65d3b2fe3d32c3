(function (app) {
    app.controller('orderOfflineAddController', orderOfflineAddController);

    orderOfflineAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$rootScope', '$timeout', '$uibModal', '$sce', '$http', '$q'];

    function orderOfflineAddController(apiService, $window, $scope, notificationService, $state, $rootScope, $timeout, $uibModal, $sce, $http, $q) {
        $scope.userId = JSON.parse(localStorage.getItem("userId"));
        $scope.order = {
            Status: true
        }
        $scope.customer = {
            diem: 0
        };
        $scope.currentDate = new Date();
        $scope.convertCreatedDate = "";
        $scope.orders = [];
        $scope.selectedProduct = null;
        $scope.searchedProducts = [];
        $scope.searchText = null;
        $scope.addedProductIds = [];
        $scope.product = {};
        $scope.loading = true;
        $scope.detailOrders = [];
        $scope.isAdding = false;
        $scope.anotherFee = 0;
        $scope.cod = 0;
        $scope.VTPCod = 0;
        $scope.paid = null;
        $scope.searchPhone = null;
        $scope.diemsp = 0;//kèm phí giảm cho thành viên + giảm giá riêng
        $scope.giamthanhvien = 0;
        $scope.datru_diem = 0;//phí giảm cho thành viên
        $scope.searchType = 'masp';
        $scope.paymentCity = [
            { name: 'Tiền mặt', value: 1 },
            { name: 'Quẹt thẻ', value: 7 },
            { name: 'Chuyển khoản', value: 2 },
            { name: 'QRCode VNPAY', value: 6 },
        ]
        $scope.paymentHCMDefault = {
            name: 'Tiền mặt',
            value: 1
        };
        $scope.deliveryTimeHCMDefault = {
            name: '08h – 17h giờ hành chánh',
            value: 1
        };
        $scope.paymentFUTA = [
            { name: 'Chuyển khoản', value: 2 },
        ]
        $scope.deliveryTime = [
            { name: '08h – 17h giờ hành chánh', value: 1 },
            { name: '17h – 22h ngoài giờ hành chánh', value: 2 },
            { name: 'Bất kỳ giờ nào trong ngày', value: 4 },
            { name: 'Ngày chủ nhật', value: 3 }
        ];
        $scope.deliveryTimeFast = [
            { name: 'Nhanh, chỉ trong 3 tiếng', value: 5 },
            { name: '08h – 17h giờ hành chánh', value: 1 },
            { name: '17h – 22h ngoài giờ hành chánh', value: 2 },
            { name: 'Bất kỳ giờ nào trong ngày', value: 4 },
            { name: 'Ngày chủ nhật', value: 3 }
        ];
        $scope.deliveryTimeSubUrban = [
            { name: '08h – 17h giờ hành chánh', value: 1 },
            { name: 'Ngày chủ nhật', value: 3 }
        ];
        //$scope.selectedPayment = null;
        $scope.selectedPayment = $scope.paymentHCMDefault;
        $scope.selectedDeliveryTime = null;
        $scope.feeShip = 0;
        $scope.HCMType = 0;
        $scope.feeDeliveryTime = 0;
        $scope.foodCart = false;
        $scope.freeShipCart = false;
        $scope.VTPFeeHCM = 0;
        $scope.GHTKFeeHCM = 0;
        $scope.VNPFeeHCM = 0;
        $scope.VNEPFeeHCM = 0;
        $scope.VTPEcoFee = 0;
        $scope.VTPFastFee = 0;
        $scope.VNPEcoFee = 0;
        $scope.VNPFastFee = 0;
        $scope.GHTKFee = 0;
        $scope.VNEPFee = 0;
        $scope.FUTAFee = 0;
        $scope.VTPEcoTime = 0;
        $scope.VTPFastTime = 0;
        $scope.VNPEcoTime = 0;
        $scope.VNPFastTime = 0;
        $scope.GHTKTime = 0;
        $scope.VNEPTime = 0;
        $scope.VNPTime = 0;
        $scope.FUTATime = 0;
        $scope.FUTAAddress = '';
        $scope.selectedTypeShipHCM = null;
        $scope.selectedTypeShipOutHCM = null;
        $scope.freeShipType = 0;
        $scope.passGHTKRules = true;
        $scope.passVNEPRules = true;
        $scope.coachInfo = '';
        $scope.nameTypeShip = '';
        $scope.orderReturn = {
        }
        $scope.totalMoneyPrint = 0;
        $scope.channelName = '';
        $scope.addingOrder = false;
        $scope.searching = false;
        $scope.kg = 0;
        $scope.historyOrder = null;
        $scope.SPEChannelId = 4;
        $scope.ONLChannelId = 2;
        $scope.ONLChannelName = "Online";
        $scope.CHAChannelId = 1;
        $scope.CHAChannelName = "Cửa hàng";

        $scope.addOrder = addOrder;
        $scope.saveOrder = saveOrder;
        $scope.sumMoney = sumMoney;
        $scope.sumMoneyCartNoDiscount = sumMoneyCartNoDiscount;
        $scope.totalMoney = totalMoney;
        $scope.removeFromList = removeFromList;
        $scope.resetOrderDetails = resetOrderDetails;
        $scope.resetOrder = resetOrder;
        $scope.init = init;
        $scope.updateDiscountMoney = updateDiscountMoney;
        $scope.updateDiscountPercent = updateDiscountPercent;
        $scope.updateDiscountCode = updateDiscountCode;
        $scope.updateDiscountMember = updateDiscountMember;
        $scope.loadChannels = loadChannels;
        $scope.loadUsers = loadUsers;
        $scope.loadCities = loadCities;
        $scope.searchProduct = searchProduct;
        $scope.addToList = addToList;
        $scope.updateCity = updateCity;
        $scope.updateDistrict = updateDistrict;
        $scope.searchCustomerByPhone = searchCustomerByPhone;
        $scope.initCity = initCity;
        $scope.initCustomer = initCustomer;
        $scope.sumPoint = sumPoint;
        $scope.total = total;
        $scope.checkFastDelivery = checkFastDelivery;
        $scope.checkSubUrban = checkSubUrban;
        $scope.updateDeliveryTime = updateDeliveryTime;
        $scope.getFeeShip = getFeeShip;
        $scope.updateTypeShipHCM = updateTypeShipHCM;
        $scope.updateTypeShipOutHCM = updateTypeShipOutHCM;
        $scope.updatePayment = updatePayment;
        $scope.updateSelectedChannel = updateSelectedChannel;
        $scope.validSelectedPayment = validSelectedPayment;
        $scope.validSelectedDistrict = validSelectedDistrict;
        $scope.validDeliveryTimeSubUrban = validDeliveryTimeSubUrban;
        $scope.validDeliveryTime = validDeliveryTime;
        $scope.validDeliveryTimeFast = validDeliveryTimeFast;
        $scope.validSelectedTypeShipHCM = validSelectedTypeShipHCM;
        $scope.validSelectedTypeShipOutHCM = validSelectedTypeShipOutHCM;
        $scope.sumKg = sumKg;
        $scope.updateDataOrderWindow = updateDataOrderWindow;

        function loadChannels() {
            $scope.loading = true;
            apiService.get('/api/channel/getall', null, function (result) {
                var channels = result.data;
                var shopeeIndex = 0;
                $.each(channels, function (index, value) {
                    if (value.Code == 'SPE')
                        shopeeIndex = index;
                });
                channels.splice(shopeeIndex, 1)
                $scope.channels = channels;
                $scope.selectedChannel = $scope.channels[0];

                $scope.order.ChannelId = $scope.CHAChannelId;
                $scope.channelName = $scope.CHAChannelName.toUpperCase();

                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadUsers() {
            $scope.loading = true;
            apiService.get('api/applicationuser/getall', null, function (result) {
                $scope.users = result.data;
                //$scope.selectedShipper = $scope.users[0];
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        //var usersPromise = $http({
        //    method: 'GET',
        //    url: "api/applicationuser/getall"
        //}).then(function (result, status, headers, config) {
        //    $scope.users = result.data;
        //},
        //function (result, status, headers, config) {

        //});
        function loadCities() {
            $scope.loading = true;
            apiService.get('api/city/getall', null, function (result) {
                $scope.cities = result.data;
                initCity()
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadFeeRegionCode() {
            $scope.loading = true;
            apiService.get('api/order/getfeeregioncode', null, function (result) {
                $scope.regions = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadFutaAddresses() {
            $scope.loading = true;
            apiService.get('api/order/getfutaaddress', null, function (result) {
                $scope.futaAddresses = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function searchProduct() {
            $scope.searchedProducts = [];
            if ($scope.searchText.length > 1) {
                var config = {
                    params: {
                        searchType: $scope.searchType,
                        channelId: $scope.selectedChannel.Id,
                        searchText: $scope.searchText
                    }
                }
                apiService.get('/api/product/getbystring', config, function (result) {
                    if (result.data.length == 1) {
                        addToList(result.data[0]);
                    }
                    else
                        $scope.searchedProducts = result.data;

                }, function (error) {
                    //notificationService.displayError(error);
                });
            }
        }
        function addOrder() {
            $scope.orderPrint = {};
            $scope.addingOrder = true;
            $scope.orderReturn = {};
            if (localStorage.getItem("userId")) {
                $scope.order.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
            $scope.order.ChannelId = $scope.selectedChannel.Id;
            $scope.order.BranchId = $scope.branchSelectedRoot.Id;
            if (!isNullOrEmpty($scope.customer)) {
                $scope.customer.dienthoai = $scope.searchPhone;
                $scope.customer.idtp = $scope.selectedCity.id;
            }
            if ($scope.selectedDistrict && !isNullOrEmpty($scope.customer))
                $scope.customer.idquan = $scope.selectedDistrict.id;
            if ($scope.selectedShipper && !isNullOrEmpty($scope.customer))
                $scope.order.ShipperId = $scope.selectedShipper.Id;
            $scope.order.Status = 3;
            $scope.order.ghichu = $scope.ghichu;
            $scope.order.diemsp = Math.floor(($scope.diemsp - $scope.discount) / 1000);
            $scope.order.datru_diem = Math.ceil($scope.datru_diem);
            $scope.order.Discount = $scope.discount;
            var anotherfee = 0;
            if ($scope.anotherFee != null && $scope.anotherFee != "")
                var anotherfee = parseInt($scope.anotherFee);
            $scope.order.ship = Math.floor(anotherfee + $scope.feeShip + $scope.feeDeliveryTime);
            $scope.order.phithuho = $scope.cod;
            $scope.order.thongtinxedo = $scope.coachInfo;
            $scope.order.tongtien = totalMoney();
            if ($scope.selectedPayment)
                $scope.order.pttt = parseInt($scope.selectedPayment.value);
            if ($scope.selectedCity.id == 1 && $scope.HCMType == 3 && $scope.selectedTypeShipHCM) {
                $scope.order.ptgh = parseInt($scope.selectedTypeShipHCM);
                $scope.order.tenptgh = $scope.nameTypeShip;
            }
            else if ($scope.selectedCity.id != 1 && $scope.selectedDistrict && $scope.selectedTypeShipOutHCM) {
                $scope.order.ptgh = parseInt($scope.selectedTypeShipOutHCM);
                $scope.order.tenptgh = $scope.nameTypeShip;
            }
            if ($scope.selectedDeliveryTime)
                $scope.order.idgiogiao = $scope.selectedDeliveryTime.value;
            $scope.order.OrderDetails = $scope.detailOrders;
            if (!isNullOrEmpty($scope.customer)) {
                $scope.order.Customer = $scope.customer;
                $scope.order.Customer.diem = sumPoint();
            }
            var currentDate = $scope.currentDate;
            $scope.order.Code = $scope.branchSelectedRoot.Code + ('0' + (currentDate.getDate())).slice(-2) + ('0' + (currentDate.getMonth() + 1)).slice(-2) + String(currentDate.getFullYear()).slice(-2) + $scope.selectedChannel.Code;

            apiService.post('api/order/add/', $scope.order,
                function (result) {
                    $scope.addingOrder = false;
                    notificationService.displaySuccess('Đơn đã được hoàn thành.');
                    $rootScope.channel = null;
                    $rootScope.customer = null;
                    $rootScope.historyOrder = null;
                    $rootScope.detailOrders = null;
                    $scope.historyOrder = null;
                    $scope.orderReturn = result.data;
                    var sum = 0;
                    $.each($scope.orderReturn.donhang_ct, function (index, value) {
                        sum += value.Soluong * value.Dongia;
                    });
                    $scope.totalMoneyPrint = sum;
                    if ($scope.orderReturn.ChannelId == 1)
                        setTimeout(function () {
                            var innerContents = document.getElementById("printDivCH").innerHTML;
                            var popupWinindow = window.open();
                            popupWinindow.window.focus();
                            popupWinindow.document.open();
                            popupWinindow.document.write('<!DOCTYPE html><html><head>'
                                + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
                                + '</head><body onload="window.print(); window.close();"><div>'
                                + innerContents + '</div></html>');
                            popupWinindow.document.close();
                        }, 500);
                    resetOrder();
                    init();
                    updateDataOrderWindow();
                }, function (error) {
                    notificationService.displayError(error.data);
                    $scope.addingOrder = false;
                });
        }
        function updateDataOrderWindow() {
            if (localStorage.getItem("userId")) {
                $scope.order.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
            $scope.order.ChannelId = $scope.selectedChannel.Id;
            $scope.order.BranchId = $scope.branchSelectedRoot.Id;
            if (!isNullOrEmpty($scope.customer)) {
                $scope.customer.dienthoai = $scope.searchPhone;
                $scope.customer.idtp = $scope.selectedCity.id;
            }
            if ($scope.selectedDistrict && !isNullOrEmpty($scope.customer))
                $scope.customer.idquan = $scope.selectedDistrict.id;
            if ($scope.selectedShipper && !isNullOrEmpty($scope.customer))
                $scope.order.ShipperId = $scope.selectedShipper.Id;
            $scope.order.Status = 3;
            $scope.order.ghichu = $scope.ghichu;
            //$scope.order.diemsp = Math.floor(($scope.diemsp - $scope.discount) / 1000);
            $scope.order.datru_diem = Math.ceil($scope.datru_diem);
            $scope.order.Discount = $scope.discount;
            var anotherfee = 0;
            if ($scope.anotherFee != null && $scope.anotherFee != "")
                var anotherfee = parseInt($scope.anotherFee);
            //$scope.order.ship = Math.floor(anotherfee + $scope.feeShip + $scope.feeDeliveryTime);
            $scope.order.phithuho = $scope.cod;
            $scope.order.thongtinxedo = $scope.coachInfo;
            $scope.order.tongtienOff = totalMoney();
            if ($scope.selectedPayment)
                $scope.order.pttt = parseInt($scope.selectedPayment.value);
            if ($scope.selectedDeliveryTime)
                $scope.order.idgiogiao = $scope.selectedDeliveryTime.value;
            $scope.order.OrderDetails = $scope.detailOrders;
            if (!isNullOrEmpty($scope.customer)) {
                $scope.order.Customer = $scope.customer;
                $scope.order.Customer.diemOff = sumPoint();
            }
            var currentDate = $scope.currentDate;
            $scope.order.Code = $scope.branchSelectedRoot.Code + ('0' + (currentDate.getDate())).slice(-2) + ('0' + (currentDate.getMonth() + 1)).slice(-2) + String(currentDate.getFullYear()).slice(-2) + $scope.selectedChannel.Code;
            $scope.order.tonggiam = $scope.discount + $scope.datru_diem;
            $scope.order.thanhtoan = total();
            $scope.order.phikhac = $scope.anotherFee;
            $scope.order.khachdua = $scope.paid;
            $scope.order.tienthua = $scope.paid - totalMoney();
            $scope.order.datru_diem = $scope.datru_diem;
            $scope.order.giamgia = $scope.discount;
            var data = {
                userId: $scope.userId,
                value: JSON.stringify($scope.order)
            }
            apiService.post('api/order/addofflineorderwindow', data, function (result) {
                $scope.chatHub.server.update_order_window($scope.userId);
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function saveOrder() {
            $scope.addingOrder = true;
            if (localStorage.getItem("userId")) {
                $scope.order.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
            $scope.order.ChannelId = $scope.selectedChannel.Id;
            $scope.order.BranchId = $scope.branchSelectedRoot.Id;
            if (!isNullOrEmpty($scope.customer)) {
                $scope.customer.dienthoai = $scope.searchPhone;
                $scope.customer.idtp = $scope.selectedCity.id;
            }
            if ($scope.selectedDistrict)
                $scope.customer.idquan = $scope.selectedDistrict.id;
            if ($scope.selectedShipper)
                $scope.order.ShipperId = $scope.selectedShipper.Id;
            $scope.order.Status = 1;
            $scope.order.ghichu = $scope.ghichu;
            $scope.order.diemsp = Math.floor(($scope.diemsp - $scope.discount) / 1000);
            $scope.order.datru_diem = Math.ceil($scope.datru_diem);
            $scope.order.Discount = $scope.discount;
            var anotherfee = 0;
            if ($scope.anotherFee != null && $scope.anotherFee != "")
                var anotherfee = parseInt($scope.anotherFee);
            $scope.order.ship = Math.floor(anotherfee + $scope.feeShip + $scope.feeDeliveryTime);
            $scope.order.phithuho = $scope.cod;
            $scope.order.thongtinxedo = $scope.coachInfo;
            $scope.order.tongtien = totalMoney();
            if ($scope.selectedPayment) {
                var pm = parseInt($scope.selectedPayment.value);
                if (pm == 1 && $scope.selectedCity.id > 1)
                    pm = 3;
                $scope.order.pttt = pm;
            }
            if ($scope.selectedCity.id == 1 && $scope.HCMType == 3 && $scope.selectedTypeShipHCM) {
                $scope.order.ptgh = parseInt($scope.selectedTypeShipHCM);
                $scope.order.tenptgh = $scope.nameTypeShip;
            }
            else if ($scope.selectedCity.id != 1 && $scope.selectedDistrict && $scope.selectedTypeShipOutHCM) {
                $scope.order.ptgh = parseInt($scope.selectedTypeShipOutHCM);
                $scope.order.tenptgh = $scope.nameTypeShip;
            }
            if ($scope.selectedDeliveryTime)
                $scope.order.idgiogiao = $scope.selectedDeliveryTime.value;
            $scope.order.OrderDetails = $scope.detailOrders;
            if (!isNullOrEmpty($scope.customer)) {
                $scope.order.Customer = $scope.customer;
            }
            var currentDate = $scope.currentDate;
            $scope.order.Code = $scope.branchSelectedRoot.Code + ('0' + (currentDate.getDate())).slice(-2) + ('0' + (currentDate.getMonth() + 1)).slice(-2) + String(currentDate.getFullYear()).slice(-2) + $scope.selectedChannel.Code;
            apiService.post('api/order/save/', $scope.order,
                function (result) {
                    $scope.orderReturn = result.data;
                    var sum = 0;
                    $.each($scope.orderReturn.donhang_ct, function (index, value) {
                        sum += value.Soluong * value.Dongia;
                    });
                    $scope.totalMoneyPrint = sum;
                    if ($scope.orderReturn.ChannelId == 2)
                        setTimeout(function () {
                            var innerContents = document.getElementById("printDivNotCH").innerHTML;
                            var popupWinindow = window.open();
                            popupWinindow.window.focus();
                            popupWinindow.document.open();
                            popupWinindow.document.write('<!DOCTYPE html><html><head>'
                                + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
                                + '</head><body onload="window.print(); window.close();"><div>'
                                + innerContents + '</div></html>');
                            popupWinindow.document.close();
                        }, 500);
                    $scope.addingOrder = false;
                    notificationService.displaySuccess('Tạo đơn thành công.');
                    $rootScope.channel = null;
                    $rootScope.customer = null;
                    $rootScope.historyOrder = null;
                    $rootScope.detailOrders = null;
                    $scope.historyOrder = null;
                    resetOrder();
                    init();
                    //$state.go('orders');
                }, function (error) {
                    notificationService.displayError(error.data);
                    $scope.addingOrder = false;
                });
        }
        function addToList(item) {
            $scope.selectedProduct = null;
            $scope.isAdding = true;
            $scope.selectedProduct = item;
            if ((!item.kg && (!item.chieucao && !item.chieudai && !item.chieurong)) && $scope.selectedChannel.Id != 1) {
                var modalInstance = $uibModal.open({
                    templateUrl: '/app/components/orders/productKgUpdateModal.html' + BuildVersion,
                    controller: 'productKgUpdateController',
                    scope: $scope,
                    backdrop: 'static',
                    keyboard: false
                });
                modalInstance.result.then(function (data) {
                    if (data) {
                        item.kg = data.kg;
                        item.chieucao = data.chieucao;
                        item.chieudai = data.chieudai;
                        item.chieurong = data.chieurong;
                    }

                    if ($scope.detailOrders.length > 0) {
                        if ($scope.addedProductIds.indexOf(item.id) == -1) {
                            $scope.detailOrders.push(item);
                            $scope.addedProductIds.push(item.id);
                        }
                        else {
                            $.each($scope.detailOrders, function (index, value) {
                                if (value.id == item.id)
                                    value.Quantity += 1;
                            });
                        }
                    }
                    else {
                        $scope.detailOrders.push(item);
                        $scope.addedProductIds.push(item.id);
                    }
                    setFoodCart();
                    setFreeShipCart();
                    updateDistrict();
                    $scope.searchText = "";
                    $scope.searchedProducts = [];
                    $scope.selectedProduct = null;
                }, function () {

                });
            }
            else {
                if ($scope.detailOrders.length > 0) {
                    if ($scope.addedProductIds.indexOf(item.id) == -1) {
                        $scope.detailOrders.push(item);
                        $scope.addedProductIds.push(item.id);
                    }
                    else {
                        $.each($scope.detailOrders, function (index, value) {
                            if (value.id == item.id)
                                value.Quantity += 1;
                        });
                    }
                }
                else {
                    $scope.detailOrders.push(item);
                    $scope.addedProductIds.push(item.id);
                }
                setFoodCart();
                setFreeShipCart();
                updateDistrict();
                $scope.searchText = "";
                $scope.searchedProducts = [];
                $scope.selectedProduct = null;
            }
        }
        function removeFromList(index) {
            $scope.detailOrders.splice(index, 1);
            $scope.addedProductIds.splice(index, 1);
            setFoodCart();
            setFreeShipCart();
            updateDistrict();
            //getFeeShip();
            updateDataOrderWindow();
        }
        function sumMoneyCartNoDiscount() {
            var total = 0;
            $.each($scope.detailOrders, function (index, item) {
                total += item.Price * item.Quantity;
            });
            return total;
        }
        function sumMoney() {
            var total = 0;
            var totalDis = 0;
            if ($scope.useDiscountMember == 1) {
                $.each($scope.detailOrders, function (index, item) {
                    if (item.NotDiscountMember == true || item.PriceBeforeDiscount > 0 || item.CategoryId == 9 || item.CategoryId == 18) {
                        total += item.Price * item.Quantity;
                    }
                    else {
                        total += (item.Price * item.Quantity) * 0.95;
                        totalDis += (item.Price * item.Quantity) * 0.05;
                    }

                });
                $scope.datru_diem = totalDis;
                $scope.giamthanhvien = 1000;
            }
            else {
                $.each($scope.detailOrders, function (index, item) {
                    total += item.Price * item.Quantity;
                });
                $scope.giamthanhvien = 0;
                $scope.datru_diem = 0;
            }
            $scope.diemsp = total;
            return total;
        }
        function total() {
            return sumMoney() - $scope.discount;
        }
        function totalMoney() {
            var anotherfee = 0;
            if ($scope.anotherFee != null && $scope.anotherFee != "")
                var anotherfee = parseInt($scope.anotherFee);
            return total() + anotherfee + $scope.cod + $scope.feeShip + $scope.feeDeliveryTime;
        }
        function totalMoneyNoCOD() {
            var anotherfee = 0;
            if ($scope.anotherFee != null && $scope.anotherFee != "")
                var anotherfee = parseInt($scope.anotherFee);
            return total() + anotherfee + $scope.feeShip + $scope.feeDeliveryTime;
        }
        function resetOrderDetails() {
            $scope.detailOrders = [];
            $scope.addedProductIds = [];
        }
        function resetOrder() {
            $scope.order = {
                Status: true
            }
            $scope.customer = {
                diem: 0
            };
            $scope.currentDate = new Date();
            $scope.convertCreatedDate = "";
            $scope.orders = [];
            $scope.selectedProduct = null;
            $scope.searchedProducts = [];
            $scope.searchText = null;
            $scope.addedProductIds = [];
            $scope.product = {};
            $scope.detailOrders = [];
            $scope.isAdding = false;
            $scope.anotherFee = null;
            $scope.cod = 0;
            $scope.paid = null;
            $scope.searchPhone = null;
            $scope.diemsp = 0;
            $scope.giamthanhvien = 0;
            $scope.datru_diem = 0;
            $scope.useDiscountMember = null;
            $scope.coachInfo = '';
            $scope.ghichu = '';
            initDiscount();
            loadChannels();
            loadUsers();
            loadCities();
            $scope.selectedDistrict = null;
            resetAfterSelectDistrict();
        }
        function resetAfterSelectDistrict() {
            //$scope.selectedPayment = null;
            $scope.selectedPayment = null;
            $scope.selectedDeliveryTime = null;
            $scope.selectedShipper = null;
            $scope.selectedTypeShipHCM = null;
            $scope.selectedTypeShipOutHCM = null;
            $scope.nameTypeShip = null;
            $scope.cod = 0;
            $scope.feeDeliveryTime = 0;
            $scope.feeShip = 0;
        }
        function init() {
            $scope.discountType = 'money';
            var currentDate = $scope.currentDate;
            $scope.convertCreatedDate = currentDate.getDate() + '/' + (currentDate.getMonth() + 1) + '/' + currentDate.getFullYear() + " " + currentDate.getHours() + ':' + currentDate.getMinutes();
            //$scope.createdDate = currentDate.getFullYear() + '-' +
            //('00' + (currentDate.getMonth() + 1)).slice(-2) + '-' +
            //('00' + currentDate.getcurrentDate()).slice(-2) + ' ' +
            //('00' + currentDate.getHours()).slice(-2) + ':' +
            //('00' + currentDate.getMinutes()).slice(-2) + ':' +
            //('00' + currentDate.getSeconds()).slice(-2);

            //if (localStorage.getItem("userName")) {
            //    $scope.order.CreatedBy = JSON.parse(localStorage.getItem("userName"));
            //}
            if (localStorage.getItem("userId")) {
                $scope.order.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
            if ($rootScope.channel) {
                $scope.selectedChannel = $rootScope.channel;
                $scope.customer = $rootScope.customer;
                if (!isNullOrEmpty($scope.customer))
                    $scope.searchPhone = $scope.customer.dienthoai;
                $scope.historyOrder = $sce.trustAsHtml($rootScope.historyOrder);

                //$q.all([usersPromise]).then(function (result) {
                //    debugger
                //    if ($rootScope.shipperId > 0)
                //    $.each($scope.users, function (index, value) {
                //        if (value.Id == $rootScope.shipperId) {
                //            $scope.selectedShipper = value;
                //            return false;
                //        }

                //    });
                //});

                $.each($rootScope.detailOrders, function (index, value) {
                    $scope.detailOrders.push(value);
                    $scope.addedProductIds.push(value.id);
                });
                setFoodCart();
                setFreeShipCart();
                getFeeShip();
            }
        }
        function updateSelectedChannel() {
            if ($scope.selectedChannel) {
                var channelName = $scope.selectedChannel.Name;
                $scope.channelName = channelName.toUpperCase();
                var currentDate = $scope.currentDate;
                $scope.order.ChannelId = $scope.selectedChannel.Id;
                resetOrderDetails();
                $scope.order.Code = $scope.branchSelectedRoot.Code + ('0' + (currentDate.getDate())).slice(-2) + ('0' + (currentDate.getMonth() + 1)).slice(-2) + String(currentDate.getFullYear()).slice(-2) + $scope.selectedChannel.Code;
                $scope.selectedChannel = $scope.selectedChannel;
                localStorage.setItem("selectedChannel", JSON.stringify($scope.selectedChannel));
                resetOrder();
                if ($scope.selectedChannel.Id == 1)
                    $scope.selectedPayment = $scope.paymentHCMDefault;
                else
                    $scope.selectedPayment = null;
            }
        }
        function updateDiscountMoney() {
            var discountMoney = 0;
            if ($scope.discountMoney != null && $scope.discountMoney != "")
                var discountMoney = parseInt($scope.discountMoney);
            $scope.discount = discountMoney;
            $scope.order.DiscountMoney = discountMoney;
        }
        function updateDiscountPercent() {
            $scope.discount = (sumMoney() * $scope.discountPercent) / 100;
            $scope.order.DiscountPercent = $scope.discountPercent;
        }
        function updateDiscountCode() {
            //$scope.discount = $scope.discountCode;
            $scope.order.DiscountCode = $scope.discountCode;
        }
        function updateCity() {
            let orderedTinh = _.orderBy($scope.selectedCity.donhang_chuyenphat_tinh, ['priority'], ['asc']);
            $scope.districts = orderedTinh;
            //$scope.selectedDistrict = $scope.districts[0];
            $scope.selectedDistrict = null;
            resetAfterSelectDistrict();
        }
        function updateDistrict() {
            resetAfterSelectDistrict();
            if ($scope.selectedCity) {
                if ($scope.selectedCity.id == 1) {
                    setHCMType();
                    if ($scope.selectedChannel.Id == 2 && $scope.selectedDistrict && $scope.selectedDistrict.id > 0) {
                        $scope.selectedPayment = $scope.paymentHCMDefault;
                        $scope.selectedDeliveryTime = $scope.deliveryTimeHCMDefault;
                    }
                }
            }
            getFeeShip();

        }
        function updateDiscountMember() {
            sumMoney();
            if ($scope.useDiscountMember != 1)
                initDiscount();
        }
        function searchCustomerByPhone() {
            if ($scope.searchPhone.length >= 8) {
                var config = {
                    params: {
                        phone: $scope.searchPhone
                    }
                }
                apiService.get('/api/customer/getbyphone', config, function (result) {
                    if (result.data != null) {
                        $scope.customer = result.data;
                        $scope.customer.diem = parseInt($scope.customer.diem);
                        $.each($scope.cities, function (i, item) {
                            if (item.id == $scope.customer.idtp) {
                                $scope.selectedCity = item;
                                return false;
                            }
                        });
                        $scope.districts = $scope.selectedCity.donhang_chuyenphat_tinh;
                        $.each($scope.districts, function (i, item) {
                            if (item.id == $scope.customer.idquan) {
                                $scope.selectedDistrict = item;
                                return false;
                            }
                        });
                        updateDistrict();
                    }
                    else {
                        initCustomer();
                        initCity();
                    }
                }, function (error) {
                    //notificationService.displayError(error);
                });
            }
            else {
                initCustomer();
                initCity();
            }
        }
        function initCity() {
            $scope.selectedCity = $scope.cities[0];
            let orderedTinh = _.orderBy($scope.cities[0].donhang_chuyenphat_tinh, ['priority'], ['asc']);
            $scope.districts = orderedTinh;
            //$scope.selectedDistrict = $scope.districts[0];
        }
        function initCustomer() {
            $scope.customer = {};
            //$scope.customer.hoten = 'Khách vãng lai';
            $scope.customer.diem = 0;
        }
        function initDiscount() {
            $scope.discountType = 'money';
            $scope.discountMoney = null;
            $scope.discountPercent = null;
            $scope.discountCode = null;
            $scope.discount = 0;
        }
        function sumPoint() {
            var diem = 0;
            if (!isNullOrEmpty($scope.customer))
                diem = $scope.customer.diem;
            return diem + Math.floor($scope.diemsp / 1000) - Math.floor($scope.discount / 1000) - $scope.giamthanhvien;
        }
        function checkFastDelivery() {
            var result = false;
            var d = new Date();
            var hour = d.getHours();
            if (hour >= 9 && hour <= 20)
                result = true;
            return result;
        }
        function checkSubUrban() {
            if ($scope.selectedDistrict) {
                var result = false;
                if ($scope.selectedDistrict.id == 123 || $scope.selectedDistrict.id == 126 || $scope.selectedDistrict.id == 132 || $scope.selectedDistrict.id == 133 || $scope.selectedDistrict.id == 116 || $scope.selectedDistrict.id == 121 || $scope.selectedDistrict.id == 122) {
                    result = true;
                }
                return result;
            }
        }
        function getFeeShip() {
            if ($scope.selectedDistrict && $scope.selectedChannel.Id != 1) {
                if ($scope.selectedCity.id == 1) {
                    if ($scope.HCMType != 3) {
                        if ($scope.freeShipCart == true)
                            $scope.feeShip = 0;
                        else if ($scope.HCMType == 1) {
                            if ($scope.foodCart == true) {
                                if (sumMoneyCartNoDiscount() <= 400000)
                                    $scope.feeShip = $scope.selectedDistrict.ship;
                                else
                                    $scope.feeShip = 0;
                            }
                            else {
                                if (sumMoneyCartNoDiscount() <= 200000)
                                    $scope.feeShip = $scope.selectedDistrict.ship;
                                else
                                    $scope.feeShip = 0;
                            }
                        }
                        else {
                            if ($scope.foodCart == true) {
                                if (sumMoneyCartNoDiscount() <= 500000)
                                    $scope.feeShip = $scope.selectedDistrict.ship;
                                else
                                    $scope.feeShip = 0;
                            }
                            else {
                                if (sumMoneyCartNoDiscount() <= 300000)
                                    $scope.feeShip = $scope.selectedDistrict.ship;
                                else
                                    $scope.feeShip = 0;
                            }
                        }
                    }
                    else {
                        var isTwoProductCart = false;
                        var dem = 0;
                        $.each($scope.detailOrders, function (index, value) {
                            if (value.id > 0)
                                dem += value.Quantity;
                        });
                        if (dem > 1)
                            isTwoProductCart = true;
                        isTwoProductCart = false;

                        //check GHTK, rules
                        $scope.passGHTKRules = true;
                        $scope.passVNEPRules = true;
                        var maxChieuDai = $scope.detailOrders[0].chieudai;
                        var maxChieuRong = $scope.detailOrders[0].chieurong;
                        var maxChieuCao = $scope.detailOrders[0].chieucao;
                        $.each($scope.detailOrders, function (index, value) {
                            if ((value.chieudai == null || value.chieudai == 0)
                                && (value.chieucao == null || value.chieucao == 0)
                                && (value.chieurong == null || value.chieurong == 0)) {
                                $scope.passGHTKRules = false;
                                $scope.passVNEPRules = false;
                                return false;
                            }
                            else {
                                if (value.chieudai >= 30 || value.chieurong >= 30 || value.chieucao >= 30 || value.kg >= 3)
                                    $scope.passVNEPRules = false;
                                if (value.chieudai > maxChieuDai)
                                    maxChieuDai = value.chieudai;
                                if (value.chieurong > maxChieuRong)
                                    maxChieuRong = value.chieurong;
                                if (value.chieucao > maxChieuCao)
                                    maxChieuCao = value.chieucao;
                            }
                        });
                        if (maxChieuDai >= 80 || maxChieuRong >= 80 || maxChieuCao >= 80 || sumKg() >= 20) {
                            $scope.passGHTKRules = false;
                        }

                        var kg = sumKg();
                        var kgLWHFast = sumKgLWHFast();
                        if (kg < kgLWHFast)
                            kg = kgLWHFast;
                        kg = kg * 1.1;
                        //VT Huyen HCM
                        var VTPFeeHCM = 0;
                        if (kg <= 3) {
                            VTPFeeHCM = 25000;
                        }
                        else {
                            var kgdu = kg - 3;
                            var rm = Math.ceil(kgdu);
                            VTPFeeHCM = 25000 + (rm * 5000);
                        }
                        VTPFeeHCM = VTPFeeHCM * 1.1;
                        if (isTwoProductCart == true)
                            VTPFeeHCM = VTPFeeHCM * 1.2;
                        VTPFeeHCM = Math.ceil(VTPFeeHCM / 100);
                        VTPFeeHCM = VTPFeeHCM * 100;
                        $scope.VTPFeeHCM = VTPFeeHCM;

                        //GHTK Huyen HCM
                        var GHTKFeeHCM = 0;
                        if (kg <= 3) {
                            GHTKFeeHCM = 35000;
                        }
                        else {
                            var kgdu = kg - 3;
                            var rm = Math.ceil(kgdu);
                            GHTKFeeHCM = 35000 + (rm * 2500);
                        }
                        if (isTwoProductCart == true)
                            GHTKFeeHCM = GHTKFeeHCM * 1.2;
                        GHTKFeeHCM = Math.ceil(GHTKFeeHCM / 100);
                        GHTKFeeHCM = GHTKFeeHCM * 100;
                        $scope.GHTKFeeHCM = GHTKFeeHCM;

                        //VnExpress Huyen HCM
                        var VNEPFeeHCM = 0;
                        if (kg <= 3) {
                            VNEPFeeHCM = 35000;
                        }
                        else {
                            var kgdu = kg - 3;
                            var rm = Math.ceil(kgdu);
                            VNEPFeeHCM = 35000 + (rm * 2500);
                        }
                        VNEPFeeHCM = VNEPFeeHCM * 1.1;
                        if (isTwoProductCart == true)
                            VNEPFeeHCM = VNEPFeeHCM * 1.2;
                        VNEPFeeHCM = Math.ceil(VNEPFeeHCM / 100);
                        VNEPFeeHCM = VNEPFeeHCM * 100;
                        $scope.VNEPFeeHCM = VNEPFeeHCM;

                        //VNPost huyen HCM
                        var VNPFeeHCM = 0;
                        var regionsVNPHCM = [];
                        $.each($scope.regions, function (index, value) {
                            if (value.mavung == $scope.selectedDistrict.mavungvnpost)
                                regionsVNPHCM.push(value);

                        })
                        if (regionsVNPHCM.length > 0) {
                            var kglessthan10k = 0;
                            if (kg <= 10) {
                                switch (true) {
                                    case (kg <= 1):
                                        $.each(regionsVNPHCM, function (index, value) {
                                            if (value.kilogram == 1)
                                                VNPFeeHCM = value.ship;
                                        });
                                        break;
                                    case (kg > 1 && kg <= 2):
                                        $.each(regionsVNPHCM, function (index, value) {
                                            if (value.kilogram == 2)
                                                VNPFeeHCM = value.ship;
                                        });
                                        break;
                                    case (kg > 2 && kg <= 5):
                                        $.each(regionsVNPHCM, function (index, value) {
                                            if (value.kilogram == 5)
                                                VNPFeeHCM = value.ship;
                                        });
                                        break;
                                    case (kg > 5 && kg <= 10):
                                        $.each(regionsVNPHCM, function (index, value) {
                                            if (value.kilogram == 10)
                                                VNPFeeHCM = value.ship;
                                        });
                                }
                                kglessthan10k = VNPFeeHCM;
                            }
                            else {
                                var kg10 = 0;
                                var kg1 = 0;
                                $.each(regionsVNPHCM, function (index, value) {
                                    if (value.kilogram == 10)
                                        kg10 = value.ship;
                                    if (value.kilogram == 0)
                                        kg1 = value.ship;
                                });
                                var kgdu = Math.ceil(kg - 10);
                                VNPFeeHCM = kg10 + (kgdu * kg1);
                            }
                            if (isTwoProductCart == true)
                                VNPFeeHCM = VNPFeeHCM * 1.2;
                            VNPFeeHCM = Math.ceil(VNPFeeHCM / 100);
                            VNPFeeHCM = VNPFeeHCM * 100;
                            $scope.VNPFeeHCM = VNPFeeHCM;
                        }

                        //min ship
                        if ($scope.freeShipCart == true) {
                            //$scope.VTPFeeHCM /= 2;
                            $scope.VTPFeeHCM = 9999999;
                            $scope.GHTKFeeHCM /= 2;
                            //$scope.VNEPFeeHCM /= 2;
                            $scope.VNEPFeeHCM = 9999999;
                            var min = Math.min(VTPFeeHCM, GHTKFeeHCM, VNEPFeeHCM, VNPFeeHCM);
                            if (VTPFeeHCM == min) {
                                $scope.freeShipType = 1;
                                $scope.VTPFeeHCM = 0;
                            }
                            else if (VNPFeeHCM == min) {
                                $scope.freeShipType = 2;
                                $scope.GHTKFeeHCM = 0;
                            }
                            else if (VNPFeeHCM == min) {
                                $scope.freeShipType = 3;
                                $scope.VNPFeeHCM = 0;
                            }
                            else {
                                $scope.freeShipType = 4;
                                $scope.VNEPFeeHCM = 0;
                            }
                        }
                        else
                            $scope.freeShipType = 0;
                    }
                }
                else if ($scope.selectedCity.id > 1) {
                    var isTwoProductCart = false;
                    var dem = 0;
                    $.each($scope.detailOrders, function (index, value) {
                        if (value.id > 0)
                            dem += value.Quantity;
                    });
                    if (dem > 1)
                        isTwoProductCart = true;
                    isTwoProductCart = false;
                    //check GHTK, rules
                    $scope.passGHTKRules = true;
                    $scope.passVNEPRules = true;
                    var maxChieuDai = $scope.detailOrders[0].chieudai;;
                    var maxChieuRong = $scope.detailOrders[0].chieurong;;
                    var maxChieuCao = $scope.detailOrders[0].chieucao;
                    $.each($scope.detailOrders, function (index, value) {
                        if (value.chieudai == null && value.chieucao == null && value.chieurong == null) {
                            $scope.passGHTKRules = false;
                            $scope.passVNEPRules = false;
                            return false;
                        }
                        else {
                            if (value.chieudai >= 30 || value.chieurong >= 30 || value.chieucao >= 30 || value.kg >= 3)
                                $scope.passVNEPRules = false;
                            if (value.chieudai > maxChieuDai)
                                maxChieuDai = value.chieudai;
                            if (value.chieurong > maxChieuRong)
                                maxChieuRong = value.chieurong;
                            if (value.chieucao > maxChieuCao)
                                maxChieuCao = value.chieucao;
                        }
                    });
                    if (maxChieuDai >= 80 || maxChieuRong >= 80 || maxChieuCao >= 80 || sumKg() >= 20) {
                        $scope.passGHTKRules = false;
                    }
                    var kg = sumKg();
                    var kgLWHFast = sumKgLWHFast();
                    if (kg < kgLWHFast)
                        kg = kgLWHFast;
                    kg = kg * 1.1;

                    //GHTKFee out HCM
                    var GHTKFee = 0;
                    var GHTKTime = 0;
                    var dataGHTKFee = $scope.selectedDistrict.phightk;
                    if (dataGHTKFee > 0) {
                        var kgdu = kg - 0.5;
                        if (isNoiMienGHTK() == true) {
                            GHTKFee = (Math.ceil(kgdu / 0.5) * 5000) + dataGHTKFee;
                        }
                        else
                            GHTKFee = (Math.ceil(kgdu / 0.5) * 10000) + dataGHTKFee;
                    }
                    if ($scope.selectedDistrict.tgghtk)
                        GHTKTime = $scope.selectedDistrict.tgghtk;
                    var GHTKTimeDay = Math.floor(GHTKTime / 24);
                    var GHTKTimeHour = GHTKTime - (GHTKTimeDay * 24);
                    if (GHTKTimeHour > 0)
                        $scope.GHTKTime = GHTKTimeDay + 'd ' + GHTKTimeHour + 'h - ' + parseInt(GHTKTimeDay + 1) + 'd ' + GHTKTimeHour + 'h';
                    else
                        $scope.GHTKTime = GHTKTimeDay + 'd - ' + parseInt(GHTKTimeDay + 1) + 'd';
                    if (isTwoProductCart == true)
                        GHTKFee = GHTKFee * 1.2;
                    GHTKFee = Math.ceil(GHTKFee / 100);
                    GHTKFee = GHTKFee * 100;
                    $scope.GHTKFee = GHTKFee;

                    //VNEPFee out HCM                   
                    var VNEPFee = 0;
                    var VNEPTime = 0;
                    var dataVNEPFee = $scope.selectedDistrict.phivnep;
                    if (dataVNEPFee > 0) {
                        var kgdu = kg - 0.5;
                        if (isNoiMienGHTK() == true) {
                            VNEPFee = (Math.ceil(kgdu / 0.5) * 5000) + dataVNEPFee;
                        }
                        else
                            VNEPFee = (Math.ceil(kgdu / 0.5) * 10000) + dataVNEPFee;
                    }
                    if ($scope.selectedDistrict.tgvnep)
                        VNEPTime = $scope.selectedDistrict.tgvnep;
                    var VNEPTimeDay = Math.floor(VNEPTime / 24);
                    var VNEPTimeHour = VNEPTime - (VNEPTimeDay * 24);
                    if (VNEPTimeHour > 0)
                        $scope.VNEPTime = VNEPTimeDay + 'd ' + VNEPTimeHour + 'h - ' + parseInt(VNEPTimeDay + 1) + 'd ' + VNEPTimeHour + 'h';
                    else
                        $scope.VNEPTime = VNEPTimeDay + 'd - ' + parseInt(VNEPTimeDay + 1) + 'd';
                    VNEPFee = VNEPFee * 1.1;
                    if (isTwoProductCart == true)
                        VNEPFee = VNEPFee * 1.2;
                    VNEPFee = Math.ceil(VNEPFee / 100);
                    VNEPFee = VNEPFee * 100;
                    $scope.VNEPFee = VNEPFee;

                    //VTPEcoFee out HCM
                    var VTPEcoFee = 0;
                    var VTPEcoTime = 0;
                    var kglessthan2k = 0;
                    var regionsVTPEco = [];
                    $.each($scope.regions, function (index, value) {
                        if (value.mavung == $scope.selectedCity.mavung)
                            regionsVTPEco.push(value);

                    })
                    if (kg <= 2) {
                        switch (true) {
                            case (kg <= 0.05):
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 0.05)
                                        VTPEcoFee = value.ship;
                                })
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 0.05)
                                        VTPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.05 && kg <= 0.1):
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 0.1)
                                        VTPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.1 && kg <= 0.25):
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 0.25)
                                        VTPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.25 && kg <= 0.50):
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 0.50)
                                        VTPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.5 && kg <= 1):
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 1)
                                        VTPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 1 && kg <= 1.5):
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 1.5)
                                        VTPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 1.5 && kg <= 2):
                                $.each(regionsVTPEco, function (index, value) {
                                    if (value.kilogram == 2)
                                        VTPEcoFee = value.ship;
                                });
                                break;
                        }
                        kglessthan2k = VTPEcoFee;
                    }
                    else {
                        var kg2 = 0;
                        $.each(regionsVTPEco, function (index, value) {
                            if (value.kilogram == 2)
                                kg2 = value.ship;
                        });
                        var kg05 = 0;
                        $.each(regionsVTPEco, function (index, value) {
                            if (value.kilogram == 0)
                                kg05 = value.ship;
                        });
                        var kgdu = kg - 2;
                        var rm = Math.ceil(kgdu / 0.5);
                        VTPEcoFee = kg2 + (rm * kg05);
                    }
                    VTPEcoFee = VTPEcoFee * 1.15 * 1.1;
                    if ($scope.selectedDistrict.vungsau == true) {
                        var phituyenhuyen = 0;
                        var phiketnoitren1kg = 0;
                        if (kg <= 2) {
                            phituyenhuyen = kglessthan2k * 0.2 * 1.1;
                            VTPEcoFee = VTPEcoFee + phituyenhuyen;
                        }
                        else {
                            if (kg <= 10) {
                                phiketnoitren1kg = 1500;
                            }
                            else {
                                phiketnoitren1kg = 1000;
                            }
                            phituyenhuyen = ((kg2 * 0.2) + (Math.ceil(kgdu) * phiketnoitren1kg)) * 1.1;
                            VTPEcoFee = VTPEcoFee + phituyenhuyen;
                        }
                    }
                    if ($scope.selectedCity.thoigian)
                        VTPEcoTime = parseInt($scope.selectedCity.thoigian);
                    var VTPEcoTimeDay = Math.floor(VTPEcoTime / 24);
                    var VTPEcoTimeHour = VTPEcoTime - (VTPEcoTimeDay * 24);
                    if (VTPEcoTimeHour > 0)
                        $scope.VTPEcoTime = VTPEcoTimeDay + 'd ' + VTPEcoTimeHour + 'h - ' + parseInt(VTPEcoTimeDay + 1) + 'd ' + VTPEcoTimeHour + 'h';
                    else
                        $scope.VTPEcoTime = VTPEcoTimeDay + 'd - ' + parseInt(VTPEcoTimeDay + 1) + 'd';
                    if (isTwoProductCart == true)
                        VTPEcoFee = VTPEcoFee * 1.2;
                    VTPEcoFee = Math.ceil(VTPEcoFee / 100);
                    VTPEcoFee = VTPEcoFee * 100;
                    $scope.VTPEcoFee = VTPEcoFee;

                    //VTPFastFee out HCM
                    var VTPFastFee = 0;
                    var VTPFastTime = 0;
                    var kglessthan2k = 0;
                    var regionsVTPFast = [];
                    $.each($scope.regions, function (index, value) {
                        if (value.mavung == $scope.selectedCity.mavungcpn)
                            regionsVTPFast.push(value);
                    })
                    if (kg <= 2) {
                        switch (true) {
                            case (kg <= 0.05):
                                $.each(regionsVTPFast, function (index, value) {
                                    if (value.kilogram == 0.05)
                                        VTPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.05 && kg <= 0.1):
                                $.each(regionsVTPFast, function (index, value) {
                                    if (value.kilogram == 0.1)
                                        VTPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.1 && kg <= 0.25):
                                $.each(regionsVTPFast, function (index, value) {
                                    if (value.kilogram == 0.25)
                                        VTPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.25 && kg <= 0.50):
                                $.each(regionsVTPFast, function (index, value) {
                                    if (value.kilogram == 0.50)
                                        VTPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.5 && kg <= 1):
                                $.each(regionsVTPFast, function (index, value) {
                                    if (value.kilogram == 1)
                                        VTPFastFee = value.ship;
                                });
                                break;
                            case (kg > 1 && kg <= 1.5):
                                $.each(regionsVTPFast, function (index, value) {
                                    if (value.kilogram == 1.5)
                                        VTPFastFee = value.ship;
                                });
                                break;
                            case (kg > 1.5 && kg <= 2):
                                $.each(regionsVTPFast, function (index, value) {
                                    if (value.kilogram == 2)
                                        VTPFastFee = value.ship;
                                });
                                break;
                        }
                        kglessthan2k = VTPFastFee;
                    }
                    else {
                        var kg2 = 0;
                        $.each(regionsVTPFast, function (index, value) {
                            if (value.kilogram == 2)
                                kg2 = value.ship;
                        });
                        var kg05 = 0;
                        $.each(regionsVTPFast, function (index, value) {
                            if (value.kilogram == 0)
                                kg05 = value.ship;
                        });
                        var kgdu = kg - 2;
                        var rm = Math.ceil(kgdu / 0.5);
                        VTPFastFee = kg2 + (rm * kg05);
                    }
                    VTPFastFee = VTPFastFee * 1.15 * 1.1;
                    if ($scope.selectedDistrict.vungsau == true) {
                        var phituyenhuyen = 0;
                        if (kg <= 2) {
                            phituyenhuyen = kglessthan2k * 0.2 * 1.1;
                        }
                        else {
                            phituyenhuyen = (kg2 + (rm * kg05)) * 0.2 * 1.1;
                        }
                        VTPFastFee = VTPFastFee + phituyenhuyen;
                    }
                    if ($scope.selectedCity.thoigiancpn)
                        VTPFastTime = $scope.selectedCity.thoigiancpn;
                    if ($scope.selectedDistrict.tgcongthem)
                        VTPFastTime += $scope.selectedDistrict.tgcongthem;
                    var VTPFastTimeDay = Math.floor(VTPFastTime / 24);
                    var VTPFastTimeHour = VTPFastTime - (VTPFastTimeDay * 24);
                    if (VTPFastTimeHour > 0)
                        $scope.VTPFastTime = VTPFastTimeDay + 'd ' + VTPFastTimeHour + 'h - ' + parseInt(VTPFastTimeDay + 1) + 'd ' + VTPFastTimeHour + 'h';
                    else
                        $scope.VTPFastTime = VTPFastTimeDay + 'd - ' + parseInt(VTPFastTimeDay + 1) + 'd';
                    if (isTwoProductCart == true)
                        VTPFastFee = VTPFastFee * 1.2;
                    VTPFastFee = Math.ceil(VTPFastFee / 100);
                    VTPFastFee = VTPFastFee * 100;
                    $scope.VTPFastFee = VTPFastFee;

                    //VTP COD
                    var phithuho = 0;
                    if ($scope.selectedDistrict.vungsau == true) {
                        phithuho = totalMoneyNoCOD() * 0.013 * 1.1;
                        if (phithuho <= 22000)
                            phithuho = 22000;
                    }
                    else {
                        phithuho = totalMoneyNoCOD() * 0.008 * 1.1;
                        if (phithuho <= 16500)
                            phithuho = 16500;
                    }
                    $scope.VTPCod = phithuho;

                    //VNPostFast
                    var VNPFastFee = 0;
                    var VNPFastTime = 0;

                    var regionsVNPFast = [];
                    $.each($scope.regions, function (index, value) {
                        if (value.mavung == $scope.selectedDistrict.mavungvnpostnhanh)
                            regionsVNPFast.push(value);
                    })
                    var kglessthan2k = 0;
                    if (kg <= 2) {
                        switch (true) {
                            case (kg <= 0.05):
                                $.each(regionsVNPFast, function (index, value) {
                                    if (value.kilogram == 0.05)
                                        VNPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.05 && kg <= 0.1):
                                $.each(regionsVNPFast, function (index, value) {
                                    if (value.kilogram == 0.1)
                                        VNPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.1 && kg <= 0.25):
                                $.each(regionsVNPFast, function (index, value) {
                                    if (value.kilogram == 0.25)
                                        VNPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.25 && kg <= 0.50):
                                $.each(regionsVNPFast, function (index, value) {
                                    if (value.kilogram == 0.50)
                                        VNPFastFee = value.ship;
                                });
                                break;
                            case (kg > 0.5 && kg <= 1):
                                $.each(regionsVNPFast, function (index, value) {
                                    if (value.kilogram == 1)
                                        VNPFastFee = value.ship;
                                });
                                break;
                            case (kg > 1 && kg <= 1.5):
                                $.each(regionsVNPFast, function (index, value) {
                                    if (value.kilogram == 1.5)
                                        VNPFastFee = value.ship;
                                });
                                break;
                            case (kg > 1.5 && kg <= 2):
                                $.each(regionsVNPFast, function (index, value) {
                                    if (value.kilogram == 2)
                                        VNPFastFee = value.ship;
                                });
                                break;
                        }

                        kglessthan2k = VNPFastFee;
                    }
                    else {
                        var kg2 = 0;
                        var kg05 = 0;
                        $.each(regionsVNPFast, function (index, value) {
                            if (value.kilogram == 2)
                                kg2 = value.ship;
                            if (value.kilogram == 0)
                                kg05 = value.ship;
                        });
                        var kgdu = kg - 2;
                        var rm = Math.ceil(kgdu / 0.5);
                        VNPFastFee = kg2 + (rm * kg05);
                    }

                    VNPFastFee = VNPFastFee * 1.1;
                    if ($scope.selectedDistrict.vungsauvnpost == true) {
                        VNPFastFee = VNPFastFee * 1.2;
                    }
                    if ($scope.selectedCity.thoigianvnpostnhanh)
                        VNPFastTime = $scope.selectedCity.thoigianvnpostnhanh;
                    var VNPFastTimeDay = Math.floor(VNPFastTime / 24);
                    var VNPFastTimeHour = VNPFastTime - (VNPFastTimeDay * 24);
                    if (VNPFastTimeHour > 0)
                        $scope.VNPFastTime = VNPFastTimeDay + 'd ' + VNPFastTimeHour + 'h - ' + parseInt(VNPFastTimeDay + 1) + 'd ' + VNPFastTimeHour + 'h';
                    else
                        $scope.VNPFastTime = VNPFastTimeDay + 'd - ' + parseInt(VNPFastTimeDay + 1) + 'd';
                    if (isTwoProductCart == true)
                        VNPFastFee = VNPFastFee * 1.2;
                    VNPFastFee = Math.ceil(VNPFastFee / 100);
                    VNPFastFee = VNPFastFee * 100;
                    $scope.VNPFastFee = VNPFastFee;

                    //VNPostEco
                    var VNPEcoFee = 0;
                    var VNPEcoTime = 0;
                    var kg = sumKg();
                    var kgLWH = sumKgLWH();
                    if (kg < kgLWH)
                        kg = kgLWH;
                    kg = kg * 1.1;
                    var regionsVNPEco = [];
                    $.each($scope.regions, function (index, value) {
                        if (value.mavung == $scope.selectedCity.mavungvnpostbuukien)
                            regionsVNPEco.push(value);
                    })
                    var kg2 = 0;
                    var kg10 = 0;
                    var kg30 = 0;
                    $.each(regionsVNPEco, function (index, value) {
                        if (value.kilogram == 2)
                            kg2 = value.ship;
                        if (value.kilogram == 10)
                            kg10 = value.ship;
                        if (value.kilogram == 30)
                            kg30 = value.ship;
                    });
                    var kglessthan2k = 0;
                    if (kg <= 2) {
                        switch (true) {
                            case (kg <= 0.1):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 0.1)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.1 && kg <= 0.25):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 0.25)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.25 && kg <= 0.50):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 0.50)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.5 && kg <= 0.75):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 0.75)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 0.75 && kg <= 1):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 1)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 1 && kg <= 1.25):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 1.25)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 1.25 && kg <= 1.5):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 1.5)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 1.5 && kg <= 1.75):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 1.75)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                            case (kg > 1.75 && kg <= 2):
                                $.each(regionsVNPEco, function (index, value) {
                                    if (value.kilogram == 2)
                                        VNPEcoFee = value.ship;
                                });
                                break;
                        }

                        kglessthan2k = VNPEcoFee;
                    }
                    else if (kg > 2 && kg <= 10) {
                        var kgdu = kg - 2;
                        VNPEcoFee = kg2 + (Math.ceil(kgdu) * kg10);
                    }
                    else {
                        var kgdu10 = kg - 10;
                        VNPEcoFee = kg2 + 8 * kg10 + (Math.ceil(kgdu10) * kg30);
                    }

                    VNPEcoFee = VNPEcoFee * 1.1;
                    if ($scope.selectedDistrict.vungsauvnpost == true) {
                        VNPEcoFee = VNPEcoFee * 1.2;
                    }
                    if ($scope.selectedCity.thoigianvnpostbuukien)
                        VNPEcoTime = $scope.selectedCity.thoigianvnpostbuukien;
                    var VNPEcoTimeDay = Math.floor(VNPEcoTime / 24);
                    var VNPEcoTimeHour = VNPEcoTime - (VNPEcoTimeDay * 24);
                    if (VNPEcoTimeHour > 0)
                        $scope.VNPEcoTime = VNPEcoTimeDay + 'd ' + VNPEcoTimeHour + 'h - ' + parseInt(VNPEcoTimeDay + 1) + 'd ' + VNPEcoTimeHour + 'h';
                    else
                        $scope.VNPEcoTime = VNPEcoTimeDay + 'd - ' + parseInt(VNPEcoTimeDay + 1) + 'd';
                    if (isTwoProductCart == true)
                        VNPEcoFee = VNPEcoFee * 1.2;
                    VNPEcoFee = Math.ceil(VNPEcoFee / 100);
                    VNPEcoFee = VNPEcoFee * 100;
                    $scope.VNPEcoFee = VNPEcoFee;

                    //VNP COD
                    var phithuho = 0;
                    switch (true) {
                        case (totalMoney() <= 300000):
                            phithuho = 13000;
                            break;
                        case (totalMoney() > 300000 && totalMoney() <= 600000):
                            phithuho = 15000;
                            break;
                        case (totalMoney() > 600000 && totalMoney() <= 1000000):
                            phithuho = 17000;
                            break;
                        case (totalMoney() > 1000000 && totalMoney() <= 1500000):
                            phithuho = 18000;
                            break;
                        case (totalMoney() > 1500000):
                            phithuho = totalMoney() * 0.012;
                            break;
                    }
                    $scope.VNPCod = phithuho;

                    //FUTAFee out HCM
                    var FUTAFee = 0;
                    var FUTATime = 0;
                    var FUTAAddress = '';
                    var regionsFUTA = [];
                    var kg = sumKg();
                    var kgLWH = sumKgLWH();
                    if (kg < kgLWH)
                        kg = kgLWH;
                    kg = kg * 1.1;
                    $.each($scope.regions, function (index, value) {
                        if (value.mavung == $scope.selectedDistrict.mavungfuta)
                            regionsFUTA.push(value);
                    })
                    $.each($scope.futaAddresses, function (index, value) {
                        if ($scope.selectedCity.id == value.idtp)
                            FUTAAddress = value.diachi;
                    });
                    $scope.FUTAAddress = FUTAAddress;
                    if (FUTAAddress) {
                        var kg_tmp = 0;
                        if (kg < 10) {
                            kg_tmp = (Number(kg.toString().substr(0, 1)));
                        }
                        else {
                            kg_tmp = (Number(kg.toString().substr(0, 2)));
                        }
                        if (kg <= 40) {
                            switch (true) {
                                case (kg <= 0.5):
                                    $.each(regionsFUTA, function (index, value) {
                                        if (value.kilogram == 0.5)
                                            FUTAFee = value.ship;
                                    });
                                    break;
                                case (kg > 0.5 && kg <= 0.9):
                                    $.each(regionsFUTA, function (index, value) {
                                        if (value.kilogram == 0.9)
                                            FUTAFee = value.ship;
                                    });
                                    break;
                                case (kg == 1):
                                    $.each(regionsFUTA, function (index, value) {
                                        if (value.kilogram == 1)
                                            FUTAFee = value.ship;
                                    });
                                    break;
                                case (kg > 1 && kg <= 40):
                                    $.each(regionsFUTA, function (index, value) {
                                        if (kg > kg_tmp) {
                                            if (value.kilogram == kg_tmp + 1)
                                                FUTAFee = value.ship;
                                        }
                                        else {
                                            if (value.kilogram == kg_tmp)
                                                FUTAFee = value.ship;
                                        }
                                    });
                                    break;
                            }
                        }
                        else {
                            var kg1 = 0;
                            $.each(regionsFUTA, function (index, value) {
                                if (value.kilogram == 0)
                                    kg1 = value.ship;
                            });
                            var kg40 = 0;
                            $.each(regionsFUTA, function (index, value) {
                                if (value.kilogram == 40)
                                    kg40 = value.ship;
                            });
                            var kgdu = Math.ceil(kg - 40);
                            FUTAFee = kgdu * kg1 + kg40;
                        }

                        if (isTwoProductCart == true)
                            FUTAFee = FUTAFee * 1.2;
                        FUTAFee = Math.ceil(FUTAFee / 100);
                        FUTAFee = FUTAFee * 100;
                        $scope.FUTAFee = FUTAFee;
                        if ($scope.selectedDistrict.tgxekhachfuta > 0)
                            FUTATime = $scope.selectedDistrict.tgxekhachfuta;
                        var FUTATimeDay = Math.floor(FUTATime / 24);
                        var FUTATimeHour = FUTATime - (FUTATimeDay * 24);
                        if (FUTATimeHour > 0)
                            $scope.FUTATime = FUTATimeDay + 'd ' + FUTATimeHour + 'h - ' + parseInt(FUTATimeDay + 1) + 'd ' + FUTATimeHour + 'h';
                        else
                            $scope.FUTATime = FUTATimeDay + 'd - ' + parseInt(FUTATimeDay + 1) + 'd';
                    }

                    //min ship
                    if ($scope.freeShipCart == true) {
                        //$scope.VTPEcoFee /= 2;
                        $scope.VTPEcoFee = 99999999;
                        //$scope.VTPFastFee /= 2;
                        $scope.VTPFastFee = 99999999;
                        $scope.GHTKFee /= 2;
                        //$scope.VNEPFee /= 2;
                        $scope.VNEPFee = 99999999;
                        $scope.VNPFastFee /= 2;
                        $scope.VNPEcoFee /= 2;
                        $scope.FUTAFee /= 2;
                        var min = Math.min(VTPEcoFee, VTPFastFee, GHTKFee, VNEPFee, FUTAFee, VNPFastFee, VNPEcoFee);
                        if (VTPEcoFee == min) {
                            $scope.freeShipType = 1;
                            $scope.VTPEcoFee = 0;
                        }
                        else if (VTPFastFee == min) {
                            $scope.freeShipType = 3;
                            $scope.VTPFastFee = 0;
                        }
                        else if (GHTKFee == min) {
                            $scope.freeShipType = 4;
                            $scope.GHTKFee = 0;
                        }
                        else if (FUTAFee == min) {
                            $scope.freeShipType = 5;
                            $scope.FUTAFee = 0;
                        }
                        else if (VNPFastFee == min) {
                            $scope.freeShipType = 7;
                            $scope.VNPFastFee = 0;
                        }
                        else if (VNPEcoFee == min) {
                            $scope.freeShipType = 8;
                            $scope.VNPEcoFee = 0;
                        }
                        else {
                            $scope.freeShipType = 9;
                            $scope.VNEPFee = 0;
                        }
                    }
                    else
                        $scope.freeShipType = 0;
                }
            }
            else
                $scope.feeShip = 0;
        }
        function setHCMType() {
            if ($scope.selectedDistrict) {
                if ($scope.selectedDistrict.id == 115 || $scope.selectedDistrict.id == 117 || $scope.selectedDistrict.id == 118 || $scope.selectedDistrict.id == 119 || $scope.selectedDistrict.id == 120 || $scope.selectedDistrict.id == 122 || $scope.selectedDistrict.id == 124 || $scope.selectedDistrict.id == 125 || $scope.selectedDistrict.id == 127 || $scope.selectedDistrict.id == 128 || $scope.selectedDistrict.id == 129 || $scope.selectedDistrict.id == 130 || $scope.selectedDistrict.id == 131) {
                    $scope.HCMType = 1;
                }
                else if ($scope.selectedDistrict.id == 123 || $scope.selectedDistrict.id == 126 || $scope.selectedDistrict.id == 132 || $scope.selectedDistrict.id == 133 || $scope.selectedDistrict.id == 116 || $scope.selectedDistrict.id == 121) {
                    $scope.HCMType = 2;
                }
                else if ($scope.selectedDistrict.id == 134 || $scope.selectedDistrict.id == 135 || $scope.selectedDistrict.id == 136 || $scope.selectedDistrict.id == 137 || $scope.selectedDistrict.id == 889) {
                    $scope.HCMType = 3;
                }
            }
            else
                $scope.HCMType = 0;
        }
        function setFoodCart() {
            $scope.foodCart = true;
            $.each($scope.detailOrders, function (index, value) {
                if (!value.masp.startsWith('T')) {
                    $scope.foodCart = false;
                    return false;
                }
            });
        }
        function setFreeShipCart() {
            $scope.freeShipCart = false;
            $.each($scope.detailOrders, function (index, value) {
                if (value.freeship == true) {
                    $scope.freeShipCart = true;
                    return false;
                }
            });
        }
        function updateDeliveryTime() {
            if ($scope.selectedDeliveryTime) {
                if ($scope.selectedCity.id == 1 && $scope.HCMType != 3) {
                    if ($scope.selectedDeliveryTime.value == 2 || $scope.selectedDeliveryTime.value == 3 || $scope.selectedDeliveryTime.value == 5) {
                        $scope.feeDeliveryTime = $scope.selectedDistrict.phighnhanh;
                    }
                    else
                        $scope.feeDeliveryTime = 0;
                }
                else
                    $scope.feeDeliveryTime = 0;
            }
            else {
                $scope.feeDeliveryTime = 0;
            }
        }
        function sumKg() {
            var total = 0;
            $.each($scope.detailOrders, function (index, value) {
                total += value.kg * parseFloat(value.Quantity);
            });
            //$scope.kg = roundToTwo(total);
            $scope.kg = Math.floor(total * 1000) / 1000;
            return total;
        }
        function sumKgLWH() {
            var total = 0;
            $.each($scope.detailOrders, function (index, value) {
                if (!value.chieudai)
                    value.chieudai = 0;
                if (!value.chieurong)
                    value.chieurong = 0;
                if (!value.chieucao)
                    value.chieucao = 0;
                total += ((value.chieudai * value.chieurong * value.chieucao) / 4000) * parseFloat(value.Quantity);
            });
            return total;
        }
        function sumKgLWHFast() {
            var total = 0;
            $.each($scope.detailOrders, function (index, value) {
                if (!value.chieudai)
                    value.chieudai = 0;
                if (!value.chieurong)
                    value.chieurong = 0;
                if (!value.chieucao)
                    value.chieucao = 0;
                total += ((value.chieudai * value.chieurong * value.chieucao) / 6000) * parseFloat(value.Quantity);
            });
            return total;
        }
        function updateTypeShipHCM() {
            if ($scope.selectedTypeShipHCM == 1) {
                $scope.feeShip = $scope.VTPFeeHCM;
                $scope.nameTypeShip = 'Viettel Post 24h';
            }
            if ($scope.selectedTypeShipHCM == 2) {
                $scope.feeShip = $scope.GHTKFeeHCM;
                $scope.nameTypeShip = 'GH Tiết Kiêm 24h';
                var ghtkShipper = {};
                $.each($scope.users, function (index, value) {
                    if (value.Id == 11)
                        ghtkShipper = value;
                })
                $scope.selectedShipper = ghtkShipper;
            }
            if ($scope.selectedTypeShipHCM == 3) {
                $scope.feeShip = $scope.VNPFeeHCM;
                $scope.nameTypeShip = 'VNPost 48h';
                var vnpShipper = {};
                $.each($scope.users, function (index, value) {
                    if (value.Id == 10)
                        vnpShipper = value;
                })
                $scope.selectedShipper = vnpShipper;
            }
            if ($scope.selectedTypeShipHCM == 4) {
                $scope.feeShip = $scope.VNEPFeeHCM;
                $scope.nameTypeShip = 'GH Vnexpress 24h';
            }
        }
        function isNoiMienGHTK() {
            if ($scope.selectedCity.id == 82 || $scope.selectedCity.id == 83 || $scope.selectedCity.id == 86 || $scope.selectedCity.id == 88 || $scope.selectedCity.id == 90 || $scope.selectedCity.id == 91 || $scope.selectedCity.id == 92 || $scope.selectedCity.id == 93 || $scope.selectedCity.id == 94 || $scope.selectedCity.id == 99 || $scope.selectedCity.id == 100 || $scope.selectedCity.id == 109 || $scope.selectedCity.id == 114 || $scope.selectedCity.id == 116 || $scope.selectedCity.id == 120 || $scope.selectedCity.id == 124 || $scope.selectedCity.id == 132 || $scope.selectedCity.id == 134 || $scope.selectedCity.id == 139 || $scope.selectedCity.id == 140 || $scope.selectedCity.id == 142)
                return true;
            else
                return false;
        }
        function updateTypeShipOutHCM() {
            $scope.cod = 0;
            $scope.feeShip = 0;
            if ($scope.selectedTypeShipOutHCM == 1) {
                $scope.feeShip = $scope.VTPEcoFee;
                $scope.nameTypeShip = 'Viettel Post Tiết Kiệm ' + $scope.VTPEcoTime;
            }
            if ($scope.selectedTypeShipOutHCM == 3) {
                $scope.feeShip = $scope.VTPFastFee;
                $scope.nameTypeShip = 'Viettel Post Nhanh ' + $scope.VTPFastTime;
            }
            if ($scope.selectedTypeShipOutHCM == 4) {
                $scope.feeShip = $scope.GHTKFee;
                $scope.nameTypeShip = 'GH Tiết Kiệm ' + $scope.GHTKTime;
                var ghtkShipper = {};
                $.each($scope.users, function (index, value) {
                    if (value.Id == 11)
                        ghtkShipper = value;
                })
                $scope.selectedShipper = ghtkShipper;
            }
            if ($scope.selectedTypeShipOutHCM == 9) {
                $scope.feeShip = $scope.VNEPFee;
                $scope.nameTypeShip = 'GH Vnexpress ' + $scope.VNEPTime;
            }
            if ($scope.selectedTypeShipOutHCM == 5) {
                $scope.nameTypeShip = 'Xe Phương Trang ' + $scope.FUTATime;
                if (sumMoneyCartNoDiscount() <= 200000)
                    $scope.feeShip = $scope.FUTAFee + 20000;
                else
                    $scope.feeShip = $scope.FUTAFee;
            }
            if ($scope.selectedTypeShipOutHCM == 6) {
                $scope.nameTypeShip = 'Xe Khách Khác';
            }
            if ($scope.selectedTypeShipOutHCM == 7) {
                $scope.feeShip = $scope.VNPFastFee;
                $scope.nameTypeShip = 'VNPost nhanh ' + $scope.VNPFastTime;
                var vnpShipper = {};
                $.each($scope.users, function (index, value) {
                    if (value.Id == 10)
                        vnpShipper = value;
                })
                $scope.selectedShipper = vnpShipper;
            }
            if ($scope.selectedTypeShipOutHCM == 8) {
                $scope.feeShip = $scope.VNPEcoFee;
                $scope.nameTypeShip = 'VNPost thường ' + $scope.VNPEcoTime;
                var vnpShipper = {};
                $.each($scope.users, function (index, value) {
                    if (value.Id == 10)
                        vnpShipper = value;
                })
                $scope.selectedShipper = vnpShipper;
            }
            //$scope.selectedPayment = null;
            $scope.selectedPayment = null;
        }
        function updatePayment() {
            if ($scope.selectedCity.id != 1 && $scope.selectedDistrict) {
                if ($scope.selectedPayment) {
                    if ($scope.selectedPayment.value == 1 && ($scope.selectedTypeShipOutHCM == 1 || $scope.selectedTypeShipOutHCM == 3))
                        $scope.cod = $scope.VTPCod;
                    if ($scope.selectedPayment.value == 1 && ($scope.selectedTypeShipOutHCM == 7 || $scope.selectedTypeShipOutHCM == 8))
                        $scope.cod = $scope.VNPCod;
                    if ($scope.selectedPayment.value != 1)
                        $scope.cod = 0;
                }
            }
            else
                $scope.cod = 0;
        }
        function validSelectedDistrict() {
            if ($scope.selectedChannel)
                if ($scope.selectedChannel.Id > 1)
                    return true
                else
                    return false
            else
                return false
        }
        function validSelectedPayment() {
            if ($scope.selectedCity && $scope.selectedDistrict && $scope.selectedChannel)
                if ($scope.selectedDistrict.id > 0 && $scope.selectedChannel.Id > 1)
                    return true
                else
                    return false
            else
                return false
        }
        function validDeliveryTimeSubUrban() {
            if ($scope.selectedCity && $scope.selectedDistrict && $scope.selectedChannel)
                if ($scope.selectedCity.id == 1 && $scope.selectedDistrict.id > 0 && checkSubUrban() == true && $scope.selectedChannel.Id > 1)
                    return true
                else
                    return false
            else
                return false
        }
        function validDeliveryTime() {
            if ($scope.selectedCity && $scope.selectedDistrict && $scope.selectedChannel)
                if ($scope.selectedCity.id == 1 && $scope.selectedDistrict.id > 0 && checkSubUrban() == false && checkFastDelivery() == false && $scope.selectedChannel.Id > 1)
                    return true
                else
                    return false
            else
                return false
        }
        function validDeliveryTimeFast() {
            if ($scope.selectedCity && $scope.selectedDistrict && $scope.selectedChannel) {

                if ($scope.selectedCity.id == 1 && $scope.selectedDistrict.id > 0 && checkSubUrban() == false && checkFastDelivery() == true && $scope.HCMType != 3 && $scope.selectedChannel.Id > 1) {
                    return true
                }

                else
                    return false
            }
            else
                return false
        }
        function validSelectedTypeShipHCM() {
            if ($scope.selectedCity && $scope.selectedDistrict && $scope.selectedChannel) {

                if ($scope.selectedCity.id == 1 && $scope.selectedDistrict.id > 0 && $scope.HCMType == 3 && $scope.selectedChannel.Id > 1)
                    return true
                else
                    return false
            }
            else
                return false
        }
        function validSelectedTypeShipOutHCM() {
            if ($scope.selectedCity && $scope.selectedDistrict && $scope.selectedChannel) {
                if ($scope.selectedCity.id != 1 && $scope.selectedDistrict.id > 0 && $scope.selectedChannel.Id > 1)
                    return true
                else
                    return false
            }
            else
                return false
        }
        function roundToTwo(num) {
            return +(Math.round(num + "e+2") + "e-2");
        }

        $scope.chatHub = null;
        $scope.chatHub = $.connection.chatHub; // initializes hub
        $.connection.hub.start(); // starts hub

        $scope.$watch('discountType', function (n, o) {
            if (n) {
                switch (n) {
                    case 'money':
                        $scope.discountPercent = null;
                        $scope.discountCode = null;
                        $scope.discount = 0;
                        break;
                    case 'percent':
                        $scope.discountMoney = null;
                        $scope.discountCode = null;
                        $scope.discount = 0;
                        break;
                    case 'code':
                        $scope.discountPercent = null;
                        $scope.discountMoney = null;
                        $scope.discount = 0;
                        break;

                }
            }
        });
        $scope.$watch('searchText', function (tmpStr) {
            if (!tmpStr || tmpStr.length == 0) {
                $scope.searchedProducts = [];
                return 0;
            }
            var config = {
                params: {
                    searchType: $scope.searchType,
                    channelId: $scope.selectedChannel.Id,
                    searchText: $scope.searchText
                }
            }
            $scope.searching = true;
            apiService.get('/api/product/getbystring', config, function (result) {
                $scope.searching = false;
                if (result.data.length == 1) {
                    addToList(result.data[0]);
                }
                else
                    $scope.searchedProducts = result.data;
                updateDataOrderWindow();
            }, function (error) {
                //notificationService.displayError(error);
            });
        });
        $scope.$watch('searchType', function (n, o) {
            if (n) {
                $scope.searchText = "";
            }
        });

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadChannels();
            loadUsers();
            loadCities();
            loadFeeRegionCode();
            loadFutaAddresses();
            init();
            $window.document.title = "Thêm mới đơn hàng";
        }
    }

})(angular.module('softbbm.orders'));