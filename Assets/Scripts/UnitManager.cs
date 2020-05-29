using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace Assets.Scripts
{
    public class UnitManager : MonoBehaviour
    {
        private Dictionary<int, UnitStats> stats;
        private List<SkillStats> skills;
        private List<MonsterData> monsterData;
        private Sprite[] sprites;

        public GameManager gameManager;
        public SpriteAtlas monsterAtlas;
        public Monster[] monsters = new Monster[Constants.MAX_ENEMIES];

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Done");
        }

        internal UnitStats LoadStats(int id)
        {
            if (stats == null)
            {
                stats = new Dictionary<int, UnitStats>();
            }
            if (stats.ContainsKey(id))
            {
                if (stats[id].Id == id)
                {
                    return stats[id];
                }
            }
            using (var reader = new StreamReader(Application.dataPath + Constants.DATA_PATH + Constants.UNIT_PATH))
            using (var csvUnit = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvUnit.Configuration.Delimiter = "\t";
                IEnumerable records = csvUnit.GetRecords<UnitStats>();
                foreach (UnitStats stat in records)
                {
                    if (stats.ContainsKey(stat.Id))
                    {
                        continue;
                    }
                    stats.Add(stat.Id, stat);
                    if (stat.Id == id)
                    {
                        break;
                    }
                }
            }
            return stats[id];
        }

        internal void LoadSkills()
        {
            skills = new List<SkillStats>();
            using (var reader = new StreamReader(Application.dataPath + Constants.DATA_PATH + Constants.SKILL_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                var records = csv.GetRecords<SkillStats>();
                foreach (SkillStats data in records)
                {
                    skills.Add(data);
                }
            }
        }

        internal SkillStats GetSkill(int id)
        {
            if (skills == null)
            {
                LoadSkills();
            }
            return skills[id];
        }

        internal void LoadMonsterData()
        {
            monsterData = new List<MonsterData>();
            using (var reader = new StreamReader(Application.dataPath + Constants.DATA_PATH + Constants.MONSTER_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                var records = csv.GetRecords<MonsterData>();
                foreach (MonsterData data in records)
                {
                    monsterData.Add(data);
                }
            }
        }

        internal MonsterData GetMonsterData(int id)
        {
            if (monsterData == null)
            {
                LoadMonsterData();
            }
            return monsterData[id];
        }

        internal void LinkMonster(int id)
        {
            monsterData[id].Sprite = monsterAtlas.GetSprite(monsterData[id].SpriteName);
            monsterData[id].Unit = LoadStats(monsterData[id].UnitId);
        }

        internal SkillStats[] ChooseSkills(MonsterData monster)
        {
            SkillStats[] chosenSkills = new SkillStats[monster.SkillTypeFull.Length+Constants.FORCED_SKILLS];
            int minValue = monster.Value - (int)(monster.Value * Constants.COST_SKILL_VARIANCE);
            int maxValue = monster.Value + (int)(monster.Value * Constants.COST_SKILL_VARIANCE);
            List<SkillStats> match;
            //Add forced skills
            chosenSkills[0] = GetSkill(0);
            for (int i=Constants.FORCED_SKILLS; i<chosenSkills.Length; i++)
            {
                match = FindSkills(minValue, maxValue, monster.SkillTypeFull[i-Constants.FORCED_SKILLS]);
                if (match.Count == 0)
                {
                    continue;
                }
                int chosen = Damage.RandomInt(0, match.Count - 1);
                chosenSkills[i] = match[chosen];
            }
            return chosenSkills;
        }

        internal List<SkillStats> FindSkills(int minValue, int maxValue, Constants.SkillTypes type)
        {
            if (skills == null)
            {
                LoadSkills();
            }
            List<SkillStats> foundSkills = skills.FindAll(x => x.Value >= minValue && x.Value <= maxValue && x.SkillType == type);
            return foundSkills;
        } 

        internal void MakeMonster(int position, MonsterData monster, List<SkillStats> monsterSkills)
        {
            monsterSkills.RemoveAll(x => x == null);
            LinkMonster(monster.Id);
            monsters[position].ChangeMonster(monster.Unit.Copy(), monsterSkills, monster.Sprite);
        }

        internal MonsterData GetMonsterByName(string name)
        {
            if (monsterData == null)
            {
                LoadMonsterData();
            }
            return monsterData.Find(x => x.Name.Equals(name));
        }

        internal void ClearMonsters()
        {
            for (int i=0; i<monsters.Length; i++)
            {
                monsters[i].Die();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}