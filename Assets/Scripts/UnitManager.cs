using System.Collections;
using System.Collections.Generic;
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
        public Monster[] monsters = new Monster[5];

        // Start is called before the first frame update
        void Start()
        {
            sprites = new Sprite[2000];
            Sprite testSprite = monsterAtlas.GetSprite("cherub");
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
            IEnumerable<UnitStats> statEnumerable = (IEnumerable<UnitStats>)DataManager.ReadUnits(Application.dataPath + Constants.DATA_PATH + Constants.UNIT_PATH);
            foreach (UnitStats stat in statEnumerable)
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
            return stats[id];
        }

        internal void LoadSkills()
        {
            IEnumerable<SkillStats> skillEnumerable = (IEnumerable<SkillStats>)DataManager.ReadSkills(Application.dataPath + Constants.DATA_PATH + Constants.SKILL_PATH);
            foreach (SkillStats data in skillEnumerable)
            {
                skills.Add(data);
            }
        }

        internal SkillStats GetSkill(int id)
        {
            return skills[id];
        }

        internal void LoadMonsterData()
        {
            IEnumerable<MonsterData> monsterEnumerable = (IEnumerable<MonsterData>)DataManager.ReadMonsters(Application.dataPath + Constants.DATA_PATH + Constants.MONSTER_PATH);
            foreach (MonsterData data in monsterEnumerable)
            {
                monsterData.Add(data);
            }
        }

        internal MonsterData GetMonsterData(int id)
        {
            return monsterData[id];
        }

        internal void LinkMonster(int id)
        {
            monsterData[id].Sprite = monsterAtlas.GetSprite(monsterData[id].SpriteName);
            monsterData[id].Unit = LoadStats(monsterData[id].UnitId);
        }

        internal SkillStats[] ChooseSkills(MonsterData monster)
        {
            SkillStats[] chosenSkills = new SkillStats[monster.SkillTypeFull.Length];
            int minValue = monster.Value - (int)(monster.Value * Constants.COST_SKILL_VARIANCE);
            int maxValue = monster.Value + (int)(monster.Value * Constants.COST_SKILL_VARIANCE);
            List<SkillStats> match;
            for (int i=0; i<monster.SkillTypeFull.Length; i++)
            {
                match = FindSkills(minValue, maxValue, monster.SkillTypeFull[i]);
                int chosen = Damage.RandomInt(0, match.Count - 1);
                chosenSkills[i] = match[chosen];
            }
            return chosenSkills;
        }

        internal List<SkillStats> FindSkills(int minValue, int maxValue, Constants.SkillTypes type)
        {
            List<SkillStats> foundSkills = skills.FindAll(x => x.Value >= minValue && x.Value <= maxValue && x.SkillType == type);
            return foundSkills;
        } 

        // Update is called once per frame
        void Update()
        {

        }
    }
}