(function (app) {
    app.controller('productCategoryAddController', productCategoryAddController);

    productCategoryAddController.$inject = ['apiService', '$window', '$scope', 'notificationService', '$state'];

    function productCategoryAddController(apiService, $window, $scope, notificationService, $state) {
        $scope.productCategory = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.loading = false;
        $scope.addProductCategory = addProductCategory;

        function addProductCategory() {
            $scope.loading = true;
            apiService.post('api/productcategory/add', $scope.productCategory,
                function (result) {
                    notificationService.displaySuccess('Thêm mới thành công !');
                    $scope.loading = false;
                    $state.go('product_categories');
                }, function (error) {
                    notificationService.displayError(error.data);
                });
        }
        function authen() {
            apiService.get('api/productcategory/authenadd', null, function (result) {
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function init() {
            if (localStorage.getItem("userId")) {
                $scope.productCategory.createdBy = JSON.parse(localStorage.getItem("userId"));
            }
        }

        $window.document.title = "Thêm mới nhóm sản phẩm";
        init();
        authen();
    }

})(angular.module('softbbm.product_categories'));