app.controller('joggingModal',
    [
        '$scope',
        '$http',
        '$uibModalInstance',
        'userSettings',
        function ($scope, $http, $uibModalInstance, userSettings) {
            $scope.jogging = $scope.$resolve.settings || {};
            $scope.isDtpOpen = false;

            var date;
            if ($scope.jogging.id) {
                date = new Date(moment($scope.jogging.created).utcOffset(6).format('YYYY-MM-DD HH:mm'));
            } else {
                date = new Date();
            }

            $scope.dateJogging = date;
            $scope.timeJogging = date;

            $scope.submitJogging = function () {
                var dateRes = $scope.dateJogging.toLocaleDateString();
                var timeRes = $scope.timeJogging.toLocaleTimeString();

                $scope.jogging.created = new Date(dateRes + ' ' + timeRes).toISOString();
                if ($scope.jogging.id) {
                    $http.put('/joggings', $scope.jogging)
                        .then(function() {
                                $scope.closeInstance();
                            },
                            function(err) {
                                //todo handle
                            });
                } else {
                    $scope.jogging.userId = userSettings.userId;
                    $http.post('/joggings', $scope.jogging)
                        .then(function () {
                            $scope.closeInstance();
                        }, function (err) {
                            //todo handle
                        });
                }
            };

            $scope.closeInstance = function () {
                $uibModalInstance.close($scope.jogging);
            }

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.openDtp = function() {
                $scope.isDtpOpen = true;
            }

            $scope.deleteJogging = function() {
                var result = confirm('Are you sure you want to remove this jogging session? The acction cannot be undone!');

                if (result) {
                    $http.delete('/joggings/' + $scope.jogging.id)
                        .then(function () {
                            $scope.closeInstance();
                        }, function (err) {
                            //todo handle
                        });
                }
            }
        }
    ]);