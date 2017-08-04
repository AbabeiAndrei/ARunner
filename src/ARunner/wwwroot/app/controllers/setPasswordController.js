app.controller('setPasswordController',
    [
        '$scope',
        '$http',
        function ($scope, $http) {

            $scope.setPassModel = {
                token: location.search.replace('?token=', '')
            };

            $scope.setPass = function () {
                $scope.errors = undefined;

                if ($scope.setPassModel.password !== $scope.setPassModel.password2) {
                    $scope.errors = 'Password must be the same';
                    return;
                }

                $http.post('/Account/SetPassword', $scope.setPassModel)
                    .then(function () {
                        window.location = '/Account/Login';
                    }, function (err) {
                        $scope.errors = err.date;
                    });
            };
        }
    ]);