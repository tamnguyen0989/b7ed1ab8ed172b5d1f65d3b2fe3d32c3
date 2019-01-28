(function (app) {
    app.controller('channelSalesReportController', channelSalesReportController);

    channelSalesReportController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state', '$rootScope']

    function channelSalesReportController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state, $rootScope) {

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
        $scope.series = ['Doanh số','Lợi nhuận'];
        $scope.chartdata = [];
        $scope.visible = false;
        $scope.loading = false;
        $scope.totalQuantities = 0;
        $scope.totalSales = 0;
        $scope.totalRevenues = 0;
        $scope.colours = ['#3498DB', '#F08080'];

        $scope.openStartDateFilter = openStartDateFilter;
        $scope.openEndDateFilter = openEndDateFilter;
        $scope.resetTimeFilter = resetTimeFilter;
        $scope.exportReport = exportReport;

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
        function exportReport() {
            $scope.totalQuantities = 0;
            $scope.totalSales = 0;
            $scope.totalRevenues = 0;
            $scope.loading = true;
            if ($scope.startDateFilter && $scope.endDateFilter) {
                $scope.visible = false;
                $scope.config = {
                    startDate: $scope.startDateFilter,
                    endDate: $scope.endDateFilter,
                    branchId: $scope.branchSelectedRoot.Id
                };
                apiService.post('api/order/channelsalesreport', $scope.config,
                function (result) {
                    if (result.data.length == 0){
                        $scope.visible = false;
                        notificationService.displayError("Không có dữ liệu!");
                    }
                    else
                    {
                        $scope.tabledata = result.data;
                        var labels = [];
                        var chartData = [];
                        var sales = [];
                        var revenues = [];
                        $.each(result.data, function (i, item) {
                            labels.push(item.Name);
                            sales.push(item.Sales/1000);
                            revenues.push(item.Revenues/1000)
                            $scope.totalQuantities += item.Quantity;
                            $scope.totalSales += item.Sales;
                            $scope.totalRevenues += item.Revenues;
                        });
                        chartData.push(sales);
                        chartData.push(revenues);
                        $scope.labels = labels;
                        $scope.chartdata = chartData;
                        $scope.visible = true;
                        $scope.loading = false;
                    }
                }, function (error) {
                    $scope.loading = false;
                    notificationService.displayError(error.data);
                });
            }
            else {
                notificationService.displayError('Chọn ngày bắt đầu, kết thúc !');
                $scope.loading = false;
            }
        }


        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            $window.document.title = "Kênh tất cả - Doanh số";
        }
    }

})(angular.module('softbbm.reports'));