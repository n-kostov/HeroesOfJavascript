﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Store.Data;
using Store.Models;
using Store.Services.Attributes;
using Store.Services.Models;

namespace Store.Services.Controllers
{
    public class UsersController : BaseApiController
    {
        private const int MinUsernameLength = 6;

        private const int MaxUsernameLength = 30;

        private const string ValidUsernameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";

        private const string ValidNicknameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_. -";

        private const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";

        private const int SessionKeyLength = 50;

        private const int Sha1Length = 40;

        private static readonly Random Rand = new Random();

        public UsersController()
        {
        }

        [ActionName("register")]
        public HttpResponseMessage PostRegisterUser(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = new StoreContext();
                    using (context)
                    {
                        this.ValidateUsername(model.Username);
                        this.ValidateNickname(model.DisplayName);
                        this.ValidateAuthCode(model.AuthCode);
                        var usernameToLower = model.Username.ToLower();
                        var displayNameToLower = model.DisplayName.ToLower();
                        var user = context.Users.FirstOrDefault(
                            usr => usr.Username == usernameToLower
                            || usr.DisplayName.ToLower() == displayNameToLower);

                        if (user != null)
                        {
                            throw new InvalidOperationException("Users already exists!");
                        }

                        user = new User()
                        {
                            Username = usernameToLower,
                            DisplayName = model.DisplayName,
                            AuthCode = model.AuthCode
                        };

                        context.Users.Add(user);
                        context.SaveChanges();

                        user.SessionKey = this.GenerateSessionKey(user.UserId);
                        context.SaveChanges();

                        var loggedModel = new LoggedUserModel()
                        {
                            DisplayName = user.DisplayName,
                            SessionKey = user.SessionKey
                        };

                        var response =
                            this.Request.CreateResponse(HttpStatusCode.Created, loggedModel);
                        return response;
                    }
                });

            return responseMsg;
        }

        [ActionName("login")]
        public HttpResponseMessage PostLoginUser(UserLoginModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var context = new StoreContext();
                  using (context)
                  {
                      this.ValidateUsername(model.Username);
                      this.ValidateAuthCode(model.AuthCode);
                      var usernameToLower = model.Username.ToLower();
                      var user = context.Users.FirstOrDefault(
                          usr => usr.Username == usernameToLower
                          && usr.AuthCode == model.AuthCode);

                      if (user == null)
                      {
                          throw new InvalidOperationException("Invalid username or password");
                      }

                      if (user.SessionKey == null)
                      {
                          user.SessionKey = this.GenerateSessionKey(user.UserId);
                          context.SaveChanges();
                      }

                      var loggedModel = new LoggedUserModel()
                      {
                          DisplayName = user.DisplayName,
                          SessionKey = user.SessionKey
                      };

                      var response = this.Request.CreateResponse(HttpStatusCode.Created, loggedModel);
                      return response;
                  }
              });

            return responseMsg;
        }

        [ActionName("logout")]
        public HttpResponseMessage PutLogoutUser(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  ValidateSessionKey(sessionKey);
                  var context = new StoreContext();
                  using (context)
                  {
                      var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                      if (user == null)
                      {
                          throw new ArgumentOutOfRangeException("Invalid session key!");
                      }

                      user.SessionKey = null;
                      context.SaveChanges();

                      var response =
                          this.Request.CreateResponse(HttpStatusCode.OK);
                      return response;
                  }
              });

            return responseMsg;
        }

        [ActionName("byId")]
        public IQueryable<UserAdminModel> GetUserById(int id, [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var response = this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new StoreContext();

                var admin = context.Admins.FirstOrDefault(a => a.SessionKey == sessionKey);
                if (admin == null)
                {
                    throw new InvalidOperationException("Invalid session key");
                }

                var user =
                    (from u in context.Users
                     where u.UserId == id
                     join h in context.Heros on u.Hero.HeroId equals h.HeroId
                     select new UserAdminModel
                     {
                         UserId = u.UserId,
                         Username = u.Username,
                         DisplayName = u.DisplayName,
                         Gold = u.Gold,
                         Hero = new HeroModel
                         {
                             Name = u.Hero.Name,
                             HP = u.Hero.HP,
                             MP = u.Hero.MP,
                             MagicAttack = u.Hero.MagicAttack,
                             MeleAttack = u.Hero.MeleAttack,
                             MagicDefense = u.Hero.MagicDefense,
                             MeleDefense = u.Hero.MeleDefense,
                             Experience = u.Hero.Experience,
                             Level = u.Hero.Experience,
                             Items =
                                (from i in u.Hero.Items
                                 join m in context.Items on i.ItemId equals m.ItemId
                                 select new ItemModel
                                 {
                                     Name = i.Name,
                                     Description = i.Description,
                                     MagicAttack = i.MagicAttack,
                                     MeleAttack = i.MeleAttack,
                                     MagicDefense = i.MagicDefense,
                                     MeleDefense = i.MeleDefense,
                                     ItemCategory = i.ItemCategory.Name
                                 }).AsQueryable()
                         }
                     });
                return user;
            });

            return response;
        }

        [HttpGet]
        public IQueryable<UserAdminSimpleModel> GetAll([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var response = this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new StoreContext();

                var admin = context.Admins.FirstOrDefault(a => a.SessionKey == sessionKey);
                if (admin == null)
                {
                    throw new InvalidOperationException("Invalid session key");
                }

                var users =
                    from u in context.Users
                    select new UserAdminSimpleModel
                    {
                        UserId = u.UserId,
                        Username = u.Username
                    };

                return users;
            });

            return response;
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = Rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }

            return skeyBuilder.ToString();
        }

        private void ValidateSessionKey(string sessionKey)
        {
            if (sessionKey == null || sessionKey.Length != SessionKeyLength)
            {
                throw new ArgumentOutOfRangeException("Invalid session key!");
            }
        }

        private void ValidateAuthCode(string authCode)
        {
            if (authCode == null || authCode.Length != Sha1Length)
            {
                throw new ArgumentOutOfRangeException("Password should be encrypted");
            }
        }

        private void ValidateNickname(string nickname)
        {
            if (nickname == null)
            {
                throw new ArgumentNullException("Nickname cannot be null");
            }
            else if (nickname.Length < MinUsernameLength)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Nickname must be at least {0} characters long",
                    MinUsernameLength));
            }
            else if (nickname.Length > MaxUsernameLength)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Nickname must be less than {0} characters long",
                    MaxUsernameLength));
            }
            else if (nickname.Any(ch => !ValidNicknameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "Nickname must contain only Latin letters, digits, ., _,  , -");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("Username cannot be null");
            }
            else if (username.Length < MinUsernameLength)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Username must be at least {0} characters long",
                    MinUsernameLength));
            }
            else if (username.Length > MaxUsernameLength)
            {
                throw new ArgumentOutOfRangeException(string.Format(
                    "Username must be less than {0} characters long",
                    MaxUsernameLength));
            }
            else if (username.Any(ch => !ValidUsernameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "Username must contain only Latin letters, digits .,_");
            }
        }
    }
}
