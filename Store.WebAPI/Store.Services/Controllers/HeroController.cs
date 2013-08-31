using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Store.Services.Models;
using Store.Data;
using Store.Models;
using System.Web.Http.ValueProviders;
using Store.Services.Attributes;

namespace Store.Services.Controllers
{
    public class HeroController : BaseApiController
    {
        public HttpResponseMessage PostCreateHero(HeroCreateModel heroModel,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var response = this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new StoreContext();

                var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new InvalidOperationException("Invalid session key");
                }

                Hero newHero = new Hero
                {
                    Name = heroModel.Name,
                    HP = heroModel.HP,
                    MP = heroModel.MP,
                    MagicAttack = heroModel.MagicAttack,
                    MeleAttack = heroModel.MeleAttack,
                    MagicDefense = heroModel.MagicDefense,
                    MeleDefense = heroModel.MeleDefense,
                    Experience = heroModel.Experience,
                    Level = heroModel.Level,
                    User = user
                };

                context.Heros.Add(newHero);
                context.SaveChanges();

                user.Hero = newHero;
                context.SaveChanges();

                var entity = new HeroCreateReturnModel
                {
                    ID = newHero.HeroId,
                    Name = newHero.Name
                };

                var responseMsg = this.Request.CreateResponse(HttpStatusCode.Created, entity);
                return responseMsg;
            });

            return response;
        }

        public HttpResponseMessage PutUpdateHero(HeroModel heroModel,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                         () =>
                         {
                             var context = new StoreContext();
                             using (context)
                             {
                                 var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                                 if (user == null)
                                 {
                                     throw new ArgumentOutOfRangeException("Invalid session key!");
                                 }

                                 var hero = user.Hero;
                                 if (hero == null)
                                 {
                                     throw new ArgumentNullException("hero", "Cannot update non-existing hero!");
                                 }

                                 UpdateHero(hero, heroModel, context);
                                 context.SaveChanges();

                                 var response =
                                     this.Request.CreateResponse(HttpStatusCode.OK);
                                 return response;
                             }
                         });

            return responseMsg;
        }

        private void UpdateHero(Hero hero, HeroModel model, StoreContext context)
        {
            if (model.Name != null)
            {
                hero.Name = model.Name;
            }

            if (model.Items != null)
            {
                foreach (var i in model.Items)
                {
                    var item = context.Items.FirstOrDefault(x => x.Name == i.Name);
                    if (item == null)
                    {
                        throw new ArgumentNullException("item", "Cannot use invalid item!");
                    }

                    hero.Items.Add(item);
                }
            }

            hero.MagicAttack = model.MagicAttack;
            hero.MagicDefense = model.MagicDefense;
            hero.MeleAttack = model.MeleAttack;
            hero.MeleDefense = model.MeleDefense;
            hero.Experience = model.Experience;
            hero.HP = model.HP;
            hero.MP = model.MP;
            hero.Level = model.Level;
        }
    }
}
