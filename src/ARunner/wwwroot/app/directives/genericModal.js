app.directive('genericModal', ['$rootScope', '$uibModal', 'routeConfigurator', function ($rootScope, $uibModal, routeConfigurator) {
    return {
        restrict: 'A',
        replace: false,
        link: function (scope, element, attributes) {
            $(element).click(function () {
                var instanceOptions = {
                    animation: true
                };

                if (attributes.genericModalTemplateUrl) {
                    instanceOptions.templateUrl = attributes.genericModalTemplateUrl;
                } else {
                    instanceOptions.templateUrl = routeConfigurator.modalMapping(attributes.genericModal);
                }

                instanceOptions.resolve = {
                    settings: function () {
                        if (attributes.genericModalData !== 'undefined' && attributes.genericModalData !== null) {
                            return scope.$eval(attributes.genericModalData);
                        }
                        return 'invalid';
                    }
                };

                if (attributes.size !== undefined) {
                    instanceOptions.size = attributes.size;
                }
                instanceOptions.controller = attributes.genericModal;

                var modalInstance = $uibModal.open(instanceOptions);
                modalInstance.result.then(function() {
                    if (attributes.genericModalSuccess !== undefined && attributes.genericModalSuccess !== null)
                        scope.$eval(attributes.genericModalSuccess);
                }, function() {
                    if(attributes.genericModalCancel !== undefined && attributes.genericModalCancel !== null)
                        scope.$eval(attributes.genericModalCancel);
                });
            });
        }
    };
}]);