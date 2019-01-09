(function (app) {
    app.controller('productCategoryListController', productCategoryListController);

    productCategoryListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state']
    function productCategoryListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal, $state) {
        $scope.productCategories = [];
        $scope.selectedProductCategory = {};
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10';
        $scope.userId = 0;

        $scope.enabledEdit = [];
        $scope.editProductCategory = editProductCategory;
        $scope.edit2ProductCategory = edit2ProductCategory;
        $scope.updateProductCategory = updateProductCategory;
        $scope.cancelProductCategory = cancelProductCategory;

        $scope.search = search;
        $scope.refeshPage = refeshPage;

        function editProductCategory(productCategory, index) {
            authen();
            $scope.selectedProductCategory = productCategory;
            $scope.enabledEdit[index] = true;
        }
        function edit2ProductCategory(productCategory) {
            $scope.selectedProductCategory = productCategory;
            $scope.selectedProductCategory.updatedBy = $scope.userId;
            apiService.post('api/productcategory/update', $scope.selectedProductCategory,
                function (result) {
                    notificationService.displaySuccess('Cập nhật thành công!');
                    $scope.selectedProductCategory = {};
                    $scope.enabledEdit[index] = false;
                    search($scope.page);
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        function updateProductCategory(productCategory, index) {
            $scope.selectedProductCategory = productCategory;
            $scope.selectedProductCategory.updatedBy = $scope.userId;
            apiService.post('api/productcategory/update', $scope.selectedProductCategory,
                function (result) {
                    notificationService.displaySuccess('Cập nhật thành công!');
                    $scope.selectedProductCategory = {};
                    $scope.enabledEdit[index] = false;
                    search($scope.page);
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        function cancelProductCategory(productCategory, index) {
            $scope.selectedProductCategory = {};
            $scope.enabledEdit[index] = false;
            search($scope.page);
        }
        function search(page) {
            page = page || 0;

            $scope.loading = true;

            var config = {
                params: {
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber),
                    filter: $scope.filterProductCategory
                }
            };

            apiService.get('/api/productcategory/search/', config, function (result) {
                $scope.productCategories = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterProductCategory && $scope.filterProductCategory.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' kênh tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterProductCategory = '';
            search();
        }
        function authen() {
            apiService.get('api/productcategory/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.userId = JSON.parse(localStorage.getItem("userId"));
            }
        }

        $window.document.title = "DS nhóm sản phẩm";
        $scope.search();
        init();
    }
})(angular.module('softbbm.product_categories'));