/// <reference path="../_references.js" />

window.controllers = (function () {
    var baseUrl = "http://localhost:44998/api/";

    var nickname = localStorage.getItem("nickname");
    var sessionKey = localStorage.getItem("sessionKey");
    function saveUserData(userData) {
        localStorage.setItem("nickname", userData.displayName);
        localStorage.setItem("sessionKey", userData.sessionKey);
        nickname = userData.displayName;
        sessionKey = userData.sessionKey;
    }
    function clearUserData() {
        localStorage.removeItem("nickname");
        localStorage.removeItem("sessionKey");
        nickname = "";
        sessionKey = "";
    }
    function isUserLoggedIn() {
        var isLoggedIn = nickname != null && sessionKey != null;
        return isLoggedIn;
    }

    function AdminController($scope, $http, $location) {
        $scope.user = {
            username: "",
            displayName: "",
            authCode: ""
        };

        $scope.loginAdmin = function () {
            console.log($scope.user);
            var userToLogin = {
                username: $scope.user.username,
                authCode: CryptoJS.SHA1($scope.user.authCode).toString()
            };
            $http.post(baseUrl + "admins/login", userToLogin)
                .success(function (data) {
                    saveUserData(data);
                    $scope.user.authCode = "";
                }).error(function (err) {
                    $location.path("#/admin/login");
                    alert(err.Message);
                });
        }

        $scope.logoutAdmin = function () {
            debugger
            var config = { headers: { "X-sessionKey": sessionKey } }
            $http.put(baseUrl + "admins/logout", {}, config)
                .success(function () {
                $scope.user = {
                    username: "",
                    displayName: "",
                    authCode: ""
                };
                clearUserData();
            }).error(function (err) {
                alert(err.Message);
            });
        }

        $scope.registerAdmin = function () {
            var userToRegister = {
                username: $scope.user.username,
                displayName: $scope.user.displayName,
                authCode: CryptoJS.SHA1($scope.user.authCode).toString()
            };
            $http.post(baseUrl + "admins/register", userToRegister)
                .success(function (data) {
                    saveUserData(data);
                    $scope.user.authCode = "";
                });
        }
    }

    function HomeController($location) {
        if (!isUserLoggedIn()) {
            $location.path("#/admin/login");
        }
    }

    function UsersController($scope, $http) {
        $scope.users = [];

        $http.get(baseUrl + "users").success(function (data) {
            for (var i in data) {
                $scope.users.push(data[i]);
            }
        });
    }

    return {
        getAdminController: AdminController,
        getUsersController: UsersController
    };
})();