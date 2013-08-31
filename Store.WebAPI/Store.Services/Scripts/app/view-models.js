/// <reference path="../libs/_references.js" />
window.VMfactory = (function () {
    var data = persisters.get("/api");

    function getLoginVeiwModel(successCallback) {
        var viewModel = {
            username: "gargamel",
            password: "gargamel",
            login: function () {
                //TODO verify data
                data.users.login(this.get('username'), this.get('password'))
                    .then(function () {
                        if (successCallback) {
                            successCallback();
                        }
                    })
            },
            register: function () {
                //TODO verify data
                data.users.register(this.get('username'), this.get('displayName'), this.get('password'))
                    .then(function () {
                        if (successCallback) {
                            successCallback();
                        }
                    })
            },
            setAvatar: function () {
                data.users
            }
        };
        return kendo.observable(viewModel);
    };

    function getGameViewModel(successCallback) {
        var viewModel = {
            logout: function () {
                data.users.logout()
                    .then(function () {
                        if (successCallback) {
                            successCallback();
                        }
                    })
            }
        };
        return kendo.observable(viewModel);
    };

    function getHeroViewModel() {
        data.heros.get()
        var viewModel = {
            name: "",
            hp: 0,
            mp: 0,
            magicAttack: 0,
            meleAttack: 0,
            magicDefense: 0,
            meleDefense: 0,
            experience: 0,
            level: 0,
            items: "",
        };
        return kendo.observable(viewModel);
    };

    function getMonsterVeiwModel() {
        var viewModel = {

        };
        return kendo.observable(viewModel);
    }

    function getSomeVeiwModel() {
        var viewModel = {

        };
        return kendo.observable(viewModel);
    }

    return {
        setPersister: function (persister) {
            data = persister;
        },
        getLoginVM: getLoginVeiwModel,
        getGameVM: getGameViewModel,
        getHeroVM: getHeroViewModel,
        getMonsterVM: getMonsterVeiwModel,
    }
}());