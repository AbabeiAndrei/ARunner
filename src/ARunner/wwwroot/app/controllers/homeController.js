app.controller('homeController',
        function($scope) {
            $scope.test = 'yo';
        })
    .directive('dashboard',
        function() {
            return {
                restrict: 'E',
                templateUrl: 'app/partials/dashboard.html'
            }
        })
    .directive('joggings',
        function() {
            return {
                restrict: 'E',
                templateUrl: 'app/partials/joggings.html',
                scope: {
                    customUserId: '@',
                    customNumerOfEntries: '@'
                }
            }
        })
    .directive('users',
        function() {
            return {
                restrict: 'E',
                templateUrl: 'app/partials/users.html'
            }
        });

