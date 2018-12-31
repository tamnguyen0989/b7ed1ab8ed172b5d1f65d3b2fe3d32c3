(function (app) {
    app.controller('baseController', baseController);

    baseController.$inject = ['notificationService', 'authenticationService', 'apiService', '$window', 'authData', '$scope', '$rootScope'];
    function baseController(notificationService, authenticationService, apiService, $window, authData, $scope, $rootScope) {
        authenticationService.setHeader();
        //if (authData.authenticationData.IsAuthenticated == true) {
        //    if (localStorage.getItem("selectedBranch") && localStorage.getItem("selectedBranch") != "undefined") {
        //        var _selectedBranch = JSON.parse(localStorage.getItem("selectedBranch"));
        //        $scope.branchSelectedRoot = _selectedBranch;
        //    }
        //    else {
        //        var branches = JSON.parse(localStorage.getItem("branches"));
        //        $scope.branchSelectedRoot = branches[0];
        //        authData.authenticationData.selectedBranch = branches[0];
        //        localStorage.setItem("selectedBranch", JSON.stringify(branches[0]));
        //    }
        //}
        if (localStorage.getItem("userId")) {
            $scope.userId = JSON.parse(localStorage.getItem("userId"));
        }
    }
})(angular.module('softbbm'));