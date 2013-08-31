/// <reference path="../libs/_references.js" />
window.viewsFactory = (function () {
    var rootUrl = "Scripts/partials/";
    var templates = {};

    function getTempate(name) {
        var promise = new RSVP.Promise(function (resolve, reject) {
            if (templates[name]) {
                resolve(templates[name]);
            }
            else {
                $.get(rootUrl + name + '.html')
                    .success(function (templateHtml) {
                        templates[name] = templateHtml;
                        resolve(templateHtml);
                    }).error(function (err) {
                        reject(err);
                    });
            }
        });
        return promise;
    };

    //function getLoginView() {
    //    return getTempate("login-form");
    //};

    //function getHeroView() {
    //    return getTempate("hero-main");
    //};

    return {
        getTemplate: getTempate,
        //loginView: getLoginView
    };
}());