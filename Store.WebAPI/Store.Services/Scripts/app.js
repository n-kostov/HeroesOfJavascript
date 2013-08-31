/// <reference path="libs/_references.js" />
(function () {
    var appLayout = new kendo.Layout('<div id="main-content"></div>');
    var heroLayout = new kendo.Layout('<div id="hero-main"></div>');
    var data = persisters.get("api/");

    VMfactory.setPersister(data);

    var router = new kendo.Router();
    router.route("/", function () {
        if (!data.users.currentUser()) {
            router.navigate("/login");
        } else {
            router.navigate("/home");
        }
    });
    router.route("/login", function () {
        if (data.users.currentUser()) {
            router.navigate("/");
        } else {
            viewsFactory.getTemplate('login-form')
                .then(function (viewHtml) {
                    var viewModel = VMfactory.getLoginVM(
                        function () {
                            router.navigate("/home");
                        });
                    var view = new kendo.View(viewHtml, { model: viewModel });
                    appLayout.showIn("#main-content", view);
                })
        }
    });
    router.route("/home", function () {
        if (!data.users.currentUser()) {
            router.navigate("/");
        } else {
            viewsFactory.getTemplate("game")
                .then(function (viewHtml) {
                    var viewModel = VMfactory.getGameVM(
                        function () {
                            router.navigate("/");
                        });
                    var view = new kendo.View(viewHtml, { model: viewModel });
                    appLayout.showIn("#main-content", view);
                    heroLayout.render("#main-content>div");
                    return;
                })
                .then(function () {
                    viewsFactory.getTemplate('hero-main')
                        .then(function (viewHtml) {
                            var viewModel = VMfactory.getHeroVM();
                            var view = new kendo.View(viewHtml, { model: viewModel });
                            appLayout.showIn("#hero-main", view);
                        })
                }
            );
        }
    });
    
    $(function () {
        appLayout.render('#app');

        router.start();
    })
}());