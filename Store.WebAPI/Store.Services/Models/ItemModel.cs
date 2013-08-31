using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using System.Web;

namespace Store.Services.Models
{
    [DataContract]
    public class ItemModel
    {
        public static Expression<Func<Item, ItemModel>> FromItem
        {
            get
            {
                return x => new ItemModel
                {
                    ItemId = x.ItemId,
                    Name = x.Name,
                    MeleAttack = x.MeleAttack,
                    MagicDefense = x.MagicDefense,
                    MeleDefense = x.MeleDefense,
                    MagicAttack = x.MagicAttack,
                    ItemCategory = x.ItemCategory.Name,
                    Description = x.Description
                };
            }
        }

        [DataMember(Name = "itemId")]
        public int ItemId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "magicAttack")]
        public int MagicAttack { get; set; }

        [DataMember(Name = "meleAttack")]
        public int MeleAttack { get; set; }

        [DataMember(Name = "magicDefense")]
        public int MagicDefense { get; set; }

        [DataMember(Name = "meleDefense")]
        public int MeleDefense { get; set; }

        [DataMember(Name = "itemCategory")]
        public string ItemCategory { get; set; }
    }

    [DataContract]
    public class UpdatingItemModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "magicAttack")]
        public int MagicAttack { get; set; }

        [DataMember(Name = "meleAttack")]
        public int MeleAttack { get; set; }

        [DataMember(Name = "magicDefense")]
        public int MagicDefense { get; set; }

        [DataMember(Name = "meleDefense")]
        public int MeleDefense { get; set; }

        [DataMember(Name = "itemCategory")]
        public string ItemCategory { get; set; }
    }
}