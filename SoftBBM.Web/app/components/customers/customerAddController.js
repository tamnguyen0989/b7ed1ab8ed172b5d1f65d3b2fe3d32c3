(function (app) {
    app.controller('customerAddController', customerAddController);

    customerAddController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state'];

    function customerAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.customer = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.loading = false;
        $scope.userId = 0;

        $scope.addCustomer = addCustomer;
        $scope.updateDistricts = updateDistricts;

        function addCustomer() {
            $scope.loading = true;
            $scope.customer.userId = $scope.userId;
            apiService.post('api/customer/add', $scope.customer,
                function (result) {
                    notificationService.displaySuccess('Thêm mới thành công!');
                    $scope.loading = false;
                    $state.go('customers');
                }, function (error) {
                    $scope.loading = false;
                    notificationService.displayError(error.data);
                });
        }
        function authen() {
            apiService.get('api/customer/authenadd', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadCities() {
            apiService.get('api/city/getall', null, function (result) {
                $scope.cities = result.data;
                $scope.customer.City = $scope.cities[0];
                $scope.districts = $scope.cities[0].donhang_chuyenphat_tinh;
                $scope.customer.District = $scope.districts[0];
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function updateDistricts() {
            $scope.districts = $scope.customer.City.donhang_chuyenphat_tinh;
            $scope.customer.District = $scope.districts[0];
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
        }

        $window.document.title = "Thêm mới khách hàng";
        authen();
        loadCities();
        init();
    }

})(angular.module('softbbm.customers'));