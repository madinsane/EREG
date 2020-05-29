﻿using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            List<SkillStats> match;
            //Add forced skills
            chosenSkills[0] = GetSkill(0);
            for (int i=Constants.FORCED_SKILLS; i<chosenSkills.Length; i++)
            {
                match = FindSkills(gameManager.Level, monster.SkillTypeFull[i-Constants.FORCED_SKILLS]);
                if (match.Count == 0)
                {
                    continue;
                }
                int chosen = Damage.RandomInt(0, match.Count - 1);
                chosenSkills[i] = match[chosen];
            }
            return chosenSkills;
        }

        internal List<SkillStats> FindSkills(int level, Constants.SkillTypes type)
        {
            if (skills == null)
            {
                LoadSkills();
            }
            List<SkillStats> foundSkills;
            if (level < 5)
            {
                foundSkills = skills.FindAll(x => x.RLvl <= level && x.SkillType == type);
            }
            else
            {
                foundSkills = skills.FindAll(x => x.RLvl + Constants.MONSTER_SKILL_RLVL_PENALTY <= level && x.SkillType == type);
            }
            if (foundSkills.Count == 0)
            {
                return foundSkills;
            }
            int maxRlvl = foundSkills.Max(x => x.RLvl);
            foundSkills.RemoveAll(x => x.RLvl < maxRlvl - Constants.SKILL_RLVL_LENIANCE);
            return foundSkills;
        } 

        internal void MakeMonster(int position, MonsterData monster, List<SkillStats> monsterSkills)
        {
            monsterSkills.RemoveAll(x => x == null);
            LinkMonster(monster.Id);
            monsters[position].ChangeMonster(ScaleMonster(monster.Unit, monster.StatMulti), monsterSkills, monster.Sprite);
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

        internal UnitStats ScaleMonster(UnitStats unitToScale, float statMulti)
        {
            //Get base
            UnitStats baseStats = LoadStats(1).Copy();
            //Get scale
            UnitStats scaling = LoadStats(2);
            //Apply Scale
            baseStats.MaxHealth = ScaleStat(baseStats.MaxHealth, scaling.MaxHealth, unitToScale.MaxHealth, statMulti);
            baseStats.MaxMana = ScaleStat(baseStats.MaxMana, scaling.MaxMana, unitToScale.MaxMana, statMulti);
            baseStats.AttackPower = ScaleStat(baseStats.AttackPower, scaling.AttackPower, unitToScale.AttackPower, statMulti);
            baseStats.MagicPower = ScaleStat(baseStats.MagicPower, scaling.MagicPower, unitToScale.MagicPower, statMulti);
            baseStats.AttackDefense = ScaleStat(baseStats.AttackDefense, scaling.AttackDefense, unitToScale.AttackDefense, statMulti);
            baseStats.MagicDefense = ScaleStat(baseStats.MagicDefense, scaling.MagicDefense, unitToScale.MagicDefense, statMulti);
            baseStats.Accuracy = ScaleStat(baseStats.Accuracy, scaling.Accuracy, unitToScale.Accuracy, statMulti);
            baseStats.Evasion = ScaleStat(baseStats.Evasion, scaling.Evasion, unitToScale.Evasion, statMulti);
            baseStats.CritChance = ScaleStat(baseStats.CritChance, scaling.CritChance, unitToScale.CritChance, statMulti);
            baseStats.CritMulti = ScaleStat(baseStats.CritMulti, scaling.CritMulti, unitToScale.CritMulti, 0);
            baseStats.IncCritChance = ScaleStat(baseStats.IncCritChance, scaling.IncCritChance, unitToScale.IncCritChance, 0);
            baseStats.TypeStatusChance = ScaleStat(baseStats.TypeStatusChance, scaling.TypeStatusChance, unitToScale.TypeStatusChance, statMulti);
            baseStats.MentalStatusChance = ScaleStat(baseStats.MentalStatusChance, scaling.MentalStatusChance, unitToScale.MentalStatusChance, statMulti);
            baseStats.StatusPower = ScaleStat(baseStats.StatusPower, scaling.StatusPower, unitToScale.StatusPower, 0);
            baseStats.IncTypeStatus = ScaleStat(baseStats.IncTypeStatus, scaling.IncTypeStatus, unitToScale.IncTypeStatus, 0);
            baseStats.IncMentalStatus = ScaleStat(baseStats.IncMentalStatus, scaling.IncMentalStatus, unitToScale.IncMentalStatus, 0);
            return baseStats;
        }

        private int ScaleStat(int statBase, int statScaling, int unitScaling, float statMulti)
        {
            statBase = (int)(statBase * (statMulti * (((float)statScaling / 100) * gameManager.Level)) * ((float)unitScaling / 100));
            return statBase;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}