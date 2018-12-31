(function (app) {
    app.controller('branchListController', branchListController);

    branchListController.$inject = ['$scope', 'apiService', '$window',  'notificationService', '$ngBootbox','$state']
    function branchListController($scope, apiService, $window, notificationService, $ngBootbox,$state) {
        $scope.branches = [];
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10'

        $scope.enabledEdit = [];
        $scope.editBranch = editBranch;
        $scope.updateBranch = updateBranch;
        $scope.cancelBranch = cancelBranch;

        $scope.search = search;
        $scope.refeshPage = refeshPage;

        function search(page) {
            page = page || 0;

            $scope.loading = true;

            var config = {
                params: {
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber),
                    filter: $scope.filterBranches
                }
            };

            apiService.get('/api/branch/search/', config, function (result) {
                $scope.branches = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterBranches && $scope.filterBranches.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' Kho tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterBranches = '';
            search();
        }
        function editBranch(branch, index) {
            authen();
            $scope.selectedBranch = branch;
            $scope.enabledEdit[index] = true;
        }
        function updateBranch(branch, index) {
            $scope.selectedBranch = branch;
            apiService.post('api/branch/update', $scope.selectedBranch,
                function (result) {
                    notificationService.displaySuccess('Kho ' + result.data.Id + ' đã được cập nhật.');
                    $scope.selectedBranch = {};
                    $scope.enabledEdit[index] = false;
                    search($scope.page);
                }, function (error) {
                    //notificationService.displayError('Cập nhật không thành công.');
                    debugger
                    notificationService.displayError(error.data);
                });
        }
        function cancelBranch(branch, index) {
            $scope.selectedBranch = {};
            $scope.enabledEdit[index] = false;
            search($scope.page);
        }
        function authen() {
            apiService.get('api/branch/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        $scope.search();
        $window.document.title = "DS kho";
    }
})(angular.module('softbbm.branches'));