using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Store.Services.Models
{
    public class MonsterModel
    {
        public int MonsterId { get; set; }

        public string Name { get; set; }

        public int MeleAttack { get; set; }

        public int MagicAttack { get; set; }

        public int MagicDefense { get; set; }

        public int MeleDefense { get; set; }

        public int HP { get; set; }
    }

    public class CreatedMonsterModel
    {
        public int Monsterid { get; set; }

        public string Name { get; set; }
    }
}