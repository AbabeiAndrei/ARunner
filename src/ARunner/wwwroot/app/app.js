var app = angular.module('jgApp', ['ui.bootstrap', 'ae-datetimepicker', 'ui.bootstrap.typeahead']);

var modalControllerTemplates = {
    'joggingModal': '/app/partials/modal/jogging.html',
    'usersModal': '/app/partials/modal/users.html'
};

app.provider('routeConfigurator', function () {

    var self = this;

    this.$get = [function () {
        return self;
    }];

    self.modalMapping = function (key) {
        return modalControllerTemplates[key];
    };
});

app.provider('loader', function() {

    var self = this;

    this.$get = [function () {
        return self;
    }];

    self.show = function() {

    };

    self.hide = function () {

    };
});
//app.factory('userSettings', function () {
//    return {
//        units: 'metric',
//        language: 'en-US'
//    }
//});

app.provider('userSettings', function () {

    var self = this;

    this.units = 'metric';
    this.language = 'en-US';

    this.$get = [function () {
        return self;
    }];

    self.modalMapping = function (key) {
        return modalControllerTemplates[key];
    };
});
app.factory('urlBuilder', function ($httpParamSerializer) {
    function buildUrl(url, params) {
        var serializedParams = $httpParamSerializer(params);

        if (serializedParams.length > 0) {
            url += ((url.indexOf('?') === -1) ? '?' : '&') + serializedParams;
        }

        return url;
    }

    var builder = {
        build: buildUrl
    };

    return builder;
});
app.filter('distanceUnit', ['userSettings', function (userSettings) {
    return function (input) {
        input = input || 0;
        if (userSettings === undefined || userSettings.units === undefined || userSettings.units === 'metric') {
            var km = Math.trunc(input / 1000);
            var meters = input % 1000;

            if (km > 0) {
                return km + ' km ' + meters + ' m';
            }
            return meters + ' m';
        }
        if (userSettings.units === 'imperial') {
            return convertMetersToFeet(input) + ' f';
        }

        return input;
    };
}]);

app.filter('time', function () {
    return function (input) {
        input = input || 0;
        var hours = Math.trunc(input / 60);
        var minutes = input % 60;

        if (hours > 0) {
            return hours + ' h ' + minutes + ' m';
        }

        return minutes + ' m';
    };
});

app.filter('speedUnit', ['userSettings', function (userSettings) {
    return function (input) {
        input = input || 0;

        if (userSettings === undefined || userSettings.units === undefined || userSettings.units === 'metric') {
            return (input * 0.06).toFixed(2) + ' km/h';
        }
        if (userSettings.units === 'imperial') {
            return convertMetersToFeet(input) + ' f/s';
        }

        return input;
    };
}]);

app.filter('dateTimeCustom', function() {
    return function (input) {
        input = input || 0;
        return moment(input).utcOffset(6).format('MM/DD/YYYY HH:mm');
    };
});

