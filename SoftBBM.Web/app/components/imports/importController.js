(function (app) {
    app.controller('importController', importController);

    importController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state', '$rootScope', '$uibModal'];

    function importController(apiService, $window, $scope, notificationService, $state, $rootScope, $uibModal) {

        $scope.files = [];
        $scope.successMessageChannelPrice = '';
        $scope.errorMessageChannelPrice = '';
        $scope.successMessageProduct = '';

        $scope.authenImportProduct = authenImportProduct;
        $scope.authenImportChannelPrice = authenImportChannelPrice;
        $scope.loadChannels = loadChannels;
        $scope.closeSuccessMessageChannelPrice = closeSuccessMessageChannelPrice;
        $scope.closeErrorMessageChannelPrice = closeErrorMessageChannelPrice;
        $scope.closeSuccessMessageProduct = closeSuccessMessageProduct;

        function importProductsExcel() {        
            if ($scope.files.length > 0) {
                var modalInstance = $uibModal.open({
                    templateUrl: '/app/components/imports/productProcessImportModal.html',
                    controller: 'productProcessImportController',
                    scope: $scope,
                    backdrop: 'static',
                    keyboard: false,
                    size: 'sm'
                });
                modalInstance.result.then(function (data) {
                    $scope.successMessageProduct = data.SuccessMessage;
                }, function () {

                });
            }
            else
                notificationService.displayError("Chưa chọn file import !");
        }
        function authenImportProduct() {
            apiService.get('api/stock/authenimport', null, function (result) {
                importProductsExcel();
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function importChannelPricesExcel() {
            if ($scope.files.length > 0) {
                $scope.successMessageChannelPrice = '';
                $scope.errorMessageChannelPrice = '';
                var modalInstance = $uibModal.open({
                    templateUrl: '/app/components/imports/channelPriceProcessImportModal.html',
                    controller: 'channelPriceProcessImportController',
                    scope: $scope,
                    backdrop: 'static',
                    keyboard: false,
                    size: 'sm'
                });
                modalInstance.result.then(function (data) {
                    $scope.successMessageChannelPrice = data.SuccessMessage;
                    $scope.errorMessageChannelPrice = data.ErrorMessage;
                }, function () {

                });
            }
            else
                notificationService.displayError("Chưa chọn file import !");
        }
        function authenImportChannelPrice() {
            apiService.get('api/stock/authenimport', null, function (result) {
                importChannelPricesExcel();
            }, function (error) {
                notificationService.displayError(error.data);
            });
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
        function closeSuccessMessageChannelPrice() {
            $scope.successMessageChannelPrice = '';
        }
        function closeErrorMessageChannelPrice() {
            $scope.errorMessageChannelPrice = '';
        }
        function closeSuccessMessageProduct() {
            $scope.successMessageProduct = '';
        }

        //listen for the file selected event
        $scope.$on("fileSelected", function (event, args) {
            $scope.$apply(function () {
                //add the file object to the scope's files collection
                
                $scope.files = [];
                $scope.files.push(args.file);
            });
        });

        if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
            notificationService.displayError("Hãy chọn kho");
            $state.go('home');
        }
        else {
            $window.document.title = "Import Product";
            loadChannels();
        }
    }

})(angular.module('softbbm.imports'));