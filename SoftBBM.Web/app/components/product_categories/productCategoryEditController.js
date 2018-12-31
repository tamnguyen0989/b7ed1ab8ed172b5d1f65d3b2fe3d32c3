(function (app) {
    app.controller('productCategoryEditController', productCategoryEditController);

    productCategoryEditController.$inject = ['apiService', '$window',  '$scope', 'notificationService', '$state', '$stateParams', '$filter'];
    function productCategoryEditController(apiService, $window, $scope, notificationService, $state, $stateParams, $filter) {
        $scope.productCategory = {
        }
        $scope.updating = false;
        $scope.updateProductCategory = updateProductCategory;

        function loadProductCategoryDetail() {
            var id = parseInt($stateParams.id);
            apiService.get('api/productcategory/detail/' + id, null, function (result) {
                $scope.productCategory = result.data;
                $window.document.title = "Cập nhật Nhóm sản phẩm - " + $scope.productCategory.Id;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function updateProductCategory() {
            $scope.updating = true;
            apiService.post('api/productcategory/update', $scope.productCategory, function (result) {
                $scope.updating = false;
                notificationService.displaySuccess('Cập nhật nhóm sản phẩm thành công!');
                $state.go('product_categories');
            }, function (error) {
                notificationService.displayError('Cập nhật không thành công');
            });
        }
        function authen() {
            apiService.get('api/productcategory/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}

        authen();
        loadProductCategoryDetail();
    }
})(angular.module('softbbm.product_categories'));