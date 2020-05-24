using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Assets.Scripts
{
    public class UnitManager : MonoBehaviour
    {
        private Dictionary<int, UnitStats> stats;
        private Dictionary<int, SkillStats> skills;
        private Sprite[] sprites;

        public GameManager gameManager;
        public SpriteAtlas monsterAtlas;

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

        internal SkillStats LoadSkills(int id)
        {
            if (skills == null)
            {
                skills = new Dictionary<int, SkillStats>();
            }
            if (skills.ContainsKey(id))
            {
                if (skills[id].Id == id)
                {
                    return skills[id];
                }
            }
            IEnumerable<SkillStats> statEnumerable = (IEnumerable<SkillStats>)DataManager.ReadUnits(Application.dataPath + Constants.DATA_PATH + Constants.SKILL_PATH);
            foreach (SkillStats skill in statEnumerable)
            {
                if (skills.ContainsKey(skill.Id))
                {
                    continue;
                }
                skills.Add(skill.Id, skill);
                if (skill.Id == id)
                {
                    break;
                }
            }
            return skills[id];
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}