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