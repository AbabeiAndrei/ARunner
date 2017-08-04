app.controller('usersController',
    [
        '$scope',
        '$http',
        'urlBuilder',
        'loader',
        function ($scope, $http, urlBuilder, loader) {

            $scope.showFilter = false;
            $scope.filterName = '';
            $scope.activePage = -1;

            $scope.refresh = function () {
                $scope.selectPage(0);
            };

            $scope.toggleFilter = function () {
                $scope.showFilter = !$scope.showFilter;
            };

            $scope.pageIsActive = function (page) {
                if (page === $scope.activePage)
                    return 'active';
                return '';
            };

            $scope.selectPage = function (page) {
                if ($scope.data !== undefined && $scope.data.items.length !== 0 && (page < 0 || page >= $scope.data.pages.length))
                    return;

                $scope.activePage = page;
                var url = urlBuilder.build('/users', generateFilter(page));
                loader.show();
                $http.get(url)
                    .then(function (res) {
                            $scope.data = res.data;
                            var pages = res.data.pages;
                            $scope.data.pages = [];
                            for (var i = 0; i < pages && pages < 10; i++)
                                $scope.data.pages.push(i);
                        },
                        function (error) {
                            //todo handle
                            alert(error);
                        })
                    .finally(function () {
                        loader.hide();
                    });
            };

            function generateFilter(page) {
                var result = {
                    page: page
                };

                if ($scope.showFilter) {
                    result.name = $scope.filterName;
                }

                return result;
            };

            $scope.remove = function (item) {
                var result =
                    confirm('Are you sure you want to remove this user? The action cannot be undone!');

                if (result) {
                    $http.delete('/users/' + item.id)
                        .then(function () {
                                $scope.refresh();
                            },
                            function (err) {
                                //todo handle
                            });
                }
            };

            $scope.$on('refresh', function () {
                $scope.refresh();
            });

            $scope.refresh();
        }
    ]);