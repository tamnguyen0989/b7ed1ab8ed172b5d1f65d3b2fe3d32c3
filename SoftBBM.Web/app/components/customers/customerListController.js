(function (app) {
    app.controller('customerListController', customerListController);

    customerListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state']
    function customerListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state) {
        $scope.customers = [];
        $scope.cities = [];
        $scope.districts =[];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10'
        $scope.userId = 0;

        $scope.enabledEdit = [];
        $scope.editCustomer = editCustomer;
        $scope.updateCustomer = updateCustomer;
        $scope.cancelCustomer = cancelCustomer;

        $scope.search = search;
        $scope.refeshPage = refeshPage;

        function editCustomer(customer, index) {
            authen();
            $scope.selectedCustomer = customer;
            $scope.enabledEdit[index] = true;
        }
        function updateCustomer(customer, index) {
            $scope.selectedCustomer = customer;
            $scope.selectedCustomer.userId = $scope.userId;
            apiService.post('api/customer/update', $scope.selectedCustomer,
                function (result) {
                    notificationService.displaySuccess('Cập nhật thành công.');
                    $scope.selectedCustomer = {};
                    $scope.enabledEdit[index] = false;
                    search($scope.page);
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function cancelCustomer(customer, index) {
            $scope.selectedCustomer = {};
            $scope.enabledEdit[index] = false;
            search($scope.page);
        }
        function search(page) {
            page = page || 0;

            $scope.loading = true;

            var config = {};
            config.page = page;
            config.pageSize = parseInt($scope.pageSizeNumber);
            config.filter = $scope.filterCustomers;

            apiService.post('/api/customer/search/', config, function (result) {

                $scope.customers = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterCustomers && $scope.filterCustomers.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' khách hàng tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterCustomers = '';
            search();
        }
        function authen() {
            apiService.get('api/customer/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadCities() {
            apiService.get('api/city/getall', null, function (result) {
                $scope.cities = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadDistricts() {
            apiService.get('api/district/getall', null, function (result) {
                $scope.districts = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
        }

        loadCities();
        loadDistricts();
        $scope.search();
        init();
        $window.document.title = "DS khách hàng";
    }
})(angular.module('softbbm.customers'));