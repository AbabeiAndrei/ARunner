app.controller('registerController',
    [
        '$scope',
        '$http',
        function ($scope, $http) {

            $scope.registerModel = {
            };

            $scope.register = function () {
                $scope.errors = undefined;

                $http.post('/Account/Register', $scope.registerModel)
                    .then(function () {
                        window.location = '/Account/Login';
                    }, function (err) {
                        $scope.errors = err.date;
                    });
            };
        }
    ]);