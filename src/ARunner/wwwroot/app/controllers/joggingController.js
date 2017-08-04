app.controller('joggingController',
    [
        '$scope',
        '$http',
        'urlBuilder',
        'loader',
        'userSettings',
        function ($scope, $http, urlBuilder, loader, userSettings) {

            $scope.date = new Date();
            $scope.showFilter = false;
            $scope.filterFrom = $scope.date;
            $scope.filterTo = $scope.date;
            $scope.activePage = -1;
            $scope.isDtpFromOpen = false;
            $scope.isDtpToOpen = false;
            $scope.selectedUser = undefined;

            $scope.$watch('filterFrom', function (newValue) {
                if ($scope.filterTo < newValue)
                    $scope.filterTo = newValue;
            });

            $scope.$watch('filterTo', function (newValue) {
                if ($scope.filterFrom > newValue)
                    $scope.filterFrom = newValue;
            });
            
            $scope.refresh = function () {
                $scope.selectPage(0);
            };

            $scope.toggleFilter = function() {
                $scope.showFilter = !$scope.showFilter;
            };
            
            $scope.pageIsActive = function(page) {
                if (page === $scope.activePage)
                    return 'active';
                return '';
            };

            $scope.selectPage = function (page) {
                if ($scope.data !== undefined && $scope.data.items.length !== 0 && (page < 0 || page >= $scope.data.pages.length))
                    return;

                $scope.activePage = page;
                var url = urlBuilder.build('/joggings', generateFilter(page));
                loader.show();
                $http.get(url)
                    .then(function(res) {
                            $scope.data = res.data;
                            var pages = res.data.pages;
                            $scope.data.pages = [];
                            for (var i = 0; i < pages && pages < 10; i++)
                                $scope.data.pages.push(i);
                        },
                        function(error) {
                            //todo handle
                            alert(error);
                        })
                        .finally(function() {
                            loader.hide();
                        });
            };

            function generateFilter(page) {
                var result = { 
                    page: page,
                    userId: userSettings.userId
                };

                if ($scope.selectedUser !== undefined && $scope.selectedUser.id !== undefined)
                    result.userId = $scope.selectedUser.id;

                if ($scope.customUserId !== undefined)
                    result.userId = $scope.customUserId;

                if ($scope.customNumerOfEntries !== undefined)
                    result.pageSize = $scope.customNumerOfEntries;

                if ($scope.showFilter) {
                    result.from = $scope.filterFrom;
                    result.to = $scope.filterTo;
                }

                return result;
            };

            $scope.toggleDtp = function(dtp) {
                if (dtp === 1) {
                    $scope.isDtpFromOpen = !$scope.isDtpFromOpen;
                } else if (dtp === 2) {
                    $scope.isDtpToOpen = !$scope.isDtpToOpen;
                }
            };

            $scope.remove = function(item) {
                var result =
                    confirm('Are you sure you want to remove this jogging session? The action cannot be undone!');

                if (result) {
                    $http.delete('/joggings/' + item.id)
                        .then(function () {
                                $scope.refresh();
                            },
                            function(err) {
                                //todo handle
                            });
                }
            };

            $scope.$on('refresh', function () {
                $scope.refresh();
            });

            $scope.getUsers = function(val) {
                return $http.get('users/sugestions/' + val)
                            .then(function(result) {
                                return result.data;
                            });
            }

            $scope.refresh();
        }
    ]);