using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class RewardManager : MonoBehaviour
    {
        public UnitManager unitManager;
        public GameManager gameManager;

        private List<Modifier> modifiers;
        private List<Gear> gear;

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        private void LoadModifiers()
        {
            modifiers = new List<Modifier>();
            using (var reader = new StreamReader(Application.dataPath + Constants.DATA_PATH + Constants.MODIFIER_PATH))
            using (var csvUnit = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvUnit.Configuration.Delimiter = "\t";
                IEnumerable records = csvUnit.GetRecords<Modifier>();
                foreach (Modifier mod in records)
                {
                    modifiers.Add(mod);
                }
            }
        }

        private void LoadGear()
        {
            gear = new List<Gear>();
            using (var reader = new StreamReader(Application.dataPath + Constants.DATA_PATH + Constants.GEAR_PATH))
            using (var csvUnit = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvUnit.Configuration.Delimiter = "\t";
                IEnumerable records = csvUnit.GetRecords<Gear>();
                foreach (Gear gear in records)
                {
                    this.gear.Add(gear);
                }
            }
        }
    }
}
