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