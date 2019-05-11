(function (app) {
    app.controller('salesReportController', salesReportController);

    salesReportController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state', '$rootScope']

    function salesReportController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state, $rootScope) {

        $scope.picker = {};
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            showWeeks: false
        };
        $scope.startDateFilter = null;
        $scope.endDateFilter = null;
        $scope.tabledata = [];
        $scope.labels = [];
        $scope.series = ['Doanh số'];
        $scope.chartdata = [];
        $scope.visible = false;
        $scope.loading = false;
        $scope.labelsRevenue = [];
        $scope.series = ['Doanh số', 'Lợi nhuận'];
        $scope.chartdataRevenue = [];
        $scope.totalQuantities = 0;
        $scope.totalSales = 0;
        $scope.totalRevenues = 0;
        $scope.colours = ['#3498DB', '#F08080'];
        $scope.months = [
            { name: '01', value: 1 },
            { name: '02', value: 2 },
            { name: '03', value: 3 },
            { name: '04', value: 4 },
            { name: '05', value: 5 },
            { name: '06', value: 6 },
            { name: '07', value: 7 },
            { name: '08', value: 8 },
            { name: '09', value: 9 },
            { name: '10', value: 10 },
            { name: '11', value: 11 },
            { name: '12', value: 12 },
        ]
        $scope.years = [
        ]
        $scope.startMonthMFilter = null;
        $scope.startYearMFilter = null;
        $scope.endMonthMFilter = null;
        $scope.endYearMFilter = null;
        $scope.startYearYFilter = null;
        $scope.endYearYFilter = null;

        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.resetTimeFilter = resetTimeFilter;
        $scope.resetTimeMFilter = resetTimeMFilter;
        $scope.resetTimeYFilter = resetTimeYFilter;
        $scope.exportReport = exportReport;
        $scope.loadChannels = loadChannels;
        $scope.exportMReport = exportMReport;
        $scope.exportYReport = exportYReport;

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
        function resetTimeFilter() {
            $scope.startDateFilter = null;
            $scope.endDateFilter = null;
            $scope.visible = false;
        }
        function resetTimeMFilter() {
            $scope.startMonthMFilter = null;
            $scope.endMonthMFilter = null;
            $scope.startYearMFilter = null;
            $scope.endYearMFilter = null;
            $scope.visible = false;
        }
        function resetTimeYFilter() {
            $scope.startYearYFilter = null;
            $scope.endYearYFilter = null;
            $scope.visible = false;
        }
        function exportReport() {
            $scope.exporting = true;
            $scope.totalQuantities = 0;
            $scope.totalSales = 0;
            $scope.totalRevenues = 0;
            if ($scope.startDateFilter && $scope.endDateFilter && $scope.selectedChannel) {
                $scope.visible = false;
                $scope.config = {
                    startDate: $scope.startDateFilter,
                    endDate: $scope.endDateFilter,
                    branchId: $scope.branchSelectedRoot.Id,
                    channelId: $scope.selectedChannel.Id
                };
                apiService.post('api/order/salesreport', $scope.config,
                function (result) {
                    if (result.data.length == 0) {
                        $scope.visible = false;
                        notificationService.displayError("Không có dữ liệu!");
                        $scope.exporting = false;
                    }
                    else {
                        $scope.tabledata = result.data;
                        var labels = [];
                        var chartData = [];
                        var sales = [];
                        var revenues = [];
                        $.each(result.data, function (i, item) {
                            labels.push(item.NewDayStr);
                            sales.push(item.Sales / 1000);
                            revenues.push(item.Revenues / 1000)
                            $scope.totalQuantities += item.Quantity;
                            $scope.totalSales += item.Sales;
                            $scope.totalRevenues += item.Revenues;
                        });
                        chartData.push(sales);
                        chartData.push(revenues);
                        $scope.labels = labels;
                        $scope.chartdata = chartData;
                        $scope.visible = true;
                        $scope.exporting = false;
                    }
                }, function (error) {
                    $scope.exporting = false;
                    notificationService.displayError(error.data);
                });
            }
            else {
                notificationService.displayError('Chọn ngày bắt đầu, kết thúc !');
                $scope.exporting = false;
            }
        }
        function loadChannels() {
            $scope.loading = true;
            apiService.get('/api/channel/getall', null, function (result) {
                $scope.channels = result.data;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function exportMReport() {
            $scope.exporting = true;
            $scope.totalQuantities = 0;
            $scope.totalSales = 0;
            $scope.totalRevenues = 0;
            if ($scope.startMonthMFilter && $scope.endMonthMFilter && $scope.startYearMFilter && $scope.endYearMFilter && $scope.selectedChannel) {
                $scope.visible = false;
                var startDate = new Date();
                var endDate = new Date();
                startDate.setMonth($scope.startMonthMFilter.value - 1);
                startDate.setYear($scope.startYearMFilter.value);
                endDate.setMonth($scope.endMonthMFilter.value - 1);
                endDate.setYear($scope.endYearMFilter.value);
                $scope.config = {
                    startDate: startDate,
                    endDate: endDate,
                    branchId: $scope.branchSelectedRoot.Id,
                    channelId: $scope.selectedChannel.Id
                };
                apiService.post('api/order/salesreportmonth', $scope.config,
                function (result) {
                    if (result.data.length == 0) {
                        $scope.visible = false;
                        notificationService.displayError("Không có dữ liệu!");
                        $scope.exporting = false;
                    }
                    else {
                        $scope.tabledata = result.data;
                        var labels = [];
                        var chartData = [];
                        var sales = [];
                        var revenues = [];
                        $.each(result.data, function (i, item) {
                            labels.push(item.NewDayStr);
                            sales.push(item.Sales / 1000);
                            revenues.push(item.Revenues / 1000)
                            $scope.totalQuantities += item.Quantity;
                            $scope.totalSales += item.Sales;
                            $scope.totalRevenues += item.Revenues;
                        });
                        chartData.push(sales);
                        chartData.push(revenues);
                        $scope.labels = labels;
                        $scope.chartdata = chartData;
                        $scope.visible = true;
                        $scope.exporting = false;
                    }
                }, function (error) {
                    $scope.exporting = false;
                    notificationService.displayError(error.data);
                });
            }
            else {
                notificationService.displayError('Chọn ngày bắt đầu, kết thúc !');
                $scope.exporting = false;
            }
        }
        function exportYReport() {
            $scope.exporting = true;
            $scope.totalQuantities = 0;
            $scope.totalSales = 0;
            $scope.totalRevenues = 0;
            if ($scope.startYearYFilter && $scope.endYearYFilter && $scope.selectedChannel) {
                $scope.visible = false;
                var startDate = new Date();
                var endDate = new Date();
                startDate.setYear($scope.startYearYFilter.value);
                endDate.setYear($scope.endYearYFilter.value);
                $scope.config = {
                    startDate: startDate,
                    endDate: endDate,
                    branchId: $scope.branchSelectedRoot.Id,
                    channelId: $scope.selectedChannel.Id
                };
                apiService.post('api/order/salesreportyear', $scope.config,
                function (result) {
                    if (result.data.length == 0) {
                        $scope.visible = false;
                        notificationService.displayError("Không có dữ liệu!");
                        $scope.exporting = false;
                    }
                    else {
                        $scope.tabledata = result.data;
                        var labels = [];
                        var chartData = [];
                        var sales = [];
                        var revenues = [];
                        $.each(result.data, function (i, item) {
                            labels.push(item.NewDayStr);
                            sales.push(item.Sales / 1000);
                            revenues.push(item.Revenues / 1000)
                            $scope.totalQuantities += item.Quantity;
                            $scope.totalSales += item.Sales;
                            $scope.totalRevenues += item.Revenues;
                        });
                        chartData.push(sales);
                        chartData.push(revenues);
                        $scope.labels = labels;
                        $scope.chartdata = chartData;
                        $scope.visible = true;
                        $scope.exporting = false;
                    }
                }, function (error) {
                    $scope.exporting = false;
                    notificationService.displayError(error.data);
                });
            }
            else {
                notificationService.displayError('Chọn ngày bắt đầu, kết thúc !');
                $scope.exporting = false;
            }
        }

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            $window.document.title = "Kênh tổng - Doanh số";
            loadChannels();
            var currentDate = new Date();
            var currentYear = currentDate.getFullYear();
            for (var i = 2014; i <= currentYear ; i++) {
                var year = {
                    name: i.toString(),
                    value: i
                }
                $scope.years.push(year);
            }
        }
    }

})(angular.module('softbbm.reports'));