using CsvHelper;
using UnityEngine;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages loading data files
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// Reads the units file
        /// </summary>
        /// <param name="path">File path to read from</param>
        /// <returns>Records from file</returns>
        public static IEnumerable ReadUnits(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csvUnit = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvUnit.Configuration.Delimiter = "\t";
                IEnumerable records = csvUnit.GetRecords<UnitStats>();
                return records;
            }
        }

        /// <summary>
        /// Reads the skills file
        /// </summary>
        /// <param name="path">File path to read from</param>
        /// <returns>Records from file</returns>
        public static IEnumerable ReadSkills(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csvSkill = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvSkill.Configuration.Delimiter = "\t";
                IEnumerable records = csvSkill.GetRecords<SkillStats>();
                return records;
            }
        }

        /// <summary>
        /// Reads the monsters file
        /// </summary>
        /// <param name="path">File path to read from</param>
        /// <returns>Records from file</returns>
        public static IEnumerable<MonsterData> ReadMonsters(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csvMonster = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvMonster.Configuration.Delimiter = "\t";
                var records = csvMonster.GetRecords<MonsterData>();
                return records;
            }
        }
    }
}
