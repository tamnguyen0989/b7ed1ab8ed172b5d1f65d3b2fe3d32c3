(function (app) {
    app.controller('orderListController', orderListController);

    orderListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state', '$filter', '$rootScope']
    function orderListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state, $filter, $rootScope) {
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10';
        $scope.totalMoney = 0;
        $scope.channels = [];
        $scope.animate = false;
        $scope.filters = {
            selectedOrderStatusFilters: [],
            selectedSellerFilters: [],
            selectedShipperFilters: []
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

        function search(page) {
            if ($scope.selectedChannel) {
                page = page || 0;
                $scope.loading = true;

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
                    debugger
                    $scope.orders = result.data.Items;
                    $scope.page = result.data.Page;
                    $scope.pagesCount = result.data.TotalPages;
                    $scope.totalCount = result.data.TotalCount;
                    $scope.totalMoney = result.data.TotalMoney;
                    $scope.loading = false;
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
                });
            }
        }
        function refeshPage() {
            $scope.filterOrders = '';
            $scope.filters.selectedOrderStatusFilters = [];
            $scope.filters.selectedSellerFilters = [];
            $scope.filters.selectedShipperFilters = [];
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
                templateUrl: '/app/components/orders/orderDetailModal.html',
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
            $scope.animate = !$scope.animate;
        }
        function loadOrderStatuses() {
            $scope.loading = true;
            apiService.get('/api/orderstatus/getall', null, function (result) {
                $scope.orderStatusFilters = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadUsers() {
            $scope.loading = true;
            apiService.get('api/applicationuser/getall', null, function (result) {
                $scope.userFilters = result.data;
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
                var index = $scope.selectedOrders.indexOf(val)
                $scope.selectedOrders.splice(index, 1);
            }
        }
        function updateStatusOrders(orderStatus) {
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
                        //$scope.selectedOrders = [];
                        search($scope.page);
                    }
                }, function (error) {
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
                        //$scope.selectedOrders = [];
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
                search();
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
                search();
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