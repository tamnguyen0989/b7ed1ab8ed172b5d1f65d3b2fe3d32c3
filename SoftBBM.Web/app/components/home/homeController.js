(function (app) {
    app.controller('homeController', homeController);

    homeController.$inject = ['notificationService', 'authenticationService', 'apiService', '$window', 'authData', '$scope'];

    function homeController(notificationService, authenticationService, apiService, $window, authData, $scope) {

        function migrationChannelPriceAndProduct() {
            $scope.loading = true;
            apiService.get('/api/home/migrationchannelpriceandproduct', null, function (result) {
                if (result.data == true)
                    debugger
                $scope.loading = false;
                notificationService.displaySuccess('Migration thành công');
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function updatePriceWholesale() {
            $scope.loading = true;
            apiService.get('/api/home/updatepricewholesale', null, function (result) {
                if (result.data == true)
                    $scope.loading = false;
                notificationService.displaySuccess('Migration thành công');
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function migrationBranchProductStock() {
            $scope.loading = true;
            apiService.get('/api/home/migrationbranchproductstock', null, function (result) {
                $scope.loading = false;
                notificationService.displaySuccess('Migration thành công');
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function migrationStockinSupplier() {
            $scope.loading = true;
            apiService.get('/api/home/migrationstockinsupplier', null, function (result) {
                $scope.loading = false;
                notificationService.displaySuccess('Migration thành công');
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function resetPriceDiscount() {
            $scope.loading = true;
            apiService.get('/api/home/resetpricediscount', null, function (result) {
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        //authenticationService.setHeader();        
        //migrationChannelPriceAndProduct();
        //updatePriceWholesale();
        //migrationBranchProductStock();
        //migrationStockinSupplier();    
        resetPriceDiscount();
    }

})(angular.module('softbbm'));