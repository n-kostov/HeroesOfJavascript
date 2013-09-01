/// <reference path="../libs/_references.js" />
window.VMfactory = (function () {
    var data = persisters.get("/api");

    function getLoginViewModel(successCallback) {
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
                    });
            },
            register: function () {
                //TODO verify data
                data.users.register(this.get('username'), this.get('displayName'), this.get('password'))
                    .then(function () {
                        //TODO create hero
                        if (successCallback) {
                            successCallback();
                        }
                    });
            },
            setAvatar: function () {
                data.users;
            }
        };
        return kendo.observable(viewModel);
    }

    function getGameViewModel(successCallback) {
        var viewModel = {
            logout: function () {
                data.users.logout()
                    .then(function () {
                        if (successCallback) {
                            successCallback();
                        }
                    });
            }
        };
        return kendo.observable(viewModel);
    }

    function getShopViewModel() {
        return data.items.get()
            .then(function (data) {
                var viewModel = {
                    items: data,
                    viewDetails: function () {

                    },
                };
                return kendo.observable(viewModel);
            });
    }

    function getActionsViewModel() {
        var viewModel = {

        };
        return kendo.observable(viewModel);
    }

    function getHeroViewModel(successCallback) {
        return data.heros.getUsersHero()
            .then(function (data) {
                var viewModel = {
                    name: data[0].name,
                    hp: data[0].hp,
                    mp: data[0].mp,
                    magicAttack: data[0].magicAttack,
                    meleAttack: data[0].meleAttack,
                    magicDefense: data[0].magicDefense,
                    meleDefense: data[0].meleDefense,
                    experience: data[0].experience,
                    level: data[0].level,
                    items: data[0].items,
                    imageUrl: "/Images/hero.png",
                    equipe: function (ev) {
                        var target = $(ev.currentTarget);
                        var id = parseInt(target.context.id);
                        var item = _.find(this.items, function (itm) {
                            return itm.itemId == id;
                        });
                        if (!target.hasClass('equiped')) {
                            $('#' + id).addClass('equiped');
                            this.magicAttack += item.magicAttack;
                            this.meleAttack += item.meleAttack;
                            this.magicDefense += item.magicDefense;
                            this.meleDefense += item.meleDefense;
                        } else {
                            $('#' + id).removeClass('equiped');
                            this.magicAttack -= item.magicAttack;
                            this.meleAttack -= item.meleAttack;
                            this.magicDefense -= item.magicDefense;
                            this.meleDefense -= item.meleDefense;
                        }
                        if (successCallback) {
                            successCallback(this);
                        }
                    }
                };
                return kendo.observable(viewModel);
            });
    }

    function getMonsterViewModel() {
        var viewModel = {

        };
        return kendo.observable(viewModel);
    }

    function getItemViewModel(id) {
        return data.items.getById(id)
            .then(function (data) {
                var viewModel = {
                    name: data[0].name,
                    description: data[0].name,
                    magicAttack: data[0].magicAttack,
                    meleAttack: data[0].meleAttack,
                    magicDefense: data[0].magicDefense,
                    meleDefense: data[0].meleDefense,
                    imageUrl: data[0].imageUrl,
                    itemCategory: data[0].itemCategory
                };

                return kendo.observable(viewModel);
            });
    }

    function getSomeViewModel() {
        var viewModel = {

        };
        return kendo.observable(viewModel);
    }

    return {
        setPersister: function (persister) {
            data = persister;
        },
        getLoginVM: getLoginViewModel,
        getGameVM: getGameViewModel,
        getShopVM: getShopViewModel,
        getActionsVM: getActionsViewModel,
        getHeroVM: getHeroViewModel,
        getMonsterVM: getMonsterViewModel,
        getItemVM: getItemViewModel
    };
}());