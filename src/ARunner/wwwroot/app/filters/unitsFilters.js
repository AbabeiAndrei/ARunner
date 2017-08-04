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