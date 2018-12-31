(function (app) {
    app.controller('stampAddController', stampAddController);

    stampAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$rootScope'];

    function stampAddController(apiService, $window, $scope, notificationService, $state, $rootScope) {
        $scope.stockin = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.stringSearch = '';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '5'
        $scope.searchedProducts = [];
        $scope.suppliers = [];
        $scope.productStatus = [];
        $scope.filters = [];
        $scope.bookDetails = [];
        $scope.loading = true;
        $scope.channels = [];
        $scope.selectedChannel = null;
        $scope.stockoutResult = {};
        $scope.arrayPrint = [];

        $scope.loadSuppliers = loadSuppliers;
        $scope.loadProductStatus = loadProductStatus;
        $scope.search = search;
        $scope.addBookDetails = addBookDetails;
        $scope.addFilter = addFilter;
        $scope.removeFilter = removeFilter;
        $scope.removeBookDetail = removeBookDetail
        $scope.init = init;
        $scope.loadChannels = loadChannels;
        $scope.updateSelectedChannel = updateSelectedChannel;
        $scope.printStamp = printStamp
        $scope.sumQuantity = sumQuantity
        $scope.initStamp = initStamp;
        $scope.clearStamp = clearStamp;
        function loadSuppliers() {
            $scope.loading = true;
            var config = {
                params: {
                    order: "Name"
                }

            }
            apiService.get('/api/supplier/getall', config, function (result) {
                $scope.suppliers = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadProductStatus() {
            $scope.loading = true;
            apiService.get('/api/product/getallproductstatus', null, function (result) {
                $scope.productStatus = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function search(page) {
            page = page || 0;

            $scope.loading = true;
            var config = {
                branchId: $scope.branchSelectedRoot.Id,
                stringSearch: $scope.stringSearch,
                page: page,
                pageSize: parseInt($scope.pageSizeNumber),
                FilterBookDetail: $scope.filters
            };
            apiService.post('/api/product/getallpaging/', config, function (result) {
                $scope.searchedProducts = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterStockIns && $scope.filterStockIns.length && $scope.page == 0) {
                    //notificationService.displayInfo($scope.totalCount + ' đơn tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function addBookDetails(val) {
            var exist = 0;
            if ($scope.bookDetails.length > 0) {
                $.each($scope.bookDetails, function (index, value) {
                    if (value.id == val.id) {
                        exist = 1;
                        value.Quantity += 1;
                        return false;
                    }
                });
                if (exist == 0) {
                    val.Quantity = 0;
                    $scope.bookDetails.push(val);
                    updateSelectedChannel($scope.selectedChannel);
                }
            }
            else {
                val.Quantity = 0;
                $scope.bookDetails.push(val);
                updateSelectedChannel($scope.selectedChannel);
            }
            sessionStorage.setItem("arrayPrint", JSON.stringify($scope.bookDetails));
        }
        function addFilter(val, key) {
            var newFilter = {
                key: key,
                value: val.Id,
                name: val.Name
            }
            switch (key) {
                case 0:
                    newFilter.alias = 'Nhà cung cấp';
                    break;
                case 1:
                    newFilter.alias = 'Trạng thái';
                    break;
            }
            if ($scope.filters.length > 0) {
                var exist = 0;
                $.each($scope.filters, function (index, data) {
                    if (newFilter.alias == data.alias && newFilter.key == data.key && newFilter.value == data.value) {
                        exist = 1;
                        return false;
                    }
                });
                if (exist == 0)
                    $scope.filters.push(newFilter);
            }
            else
                $scope.filters.push(newFilter);
            search();
        }
        function removeFilter(val) {
            $scope.filters.splice(val, 1);
            search();
        }
        function removeBookDetail(index) {
            $scope.bookDetails.splice(index, 1);
            sessionStorage.setItem("arrayPrint", JSON.stringify($scope.bookDetails));
            initStamp();
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.stockin.CreatedBy = JSON.parse(localStorage.getItem("userId"));
            }
            if (sessionStorage.getItem("arrayPrint")) {
                var arrayPrint = JSON.parse(sessionStorage.getItem("arrayPrint"));
                if (arrayPrint.length > 0) {
                    $.each(arrayPrint, function (index, value) {
                        $scope.bookDetails.push(value);
                    });
                }
            }
        }
        function loadChannels() {
            $scope.loading = true;
            apiService.get('/api/channel/getall', null, function (result) {

                $scope.channels = result.data;
                //$scope.selectedChannel = $scope.channels[0];
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function updateSelectedChannel(val) {
            if (val) {
                var channelId = val.Id;
                $.each($scope.bookDetails, function (index, value) {
                    var priceTmp = 0;
                    $.each(value.SoftChannelProductPrices, function (indexStock, valStock) {
                        if (valStock.ChannelId == channelId) {
                            priceTmp = valStock.Price;
                            return false;
                        }
                    });
                    value.PriceChannel = priceTmp;
                });
            }
            else {
                $.each($scope.bookDetails, function (index, value) {
                    value.PriceChannel = null;
                });
            }
            initStamp();
        }
        function printStamp() {
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
        }
        function sumQuantity() {
            var total = 0;
            $.each($scope.bookDetails, function (index, item) {
                total += item.Quantity;
            });
            return total;
        }
        function authen() {
            apiService.get('api/product/authenstampprint', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function initStamp() {
            $scope.arrayPrint = [];
            $.each($scope.bookDetails, function (indexPro, valPro) {
                var i;
                for (i = 0; i < valPro.Quantity; i++) {
                    var pro = {};
                    pro.masp = valPro.masp.trim();
                    pro.tensp = valPro.tensp;
                    pro.PriceChannel = valPro.PriceChannel;
                    $scope.arrayPrint.push(pro);
                }
            });
        }
        function clearStamp() {
            $scope.bookDetails = [];
            $scope.arrayPrint = [];
            sessionStorage.removeItem('arrayPrint');
        }
        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}

        authen();
        loadSuppliers();
        loadProductStatus();
        loadChannels();
        init();
        $window.document.title = "In tem sản phẩm";
    }

})(angular.module('softbbm.stamps'));