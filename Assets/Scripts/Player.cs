using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : Unit
    {
        private Dictionary<int, ItemStats> items;
        public Player(UnitStats stats, List<SkillStats> skills) : base(stats, skills)
        {
            
        }

        public ItemStats GetItem(int id)
        {
            if (items == null)
            {
                LoadItems();
            }
            return items[id];
        }

        private void LoadItems()
        {
            items = new Dictionary<int, ItemStats>();
            using (var reader = new StreamReader(Application.dataPath + Constants.DATA_PATH + Constants.ITEM_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                var records = csv.GetRecords<ItemStats>();
                foreach (ItemStats data in records)
                {
                    if (data.Type == Constants.ItemTypes.Health || data.Type == Constants.ItemTypes.Mana)
                    {
                        data.Quantity = Constants.START_POTIONS;
                    } else
                    {
                        data.Quantity = Constants.START_ELIXIR;
                    }
                    items.Add(data.Id, data);
                }
            }
        }
    }
}
