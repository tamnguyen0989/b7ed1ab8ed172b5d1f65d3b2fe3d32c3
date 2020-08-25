(function () {
    angular.module('softbbm.books', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('books', {
            url: '/books',
            parent: 'base',
            templateUrl: '/app/components/books/bookListView.html' + BuildVersion,
            controller: 'bookListController'
        }).state('add_book', {
            url: '/add_book',
            parent: 'base',
            templateUrl: '/app/components/books/bookAddView.html' + BuildVersion,
            controller: 'bookAddController'
        }).state('edit_book', {
            url: '/edit_book/:stockinId',
            parent: 'base',
            templateUrl: '/app/components/books/bookEditView.html' + BuildVersion,
            controller: 'bookEditController'
        }).state('branch_books', {
            url: '/branch_books',
            parent: 'base',
            templateUrl: '/app/components/books/branchbookListView.html' + BuildVersion,
            controller: 'branchbookListController'
        }).state('add_branch_book', {
            url: '/add_branch_book',
            parent: 'base',
            templateUrl: '/app/components/books/branchbookAddView.html' + BuildVersion,
            controller: 'branchbookAddController'
        }).state('edit_branch_book', {
            url: '/edit_branch_book/:stockinId',
            parent: 'base',
            templateUrl: '/app/components/books/branchbookEditView.html' + BuildVersion,
            controller: 'branchbookEditController'
        });
    }
})();