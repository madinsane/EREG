using CsvHelper;
using UnityEngine;
using System.Globalization;
using System.IO;
using System.Collections;

namespace Assets.Scripts
{
    public static class DataManager
    {
        public static IEnumerable ReadUnits(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                csv.Configuration.Delimiter = "\t";
                IEnumerable records = csv.GetRecords<UnitStats>();
                return records;
            }
        }

        public static IEnumerable ReadSkills(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                csv.Configuration.Delimiter = "\t";
                IEnumerable records = csv.GetRecords<SkillStats>();
                return records;
            }
        }
    }
}
