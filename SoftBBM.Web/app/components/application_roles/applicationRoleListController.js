(function (app) {
    app.controller('applicationRoleListController', applicationRoleListController);

    applicationRoleListController.$inject = ['$scope', 'apiService', '$window',  'notificationService', '$ngBootbox', '$filter','$state']

    function applicationRoleListController($scope, apiService, $window, notificationService, $ngBootbox, $filter,$state) {
        $scope.applicationRoles = [];

        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.keyword = '';
        $scope.pageSizeNumber = '10'

        $scope.GetApplicationRoles = GetApplicationRoles;
        $scope.SearchFirst = SearchFirst;
        $scope.Search = Search;
        $scope.DeleteApplicationRole = DeleteApplicationRole;
        $scope.SelectAll = SelectAll;
        $scope.DeleteMultiple = DeleteMultiple;
        $scope.refeshPage = refeshPage;

        $scope.$watch("applicationRoles", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);

        function GetApplicationRoles(page) {
            page = page || 0;
            var config = {
                params: {
                    filter: $scope.keyword,
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber)
                }
            }
            apiService.get('api/applicationRole/getlistpaging', config, function (result) {
                if (result.data.TotalCount == 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                //else {
                //    notificationService.displaySuccess('Đã tìm thấy ' + result.data.TotalCount+' bản ghi.');
                //}
                $scope.applicationRoles = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
            }, function () {
                console.log('Load applicationRole fail!');
            })
        }

        function Search(page) {
            page = page || 0;
            var config = {
                params: {
                    filter: $scope.keyword,
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber)
                }
            }
            apiService.get('api/applicationRole/getlistpaging', config, function (result) {
                $scope.applicationRoles = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
            }, function () {
                console.log('Load applicationRole fail!');
            })
        }
        function SearchFirst(page) {
            page = page || 0;
            var config = {
                params: {
                    filter: $scope.keyword,
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber)
                }
            }
            apiService.get('api/applicationRole/getlistpaging', config, function (result) {
                if (result.data.TotalCount == 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                else {
                    notificationService.displaySuccess('Đã tìm thấy ' + result.data.TotalCount+' bản ghi.');
                }
                $scope.applicationRoles = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
            }, function () {
                console.log('Load applicationRole fail!');
            })
        }
        function DeleteApplicationRole(id) {
            $ngBootbox.confirm('Bạn có chắc chắn muốn xoá?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('api/applicationRole/delete', config, function (result) {
                    notificationService.displaySuccess('Xoá ' + result.data.Name + '  thành công');
                    Search();
                }, function () {
                    notificationService.displayError('Xoá không thành công');
                });
            });
        }
        $scope.isAll = false;
        function SelectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.applicationRoles, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.applicationRoles, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
            }
        }
        function DeleteMultiple() {
            var listIdCheckedCategories = [];
            $.each($scope.selected, function (i, item) {
                listIdCheckedCategories.push(item.Id);
            });
            var config = {
                params: {
                    checkedApplicationRoles: JSON.stringify(listIdCheckedCategories)
                }
            }
            apiService.del('api/applicationRole/deletemulti', config, function (result) {
                notificationService.displaySuccess('Xoá thành công ' + result.data + ' bản ghi');
                Search();
            }, function (error) {
                notificationService.displayError('Xoá không thành công');
            });
        }
        function refeshPage() {
            $scope.keyword = '';
            Search();
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}

        $scope.GetApplicationRoles();
        $window.document.title = "DS Quyền";
    }
})(angular.module('softbbm.application_roles'));