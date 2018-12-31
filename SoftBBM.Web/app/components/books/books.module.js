(function () {
    angular.module('softbbm.books', ['softbbm.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('books', {
            url: '/books',
            parent: 'base',
            templateUrl: '/app/components/books/bookListView.html',
            controller: 'bookListController'
        }).state('add_book', {
            url: '/add_book',
            parent: 'base',
            templateUrl: '/app/components/books/bookAddView.html',
            controller: 'bookAddController'
        }).state('edit_book', {
            url: '/edit_book/:stockinId',
            parent: 'base',
            templateUrl: '/app/components/books/bookEditView.html',
            controller: 'bookEditController'
        }).state('branch_books', {
            url: '/branch_books',
            parent: 'base',
            templateUrl: '/app/components/books/branchbookListView.html',
            controller: 'branchbookListController'
        }).state('add_branch_book', {
            url: '/add_branch_book',
            parent: 'base',
            templateUrl: '/app/components/books/branchbookAddView.html',
            controller: 'branchbookAddController'
        }).state('edit_branch_book', {
            url: '/edit_branch_book/:stockinId',
            parent: 'base',
            templateUrl: '/app/components/books/branchbookEditView.html',
            controller: 'branchbookEditController'
        });
    }
})();