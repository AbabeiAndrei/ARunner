app.controller('layoutController',
    [
        '$scope', 'loader', 'userSettings', function ($scope, loader, userSettings) {
            $scope.show = 1;
            $scope.showLoader = false;
            $scope.setShow = function(item) {
                $scope.show = item;
                $scope.$emit('refresh', null);
            }

            $scope.showUsers = userSettings.access !== undefined && userSettings.access !== 'regular';

            //loader.show = function() {
            //    $scope.showLoader = true;
            //};

            //loader.hide = function () {
            //    $scope.showLoader = false;
            //};
        }
    ]);