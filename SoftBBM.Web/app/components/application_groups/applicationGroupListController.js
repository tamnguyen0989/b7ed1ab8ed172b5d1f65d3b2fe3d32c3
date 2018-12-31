(function (app) {
    app.controller('applicationGroupListController', applicationGroupListController);

    applicationGroupListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$filter', '$state']

    function applicationGroupListController($scope, apiService, $window, notificationService, $ngBootbox, $filter, $state) {
        $scope.applicationGroups = [];

        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.keyword = '';
        $scope.pageSizeNumber = '10';

        $scope.getApplicationGroups = getApplicationGroups;
        $scope.search = search;
        $scope.deleteApplicationGroup = deleteApplicationGroup;
        $scope.selectAll = selectAll;
        $scope.deleteMultiple = deleteMultiple;
        $scope.refeshPage = refeshPage;

        $scope.$watch("applicationGroups", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);

        function getApplicationGroups(page) {
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber)
                }
            }
            apiService.get('api/applicationgroup/getlistpaging', config, function (result) {
                if (result.data.TotalCount == 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                //else {
                //    notificationService.displaySuccess('Đã tìm thấy ' + result.data.TotalCount+' bản ghi.');
                //}
                $scope.applicationGroups = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
            }, function () {
                console.log('Load applicationGroup fail!');
            })
        }
        function search() {
            getApplicationGroups();
        }
        function deleteApplicationGroup(id) {
            $ngBootbox.confirm('Bạn có chắc chắn muốn xoá?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('api/applicationgroup/delete', config, function (result) {
                    notificationService.displaySuccess('Xoá ' + result.data.Name + '  thành công');
                    search();
                }, function () {
                    notificationService.displayError('Xoá không thành công');
                });
            });
        }
        $scope.isAll = false;
        function selectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.applicationGroups, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.applicationGroups, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
            }
        }
        function deleteMultiple() {
            var listIdCheckedCategories = [];
            $.each($scope.selected, function (i, item) {
                listIdCheckedCategories.push(item.ID);
            });
            var config = {
                params: {
                    checkedApplicationGroups: JSON.stringify(listIdCheckedCategories)
                }
            }
            apiService.del('api/applicationGroup/deletemulti', config, function (result) {
                notificationService.displaySuccess('Xoá thành công ' + result.data + ' bản ghi');
                search();
            }, function (error) {
                notificationService.displayError('Xoá không thành công');
            });
        }
        function refeshPage() {
            search();
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}
        $scope.getApplicationGroups();
        $window.document.title = "DS Nhóm";
    }
})(angular.module('softbbm.application_groups'));
