/// <reference path="libs/_references.js" />
(function () {
    var appLayout = new kendo.Layout('<div id="main-nav"></div><aside id="shop"></aside><aside id="actions"></aside><div id="main-view"></div>');
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
            viewsFactory.getView('login-form')
                .then(function (viewHtml) {
                    var viewModel = VMfactory.getLoginVM(
                        function () {
                            router.navigate("/home");
                        });
                    var view = new kendo.View(viewHtml, { model: viewModel });
                    appLayout.showIn("#main-nav", view);
                }, function (err) {
                    alert(err);
                });
        }
    });
    router.route("/home", function () {
        if (!data.users.currentUser()) {
            router.navigate("/login");
        } else {
            viewsFactory.getView("home")
                .then(function (viewHtml) {
                    var viewModel = VMfactory.getGameVM(
                        function () {
                            $("#app").children().children().not("#main-nav").html("");
                            router.navigate("/login");
                        });
                    var view = new kendo.View(viewHtml, { model: viewModel });
                    appLayout.showIn("#main-nav", view);
                })
                .then(function () {
                    viewsFactory.getView("hero")
                    .then(function (viewHtml) {
                        VMfactory.getHeroVM(function (vm) {
                            var view = new kendo.View(viewHtml, { model: vm });
                            appLayout.showIn("#main-view", view);
                        })
                        .then(function (vm) {
                            var view = new kendo.View(viewHtml, { model: vm });
                            appLayout.showIn("#main-view", view);
                        }, function (err) {
                            alert(err);
                        });
                    });
                })
                .then(function () {
                    viewsFactory.getView("shop")
                    .then(function (viewHtml) {
                        VMfactory.getShopVM()
                        .then(function (vm) {
                            var view = new kendo.View(viewHtml, { model: vm });
                            appLayout.showIn("#shop", view);
                        }, function (err) {
                            alert(err);
                        });
                    });
                })
                .then(function () {
                    viewsFactory.getView("actions")
                    .then(function (viewHtml) {
                        var viewModel = VMfactory.getActionsVM();
                        var view = new kendo.View(viewHtml, { model: viewModel });
                        appLayout.showIn("#actions", view);
                    });
                });
        }
    });
    router.route("/shop/:id", function (id) {
        if (data.users.currentUser()) {
            router.navigate("/");
        } else {
            viewsFactory.getView('item')
                .then(function (viewHtml) {
                    VMfactory.getItemVM(id)
                        .then(function (vm) {
                            var view = new kendo.View(viewHtml, { model: vm });
                            appLayout.showIn("#main-view", view);
                        },
                        function (err) {
                            alert(err);
                        });
            });
        }
    });

    $(function () {
        appLayout.render($('#app'));
        router.start();
    });
}());