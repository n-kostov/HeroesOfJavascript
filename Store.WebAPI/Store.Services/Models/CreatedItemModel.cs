using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Store.Services.Models;
using System.Runtime.Serialization;

namespace Store.Services.Controllers
{
    [DataContract]
    public class CreatedItemModel
    {
        [DataMember(Name = "itemId")]
        public int ItemId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
