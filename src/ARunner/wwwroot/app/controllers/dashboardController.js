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