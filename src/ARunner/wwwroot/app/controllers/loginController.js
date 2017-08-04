app.controller('loginController',
    [
        '$scope',
        '$http',
        'userSettings',
        function ($scope, $http, userSettings) {

            $scope.loginModel = {
            };

            $scope.login = function () {
                $scope.errors = undefined;

                $http.post('/Account/Login', $scope.loginModel)
                    .then(function (res) {
                        userSettings.userId = res.id;
                        userSettings.fullName = res.fullName;
                        userSettings.email = res.email;
                        userSettings.access = res.access;
                        window.location = '/';
                    }, function(err) {
                        $scope.errors = err.date;
                    });
            };
        }
    ]);