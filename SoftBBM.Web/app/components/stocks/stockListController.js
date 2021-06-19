(function (app) {
    app.controller('stockListController', stockListController);

    stockListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state', '$injector', 'FileSaver', '$http', 'authenticationService']
    function stockListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state, $injector, FileSaver, $http, authenticationService) {
        $scope.stocks = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10';
        $scope.setupToogleDataVal = false;
        $scope.showId = false;
        $scope.showImage = true;
        $scope.showProductCode = true;
        $scope.showName = true;
        $scope.showStock = true;
        $scope.showStockAll = true;
        $scope.showPriceBase = true;
        $scope.showPriceBaseOld = true;
        $scope.showPriceAvg = true;
        $scope.showPriceRef = true;
        $scope.showPriceWholesale = true;
        $scope.showPriceShop = true;
        $scope.showPriceChannel = true;
        $scope.showSupplier = false;
        $scope.showStatus = true;
        $scope.showCURD = true;
        $scope.showVat = false;
        $scope.showBarcode = false;
        $scope.selectedStock = {};
        $scope.suppliers = [];
        $scope.productStatuses = [];
        $scope.enabledEdit = [];
        $scope.files = [];
        $scope.filters = {
            selectedProductCategoryFilters: [],
            selectedSupplierFilters: [],
            selectedProductStatusFilters: [],
            selectedVatStatusFilters: [],
            selectedStockFilter: null,
            selectedStockFilterValue: null,
            selectedStockTotalFilter: null,
            selectedStockTotalFilterValue: null,
            selectedHideStatusFilter: [],
            selectedProductOptionFilters: null
        }
        $scope.filters.sortBy = '';
        $scope.productCodeSort = false;
        $scope.productNameSort = false;
        $scope.stockSort = false;
        $scope.stockTotalSort = false;
        $scope.priceBaseSort = false;
        $scope.channelName = '';
        $scope.dynamicPopover = {
            content: 'Hello, World!',
            templateUrl: 'myPopoverTemplate.html',
            title: 'Title'
        };
        $scope.enabledPopover = [];
        $scope.selectedStockPopover = null;
        $scope.showPriceChannelDiscount = true;
        $scope.hideStatuses = [
            { Id: 0, Name: "Hiện SP", },
            { Id: 1, Name: "Ẩn SP", }
        ];
        var productOp = [
            { Id: 1, Name: 'SP đã ẩn', CreatedDate: null, CreatedBy: null, UpdatedDate: null, UpdatedBy: null, Status: null, Prioty: null },
            { Id: 2, Name: 'SP đã hiện', CreatedDate: null, CreatedBy: null, UpdatedDate: null, UpdatedBy: null, Status: null, Prioty: null },
            { Id: 3, Name: 'SP đã đăng', CreatedDate: null, CreatedBy: null, UpdatedDate: null, UpdatedBy: null, Status: null, Prioty: null },
            { Id: 4, Name: 'SP đã đăng + ẩn', CreatedDate: null, CreatedBy: null, UpdatedDate: null, UpdatedBy: null, Status: null, Prioty: null }
        ]
        $scope.productOp = {
            dataSource: productOp,
            placeholder: "Chọn",
            dataTextField: "Name",
            dataValueField: "Id",
            filter: "contains",
            change: function (e) {
                updateFilter();
            }
        };
        $scope.picker = {};
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            showWeeks: false
        };
        $scope.waiting = false;

        $scope.init = init;
        $scope.search = search;
        $scope.openDetailModal = openDetailModal;
        $scope.refeshPage = refeshPage;
        $scope.editStock = editStock;
        $scope.updateStock = updateStock;
        $scope.cancelStock = cancelStock;
        $scope.loadSuppliers = loadSuppliers;
        $scope.loadProductStatus = loadProductStatus;
        $scope.openChannelPrices = openChannelPrices;
        $scope.openStockTotalAll = openStockTotalAll;
        $scope.setupToogleData = setupToogleData;
        $scope.exportProductsExcel = exportProductsExcel;
        $scope.importProductsExcel = importProductsExcel;
        $scope.downloadImportSample = downloadImportSample;
        $scope.authenExport = authenExport;
        $scope.authenImport = authenImport;
        $scope.openProductAddModal = openProductAddModal;
        $scope.authenAddProduct = authenAddProduct;
        $scope.loadFilter = loadFilter;
        $scope.loadProductCategories = loadProductCategories;
        $scope.updateFilter = updateFilter;
        $scope.resetStockFilter = resetStockFilter;
        $scope.resetStockTotalFilter = resetStockTotalFilter;
        $scope.sortBy = sortBy;
        $scope.loadChannels = loadChannels;
        $scope.updateSelectedChannel = updateSelectedChannel;
        $scope.openChannelPricePopover = openChannelPricePopover;
        $scope.closeChannelPricePopover = closeChannelPricePopover;
        $scope.openStartDateDiscount = openStartDateDiscount;
        $scope.openEndDateDiscount = openEndDateDiscount;
        $scope.updateChannelPricePopover = updateChannelPricePopover;
        $scope.openStockEditModal = openStockEditModal
        $scope.openChannelPricesModal = openChannelPricesModal;
        $scope.exportPriceWholesale = exportPriceWholesale;
        $scope.syncToShopee = syncToShopee;
        $scope.hideProduct = hideProduct;
        $scope.updateUrlProducts = updateUrlProducts;

        function search(page) {
            page = page || 0;
            $scope.waiting = true;
            $scope.filters.branchId = $scope.branchSelectedRoot.Id;
            $scope.filters.page = page;
            $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
            $scope.filters.stringFilter = $scope.filterStocks;
            if ($scope.filters.selectedStockFilter)
                $scope.filters.selectedStockFilter = parseInt($scope.filters.selectedStockFilter);
            if ($scope.filters.selectedStockFilterValue)
                $scope.filters.selectedStockFilterValue = parseInt($scope.filters.selectedStockFilterValue);
            if ($scope.filters.selectedStockTotalFilter)
                $scope.filters.selectedStockTotalFilter = parseInt($scope.filters.selectedStockTotalFilter);
            if ($scope.filters.selectedStockTotalFilterValue)
                $scope.filters.selectedStockTotalFilterValue = parseInt($scope.filters.selectedStockTotalFilterValue);
            if ($scope.filters.selectedProductOptionFilters)
                $scope.filters.selectedProductOptionFilters = parseInt($scope.filters.selectedProductOptionFilters);
            $scope.filters.channelId = $scope.selectedChannel.Id;
            apiService.post('/api/stock/search/', $scope.filters, function (result) {
                $scope.stocks = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.waiting = false;
                if ($scope.filterStocks && $scope.filterStocks.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' sản phẩm tìm được');
                }
            }, function (response) {
                $scope.waiting = false;
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterStocks = '';
            $scope.filters = {
                selectedProductCategoryFilters: [],
                selectedSupplierFilters: [],
                selectedProductStatusFilters: [],
                selectedVatStatusFilters: [],
                selectedStockFilter: null,
                selectedStockFilterValue: null,
                selectedStockTotalFilter: null,
                selectedStockTotalFilterValue: null,
                selectedProductOptionFilters: null,
                stringFilter: null,
                selectedHideStatusFilter: [],
                selectedProductOptionFilters: null
            }
            search();
        }
        function openDetailModal(stock) {
            $scope.editedChannel = stock;
            $modal.open({
                templateUrl: '/app/components/stocks/stockDetailModal.html' + BuildVersion,
                controller: 'stockDetailController',
                scope: $scope
            }).result.finally(function ($scope) {

            });
        }
        function editStock(stock, index) {
            authen();
            $scope.selectedStock = stock;
            $scope.enabledEdit[index] = true;
        }
        function updateStock(stock, index) {
            $scope.selectedStock = stock;
            apiService.post('api/stock/update', $scope.selectedStock,
                function (result) {
                    notificationService.displaySuccess(result.data.shop_sanpham.masp + ' đã được cập nhật.');
                    $scope.selectedStock = {};
                    $scope.enabledEdit[index] = false;
                    search($scope.page);
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        function cancelStock(stock, index) {
            $scope.selectedStock = {};
            $scope.enabledEdit[index] = false;
            //search($scope.page);
        }
        function loadSuppliers() {
            var config = {
                params: {
                    order: 'Name'
                }
            }
            apiService.get('/api/supplier/getall', config, function (result) {
                $scope.suppliers = result.data
                var suppliers = result.data
                $scope.suppliersOp = {
                    dataSource: suppliers,
                    placeholder: "Chọn",
                    dataTextField: "Name",
                    dataValueField: "Id",
                    valuePrimitive: true,
                    filter: "contains",
                    change: function (e) {
                        updateFilter();
                    }
                };
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function loadProductStatus() {
            $scope.loading = true;
            apiService.get('/api/stock/getallproductstatus', null, function (result) {
                $scope.productStatuses = result.data
                var productStatuses = result.data
                $scope.productStatusesOp = {
                    dataSource: productStatuses,
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
                $scope.loading = false;
            });
        }
        function openChannelPrices(stock) {
            $scope.selectedStock = stock;
            $uibModal.open({
                templateUrl: '/app/components/stocks/stockChannelPricesModal.html' + BuildVersion,
                controller: 'stockChannelPricesController',
                scope: $scope,
                backdrop: 'static',
                keyboard: false,
                windowClass: 'app-modal-window'
            }).result.finally(function () {
                search($scope.page);
            });
        }
        function openStockTotalAll(stock) {
            closeChannelPricePopover();
            $scope.selectedStock = stock;
            $uibModal.open({
                templateUrl: '/app/components/stocks/stockTotalAllModal.html' + BuildVersion,
                controller: 'stockTotalAllController',
                scope: $scope,
                windowClass: 'app-modal-window-small'
            }).result.finally(function ($scope) {

            });
        }
        function setupToogleData() {
            $scope.setupToogleDataVal = !$scope.setupToogleDataVal;
        }
        function authen() {
            apiService.get('api/stock/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function authenExport() {
            apiService.get('api/stock/authenexport', null, function (result) {
                exportProductsExcel();
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function exportPriceWholesale() {
            apiService.get('api/stock/authenexport', null, function (result) {
                $uibModal.open({
                    templateUrl: '/app/components/stocks/exportPriceWholesaleModal.html' + BuildVersion,
                    controller: 'exportPriceWholesaleController',
                    scope: $scope,
                    backdrop: 'static',
                    keyboard: false,
                    size: 'sm'
                }).result.finally(function ($scope) {

                });
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function authenImport() {
            apiService.get('api/stock/authenimport', null, function (result) {
                importProductsExcel();
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function exportProductsExcel() {
            $uibModal.open({
                templateUrl: '/app/components/stocks/processModal.html' + BuildVersion,
                controller: 'processModalController',
                scope: $scope,
                backdrop: 'static',
                keyboard: false,
                size: 'sm'
            }).result.finally(function ($scope) {

            });
        }
        function downloadImportSample() {
            $scope.loading = true;
            var config = {
                responseType: "arraybuffer"
            };
            apiService.get('/api/stock/downloadimportsample/', config, function (result) {
                var blob = new Blob([result.data], {
                    type: 'application/vnd.ms-excel;charset=charset=utf-8'
                });
                FileSaver.saveAs(blob, 'ImportProducts.xlsx');
                $scope.loading = false;
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function importProductsExcel() {
            if ($scope.files.length > 0) {
                $uibModal.open({
                    templateUrl: '/app/components/stocks/processImportModal.html' + BuildVersion,
                    controller: 'exportPriceWholesaleController',
                    scope: $scope,
                    backdrop: 'static',
                    keyboard: false,
                    size: 'sm'
                }).result.finally(function () {
                    $scope.files = [];
                });
            }
            else
                notificationService.displayError("Chưa chọn file import !");
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }

        }
        function openProductAddModal() {
            $uibModal.open({
                templateUrl: '/app/components/stocks/productAddModal.html' + BuildVersion,
                controller: 'productAddController',
                scope: $scope,
                size: 'lg',
                backdrop: 'static',
                keyboard: false
            }).result.finally(function () {
                search($scope.page);
            });
        }
        function authenAddProduct() {
            apiService.get('api/product/authenadd', null, function (result) {
                openProductAddModal();
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadFilter() {
            $scope.animate = !$scope.animate;
        }
        function loadProductCategories() {
            $scope.loading = true;
            apiService.get('/api/productcategory/getall', null, function (result) {
                $scope.productCategories = result.data
                var productCategories = result.data
                $scope.productCategoriesOp = {
                    dataSource: productCategories,
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
        function loadVatStatuses() {
            $scope.loading = true;
            apiService.get('api/supplier/getallvatstatus', null,
                function (result) {
                    var vatStatuses = [];
                    vatStatuses = result.data
                    $scope.vatStatusesOp = {
                        dataSource: vatStatuses,
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
                    notificationService.displayError(error.data);
                    $scope.loading = false;
                });
        }
        function updateFilter() {
            $scope.page = $scope.page || 0;
            search($scope.page);
        }
        function resetStockFilter() {
            $scope.filters.selectedStockFilter = null;
            $scope.filters.selectedStockFilterValue = null;
            $scope.filters.sortBy = '';
            $scope.productCodeSort = false;
            $scope.productNameSort = false;
            $scope.stockSort = false;
            $scope.stockTotalSort = false;
            $scope.priceBaseSort = false;
            search();
        }
        function resetStockTotalFilter() {
            $scope.filters.selectedStockTotalFilter = null;
            $scope.filters.selectedStockTotalFilterValue = null;
            $scope.filters.selectedProductOptionFilters = null;
            $scope.filters.sortBy = '';
            $scope.productCodeSort = false;
            $scope.productNameSort = false;
            $scope.stockSort = false;
            $scope.stockTotalSort = false;
            $scope.priceBaseSort = false;
            search();
        }
        function sortBy(value) {
            switch (value) {
                case 'productCode':
                    $scope.productCodeSort = !$scope.productCodeSort;
                    $scope.filters.sortBy = $scope.productCodeSort == true ? ' productCode_Des' : 'productCode_Asc';
                    break;
                case 'productName':
                    $scope.productNameSort = !$scope.productNameSort;
                    $scope.filters.sortBy = $scope.productNameSort == true ? 'productName_Des' : 'productName_Asc';
                    break;
                case 'stock':
                    $scope.stockSort = !$scope.stockSort;
                    $scope.filters.sortBy = $scope.stockSort == true ? 'stock_Des' : 'stock_Asc';
                    break;
                case 'stockTotal':
                    $scope.stockTotalSort = !$scope.stockTotalSort;
                    $scope.filters.sortBy = $scope.stockTotalSort == true ? 'stockTotal_Des' : 'stockTotal_Asc';
                    break;
                case 'priceBase':
                    $scope.priceBaseSort = !$scope.priceBaseSort;
                    $scope.filters.sortBy = $scope.priceBaseSort == true ? 'priceBase_Des' : 'priceBase_Asc';
                    break;
            }
            search();
        }
        function loadChannels() {
            $scope.loading = true;
            apiService.get('/api/channel/getall', null, function (result) {
                $scope.channels = result.data;
                $scope.loading = false;
                if (localStorage.getItem("selectedChannel") && localStorage.getItem("selectedChannel") != "undefined" && localStorage.getItem("selectedChannel") != "null") {
                    $scope.selectedChannel = JSON.parse(localStorage.getItem("selectedChannel"));
                }
                else {
                    $scope.selectedChannel = $scope.channels[0];
                    localStorage.setItem("selectedChannel", JSON.stringify($scope.selectedChannel));
                }
                $scope.channelName = $scope.selectedChannel.Name.toUpperCase();
                search();
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function updateSelectedChannel() {
            if ($scope.selectedChannel) {
                var channelName = $scope.selectedChannel.Name;
                $scope.channelName = channelName.toUpperCase();
                localStorage.setItem("selectedChannel", JSON.stringify($scope.selectedChannel));
                search($scope.page);
            }
        }
        function openChannelPricePopover(stock) {
            if ($scope.selectedChannel.Id != 7) {
                if ($scope.selectedStock)
                    closeChannelPricePopover();
                $scope.selectedStock = stock;
                $scope.enabledPopover[stock.Id] = true;
                $scope.selectedStockPopover = {
                    priceChannel: $scope.selectedStock.PriceChannel,
                    priceDiscount: $scope.selectedStock.PriceDiscount,
                    startDateDiscount: $scope.selectedStock.StartDateDiscount,
                    endDateDiscount: $scope.selectedStock.EndDateDiscount,
                    productId: $scope.selectedStock.shop_sanpham.id,
                    channelId: $scope.selectedChannel.Id
                }
            }
        }
        function closeChannelPricePopover() {
            if ($scope.selectedStock)
                $scope.enabledPopover[$scope.selectedStock.Id] = false;
            $scope.selectedStock = null;
            $scope.selectedStockPopover = null;
        }
        function openStartDateDiscount($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.startDateDiscount = true;

        }
        function openEndDateDiscount($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.picker.endDateDiscount = true;

        }
        function updateChannelPricePopover() {
            apiService.post('/api/stock/updateproductchannelprice', $scope.selectedStockPopover, function (result) {
                closeChannelPricePopover();
                notificationService.displaySuccess('Cập nhật giá kênh thành công!');
                search($scope.page);
            }, function (response) {
                notificationService.displayError(response.data);
            });

        }
        function openStockEditModal(stock) {
            closeChannelPricePopover();
            $scope.selectedStock = stock;
            $uibModal.open({
                templateUrl: '/app/components/stocks/stockEditProductModal.html' + BuildVersion,
                controller: 'stockEditProductController',
                scope: $scope,
                size: 'lg',
                backdrop: 'static',
                keyboard: false
            }).result.finally(function ($scope) {

            });
        }
        function openChannelPricesModal(stock) {
            $scope.selectedStock = {
                id: stock.shop_sanpham.id
            };
            $uibModal.open({
                templateUrl: '/app/components/stocks/stockChannelPricesModal.html' + BuildVersion,
                controller: 'stockChannelPricesController',
                scope: $scope,
                backdrop: 'static',
                keyboard: false,
                windowClass: 'app-modal-window'
            }).result.finally(function () {
                //search($scope.page);
            });
        }
        function syncToShopee(stock) {
            apiService.post('/api/stock/updateproductchannelprice', $scope.selectedStockPopover, function (result) {
                closeChannelPricePopover();
                notificationService.displaySuccess('Cập nhật giá kênh thành công!');
                search($scope.page);
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function hideProduct(productID, status) {
            var config = {
                params: {
                    productID: productID
                }
            }
            apiService.get('/api/product/hide', config, function (result) {
                if (status == true)
                    notificationService.displaySuccess('Ẩn sản phẩm thành công!');
                else
                    notificationService.displaySuccess('Hiện sản phẩm thành công!');
            }, function (response) {
                console.log(response.data);
                notificationService.displayError('Cập nhật thất bại!');
            });
        }
        function updateUrlProducts() {
            $scope.waiting = true;
            apiService.get('/api/product/updateurlproducts', null, function (result) {
                $scope.waiting = false;
                notificationService.displaySuccess('Cập nhật Url sản phẩm thành công!');
            }, function (response) {
                $scope.waiting = false;
                console.log(response.data);
                notificationService.displayError('Cập nhật thất bại!');
            });
        }

        //listen for the file selected event
        $scope.$on("fileSelected", function (event, args) {
            $scope.$apply(function () {
                //add the file object to the scope's files collection
                $scope.files.push(args.file);
            });
        });
        $scope.$watch('filterStocks', function (tmpStr) {
            if (!tmpStr || tmpStr.length == 0) {
                return 0;
            }
            $scope.loading = true;
            $scope.filters.branchId = $scope.branchSelectedRoot.Id;
            $scope.filters.page = 0;
            $scope.filters.pageSize = parseInt($scope.pageSizeNumber);
            $scope.filters.stringFilter = $scope.filterStocks;
            if ($scope.filters.selectedStockFilter)
                $scope.filters.selectedStockFilter = parseInt($scope.filters.selectedStockFilter);
            if ($scope.filters.selectedStockFilterValue)
                $scope.filters.selectedStockFilterValue = parseInt($scope.filters.selectedStockFilterValue);
            if ($scope.filters.selectedStockTotalFilter)
                $scope.filters.selectedStockTotalFilter = parseInt($scope.filters.selectedStockTotalFilter);
            if ($scope.filters.selectedStockTotalFilterValue)
                $scope.filters.selectedStockTotalFilterValue = parseInt($scope.filters.selectedStockTotalFilterValue);
            if ($scope.filters.selectedProductOptionFilters)
                $scope.filters.selectedProductOptionFilters = parseInt($scope.filters.selectedProductOptionFilters);
            $scope.filters.channelId = $scope.selectedChannel.Id;
            apiService.post('/api/stock/search/', $scope.filters, function (result) {
                $scope.stocks = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterStocks && $scope.filterStocks.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' sản phẩm tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        });

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            loadChannels();
            init();
            loadSuppliers();
            loadProductStatus();
            loadProductCategories();
            loadVatStatuses();

            $window.document.title = "DS sản phẩm";
        }
    }
})(angular.module('softbbm.stocks'));