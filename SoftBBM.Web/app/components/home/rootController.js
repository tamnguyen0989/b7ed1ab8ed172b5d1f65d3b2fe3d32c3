(function (app) {
    app.controller('rootController', rootController);

    rootController.$inject = ['$state', 'authData', 'loginService', '$scope', 'apiService', '$window', 'authenticationService', 'notificationService', '$rootScope'];

    function rootController($state, authData, loginService, $scope, apiService, $window, authenticationService, notificationService, $rootScope) {
        //$scope.logOut = logOut;
        //$scope.search = search;
        //$scope.hideNotification = hideNotification;
        //$scope.updatebranchSelectedRoot = updatebranchSelectedRoot;
        //$scope.updateIsRead = updateIsRead;
        //$scope.updateReadAll = updateReadAll;

        //$scope.notification = {};
        //$scope.OpenNotifi = false;
        //$scope.branchSelectedRoot = {};
        //$scope.userId = authData.authenticationData.userId;
        ////$scope.init = init;
        //$scope.page = 0;
        //$scope.pagesCount = 0;
        //$scope.pageSizeNumber = '10';
        //$scope.notifications = [];
        //$scope.chatHub = null;
        //$scope.chatHub = $.connection.chatHub; // initializes hub
        //$.connection.hub.start(); // starts hub
        //$scope.chatHub.client.broadcastMessage = function (branchId) {
        //    if ($scope.branchSelectedRoot.Id == branchId) {
        //        search($scope.page);
        //    }
        //};
        //$scope.authentication = authData.authenticationData;
        //$scope.sideBar = "/app/shared/views/sideBar.html";
        ////refesh lost session
        ////authenticationService.validateRequest();
        //$scope.updatingReadAll = false;


        //function search(page) {
        //    if (authData.authenticationData.IsAuthenticated == true) {
        //        page = page || 0;
        //        var config = {
        //            params: {
        //                page: page,
        //                pageSize: parseInt($scope.pageSizeNumber),
        //                branchId: authData.authenticationData.selectedBranch.Id
        //            }
        //        }
        //        apiService.get('api/notification/search', config, function (result) {
        //            $scope.totalIsRead = result.data.TotalIsRead;
        //            $scope.notifications = result.data.Items;
        //            $scope.page = result.data.Page;
        //            $scope.pagesCount = result.data.TotalPages;
        //            $scope.totalCount = result.data.TotalCount;
        //            if ($scope.OpenNotifi == true)
        //                $("#notificationList").css("display", "block");
        //        }, function (response) {
        //            notificationService.displayError(response.data);
        //        });
        //    }
        //}
        //function logOut() {
        //    loginService.logOut();
        //    $scope.notifications = [];
        //    $scope.totalIsRead = 0;
        //    $scope.branchSelectedRoot = {};
        //    $state.go('login');
        //}
        //function updatebranchSelectedRoot(branchSelectedRoot) {
        //    $scope.branchSelectedRoot = branchSelectedRoot;
        //    authData.authenticationData.selectedBranch = branchSelectedRoot;
        //    localStorage.setItem("selectedBranch", JSON.stringify($scope.branchSelectedRoot));
        //    localStorage.removeItem('selectedChannel');
        //    search($scope.page);
        //    $state.go('home');
        //}
        //function init() {
        //    if (authData.authenticationData.IsAuthenticated == true) {
        //        if (localStorage.getItem("userId")) {
        //            $scope.notification.UpdatedBy = JSON.parse(localStorage.getItem("userId"));
        //        }
        //        debugger
        //        if (localStorage.getItem("selectedBranch") && localStorage.getItem("selectedBranch") != "undefined") {
        //            var _selectedBranch = JSON.parse(localStorage.getItem("selectedBranch"));
        //            $scope.branchSelectedRoot = _selectedBranch;
        //            search($scope.page);
        //        }
        //    }
        //}
        //function updateIsRead(notification) {
        //    $scope.notification.Id = notification.Id;
        //    if (notification.Url != '#') {
        //        var url = $state.href(notification.Url);
        //        window.open(url, '_blank');
        //    }
        //    apiService.post('api/notification/updateread', $scope.notification, function (result) {
        //        if (result.data == true) {
        //            search($scope.page);
        //        }
        //    }, function (error) {
        //        notificationService.displayError(error.data);
        //    });

        //}
        //function hideNotification() {
        //    $scope.OpenNotifi = !$scope.OpenNotifi;
        //    if ($scope.OpenNotifi == false) {
        //        $("#notificationList").css("display", "none");
        //    }
        //    else
        //        $("#notificationList").css("display", "block");
        //}
        //function updateReadAll() {
        //    $scope.updatingReadAll = true;
        //    var config = {
        //        params: {
        //            branchId: $scope.branchSelectedRoot.Id,
        //            updateBy: $scope.notification.UpdatedBy
        //        }
        //    };
        //    apiService.get('api/notification/updatereadall', config, function (result) {
        //        if (result.data == true) {
        //            $scope.updatingReadAll = false;
        //            search($scope.page);
        //        }
        //    }, function (error) {
        //        notificationService.displayError(error.data);
        //    });
        //}
        //init();
    }
})(angular.module('softbbm'));