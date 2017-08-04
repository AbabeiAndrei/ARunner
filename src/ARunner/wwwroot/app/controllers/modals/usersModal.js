app.controller('usersModal',
    [
        '$scope',
        '$http',
        '$uibModalInstance',
        'userSettings',
        function ($scope, $http, $uibModalInstance, userSettings) {
            $scope.user = $scope.$resolve.settings || {};

            $scope.accessTypes = [
                'regular',
                'manager'
            ];

            if (userSettings.access === 'admin')
                $scope.accessTypes.push('admin');

            $scope.submitUsers = function () {

                if ($scope.user.id) {
                    $http.put('/users', $scope.user)
                        .then(function () {
                                $scope.closeInstance();
                            },
                            function (err) {
                                //todo handle
                            });
                } else {
                    $http.post('/users', $scope.user)
                        .then(function () {
                            $scope.closeInstance();
                        }, function (err) {
                            //todo handle
                        });
                }
            };

            $scope.closeInstance = function () {
                $uibModalInstance.close($scope.user);
            }

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.suspendUser = function() {
                var result = confirm('Are you sure you want to suspend this user?');

                if (result) {
                    $http.delete('/users/' + $scope.user.id + '/suspend')
                        .then(function() {
                                $scope.closeInstance();
                            },
                            function(err) {
                                //todo handle
                            });
                }
            };

            $scope.suspendUser = function () {
                var result = confirm('Are you sure you want to reset password of this user?');

                if (result) {
                    $http.delete('/users/' + $scope.user.id + '/resetPassword')
                        .then(function () {
                                $scope.closeInstance();
                            },
                            function (err) {
                                //todo handle
                            });
                }
            };
        }
    ]);