(function (app) {
    app.controller('orderListController', orderListController);

    orderListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state', '$filter', '$rootScope', '$q', 'authenticationService', '$http']
    function orderListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state, $filter, $rootScope, $q, authenticationService, $http) {
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10';
        $scope.totalMoney = 0;
        $scope.channels = [];
        $scope.showFilter = false;
        $scope.showExtension = false
        $scope.filters = {
            selectedOrderStatusFilters: [],
            selectedSellerFilters: [],
            selectedShipperFilters: [],
            selectedEcommerceShipperFilters: [],
            selectedPaymentFilters: [],
            selectedUltilFilters: []
        }
        $scope.filters.sortBy = '';
        $scope.sorttongtien = false;
        $scope.CreatedDate = false;
        $scope.picker = {};
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            showWeeks: false
        };
        $scope.isAll = false;
        $scope.selectedOrders = [];
        $scope.selectedOrdersModel = {};
        $scope.orderReturn = {};
        $scope.totalMoneyPrint = 0;
        $scope.ordersReturn = []
        $scope.channelName = '';
        $scope.waiting = false;
        $scope.allCheckVal = false;
        $scope.tikiCompletedOrderIds = '';
        $scope.inValidOrders = '';
        $scope.cancelOrders = '';
        $scope.ecommerceShippers = [
            { Name: 'Giao Hàng Tiết Kiệm' },
            { Name: 'Giao Hàng Nhanh' },
            { Name: 'VNPost Tiết Kiệm' },
            { Name: 'VNPost Nhanh' },
            { Name: 'Shopee Express' },
            { Name: 'Ninja Van' },
            { Name: 'NowShip' },
            { Name: 'BEST Express' },
        ];
        $scope.paymentFilters = [
            {
                Id: 1,
                Name: 'Tiền mặt'
            },
            {
                Id: 7,
                Name: 'Quẹt thẻ'
            },
            {
                Id: 6,
                Name: 'QRCode'
            },
            //{
            //    Id: 3,
            //    Name: 'Thu hộ'
            //},
            {
                Id: 2,
                Name: 'Chuyển khoản'
            },
            {
                Id: 4,
                Name: 'Thanh toán trực tuyến'
            },
            {
                Id: 5,
                Name: 'Bằng thẻ ngân hàng khi nhận hàng'
            },


        ];
        $scope.GHTKOrders = [];
        $scope.GHNOrders = [];
        $scope.VNPTKOrders = [];
        $scope.VNPNOrders = [];
        $scope.SPEOrders = [];
        $scope.NinjaVanOrders = [];
        $scope.NowShipOrders = [];
        $scope.BESTExpressOrders = [];
        //$scope.ultilFilters = [
        //    { id: 'NoTrackingNumber', name: 'Không có mã vận đơn' },
        //    { id: 'NoPrinting', name: 'Chưa in' }
        //]
        var ultilFilters = [
            { id: 'NoTrackingNumber', name: 'Không có mã vận đơn' },
            { id: 'NoPrinting', name: 'Chưa in' },
            { id: 'HaveTrackingNumber', name: 'Có mã vận đơn' }
        ]
        $scope.ultilFilterOptions = {
            dataSource: ultilFilters,
            placeholder: "Chọn",
            dataTextField: "name",
            dataValueField: "id",
            valuePrimitive: true,
            filter: "contains",
            change: function (e) {
                updateFilter();
            }
        };
        $scope.ecommerceShipperOptions = {
            dataSource: $scope.ecommerceShippers,
            placeholder: "Chọn",
            dataTextField: "Name",
            dataValueField: "Name",
            valuePrimitive: true,
            filter: "contains",
            change: function (e) {
                updateFilter();
            }
        };
        $scope.paymentOptions = {
            dataSource: $scope.paymentFilters,
            placeholder: "Chọn",
            dataTextField: "Name",
            dataValueField: "Id",
            valuePrimitive: true,
            filter: "contains",
            change: function (e) {
                updateFilter();
            }
        };
        $scope.setupToogleDataVal = false;
        $scope.showStatusFilter = true;
        $scope.showEcommerceShipperFilter = true;
        $scope.showSellerFilter = false;
        $scope.showShipperFilter = false;
        $scope.showTimeFilter = true;
        $scope.showPaymentFilter = true;
        $scope.showUltilFilter = true;

        $scope.search = search;
        $scope.refeshPage = refeshPage;
        $scope.orderDetail = orderDetail;
        $scope.loadChannels = loadChannels;
        $scope.init = init;
        $scope.loadFilter = loadFilter;
        $scope.loadOrderStatuses = loadOrderStatuses;
        $scope.loadUsers = loadUsers;
        $scope.updateFilter = updateFilter;
        $scope.sortBy = sortBy;
        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.updateDateFilter = updateDateFilter;
        $scope.selectAll = selectAll;
        $scope.selectOrder = selectOrder
        $scope.updateStatusOrders = updateStatusOrders;
        $scope.updateShipper = updateShipper;
        $scope.editOrder = editOrder;
        $scope.resetTimeFilter = resetTimeFilter
        //$scope.setMinDate = setMinDate;
        $scope.getDetailOrderToPrint = getDetailOrderToPrint;
        $scope.getDetailOrdersToPrint = getDetailOrdersToPrint;
        $scope.resetSelectedOrders = resetSelectedOrders;
        $scope.authenExport = authenExport;
        $scope.orderConfirmShopee = orderConfirmShopee;
        $scope.IsNullOrEmpty = IsNullOrEmpty;
        $scope.syncTrackingNoShopee = syncTrackingNoShopee;
        $scope.confirmShopeeOrders = confirmShopeeOrders;
        $scope.ShopeePrint = ShopeePrint;
        $scope.loadExtension = loadExtension;
        $scope.updateCompletedTikiOrders = updateCompletedTikiOrders;
        $scope.clearCompletedTikiOrders = clearCompletedTikiOrders;
        $scope.printShopeeAll = printShopeeAll;
        $scope.getLackShopeeOrdersWithDay = getLackShopeeOrdersWithDay;
        $scope.updateStatusShopeeOrdersWithDay = updateStatusShopeeOrdersWithDay;
        $scope.testAPI = testAPI;
        $scope.updateStockOfProductsNoSkuInOrders = updateStockOfProductsNoSkuInOrders
        $scope.setupToogleData = setupToogleData;
        $scope.updateStatusShopeeIncompleteOrders = updateStatusShopeeIncompleteOrders;
        //$scope.selectedOrder = {
        //    TrackingNo: ''
        //};

        function search(page) {
            if ($scope.selectedChannel) {
                page = page || 0;
                $scope.waiting = true;

                $scope.filters.branchId = $scope.branchSelectedRoot.Id;
                $scope.filters.page = page;
                $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
                $scope.filters.filter = $scope.filterOrders;
                if ($scope.selectedChannel != null)
                    $scope.filters.channelId = $scope.selectedChannel.Id;
                else
                    $scope.filters.channelId = null;
                $scope.filters.startDateFilter = $scope.startDateFilter;
                $scope.filters.endDateFilter = $scope.endDateFilter;
                apiService.post('/api/order/search', $scope.filters, function (result) {
                    $scope.orders = result.data.Items;
                    $scope.page = result.data.Page;
                    $scope.pagesCount = result.data.TotalPages;
                    $scope.totalCount = result.data.TotalCount;
                    $scope.totalMoney = result.data.TotalMoney;
                    $scope.waiting = false;
                    if ($scope.filterOrders && $scope.filterOrders.length && $scope.page == 0) {
                        //notificationService.displaySuccess($scope.totalCount + ' đơn tìm được');
                    }
                    if ($scope.selectedOrders.length > 0) {
                        $.each($scope.selectedOrders, function (iSelected, vSelected) {
                            var selectedOrder = vSelected;
                            $.each($scope.orders, function (iOrder, vOrder) {
                                if (selectedOrder.id == vOrder.id)
                                    vOrder.checked = true;
                            });
                        });
                    }
                }, function (response) {
                    notificationService.displayError(response.data);
                    $scope.waiting = false;
                });
            }
        }
        function refeshPage() {
            $scope.filterOrders = '';
            $scope.filters.selectedOrderStatusFilters = [];
            $scope.filters.selectedSellerFilters = [];
            $scope.filters.selectedShipperFilters = [];
            $scope.filters.selectedEcommerceShipperFilters = [];
            $scope.filters.selectedPaymentFilters = [];
            $scope.filters.selectedUltilFilters = [];
            $scope.filters.sortBy = '';
            $scope.filters.startDateFilter = null;
            $scope.filters.endDateFilter = null;
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            $scope.sorttongtien = false;
            $scope.CreatedDate = false;
            $scope.selectedOrders = [];
            //$scope.animate = false;
            search();
        }
        function orderDetail(selectedOrder) {
            $scope.selectedOrder = selectedOrder;
            $uibModal.open({
                templateUrl: '/app/components/orders/orderDetailModal.html' + BuildVersion,
                controller: 'orderDetailController',
                scope: $scope,
                windowClass: 'app-modal-window'
            }).result.finally(function () {

            });
        }
        function loadChannels() {
            $scope.loading = true;
            apiService.get('/api/channel/getall', null, function (result) {
                //if ($scope.branchSelectedRoot.Id == 1)
                //    result.data.splice(1, result.data.length - 1);
                //else if ($scope.branchSelectedRoot.Id == 2)
                //    result.data.splice(0, 1);
                $scope.channels = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function init() {
            if (localStorage.getItem("selectedChannel") && localStorage.getItem("selectedChannel") != "undefined") {
                $scope.selectedChannel = JSON.parse(localStorage.getItem("selectedChannel"));
            }
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
            if (localStorage.getItem("userName")) {
                $scope.userName = JSON.parse(localStorage.getItem("userName"));
            }
        }
        function loadFilter() {
            $scope.showFilter = !$scope.showFilter;
            $scope.showExtension = false;
        }
        function loadOrderStatuses() {
            $scope.loading = true;
            apiService.get('/api/orderstatus/getall', null, function (result) {
                $scope.orderStatusFilters = result.data;
                var orderStatuses = result.data
                $scope.orderStatusOptions = {
                    dataSource: orderStatuses,
                    placeholder: "Chọn",
                    dataTextField: "Name",
                    dataValueField: "Id",
                    valuePrimitive: true,
                    filter: "contains",
                    change: function (e) {
                        updateFilter();
                    }
                };

                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadUsers() {
            $scope.loading = true;
            apiService.get('api/applicationuser/getall', null, function (result) {
                $scope.userFilters = result.data;
                var sellers = result.data
                $scope.sellerOptions = {
                    dataSource: sellers,
                    placeholder: "Chọn",
                    dataTextField: "UserName",
                    dataValueField: "Id",
                    valuePrimitive: true,
                    filter: "contains",
                    change: function (e) {
                        updateFilter();
                    }
                };

                var shippers = result.data
                $scope.shipperOptions = {
                    dataSource: shippers,
                    placeholder: "Chọn",
                    dataTextField: "UserName",
                    dataValueField: "Id",
                    valuePrimitive: true,
                    filter: "contains",
                    change: function (e) {
                        updateFilter();
                    }
                };
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function updateFilter() {
            $scope.page = $scope.page || 0;
            search($scope.page);
        }
        function sortBy(value) {
            switch (value) {
                case 'tongtien':
                    $scope.sorttongtien = !$scope.sorttongtien;
                    $scope.filters.sortBy = $scope.sorttongtien == true ? 'tongtien_des' : 'tongtien_asc';
                    search();
                    break;
                case 'CreatedDate':
                    $scope.CreatedDate = !$scope.CreatedDate;
                    $scope.filters.sortBy = $scope.CreatedDate == true ? 'CreatedDate_des' : 'CreatedDate_asc';
                    search();
                    break;
            }
        }
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
        function updateDateFilter() {
            if ($scope.startDateFilter && $scope.endDateFilter) {
                if ($scope.startDateFilter <= $scope.endDateFilter) {
                    $scope.page = $scope.page || 0;
                    search($scope.page);
                }
                else {
                    notificationService.displayError("Ngày lọc không đúng")
                }
            }
        }
        function selectAll() {
            if ($scope.isAll === false) {
                $scope.selectedOrders = [];
                angular.forEach($scope.orders, function (item) {
                    item.checked = true;
                    $scope.selectedOrders.push(item);
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.orders, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
                $scope.selectedOrders = [];
            }

        }
        function selectOrder(val) {
            if (val.checked) {
                $scope.selectedOrders.push(val);
            }
            else {
                var ind = null;
                $.each($scope.selectedOrders, function (index, value) {
                    if (value.id == val.id) {
                        ind = index
                        return false;
                    }
                });
                $scope.selectedOrders.splice(ind, 1);
            }
        }
        function updateStatusOrders(orderStatus) {
            $scope.waiting = true;
            $scope.validated = true;
            if ($scope.selectedOrders.length > 0) {
                $.each($scope.selectedOrders, function (ind, val) {
                    if (val.StatusId == 3 || val.StatusId == 4 || val.StatusId == 6) {
                        notificationService.displayError('Lỗi! Tình trạng đơn ' + val.id + ' đã được cập nhật !');
                        $scope.validated = false;
                    }
                });
                if ($scope.validated == true) {
                    var urlApi = 'api/order/';
                    switch (orderStatus.Id) {
                        case 1:
                            urlApi += 'updateprocessorders';
                            break;
                        case 2:
                            urlApi += 'updateshippedorders';
                            break;
                        case 3:
                            urlApi += 'updatedoneorders';
                            break;
                        case 4:
                            urlApi += 'updatecancelorders';
                            break;
                        case 5:
                            urlApi += 'updaterefundorders';
                            break;
                        case 6:
                            urlApi += 'updateshipcancelorders';
                            break;
                    }
                    $scope.selectedOrdersModel.UserId = $scope.userId;
                    $scope.selectedOrdersModel.Orders = $scope.selectedOrders;
                    apiService.post(urlApi, $scope.selectedOrdersModel,
                        function (result) {
                            if (result.data == true) {
                                notificationService.displaySuccess('Cập nhật tình trạng thành công!');
                                resetSelectedOrders();
                                $scope.waiting = false;
                                search($scope.page);
                            }
                        }, function (error) {
                            $scope.waiting = false;
                            notificationService.displayError(error.data);
                        });
                }

            }
        }
        function updateShipper(shipper) {
            $scope.validated = true;
            if ($scope.selectedOrders.length > 0) {
                $.each($scope.selectedOrders, function (ind, val) {
                    if (val.StatusId == 3 || val.StatusId == 4 || val.StatusId == 6) {
                        notificationService.displayError('Lỗi! Đơn ' + val.id + ' đã được cập nhật !');
                        $scope.validated = false;
                    }
                });
                if ($scope.validated == true) {
                    $scope.selectedOrdersModel.UserId = $scope.userId;
                    $scope.selectedOrdersModel.Orders = $scope.selectedOrders;
                    $scope.selectedOrdersModel.ShipperId = shipper.Id;
                    apiService.post('api/order/updateshipperorders', $scope.selectedOrdersModel,
                        function (result) {
                            if (result.data == true) {
                                notificationService.displaySuccess('Cập nhật NV giao hàng thành công!');
                                resetSelectedOrders();
                                search($scope.page);
                            }
                        }, function (error) {
                            notificationService.displayError(error.data);
                        });
                }

            }
        }
        function editOrder(order) {
            $ngBootbox.confirm('Bạn có chắc chắn muốn sửa đơn? (đơn sẽ được huỷ và tạo mới!)').then(function () {
                $scope.orderUpdate = {
                    id: order.id,
                    ghichu: order.ghichu,
                    ShipperId: null,
                    UserId: $scope.userId
                }
                apiService.post('/api/order/updatecancelforedit', $scope.orderUpdate, function (result) {
                    if (result.data) {
                        $scope.orderUpdate = {};
                        $rootScope.channel = result.data.channel;
                        $rootScope.customer = result.data.customer;
                        $rootScope.detailOrders = result.data.orderDetails;
                        $rootScope.historyOrder = result.data.historyOrder;
                        //$rootScope.shipperId = result.data.shipperId;
                        $state.go('add_order');
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                });
            });
        }
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            search();
        }
        function getDetailOrderToPrint(order) {
            $scope.orderReturn = {};
            var config = {
                params: {
                    orderId: order.id,
                    username: $scope.userName
                }
            }
            apiService.get('/api/order/detailtoprint', config, function (result) {
                $scope.orderReturn = result.data;
                var sum = 0;
                $.each($scope.orderReturn.donhang_ct, function (index, value) {
                    sum += value.Soluong * value.Dongia;
                });
                $scope.totalMoneyPrint = sum;
                setTimeout(function () {
                    var innerContents = document.getElementById("printDiv").innerHTML;
                    var popupWinindow = window.open();
                    popupWinindow.window.focus();
                    popupWinindow.document.open();
                    popupWinindow.document.write('<!DOCTYPE html><html><head>'
                        + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
                        + '</head><body onload="window.print(); window.close();"><div>'
                        + innerContents + '</div></html>');
                    popupWinindow.document.close();
                }, 500);
                search($scope.page);
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function getDetailOrdersToPrint() {
            $scope.ordersReturn = [];
            var orderIds = [];
            $.each($scope.selectedOrders, function (index, value) {
                orderIds.push(value.id);
            });
            var config = {
                orderIds: orderIds,
                username: $scope.userName
            }
            apiService.post('/api/order/detailorderstoprint', config, function (result) {
                $scope.ordersReturn = result.data;
                var sum = 0;
                $.each($scope.ordersReturn, function (iOrders, vOrders) {
                    var indexOrder = iOrders;
                    $.each($scope.ordersReturn[indexOrder].donhang_ct, function (index, value) {
                        sum += value.Soluong * value.Dongia;
                    });
                    vOrders.totalMoneyPrint = sum;
                    sum = 0;
                });
                setTimeout(function () {
                    var innerContents = document.getElementById("printDivAll").innerHTML;
                    var popupWinindow = window.open();
                    popupWinindow.window.focus();
                    popupWinindow.document.open();
                    popupWinindow.document.write('<!DOCTYPE html><html><head>'
                        + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
                        + '</head><body onload="window.print(); window.close();"><div>'
                        + innerContents + '</div></html>');
                    popupWinindow.document.close();
                }, 500);
                search($scope.page);
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function resetSelectedOrders() {
            angular.forEach($scope.orders, function (item) {
                item.checked = false;
            });
            $scope.isAll = false;
            $scope.selectedOrders = [];
            $scope.allCheckVal = false;
        }
        function authenExport() {
            apiService.get('api/order/authenexport', null, function (result) {
                exportOrdersExcel();
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function exportOrdersExcel() {
            if (($scope.filters.selectedOrderStatusFilters.length == 0)
                && ($scope.filters.selectedSellerFilters.length == 0)
                && ($scope.filters.selectedShipperFilters.length == 0)
                && ($scope.filters.selectedEcommerceShipperFilters.length == 0)
                && ($scope.filters.selectedPaymentFilters.length == 0)
                && ($scope.filters.selectedUltilFilters.length == 0)
                && $scope.filters.startDateFilter == null
                && $scope.filters.endDateFilter == null) {
                if (confirm("Bạn có muốn xuất tất cả đơn hàng !")) {
                    $uibModal.open({
                        templateUrl: '/app/components/orders/orderExportProcessModal.html' + BuildVersion,
                        controller: 'orderExportProcessController',
                        scope: $scope,
                        backdrop: 'static',
                        keyboard: false,
                        size: 'sm'
                    }).result.finally(function ($scope) {

                    });
                }
            }
            else
                $uibModal.open({
                    templateUrl: '/app/components/orders/orderExportProcessModal.html' + BuildVersion,
                    controller: 'orderExportProcessController',
                    scope: $scope,
                    backdrop: 'static',
                    keyboard: false,
                    size: 'sm'
                }).result.finally(function ($scope) {

                });
        }
        function orderConfirmShopee(item) {
            $scope.waiting = true;
            apiService.post('api/order/shopeeconfirm', item,
                function (result) {
                    if (result.data == true) {
                        notificationService.displaySuccess('Xác nhận thành công!');
                        search($scope.page);
                        $scope.waiting = false;
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                    $scope.waiting = false;
                });
        }
        function IsNullOrEmpty(item) {
            return isNullOrEmpty(item);
        }
        function syncTrackingNoShopee() {
            $scope.waiting = true;
            apiService.post('api/order/shopeesynctrackingno', null,
                function (result) {
                    if (result.data == true) {
                        notificationService.displaySuccess('Đồng bộ thành công!');
                        search($scope.page);
                        $scope.waiting = false;
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                    $scope.waiting = false;
                });
        }
        //function confirmShopeeOrders() {
        //    $scope.waiting = true;
        //    var promissList = [];
        //    $.each($scope.selectedOrders, function (index, value) {
        //        authenticationService.setHeader();
        //        var confirmPromiss = $http.post('api/order/shopeeconfirm', value).then(function (result) {
        //            //search($scope.page);
        //        }, function (error) {

        //        });
        //        promissList.push(confirmPromiss);
        //    });
        //    $q.all(promissList).then(function (args) {
        //        //notificationService.displaySuccess('Xác nhận thành công!');
        //        //search($scope.page);
        //        //$scope.waiting = false;

        //        setTimeout(function () {
        //            apiService.get('/api/shopee/getlackorders', null, function (result) {
        //                notificationService.displaySuccess('Xác nhận thành công!');
        //                search($scope.page);
        //                $scope.waiting = false;
        //            }, function (error) {
        //                notificationService.displayError(error);
        //            });
        //        }, 2000);
        //    });
        //}
        function confirmShopeeOrders() {
            $scope.waiting = true;
            var selectedOrdersId = [];
            $.each($scope.selectedOrders, function (index, value) {
                selectedOrdersId.push(value.OrderIdShopeeApi);
            });
            apiService.post('api/order/shopeeconfirmall', selectedOrdersId,
                function (result) {
                    if (result.data == true) {
                        notificationService.displaySuccess('Xác nhận thành công!');
                        search($scope.page);
                        $scope.waiting = false;
                    }
                }, function (error) {
                    notificationService.displayError(error.data);
                    $scope.waiting = false;
                });
        }
        function ShopeePrint(item) {
            $scope.selectedOrder = item;
            $scope.selectedOrder.AddressBuyer = '';
            $scope.selectedOrder.PhoneBuyer = '';
            $scope.selectedOrder.RecipientSortCode = '';
            $scope.selectedOrder.OrderDetails = [];
            $scope.selectedOrder.TotalQuantity = 0;
            $scope.selectedOrder.TotalAmount = 0;
            $scope.selectedOrder.MaxWeight = 0

            $scope.waiting = true;
            var config = {
                params: {
                    orderId: item.OrderIdShopeeApi,
                    username: $scope.userName
                }
            }
            apiService.get('/api/shopee/getorderdetails', config, function (result) {
                $scope.selectedOrder.BuyerName = result.data.BuyerName;
                $scope.selectedOrder.AddressBuyer = result.data.AddressBuyer;
                $scope.selectedOrder.PhoneBuyer = result.data.PhoneBuyer;
                $scope.selectedOrder.RecipientSortCode = result.data.RecipientSortCode;
                $scope.selectedOrder.OrderDetails = result.data.OrderDetails;
                $scope.selectedOrder.CreateTime = result.data.CreateTime;
                $scope.selectedOrder.SenderSortCode = result.data.SenderSortCode;
                var totalQuantity = 0;
                if ($scope.selectedOrder.OrderDetails.length > 0)
                    totalQuantity = result.data.OrderDetails.reduce((quantity, currentDetail) => {
                        return quantity + currentDetail.variation_quantity_purchased;
                    }, 0);
                $scope.selectedOrder.TotalQuantity = totalQuantity;
                $scope.selectedOrder.TotalAmount = result.data.TotalAmount;
                //var maxWeight = 0;
                //if ($scope.selectedOrder.OrderDetails.length > 0)
                //    maxWeight = result.data.OrderDetails.reduce((weight, currentDetail) => {
                //        return weight + currentDetail.weight * currentDetail.variation_quantity_purchased;
                //    }, 0);
                $scope.selectedOrder.MaxWeight = result.data.MaxWeight;
                $scope.waiting = false;
                var trackingNo = angular.copy($scope.selectedOrder.TrackingNo);
                var recipientSortCode = angular.copy($scope.selectedOrder.RecipientSortCode);
                if (IsNullOrEmpty(recipientSortCode))
                    recipientSortCode = 0;
                setTimeout(function () {
                    var idElement = 'printDivGHN';
                    JsBarcode("#TrackingNoBarcode", trackingNo, {
                        height: 40,
                        width: 1.6,
                        displayValue: false,
                        marginLeft: 0,
                        marginRight: 0,
                        marginTop: 0,
                        marginBottom: 0,
                    });
                    switch ($scope.selectedOrder.ShipperNameShopeeApi.toLowerCase()) {
                        case 'giao hàng tiết kiệm':
                            idElement = 'printDivGHTK';
                            break;
                        case 'vnpost nhanh':
                            JsBarcode("#RecipientSortCode", recipientSortCode, {
                                height: 15,
                                width: 1.6,
                                displayValue: false,
                                marginLeft: 0,
                                marginRight: 0,
                                marginTop: 0,
                                marginBottom: 0,
                            });
                            idElement = 'printDivVNP';
                            break;
                        case 'vnpost tiết kiệm':
                            JsBarcode("#RecipientSortCode", recipientSortCode, {
                                height: 15,
                                width: 1.6,
                                displayValue: false,
                                marginLeft: 0,
                                marginRight: 0,
                                marginTop: 0,
                                marginBottom: 0,
                            });
                            idElement = 'printDivVNP';
                            break;
                        case 'shopee express':
                            var typeNumber = 4;
                            var errorCorrectionLevel = 'L';
                            var qr = qrcode(typeNumber, errorCorrectionLevel);
                            qr.addData($scope.selectedOrder.TrackingNo);
                            qr.make();
                            document.getElementById('placeHolder').innerHTML = qr.createImgTag();
                            idElement = 'printDivSPE';
                            break;
                        case 'nowship':
                            idElement = 'printDivNowShip';
                            break;
                        case 'ninja van':
                            idElement = 'printDivNinja';
                            break;
                        case 'best express':
                            idElement = 'printDivBESTExpress';
                            break;
                    }
                    var innerContents = document.getElementById(idElement).innerHTML;
                    var popupWinindow = window.open();
                    popupWinindow.window.focus();
                    popupWinindow.document.open();
                    popupWinindow.document.write('<!DOCTYPE html><html><head>'
                        + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
                        + '</head><body onload="window.print(); window.close();"><div>'
                        + innerContents + '</div></html>');
                    popupWinindow.document.close();
                }, 500);
                search($scope.page);
            }, function (error) {
                notificationService.displayError(error);
            });

        }
        function loadExtension() {
            $scope.showExtension = !$scope.showExtension;
            $scope.showFilter = false;
        }
        function updateCompletedTikiOrders() {
            $scope.totalOrder = 0;
            $scope.inValidOrdersCount = 0;
            $scope.cancelOrdersCount = 0;
            $scope.validOrderCount = 0;
            $scope.inValidOrders = '';
            $scope.cancelOrders = '';
            var test = $scope.tikiCompletedOrderIds.trim();
            var orderIds = $scope.tikiCompletedOrderIds.split("\n");
            $scope.totalOrder = orderIds.length;
            $scope.waiting = true;
            var item = {
                orderIds: orderIds,
                userId: $scope.userId
            }
            apiService.post('api/order/updatecompletedtikiorders', item,
                function (result) {
                    notificationService.displaySuccess('Cập nhật thành công!');
                    search($scope.page);
                    if (!isNullOrEmpty(result.data.inValidOrders)) {
                        $scope.inValidOrders = result.data.inValidOrders.trim();
                        var inValidOrdersSplit = $scope.inValidOrders.split("\n");
                        $scope.inValidOrdersCount = inValidOrdersSplit.length;
                    }
                    if (!isNullOrEmpty(result.data.cancelOrders)) {
                        $scope.cancelOrders = result.data.cancelOrders.trim();
                        var cancelOrdersSplit = $scope.cancelOrders.split("\n");
                        $scope.cancelOrdersCount = cancelOrdersSplit.length;
                    }
                    $scope.validOrderCount = $scope.totalOrder - $scope.inValidOrdersCount - $scope.cancelOrdersCount;
                    $scope.waiting = false;
                }, function (error) {
                    notificationService.displayError(error.data);
                    $scope.waiting = false;
                });
        }
        function clearCompletedTikiOrders() {
            $scope.inValidOrders = '';
            $scope.cancelOrders = '';
            $scope.tikiCompletedOrderIds = '';
        }
        //function printShopeeAll() {
        //    $scope.waiting = true;
        //    $scope.selectedOrdersModel.UserId = $scope.userId;
        //    $scope.selectedOrdersModel.Orders = $scope.selectedOrders;
        //    $scope.selectedOrdersModel.UserName = $scope.userName;
        //    $scope.GHTKOrders = [];
        //    $scope.GHNOrders = [];
        //    $scope.VNPOrders = [];
        //    $scope.SPEOrders = [];
        //    $scope.NinjaVanOrders = [];
        //    $scope.NowShipOrders = [];

        //    apiService.post('api/shopee/getordersdetailstoprint', $scope.selectedOrdersModel,
        //        function (result) {
        //            $.each(result.data, function (index, value) {
        //                switch (value.ShipperName.toLowerCase()) {
        //                    case "giao hàng tiết kiệm":
        //                        {
        //                            $scope.GHTKOrders.push(value);
        //                            break;
        //                        }
        //                    case "giao hàng nhanh":
        //                        {
        //                            $scope.GHNOrders.push(value);
        //                            break;
        //                        }
        //                    case "vnpost tiết kiệm":
        //                        {
        //                            $scope.VNPOrders.push(value);
        //                            break;
        //                        }
        //                    case "vnpost nhanh":
        //                        {
        //                            $scope.VNPOrders.push(value);
        //                            break;
        //                        }
        //                    case "shopee express":
        //                        {
        //                            $scope.SPEOrders.push(value);
        //                            break;
        //                        }
        //                    case "ninja van":
        //                        {
        //                            $scope.NinjaVanOrders.push(value);
        //                            break;
        //                        }
        //                    case "nowship":
        //                        {
        //                            $scope.NowShipOrders.push(value);
        //                            break;
        //                        }
        //                }
        //            });
        //            $scope.waiting = false;

        //            setTimeout(function () {
        //                var idElement = 'printDivShopeeAll';
        //                if ($scope.GHTKOrders.length > 0)
        //                    $.each($scope.GHTKOrders, function (index, value) {
        //                        printBarcode("#TrackingNoBarcodeGHTK" + index, value.TrackingNo)
        //                    });
        //                if ($scope.GHNOrders.length > 0)
        //                    $.each($scope.GHNOrders, function (index, value) {
        //                        printBarcode("#TrackingNoBarcodeGHN" + index, value.TrackingNo)
        //                    });
        //                if ($scope.VNPOrders.length > 0)
        //                    $.each($scope.VNPOrders, function (index, value) {
        //                        printBarcode("#TrackingNoBarcodeVNP" + index, value.TrackingNo)
        //                        JsBarcode("#RecipientSortCodeVNP" + index, value.RecipientSortCode, {
        //                            height: 15,
        //                            width: 1.6,
        //                            displayValue: false,
        //                            marginLeft: 0,
        //                            marginRight: 0,
        //                            marginTop: 0,
        //                            marginBottom: 0,
        //                        });
        //                    });
        //                if ($scope.SPEOrders.length > 0)
        //                    $.each($scope.SPEOrders, function (index, value) {
        //                        printBarcode("#TrackingNoBarcodeSPE" + index, value.TrackingNo)

        //                        var typeNumber = 4;
        //                        var errorCorrectionLevel = 'L';
        //                        var qr = qrcode(typeNumber, errorCorrectionLevel);
        //                        qr.addData(value.TrackingNo);
        //                        qr.make();
        //                        document.getElementById('qrCodeSPE' + index).innerHTML = qr.createImgTag();
        //                    });
        //                if ($scope.NinjaVanOrders.length > 0)
        //                    $.each($scope.NinjaVanOrders, function (index, value) {
        //                        printBarcode("#TrackingNoBarcodeNinjaVan" + index, value.TrackingNo)
        //                    });
        //                if ($scope.NowShipOrders.length > 0)
        //                    $.each($scope.NowShipOrders, function (index, value) {
        //                        printBarcode("#TrackingNoBarcodeNowShip" + index, value.TrackingNo)
        //                    });

        //                var innerContents = document.getElementById(idElement).innerHTML;
        //                var popupWinindow = window.open();
        //                popupWinindow.window.focus();
        //                popupWinindow.document.open();
        //                popupWinindow.document.write('<!DOCTYPE html><html><head>'
        //                    + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
        //                    + '</head><body onload="window.print(); window.close();"><div>'
        //                    + innerContents + '</div></html>');
        //                popupWinindow.document.close();
        //            }, 500);
        //            search($scope.page);
        //        }, function (error) {
        //            notificationService.displayError(error.data);
        //            $scope.waiting = false;
        //        });
        //}
        function printShopeeAll() {
            $scope.waiting = true;
            $scope.selectedOrdersModel.UserId = $scope.userId;

            var selectedOrdersId = [];
            $.each($scope.selectedOrders, function (index, value) {
                selectedOrdersId.push(value.OrderIdShopeeApi);
            });

            $scope.selectedOrdersModel.Orders = selectedOrdersId;
            $scope.selectedOrdersModel.UserName = $scope.userName;
            $scope.GHTKOrders = [];
            $scope.GHNOrders = [];
            $scope.VNPOrders = [];
            $scope.SPEOrders = [];
            $scope.NinjaVanOrders = [];
            $scope.NowShipOrders = [];
            $scope.BESTExpressOrders = [];

            $.ajax({
                url: "api/shopee/getordersdetailstoprint/",
                async: false,
                type: "GET",
                dataType: "json",
                headers: {
                    Accept: 'application/json',
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", authenticationService.getAccessToken());
                },
                data: { inputParams: JSON.stringify($scope.selectedOrdersModel) },
                success: function (result) {
                    $.each(result, function (index, value) {
                        switch (value.ShipperName.toLowerCase()) {
                            case "giao hàng tiết kiệm":
                                {
                                    $scope.GHTKOrders.push(value);
                                    break;
                                }
                            case "giao hàng nhanh":
                                {
                                    $scope.GHNOrders.push(value);
                                    break;
                                }
                            case "vnpost tiết kiệm":
                                {
                                    $scope.VNPOrders.push(value);
                                    break;
                                }
                            case "vnpost nhanh":
                                {
                                    $scope.VNPOrders.push(value);
                                    break;
                                }
                            case "shopee express":
                                {
                                    $scope.SPEOrders.push(value);
                                    break;
                                }
                            case "ninja van":
                                {
                                    $scope.NinjaVanOrders.push(value);
                                    break;
                                }
                            case "nowship":
                                {
                                    $scope.NowShipOrders.push(value);
                                    break;
                                }
                            case "best express":
                                {
                                    $scope.BESTExpressOrders.push(value);
                                    break;
                                }
                        }
                    });
                    $scope.waiting = false;

                    setTimeout(function () {
                        var idElement = 'printDivShopeeAll';
                        if ($scope.GHTKOrders.length > 0)
                            $.each($scope.GHTKOrders, function (index, value) {
                                printBarcode("#TrackingNoBarcodeGHTK" + index, value.TrackingNo)
                            });
                        if ($scope.GHNOrders.length > 0)
                            $.each($scope.GHNOrders, function (index, value) {
                                printBarcode("#TrackingNoBarcodeGHN" + index, value.TrackingNo)
                            });
                        if ($scope.VNPOrders.length > 0)
                            $.each($scope.VNPOrders, function (index, value) {
                                printBarcode("#TrackingNoBarcodeVNP" + index, value.TrackingNo)
                                recipientSortCode = value.RecipientSortCode;
                                if (IsNullOrEmpty(recipientSortCode))
                                    recipientSortCode = 0;
                                JsBarcode("#RecipientSortCodeVNP" + index, recipientSortCode, {
                                    height: 15,
                                    width: 1.6,
                                    displayValue: false,
                                    marginLeft: 0,
                                    marginRight: 0,
                                    marginTop: 0,
                                    marginBottom: 0,
                                });
                            });
                        if ($scope.SPEOrders.length > 0)
                            $.each($scope.SPEOrders, function (index, value) {
                                printBarcode("#TrackingNoBarcodeSPE" + index, value.TrackingNo)

                                var typeNumber = 4;
                                var errorCorrectionLevel = 'L';
                                var qr = qrcode(typeNumber, errorCorrectionLevel);
                                qr.addData(value.TrackingNo);
                                qr.make();
                                document.getElementById('qrCodeSPE' + index).innerHTML = qr.createImgTag();
                            });
                        if ($scope.NinjaVanOrders.length > 0)
                            $.each($scope.NinjaVanOrders, function (index, value) {
                                printBarcode("#TrackingNoBarcodeNinjaVan" + index, value.TrackingNo)
                            });
                        if ($scope.NowShipOrders.length > 0)
                            $.each($scope.NowShipOrders, function (index, value) {
                                printBarcode("#TrackingNoBarcodeNowShip" + index, value.TrackingNo)
                            });
                        if ($scope.BESTExpressOrders.length > 0)
                            $.each($scope.BESTExpressOrders, function (index, value) {
                                printBarcode("#TrackingNoBarcodeBESTExpressOrders" + index, value.TrackingNo)
                            });

                        var innerContents = document.getElementById(idElement).innerHTML;
                        var popupWinindow = window.open();
                        popupWinindow.window.focus();
                        popupWinindow.document.open();
                        popupWinindow.document.write('<!DOCTYPE html><html><head>'
                            + '<link rel="stylesheet" type="text/css" href="/Assets/admin/libs/bootstrap/dist/css/bootstrap.min.css" />'
                            + '</head><body onload="window.print(); window.close();"><div>'
                            + innerContents + '</div></html>');
                        popupWinindow.document.close();
                    }, 500);
                    search($scope.page);
                },
                error: function (error) {
                    notificationService.displayError(error.responseText);
                    $scope.waiting = false;
                }
            });
        }
        function getLackShopeeOrdersWithDay() {
            $scope.waiting = true;
            var config = {
                params: {
                    quantity: 5
                }
            }
            apiService.get('/api/order/shopeegetlackorderswithday', config, function (result) {
                notificationService.displaySuccess('Cập nhật đơn hàng sót ' + config.params.quantity + ' ngày trước thành công!');
                search($scope.page);
                $scope.waiting = false;
            }, function (error) {
                notificationService.displayError(error);
                $scope.waiting = false;
            });
        }
        function updateStatusShopeeOrdersWithDay() {
            $scope.waiting = true;
            var config = {
                params: {
                    quantity: 5
                }
            }
            apiService.get('/api/order/shopeeupdatestatusorderswithday', config, function (result) {
                notificationService.displaySuccess('Cập nhật tình  trạng đơn hàng từ ' + config.params.quantity + ' ngày trước thành công!');
                search($scope.page);
                $scope.waiting = false;
            }, function (error) {
                notificationService.displayError(error);
                $scope.waiting = false;
            });
        }
        function testAPI() {
            $scope.waiting = true;
            apiService.get('/api/shopee/testapi', null, function (result) {
                //notificationService.displaySuccess('Cập nhật tình  trạng đơn hàng từ ' + config.params.quantity + ' ngày trước thành công!');
                //search($scope.page);
                $scope.waiting = false;
            }, function (error) {
                notificationService.displayError(error);
                $scope.waiting = false;
            });
        }
        function updateStockOfProductsNoSkuInOrders() {
            $scope.waiting = true;
            apiService.get('/api/shopee/updatestockofproductsnoskuinorders', null, function (result) {
                $scope.waiting = false;
            }, function (error) {
                notificationService.displayError(error);
                $scope.waiting = false;
            });
        }
        function setupToogleData() {
            $scope.setupToogleDataVal = !$scope.setupToogleDataVal;
        }
        function updateStatusShopeeIncompleteOrders() {
            $scope.waiting = true;
            apiService.get('/api/order/updatestatusshopeeincompleteorders', null, function (result) {
                notificationService.displaySuccess('Cập nhật tình  trạng đơn hàng chưa hoàn tất thành công!');
                //search($scope.page);
                $scope.waiting = false;
            }, function (error) {
                notificationService.displayError(error);
                $scope.waiting = false;
            });
        }


        var printBarcode = function (elementId, value) {
            JsBarcode(elementId, value, {
                height: 40,
                width: 1.6,
                displayValue: false,
                marginLeft: 0,
                marginRight: 0,
                marginTop: 0,
                marginBottom: 0,
            });
        }

        $scope.$watch('selectedChannel', function (n, o) {
            //if (n)
            {
                if (n) {
                    var channelName = n.Name;
                    $scope.channelName = channelName.toUpperCase();
                }
                else
                    $scope.channelName = 'TẤT CẢ';
                localStorage.setItem("selectedChannel", JSON.stringify(n));
                refeshPage();
            }
        });

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadChannels();
            loadUsers();
            loadOrderStatuses();
            init();
            search()
            $window.document.title = "DS đơn hàng";
        }
    }
})(angular.module('softbbm.orders'));