function convertMetersToFeet(meters) {
    return (meters * 3.28084).toFixed(2);
}
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
app.controller('dashboardController',
[
    '$scope',
    '$http',
    'loader',
    '$filter',
    function ($scope, $http, loader, $filter) {

        $scope.refresh = function () {
            loader.show();
            $http.get('/joggings/statistics/fd965382-a45f-4095-a729-ad42c7c2856a')
                .then(function(result) {
                        $scope.statistics = createStatistics(result.data);
                        $scope.chartOptions = createOptions($scope.statistics);
                        $('#chartContainer').CanvasJSChart($scope.chartOptions);
                    },
                    function(result) {
                        //todo handle
                    })
                .finally(function() {

                    loader.hide();
                });
        };

        $scope.calculateDiference = function (newVal, oldVal) {
            newVal = newVal || 0;
            oldVal = oldVal || 0;

            var res;
            if (newVal !== oldVal) {
                var diff = newVal - oldVal;
                res = diff / oldVal * 100;
            } else
                res = 0;

            var fixRes = res.toFixed(2);
            var str = 0;
            var color = 'black';

            if (res < 0) {
                str = fixRes + ' %';
                color = '#f44336';
            }
            if (res > 0) {
                str = '+' + fixRes + ' %';
                color = '#4caf50';
            }
            if (res === 0) {
                str = '~' + fixRes + ' %';
                color = '#ffc107';
            }

            $scope.diferenceColor = color;

            return str;
        };

        $scope.$on('refresh', function() {
            $scope.refresh();
        });

        $scope.refresh();

        function createOptions(statistics) {

            var points = [];
            var start = statistics.weeks.length - 10; //for the past 10 weeks

            if (start < 0)
                start = 0;

            for (var i = start; i < statistics.weeks.length ; i++) {
                var week = statistics.weeks[i];
                points.push({
                    y: week.run,
                    label: moment(week.startWeek).format('MM/DD') + ' - ' + moment(week.endWeek).format('MM/DD')
                });
            }

            var options = {
                title: {
                    text: 'Statistics over the past few weeks'
                },
                animationEnabled: true,
                contentFormatter: function(elem) {
                    return elem + 1;
                },
                data: [
                    {
                        type: 'column',
                        dataPoints: points
                    }
                ]
            };

            return options;
        }

        function createStatistics(statistics) {
            var date = new Date();

            var dateFirstDay = getStartOfWeek(date);
            var dateEndDay = getEndOfWeek(date);

            var lastWeekDate = new Date();
            lastWeekDate.setDate(lastWeekDate.getDate() - 7);

            var lastWeekFirstDay = getStartOfWeek(lastWeekDate);
            var lastWeekEndDay = getEndOfWeek(lastWeekDate);

            var thisWeek;
            var lastWeek;

            var result = {
                weeks: []
            };

            for (var i = 0; i < statistics.length; i++) {
                var curent = statistics[i];
                var dateFrom = new Date(curent.from);
                var dateTo = new Date(curent.to);

                if (dateRangeOverlaps(dateFrom, dateTo, dateFirstDay, dateEndDay))
                    thisWeek = curent;
                else if (dateRangeOverlaps(dateFrom, dateTo, lastWeekFirstDay, lastWeekEndDay))
                    lastWeek = curent;

                var statistic = createJoggingWeek(curent, dateFrom);

                if (statistic !== null)
                    result.weeks.push(statistic);
            }

            if (thisWeek === undefined)
                thisWeek = null;

            result.thisWeek = createJoggingWeek(thisWeek, date);

            if (lastWeek === undefined)
                lastWeek = null;

            lastWeek = createJoggingWeek(lastWeek, lastWeekDate);

            result.lastWeek = lastWeek;

            return result;
        }

        function createJoggingWeek(statistic, week) {
            if ((statistic === null || statistic === undefined) && week !== null && week !== undefined) {
                var first = week.getDate() - week.getDay();
                var last = first + 6;

                var firstday = new Date(week.setDate(first));
                var lastday = new Date(week.setDate(last));

                return {
                    startWeek: firstday,
                    endWeek: lastday,
                    run: 0,
                    time: 0,
                    average: 0
                };

            }
            else if (statistic === null || statistic === undefined) {
                return null;
            }

            return {
                startWeek: new Date(statistic.from),
                endWeek: new Date(statistic.to),
                run: statistic.runTotal,
                time: statistic.timeSpendRunning,
                average: statistic.averageSpeed
            };
        }

        function dateRangeOverlaps(aStart, aEnd, bStart, bEnd) {
            if (aStart <= bStart && bStart <= aEnd) return true; // b starts in a
            if (aStart <= bEnd && bEnd <= aEnd) return true; // b ends in a
            if (bStart < aStart && aEnd < bEnd) return true; // a in b
            return false;
        }

        function getStartOfWeek(date) {
            date = new Date(date);
            var day = date.getDay(),
                diff = date.getDate() - day + (day === 0 ? -6 : 1); // adjust when day is sunday
            return new Date(date.setDate(diff));
        }

        function getEndOfWeek(date) {
            date = getStartOfWeek(date);
            date.setDate(date.getDate() + 6);
            return date;
        }
    }
]);
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