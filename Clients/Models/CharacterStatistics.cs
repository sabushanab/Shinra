using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Clients.Models
{
    public class CharacterStatistics
    {
        public Character character { get; set; }
        public List<Category> categories { get; set; } = new List<Category>();
    }

    public class Character
    {
        public string name { get; set; }
        public Realm realm { get; set; }
    }

    public class Realm
    {
        public string name { get; set; }
        public string slug { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Statistic> statistics { get; set; }
        public List<Category> sub_categories { get; set; }
    }

    public class Statistic
    {
        public int id { get; set; }
        public string name { get; set; }
        public long last_updated_timestamp { get; set; }
        public decimal quantity { get; set; }
    }
}
