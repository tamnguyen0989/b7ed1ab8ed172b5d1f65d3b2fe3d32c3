(function (app) {
    app.controller('applicationUserListController', applicationUserListController);

    applicationUserListController.$inject = ['$scope', 'apiService', '$window',  'notificationService', '$ngBootbox', '$filter','$state']

    function applicationUserListController($scope, apiService, $window, notificationService, $ngBootbox, $filter,$state) {
        $scope.applicationUsers = [];

        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.keyword = '';
        $scope.pageSizeNumber = '10'

        $scope.GetApplicationUsers = GetApplicationUsers;
        $scope.Search = Search;
        $scope.DeleteApplicationUser = DeleteApplicationUser;
        $scope.SelectAll = SelectAll;
        $scope.DeleteMultiple = DeleteMultiple;
        $scope.refeshPage = refeshPage;

        $scope.$watch("applicationUsers", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);

        function GetApplicationUsers(page) {
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    page: page,
                    pageSize: parseInt($scope.pageSizeNumber)
                }
            }
            apiService.get('api/applicationuser/getlistpaging', config, function (result) {
                if (result.data.TotalCount == 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                //else {
                //    notificationService.displaySuccess('Đã tìm thấy ' + result.data.TotalCount+' bản ghi.');
                //}
                $scope.applicationUsers = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
            }, function (error) {
                debugger
                console.log('Load applicationUser fail!');
            })
        }

        function Search() {
            GetApplicationUsers();
        }

        function DeleteApplicationUser(id) {
            $ngBootbox.confirm('Bạn có chắc chắn muốn xoá?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('api/applicationuser/delete', config, function (result) {
                    notificationService.displaySuccess('Xoá thành công');
                    Search();
                }, function (error) {
                    notificationService.displayError('Xoá không thành công' + error);
                });
            });
        }

        $scope.isAll = false;
        function SelectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.applicationUsers, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.applicationUsers, function (item) {
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
                    checkedapplicationUsers: JSON.stringify(listIdCheckedCategories)
                }
            }
            apiService.del('api/applicationUser/deletemulti', config, function (result) {
                notificationService.displaySuccess('Xoá thành công ' + result.data + ' bản ghi');
                Search();
            }, function (error) {
                notificationService.displayError('Xoá không thành công');
            });
        }
        function refeshPage() {
            Search();
        }

        //if (((Object.keys($scope.branchSelectedRoot).length === 0 && $scope.branchSelectedRoot.constructor === Object) || $scope.branchSelectedRoot == undefined) && localStorage.getItem("userId") != null) {
        //    notificationService.displayError("Hãy chọn kho");
        //    $state.go('home');
        //}

        $scope.GetApplicationUsers();
        $window.document.title = "DS User";
    }
})(angular.module('softbbm.application_users'));