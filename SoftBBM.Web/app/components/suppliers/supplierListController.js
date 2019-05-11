(function (app) {
    app.controller('supplierListController', supplierListController);

    supplierListController.$inject = ['$scope', 'apiService', '$window',  'notificationService', '$ngBootbox','$state']
    function supplierListController($scope, apiService, $window, notificationService, $ngBootbox,$state) {
        $scope.suppliers = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.enabledEdit = [];
        $scope.editSupplier = editSupplier;
        $scope.deleteSupplier = deleteSupplier;
        $scope.updateSupplier = updateSupplier;
        $scope.cancelSupplier = cancelSupplier;
        $scope.pageSizeNumber = '10';
        $scope.userId = null;

        $scope.search = search;
        $scope.refeshPage = refeshPage;
        $scope.updatePageSize = updatePageSize;
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));               
            }
        }
        
        function search(page) {
            page = page || 0;

            $scope.loading = true;

            var config = {
                params: {
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber),
                    filter: $scope.filterSuppliers
                }
            };

            apiService.get('/api/supplier/search/', config, function (result) {
                $scope.suppliers = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterSuppliers && $scope.filterSuppliers.length && $scope.page==0) {
                    //notificationService.displaySuccess($scope.totalCount + ' nhà cung cấp tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterSuppliers = '';
            search();
        }
        function editSupplier(supplier, index) {
            authen();
            $scope.selectedSupplier = supplier;
            $scope.enabledEdit[index] = true;
        }
        function updateSupplier(supplier, index) {
            $scope.selectedSupplier = supplier;
            $scope.selectedSupplier.updatedBy = JSON.parse(localStorage.getItem("userId"));
            apiService.post('api/supplier/update', $scope.selectedSupplier,
                function (result) {
                    notificationService.displaySuccess('Nhà cung cấp ' + result.data.Id + ' đã được cập nhật.');
                    $scope.selectedSupplier = {};
                    $scope.enabledEdit[index] = false;
                    search($scope.page);
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        function cancelSupplier(supplier, index) {
            $scope.selectedSupplier = {};
            $scope.enabledEdit[index] = false;
            search($scope.page);
        }
        function updatePageSize() {

        }
        function authen() {
            apiService.get('api/supplier/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function loadProductStatus() {
            apiService.get('/api/supplier/getallvatstatus', null, function (result) {
                $scope.supplierVatStatuses = result.data;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        function deleteSupplier(supplier, index) {
            $ngBootbox.confirm('Bạn có chắc chắn muốn xoá?').then(function () {
                var configs = {
                    params: {
                        supplierID: supplier.Id
                    }
                }
                $scope.loading = true;
                apiService.get('api/supplier/delete', configs, function (result) {
                    $scope.loading = false;
                    notificationService.displaySuccess("Xoá thành công");
                    search($scope.page);
                }, function (error) {
                    $scope.loading = false;
                    notificationService.displayError(error.data);
                });
            });
        }

        init();
        loadProductStatus();
        $scope.search();
        $window.document.title = "DS nhà cung cấp";

    }
})(angular.module('softbbm.suppliers'));