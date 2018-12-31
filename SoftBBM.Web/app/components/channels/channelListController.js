(function (app) {
    app.controller('channelListController', channelListController);

    channelListController.$inject = ['$scope', 'apiService', '$window', 'notificationService', '$ngBootbox', '$uibModal', '$state']
    function channelListController($scope, apiService, $window, notificationService, $ngBootbox, $uibModal,$state) {
        $scope.channels = [];
        $scope.selectedChannel = {};
        $scope.loading = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSizeNumber = '10'

        $scope.enabledEdit = [];
        $scope.editChannel = editChannel;
        $scope.updateChannel = updateChannel;
        $scope.cancelChannel = cancelChannel;

        $scope.search = search;
        $scope.refeshPage = refeshPage;

        function editChannel(channel, index) {
            authen();
            $scope.selectedChannel = channel;
            $scope.enabledEdit[index] = true;
        }
        function updateChannel(channel, index) {
            $scope.selectedChannel = channel;
            apiService.post('api/channel/update', $scope.selectedChannel,
                function (result) {
                    notificationService.displaySuccess('Kênh ' + result.data.Id + ' đã được cập nhật.');
                    $scope.selectedChannel = {};
                    $scope.enabledEdit[index] = false;
                    search($scope.page);
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công');
                });
        }
        function cancelChannel(channel, index) {
            $scope.selectedChannel = {};
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
                    filter: $scope.filterChannels
                }
            };

            apiService.get('/api/channel/search/', config, function (result) {
                $scope.channels = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
                if ($scope.filterChannels && $scope.filterChannels.length && $scope.page == 0) {
                    //notificationService.displaySuccess($scope.totalCount + ' kênh tìm được');
                }
            }, function (response) {
                notificationService.displayError(response.data);
            });
        }
        function refeshPage() {
            $scope.filterChannels = '';
            search();
        }
        function authen() {
            apiService.get('api/channel/authenedit', null, function (result) {

            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        $scope.search();
        $window.document.title = "DS kênh";
    }
})(angular.module('softbbm.channels'));