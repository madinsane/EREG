﻿using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public class UnitManager : MonoBehaviour
    {
        private Dictionary<int, UnitStats> stats;
        private List<SkillStats> skills;
        private List<MonsterData> monsterData;
        private Sprite[] sprites;
        private List<SkillStats> baseSkills;

        public Player player;
        public GameManager gameManager;
        public SpriteAtlas monsterAtlas;
        public Monster[] monsters = new Monster[Constants.MAX_ENEMIES];
        public SpriteRenderer[] targets;
        public Log log;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI mpText;
        public ResourceDisplay healthGlobe;
        public ResourceDisplay manaGlobe;

        public enum Turns
        {
            Player, Monster1, Monster2, Monster3, Monster4, Monster5
        }

        public Turns Turn { get; set; }
        private Dictionary<int, int> targetRef;
        private bool isTargeting;
        private int target;
        private SkillStats currentSkill;
        public bool IsCasting { get; set; }
        public int ActiveMonsters { get; set; }
        private List<KeyValuePair<Turns, int>> turnCounters;

        // Start is called before the first frame update
        void Start()
        {
            isTargeting = false;
            IsCasting = false;
        }

        internal void ChangeTargeting(bool isTargeting)
        {
            this.isTargeting = isTargeting;
            HideTargets();
        }

        internal void InitTargetRef()
        {
            targetRef = new Dictionary<int, int>
            {
                { 0, 4 },
                { 1, 2 },
                { 2, 0 },
                { 3, 1 },
                { 4, 3 }
            };
        }

        internal void InitPlayer()
        {
            if (baseSkills == null)
            {
                baseSkills = new List<SkillStats>
                {
                    GetSkill(1),
                    GetSkill(9),
                    GetSkill(17),
                    GetSkill(25)
                };
            }
            player.ChangeUnit(LoadStats(0), baseSkills, "You");
        }

        internal void InitTurns()
        {
            if (turnCounters == null)
            {
                turnCounters = new List<KeyValuePair<Turns, int>>();
            }
            foreach (Monster monster in monsters)
            {
                monster.TurnCounter = 0;
            }
            player.TurnCounter = player.Stats.Speed;
            Turn = Turns.Player;
        }

        internal void AdvanceTurn()
        {
            turnCounters.Clear();
            turnCounters.Add(new KeyValuePair<Turns, int>(Turns.Player, player.TurnCounter));
            for (int i=0; i<monsters.Length; i++)
            {
                if (monsters[i].enabled)
                {
                    turnCounters.Add(new KeyValuePair<Turns, int>((Turns)i+1, monsters[i].TurnCounter));
                }
            }
            turnCounters.Sort((x, y) => x.Value.CompareTo(y.Value));
            Turn = turnCounters.First().Key;
            Debug.Log(Turn);
            if (Turn == Turns.Player)
            {
                player.TurnCounter += player.Stats.Speed;
            } else
            {
                monsters[(int)Turn - 1].TurnCounter += monsters[(int)Turn - 1].Stats.Speed;
                MonsterCast((int)Turn - 1);
            }
        }

        internal void MonsterCast(int pos)
        {
            int castChoice = Damage.RandomInt(0, monsters[pos].Skills.Count-1);
            CastSkill(monsters[pos].Skills[castChoice], monsters[pos]);
        }

        internal void UpdateTarget(int direction, int counter = 0)
        {
            if (targetRef == null)
            {
                InitTargetRef();
            }
            if (target == Constants.MAX_ENEMIES)
            {
                for (int i=0; i<targets.Length; i++)
                {
                    if (monsters[i].enabled == true)
                    {
                        targets[i].enabled = true;
                    }
                }
                return;
            }
            foreach (SpriteRenderer renderer in targets)
            {
                renderer.enabled = false;
            }
            if (monsters[targetRef[target]].enabled == true)
            {
                targets[targetRef[target]].enabled = true;
            } else
            {
                if (counter == Constants.MAX_ENEMIES - 1)
                {
                    return;
                }
                if (target + direction < 0)
                {
                    target = Constants.MAX_ENEMIES - 1;
                } else if (target + direction >= Constants.MAX_ENEMIES)
                {
                    target = 0;
                } else
                {
                    target += direction;
                }
                UpdateTarget(direction, counter++);
            }
        }

        public void HideTargets()
        {
            foreach (SpriteRenderer renderer in targets)
            {
                renderer.enabled = false;
                target = 2;
            }
        }

        public void ShowTarget(int pos)
        {
            if (isTargeting)
            {
                if (targetRef == null)
                {
                    InitTargetRef();
                }
                if (monsters[targetRef[pos]].enabled == true && target != Constants.MAX_ENEMIES)
                {
                    targets[targetRef[pos]].enabled = true;
                }
            }
        }

        internal UnitStats GetPlayerStats()
        {
            return player.Stats;
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
            monsters[position].ChangeMonster(ScaleMonster(monster.Unit, monster.StatMulti), monsterSkills, monster.Sprite, monster.NameStr);
            ActiveMonsters++;
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
            ActiveMonsters = 0;
            HideTargets();
        }

        internal UnitStats ScaleMonster(UnitStats unitToScale, float statMulti)
        {
            //Get base
            UnitStats baseStats = LoadStats(1).Copy();
            //Get scale
            UnitStats scaling = LoadStats(2);
            baseStats.Id = unitToScale.Id;
            baseStats.Name = unitToScale.Name;
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
            baseStats.CritMulti = ScaleStat(baseStats.CritMulti, scaling.CritMulti, unitToScale.CritMulti, 1);
            baseStats.IncCritChance = ScaleStat(baseStats.IncCritChance, scaling.IncCritChance, unitToScale.IncCritChance, 1);
            baseStats.TypeStatusChance = ScaleStat(baseStats.TypeStatusChance, scaling.TypeStatusChance, unitToScale.TypeStatusChance, statMulti);
            baseStats.MentalStatusChance = ScaleStat(baseStats.MentalStatusChance, scaling.MentalStatusChance, unitToScale.MentalStatusChance, statMulti);
            baseStats.StatusPower = ScaleStat(baseStats.StatusPower, scaling.StatusPower, unitToScale.StatusPower, 1);
            baseStats.IncTypeStatus = ScaleStat(baseStats.IncTypeStatus, scaling.IncTypeStatus, unitToScale.IncTypeStatus, 1);
            baseStats.IncMentalStatus = ScaleStat(baseStats.IncMentalStatus, scaling.IncMentalStatus, unitToScale.IncMentalStatus, 1);
            return baseStats;
        }

        private int ScaleStat(int statBase, int statScaling, int unitScaling, float statMulti)
        {
            float unitMulti = (float)unitScaling / 100;
            int scaleAdd = statScaling * gameManager.Level;
            statBase = (int)((statBase + scaleAdd) * statMulti * unitMulti);
            return statBase;
        }

        private Monster GetFirstTarget()
        {
            for (int i=0; i<targets.Length; i++)
            {
                if (targets[i].enabled && monsters[i].enabled)
                {
                    return monsters[i];
                }
            }
            return null;
        }

        public bool CheckAlive()
        {
            bool alive = false;
            foreach (Monster monster in monsters)
            {
                if (monster.enabled)
                {
                    alive = true;
                    break;
                }
            }
            return alive;
        }

        public void PrepareSkill(SkillStats skill)
        {
            currentSkill = skill;
            ChangeTargeting(true);
            IsCasting = true;
        }

        public void CastSkill(SkillStats skill, Unit caster)
        {
            if (caster.IsPlayer)
            {
                caster.ApplyCost(skill.Cost, skill.CostType);
            }
            //Find defender(s)
            if (skill.SkillType <= Constants.SkillTypes.Dark || skill.SkillType == Constants.SkillTypes.Hidden)
            {
                //Get Targets
                if (skill.TargetType == Constants.TargetTypes.All && caster.IsPlayer)
                {
                    foreach (Monster monster in monsters)
                    {
                        DoHit(skill, caster, monster, caster.IsPlayer);
                    }
                } else if (skill.TargetType == Constants.TargetTypes.Single && caster.IsPlayer)
                {
                    DoHit(skill, caster, GetFirstTarget(), caster.IsPlayer);
                } else if (!caster.IsPlayer)
                {
                    DoHit(skill, caster, player, false);
                }
            }
            if (!CheckAlive())
            {
                gameManager.StartRound();
            } else
            {
                AdvanceTurn();
            }
        }

        private void DoHit(SkillStats skill, Unit caster, Unit target, bool isPlayer)
        {
            Damage.DamagePacket hit;
            hit = Damage.Hit(skill, caster.Stats, target.Stats);
            
            if (hit.hit)
            {
                if (isPlayer)
                {
                    log.Add("You hit " + target.NameStr + " with " + skill.NameStr + " for " + hit.damage);
                } else
                {
                    log.Add(caster.NameStr + " hit you with " + skill.NameStr + " for " + hit.damage);
                }
            }
            else
            {
                if (isPlayer)
                {
                    log.Add("You missed " + target.NameStr + " with " + skill.NameStr);
                } else
                {
                    log.Add("You dodged " + target.NameStr + "'s " + skill.NameStr);
                }
            }
            target.TakeHit(hit);
            UpdateGlobes();
        }

        public void UpdateGlobes()
        {
            float hpPercent = (float)player.CurrentHealth / player.Stats.MaxHealth;
            float mpPercent = (float)player.CurrentMana / player.Stats.MaxMana;
            healthGlobe.SetGlobe(hpPercent);
            manaGlobe.SetGlobe(mpPercent);
        }

        // Update is called once per frame
        void Update()
        {
            if (isTargeting)
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (target == 0)
                    {
                        target = Constants.MAX_ENEMIES - 1;
                    }
                    else if (target != Constants.MAX_ENEMIES)
                    {
                        target -= 1;
                    }
                    UpdateTarget(-1);
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (target == Constants.MAX_ENEMIES - 1)
                    {
                        target = 0;
                    }
                    else if (target != Constants.MAX_ENEMIES)
                    {
                        target += 1;
                    }
                    UpdateTarget(1);
                }
                if (gameManager.AnalysisEnabled)
                {
                    Monster monst = GetFirstTarget();
                    if (monst == null)
                    {
                        return;
                    }
                    gameManager.DisplayAnalysis(monst.Stats, GetMonsterByName(monst.Stats.Name));
                }
                if (IsCasting)
                {
                    if (GetFirstTarget() != null)
                    {
                        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButton((int)MouseButton.LeftMouse))
                        {
                            CastSkill(currentSkill, player);
                            IsCasting = false;
                            ChangeTargeting(false);
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                InitTurns();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                AdvanceTurn();
            }
            hpText.text = "HP: " + player.CurrentHealth + "/" + player.Stats.MaxHealth;
            mpText.text = "MP: " + player.CurrentMana + "/" + player.Stats.MaxMana;
        }
    }
}