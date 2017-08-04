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