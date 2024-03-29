﻿using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages all units
    /// </summary>
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
        public HitDisplay[] monsterDisplay;
        public HitDisplay playerDisplay;
        public TextMeshProUGUI description;
        public RewardManager rewards;
        public TextMeshProUGUI oneMoreText;

        public enum Turns
        {
            Player, Monster1, Monster2, Monster3, Monster4, Monster5, EndGame
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

        /// <summary>
        /// Changes targeting
        /// </summary>
        /// <param name="isTargeting">Should target</param>
        internal void ChangeTargeting(bool isTargeting)
        {
            this.isTargeting = isTargeting;
            HideTargets();
        }

        /// <summary>
        /// Target reference
        /// </summary>
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

        /// <summary>
        /// Initialises the player
        /// </summary>
        internal void InitPlayer()
        {
            if (baseSkills == null)
            {
                baseSkills = new List<SkillStats>
                {
                    GetSkill(73),
                    GetSkill(1),
                    GetSkill(9),
                    GetSkill(17),
                    GetSkill(25),
                    GetSkill(65)
                };
            }
            player.ChangeUnit(LoadStats(0), baseSkills, "You");
            player.SetBaseStats(player.Stats);
            player.InitGear();
        }

        /// <summary>
        /// Initialises the turn counter
        /// </summary>
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

        /// <summary>
        /// Advances the turn counter
        /// </summary>
        internal void AdvanceTurn()
        {
            foreach (Monster monster in monsters)
            {
                monster.IsDown = false;
                monster.UpdateColor();
            }
            player.IsDown = false;
            if (Turn == Turns.Player)
            {
                DoBlast(Constants.MAX_ENEMIES, player);
            }
            else
            {
                DoBlast((int)Turn - 1, monsters[(int)Turn - 1]);
            }
            HideOneMore();
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
            if (Turn == Turns.Player)
            {
                
                log.Add("Your Turn");
                player.ApplyDuration();
                player.TurnCounter += player.Stats.Speed;
                DoStatus(Constants.MAX_ENEMIES, player);
            } else
            {
                log.Add("Enemy Turn");
                monsters[(int)Turn - 1].ApplyDuration();
                monsters[(int)Turn - 1].TurnCounter += monsters[(int)Turn - 1].Stats.Speed;
                DoStatus((int)Turn - 1, monsters[(int)Turn - 1]);
            }
        }

        /// <summary>
        /// Applies blast
        /// </summary>
        /// <param name="pos">Unit position</param>
        /// <param name="unit">Unit targeted</param>
        private void DoBlast(int pos, Unit unit)
        {
            if (unit.GetStatus() == Constants.StatusTypes.Blast)
            {
                int damage = unit.ApplyBlast();
                if (pos == Constants.MAX_ENEMIES)
                {
                    log.Add("Blast hits you for " + damage);
                    UpdateGlobes();
                } else
                {
                    log.Add("Blast hits " + monsters[pos].NameStr + " for " + damage);
                    monsterDisplay[pos].UpdateFill(monsters[pos]);
                }
                unit.RemoveStatus();
            }
            if (!CheckAlive())
            {
                HideAllHealthBars();
                rewards.GenerateRewards();
            }
        }

        /// <summary>
        /// Applies status
        /// </summary>
        /// <param name="pos">Unit position</param>
        /// <param name="unit">Unit targeted</param>
        private void DoStatus(int pos, Unit unit)
        {
            Constants.StatusTypes status = unit.GetStatus();
            switch (status)
            {
                default:
                    if (!unit.IsPlayer)
                    {
                        MonsterCast((int)Turn - 1);
                    }
                    break;
                case Constants.StatusTypes.Blast:
                    if (!unit.IsPlayer)
                    {
                        StartCoroutine(DisplayText(pos, "Detonating"));
                        MonsterCast((int)Turn - 1);
                    }
                    break;
                case Constants.StatusTypes.Shock:
                    StartCoroutine(DisplayText(pos, "Shocked"));
                    if (unit.IsPlayer)
                    {
                        log.Add("You are shocked");
                    } else
                    {
                        log.Add(unit.NameStr + " is shocked");
                    }
                    AdvanceTurn();
                    break;
                case Constants.StatusTypes.Freeze:
                    StartCoroutine(DisplayText(pos, "Frozen"));
                    if (unit.IsPlayer)
                    {
                        log.Add("You are frozen");
                    }
                    else
                    {
                        log.Add(unit.NameStr + " is frozen");
                    }
                    AdvanceTurn();
                    break;
                case Constants.StatusTypes.Burn:
                    StartCoroutine(DisplayText(pos, "Burning\nTaking damage"));
                    unit.ApplyBurn();
                    if (unit.IsPlayer)
                    {
                        log.Add("You are burning");
                        UpdateGlobes();
                    }
                    else
                    {
                        log.Add(unit.NameStr + " is burning");
                        monsterDisplay[pos].UpdateFill(monsters[pos]);
                        MonsterCast((int)Turn - 1);
                    }
                    break;
                case Constants.StatusTypes.Curse:
                    StartCoroutine(DisplayText(pos, "Cursed\nLosing health"));
                    unit.ApplyCurse(-1);
                    if (unit.IsPlayer)
                    {
                        log.Add("You are cursed");
                        UpdateGlobes();
                    }
                    else
                    {
                        log.Add(unit.NameStr + " is cursed");
                        monsterDisplay[pos].UpdateFill(monsters[pos]);
                        MonsterCast((int)Turn - 1);
                    }
                    break;
                case Constants.StatusTypes.Sleep:
                    StartCoroutine(DisplayText(pos, "Sleeping\nGaining health"));
                    unit.ApplyCurse(1);
                    if (unit.IsPlayer)
                    {
                        log.Add("You are sleeping");
                        UpdateGlobes();
                    }
                    else
                    {
                        log.Add(unit.NameStr + " is sleeping");
                        monsterDisplay[pos].UpdateFill(monsters[pos]);
                    }
                    AdvanceTurn();
                    break;
                case Constants.StatusTypes.Brainwash:
                    StartCoroutine(DisplayText(pos, "Brainwashed"));
                    if (unit.IsPlayer)
                    {
                        log.Add("You are brainwashed");
                    }
                    else
                    {
                        log.Add(unit.NameStr + " is brainwashed");
                    }
                    AdvanceTurn();
                    break;
                case Constants.StatusTypes.Fear:
                    StartCoroutine(DisplayText(pos, "Terrified\nLosing willpower"));
                    if (unit.IsPlayer)
                    {
                        log.Add("You are terrified");
                        unit.ApplyCost(100, Constants.CostTypes.Spell);
                        UpdateGlobes();
                    }
                    else
                    {
                        log.Add(unit.NameStr + " is terrfied");
                    }
                    AdvanceTurn();
                    break;
            }
        }

        /// <summary>
        /// Chooses a skill reward
        /// </summary>
        /// <returns>Skill chosen</returns>
        public SkillStats ChooseSkillReward()
        {
            if (skills == null)
            {
                LoadSkills();
            }
            List<SkillStats> allowedSkills = skills.FindAll(x => x.RLvl <= gameManager.Level && x.SkillType != Constants.SkillTypes.Hidden);
            int roll = Damage.RandomInt(0, allowedSkills.Count - 1);
            return allowedSkills[roll];
        }

        /// <summary>
        /// Monster casts
        /// </summary>
        /// <param name="pos">Position to aim at</param>
        internal void MonsterCast(int pos)
        {
            List<SkillStats> activeSkills = monsters[pos].Skills.FindAll(x => x.SkillType != Constants.SkillTypes.Passive);
            int castChoice = Damage.RandomInt(0, activeSkills.Count-1);
            StartCoroutine(CastSkill(activeSkills[castChoice], monsters[pos]));
        }

        /// <summary>
        /// Shows one more text
        /// </summary>
        private void ShowOneMore()
        {
            oneMoreText.enabled = true;
        }

        /// <summary>
        /// Hides one more text
        /// </summary>
        private void HideOneMore()
        {
            oneMoreText.enabled = false;
        }

        /// <summary>
        /// Updates targeting
        /// </summary>
        /// <param name="direction">Target direction</param>
        /// <param name="counter">Target counter</param>
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

        /// <summary>
        /// Hides targets
        /// </summary>
        public void HideTargets()
        {
            foreach (SpriteRenderer renderer in targets)
            {
                renderer.enabled = false;
                target = 2;
            }
        }

        /// <summary>
        /// Shows target at position
        /// </summary>
        /// <param name="pos">Position to show</param>
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

        /// <summary>
        /// Gets player stats
        /// </summary>
        /// <returns></returns>
        internal UnitStats GetPlayerStats()
        {
            return player.Stats;
        }

        /// <summary>
        /// Loads unit stats
        /// </summary>
        /// <param name="id">Id of unit to load</param>
        /// <returns>Stats of unit</returns>
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
            using (var reader = new StreamReader(Application.streamingAssetsPath + Constants.DATA_PATH + Constants.UNIT_PATH))
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

        /// <summary>
        /// Loads skills from file
        /// </summary>
        internal void LoadSkills()
        {
            skills = new List<SkillStats>();
            using (var reader = new StreamReader(Application.streamingAssetsPath + Constants.DATA_PATH + Constants.SKILL_PATH))
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

        /// <summary>
        /// Gets skills from file
        /// </summary>
        /// <param name="id">Id of skill to get</param>
        /// <returns>Skill with id</returns>
        internal SkillStats GetSkill(int id)
        {
            if (skills == null)
            {
                LoadSkills();
            }
            return skills[id];
        }

        /// <summary>
        /// Loads monster data from file
        /// </summary>
        internal void LoadMonsterData()
        {
            monsterData = new List<MonsterData>();
            using (var reader = new StreamReader(Application.streamingAssetsPath + Constants.DATA_PATH + Constants.MONSTER_PATH))
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

        /// <summary>
        /// Gets monster data with id
        /// </summary>
        /// <param name="id">Id of data to get</param>
        /// <returns>Monster data at id</returns>
        internal MonsterData GetMonsterData(int id)
        {
            if (monsterData == null)
            {
                LoadMonsterData();
            }
            return monsterData[id];
        }

        /// <summary>
        /// Links monster data and unit
        /// </summary>
        /// <param name="id">Monster data id</param>
        internal void LinkMonster(int id)
        {
            monsterData[id].Sprite = monsterAtlas.GetSprite(monsterData[id].SpriteName);
            monsterData[id].Unit = LoadStats(monsterData[id].UnitId);
        }

        /// <summary>
        /// Clears description
        /// </summary>
        public void ClearDescription()
        {
            description.text = "";
        }

        /// <summary>
        /// Changes description
        /// </summary>
        /// <param name="newDesc">New description</param>
        public void ChangeDescription(string newDesc)
        {
            description.text = newDesc;
        }

        /// <summary>
        /// Chooses monster skills
        /// </summary>
        /// <param name="monster">Monster data</param>
        /// <returns>List of skills</returns>
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

        /// <summary>
        /// Finds skills by level and type
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="type">Skill type</param>
        /// <returns>List of matching skills</returns>
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

        /// <summary>
        /// Creates a monster
        /// </summary>
        /// <param name="position">Position to add monster</param>
        /// <param name="monster">Data of monster</param>
        /// <param name="monsterSkills">Skills of monster</param>
        internal void MakeMonster(int position, MonsterData monster, List<SkillStats> monsterSkills)
        {
            monsterSkills.RemoveAll(x => x == null);
            LinkMonster(monster.Id);
            monsters[position].ChangeMonster(ScaleMonster(monster.Unit, monster.StatMulti), monsterSkills, monster.Sprite, monster.NameStr);
            ActiveMonsters++;
        }

        /// <summary>
        /// Gets monster by name
        /// </summary>
        /// <param name="name">Name of monster</param>
        /// <returns>Matching monster data</returns>
        internal MonsterData GetMonsterByName(string name)
        {
            if (monsterData == null)
            {
                LoadMonsterData();
            }
            return monsterData.Find(x => x.Name.Equals(name));
        }

        /// <summary>
        /// Clears monsters
        /// </summary>
        internal void ClearMonsters()
        {
            for (int i=0; i<monsters.Length; i++)
            {
                monsters[i].Die();
            }
            ActiveMonsters = 0;
            HideTargets();
        }

        /// <summary>
        /// Scales monster stats
        /// </summary>
        /// <param name="unitToScale">Monster to scale</param>
        /// <param name="statMulti">Stat multiplier</param>
        /// <returns>New stats</returns>
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
            baseStats.ResistPhysical = unitToScale.ResistPhysical;
            baseStats.ResistProjectile = unitToScale.ResistProjectile;
            baseStats.ResistElectric = unitToScale.ResistElectric;
            baseStats.ResistCold = unitToScale.ResistCold;
            baseStats.ResistFire = unitToScale.ResistFire;
            baseStats.ResistWind = unitToScale.ResistWind;
            baseStats.ResistArcane = unitToScale.ResistArcane;
            baseStats.ResistPsychic = unitToScale.ResistPsychic;
            baseStats.ResistLight = unitToScale.ResistLight;
            baseStats.ResistDark = unitToScale.ResistDark;
            return baseStats;
        }

        /// <summary>
        /// Scales a stat
        /// </summary>
        /// <param name="statBase">Base value of stat</param>
        /// <param name="statScaling">Stat scaler</param>
        /// <param name="unitScaling">Unit scaler</param>
        /// <param name="statMulti">Stat multiplier</param>
        /// <returns>Scaled stat</returns>
        private float ScaleStat(float statBase, float statScaling, float unitScaling, float statMulti)
        {
            float unitMulti = (float)unitScaling / 100;
            float scaleAdd = statScaling * gameManager.Level;
            statBase = ((statBase + scaleAdd) * statMulti * unitMulti);
            return statBase;
        }

        /// <summary>
        /// Gets first target
        /// </summary>
        /// <returns>Gets first monster target</returns>
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

        /// <summary>
        /// Gets first target position
        /// </summary>
        /// <returns>Position of first target</returns>
        private int GetFirstTargetPos()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].enabled && monsters[i].enabled)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if monsters are alive
        /// </summary>
        /// <returns>If any monster is alive</returns>
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

        /// <summary>
        /// Prepares skill by id 
        /// </summary>
        /// <param name="id">Id of skill to prepare</param>
        public void PrepareSkill(int id)
        {
            if (Turn == Turns.Player)
            {
                PrepareSkill(GetSkill(id));
            }
        }

        /// <summary>
        /// Prepares skill by skill data
        /// </summary>
        /// <param name="skill">Skill data</param>
        public void PrepareSkill(SkillStats skill)
        {
            currentSkill = skill;
            if (skill.SkillType == Constants.SkillTypes.Heal || skill.TargetType == Constants.TargetTypes.All ||
                (skill.SkillType == Constants.SkillTypes.Buff && skill.Power >= 0) ||
                skill.SkillType == Constants.SkillTypes.Break)
            {
                StartCoroutine(CastSkill(skill, player));
                ChangeTargeting(false);
                IsCasting = false;
                return;
            }
            ChangeDescription("Using " + skill.NameStr + " (choose a target)");
            ChangeTargeting(true);
            IsCasting = true;
        }

        /// <summary>
        /// Casts a skill
        /// </summary>
        /// <param name="skill">Skill to cast</param>
        /// <param name="caster">Source of skill</param>
        /// <returns></returns>
        public IEnumerator CastSkill(SkillStats skill, Unit caster)
        {
            if (caster.IsPlayer)
            {
                caster.ApplyCost(skill.Cost, skill.CostType);
                ChangeDescription("Using " + skill.NameStr);
            } else
            {
                ChangeDescription(caster.NameStr + " uses " + skill.NameStr);
            }
            if (caster.GetStatus() == Constants.StatusTypes.Forget && skill.CostType == Constants.CostTypes.Spell)
            {
                log.Add(caster.NameStr + " forgot how to use spells");
                AdvanceTurn();
            }
            //Guard
            if (skill.SkillType == Constants.SkillTypes.Hidden && skill.BuffType == Constants.BuffTypes.Guard)
            {
                float waitDuration = gameManager.PlayParticle(Constants.SkillTypes.Buff, Constants.MAX_ENEMIES);
                yield return new WaitForSeconds(waitDuration * 1f);
                DoBuff(skill, caster, caster, true, Constants.MAX_ENEMIES);
            } else
            //Find defender(s)
            if (skill.SkillType <= Constants.SkillTypes.Dark || skill.SkillType == Constants.SkillTypes.Hidden)
            {
                float waitDuration;
                //Get Targets
                if (skill.TargetType == Constants.TargetTypes.All && caster.IsPlayer)
                {
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].enabled)
                        {
                            waitDuration = gameManager.PlayParticle(skill.SkillType, i);
                            yield return new WaitForSeconds(waitDuration / 2);
                            DoHit(skill, caster, monsters[i], caster.IsPlayer, i);
                        }
                    }
                }
                else if (skill.TargetType == Constants.TargetTypes.Single && caster.IsPlayer)
                {
                    int targetPos = GetFirstTargetPos();
                    if (targetPos != -1)
                    {
                        waitDuration = gameManager.PlayParticle(skill.SkillType, targetPos);
                        Monster target = GetFirstTarget();
                        yield return new WaitForSeconds(waitDuration);
                        DoHit(skill, caster, target, caster.IsPlayer, targetPos);
                    }
                }
                else if (!caster.IsPlayer)
                {
                    waitDuration = gameManager.PlayParticle(skill.SkillType, Constants.MAX_ENEMIES);
                    yield return new WaitForSeconds(waitDuration * 1f);
                    DoHit(skill, caster, player, false, Constants.MAX_ENEMIES);
                }
            } 
            else if (skill.SkillType == Constants.SkillTypes.Heal)
            {
                float waitDuration;
                if (skill.TargetType == Constants.TargetTypes.All && !caster.IsPlayer)
                {
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].enabled)
                        {
                            waitDuration = gameManager.PlayParticle(skill.SkillType, i);
                            yield return new WaitForSeconds(waitDuration / 2);
                            DoHeal(skill, caster, monsters[i], caster.IsPlayer, i);
                        }
                    }
                } else if (skill.TargetType == Constants.TargetTypes.Single && !caster.IsPlayer)
                {
                    float[] temp = new float[Constants.MAX_ENEMIES];
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].enabled)
                        {
                            temp[i] = monsters[i].CurrentHealth;
                        } else
                        {
                            temp[i] = Mathf.Infinity;
                        }
                    }
                    int pos = Array.IndexOf(temp, temp.Min());
                    waitDuration = gameManager.PlayParticle(skill.SkillType, pos);
                    yield return new WaitForSeconds(waitDuration * 1f);
                    DoHeal(skill, caster, monsters[pos], caster.IsPlayer, pos);
                } else
                {
                    waitDuration = gameManager.PlayParticle(skill.SkillType, Constants.MAX_ENEMIES);
                    yield return new WaitForSeconds(waitDuration * 1f);
                    DoHeal(skill, caster, caster, true, Constants.MAX_ENEMIES);
                }
            } else if (skill.SkillType == Constants.SkillTypes.Break)
            {
                float waitDuration;
                if (!caster.IsPlayer)
                {
                    waitDuration = gameManager.PlayParticle(skill.SkillType, Array.IndexOf(monsters, caster));
                    yield return new WaitForSeconds(waitDuration * 1f);
                } else
                {
                    waitDuration = gameManager.PlayParticle(skill.SkillType, Constants.MAX_ENEMIES);
                    yield return new WaitForSeconds(waitDuration * 1f);
                }
                if (caster.GetStatus() == skill.StatusType)
                {
                    caster.RemoveStatus();
                    log.Add("You broke free from your status affliction");
                } else
                {
                    log.Add("You tried to break, but you weren't afflicted by that status");
                }
            } else if (skill.SkillType == Constants.SkillTypes.Status || skill.SkillType == Constants.SkillTypes.Blast)
            {
                float waitDuration;
                //Get Targets
                if (skill.TargetType == Constants.TargetTypes.All && caster.IsPlayer)
                {
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].enabled)
                        {
                            waitDuration = gameManager.PlayParticle(skill.SkillType, i);
                            yield return new WaitForSeconds(waitDuration / 2);
                            DoStatus(skill, caster, monsters[i], caster.IsPlayer, i);
                        }
                    }
                }
                else if (skill.TargetType == Constants.TargetTypes.Single && caster.IsPlayer)
                {
                    int targetPos = GetFirstTargetPos();
                    if (targetPos != -1)
                    {
                        waitDuration = gameManager.PlayParticle(skill.SkillType, targetPos);
                        Monster target = GetFirstTarget();
                        yield return new WaitForSeconds(waitDuration * 1f);
                        DoStatus(skill, caster, target, caster.IsPlayer, targetPos);
                    }
                }
                else if (!caster.IsPlayer)
                {
                    waitDuration = gameManager.PlayParticle(skill.SkillType, Constants.MAX_ENEMIES);
                    yield return new WaitForSeconds(waitDuration * 1f);
                    DoStatus(skill, caster, player, false, Constants.MAX_ENEMIES);
                }
            } else if (skill.SkillType == Constants.SkillTypes.Buff && skill.Power >= 0)
            {
                //Buff
                float waitDuration;
                if (skill.TargetType == Constants.TargetTypes.All && !caster.IsPlayer)
                {
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].enabled)
                        {
                            waitDuration = gameManager.PlayParticle(skill.SkillType, i);
                            yield return new WaitForSeconds(waitDuration / 2);
                            DoBuff(skill, caster, monsters[i], caster.IsPlayer, i);
                        }
                    }
                }
                else if (skill.TargetType == Constants.TargetTypes.Single && !caster.IsPlayer)
                {
                    float[] temp = new float[Constants.MAX_ENEMIES];
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].enabled)
                        {
                            temp[i] = monsters[i].CurrentHealth;
                        }
                        else
                        {
                            temp[i] = Mathf.Infinity;
                        }
                    }
                    int pos = Array.IndexOf(temp, temp.Min());
                    waitDuration = gameManager.PlayParticle(skill.SkillType, pos);
                    yield return new WaitForSeconds(waitDuration * 1f);
                    DoBuff(skill, caster, monsters[pos], caster.IsPlayer, pos);
                }
                else
                {
                    waitDuration = gameManager.PlayParticle(skill.SkillType, Constants.MAX_ENEMIES);
                    yield return new WaitForSeconds(waitDuration * 1f);
                    DoBuff(skill, caster, caster, true, Constants.MAX_ENEMIES);
                }
            } else if (skill.SkillType == Constants.SkillTypes.Buff && skill.Power <= 0)
            {
                //Debuff
                float waitDuration;
                //Get Targets
                if (skill.TargetType == Constants.TargetTypes.All && caster.IsPlayer)
                {
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].enabled)
                        {
                            waitDuration = gameManager.PlayParticle(skill.SkillType, i);
                            yield return new WaitForSeconds(waitDuration / 2);
                            DoBuff(skill, caster, monsters[i], caster.IsPlayer, i);
                        }
                    }
                }
                else if (skill.TargetType == Constants.TargetTypes.Single && caster.IsPlayer)
                {
                    int targetPos = GetFirstTargetPos();
                    if (targetPos != -1)
                    {
                        waitDuration = gameManager.PlayParticle(skill.SkillType, targetPos);
                        Monster target = GetFirstTarget();
                        yield return new WaitForSeconds(waitDuration * 1f);
                        DoBuff(skill, caster, target, caster.IsPlayer, targetPos);
                    }
                }
                else if (!caster.IsPlayer)
                {
                    waitDuration = gameManager.PlayParticle(skill.SkillType, Constants.MAX_ENEMIES);
                    yield return new WaitForSeconds(waitDuration * 1f);
                    DoBuff(skill, caster, player, false, Constants.MAX_ENEMIES);
                }
            }
            else
            {
                log.Add("WIP skill");
            }
            ClearDescription();
            if (Turn != Turns.EndGame)
            {
                if (!CheckAlive())
                {
                    yield return new WaitForSeconds(1f);
                    HideAllHealthBars();
                    rewards.GenerateRewards();
                } else
                {
                    if (caster.OneMore)
                    {
                        caster.OneMore = false;
                        HideOneMore();
                        if (!caster.IsPlayer)
                        {
                            MonsterCast((int)Turn - 1);
                        } else
                        {
                            ChangeDescription("Knocked one down! Take another turn");
                        }
                    }
                    else
                    {
                        HideOneMore();
                        yield return new WaitForSeconds(1f);
                        AdvanceTurn();
                    }
                }
            } else
            {
                ChangeDescription("You are Dead");
                yield return new WaitForSeconds(5f);
                SceneManager.LoadScene(0);
            }
        }

        /// <summary>
        /// Applies buffs
        /// </summary>
        /// <param name="skill">Skill used</param>
        /// <param name="caster">Source</param>
        /// <param name="target">Target</param>
        /// <param name="isPlayer">Player is involved</param>
        /// <param name="pos">Target position</param>
        private void DoBuff(SkillStats skill, Unit caster, Unit target, bool isPlayer, int pos)
        {
            if (skill.Power >= 0)
            {
                if (skill.BuffType == Constants.BuffTypes.Guard)
                {
                    target.AddBuff(skill.BuffType, 1, 1);
                }
                else
                {
                    target.AddBuff(skill.BuffType, 1);
                }
                StartCoroutine(DisplayText(pos, "Buff", false, false, false));
                if (isPlayer)
                {
                    log.Add("You received a buff of " + skill.NameStr);
                }
                else
                {
                    log.Add(caster.NameStr + " buffed " + target.NameStr + " with " + skill.NameStr);
                }
            } else
            {
                target.AddBuff(skill.BuffType, 0.5f);
                StartCoroutine(DisplayText(pos, "Debuff", false, false, false));
                log.Add(caster.NameStr + " debuffed " + target.NameStr + " with " + skill.NameStr);
            }
        }

        /// <summary>
        /// Applies status
        /// </summary>
        /// <param name="skill">Skill used</param>
        /// <param name="caster">Source</param>
        /// <param name="target">Target</param>
        /// <param name="isPlayer">If player is involved</param>
        /// <param name="pos">Target position</param>
        private void DoStatus(SkillStats skill, Unit caster, Unit target, bool isPlayer, int pos)
        {
            KeyValuePair<bool, int> hit = Damage.Status(skill, caster.Stats, target.Stats);
            if (hit.Key)
            {
                StartCoroutine(DisplayText(pos, "Hit", false, false, false));
                if (isPlayer)
                {
                    log.Add("You hit " + target.NameStr + " with " + skill.NameStr);
                }
                else
                {
                    log.Add(caster.NameStr + " hit you with " + skill.NameStr);
                }
                if (skill.StatusType == Constants.StatusTypes.Blast)
                {
                    target.AddStatus(skill.StatusType, hit.Value, 1);
                }
                target.AddStatus(skill.StatusType, skill.StatusPower, hit.Value);
            }
            else
            {
                if (isPlayer)
                {
                    StartCoroutine(DisplayText(pos, "Miss", false, false, false));
                    log.Add("You missed " + target.NameStr + " with " + skill.NameStr);
                }
                else
                {
                    StartCoroutine(DisplayText(pos, "Dodge", false, false, false));
                    log.Add("You dodged " + caster.NameStr + "'s " + skill.NameStr);
                }
            }
        }

        /// <summary>
        /// Delays hiding one more text
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideOneMoreDelay()
        {
            yield return new WaitForSeconds(1f);
            HideOneMore();
        }

        /// <summary>
        /// Hides all health bars
        /// </summary>
        private void HideAllHealthBars()
        {
            foreach (HitDisplay display in monsterDisplay)
            {
                display.HideHealth();
            }
        }

        /// <summary>
        /// Updates analysis data
        /// </summary>
        /// <param name="target">Unit to analyse</param>
        /// <param name="type">Damage type</param>
        private void UpdateAnalysis(Unit target, Constants.DamageTypes type)
        {
            MonsterData monster = monsterData.Find(x => x.UnitId == target.Id);
            monster.CheckedType[(int)type-1] = true;
        }

        /// <summary>
        /// Applies heal effect
        /// </summary>
        /// <param name="skill">Skill used</param>
        /// <param name="caster">Source</param>
        /// <param name="target">Target</param>
        /// <param name="isPlayer">Is player involved</param>
        /// <param name="pos">Target position</param>
        private void DoHeal(SkillStats skill, Unit caster, Unit target, bool isPlayer, int pos)
        {
            int heal = Damage.Heal(skill, caster.Stats);
            target.ChangeHealth(heal);
            StartCoroutine(DisplayText(pos, "+" + heal.ToString(), false, false, false));
            if (isPlayer)
            {
                log.Add("You healed for " + heal);
            } else
            {
                log.Add(caster.NameStr + " healed " + target.NameStr + " for " + heal);
            }
            if (!isPlayer)
            {
                monsterDisplay[pos].UpdateFill(monsters[pos]);
            }
            UpdateGlobes();
        }

        /// <summary>
        /// Applies a hit
        /// </summary>
        /// <param name="skill">Skill used</param>
        /// <param name="caster">Source</param>
        /// <param name="target">Target</param>
        /// <param name="isPlayer">Is player involved</param>
        /// <param name="pos">Target position</param>
        private void DoHit(SkillStats skill, Unit caster, Unit target, bool isPlayer, int pos)
        {
            Damage.DamagePacket hit;
            hit = Damage.Hit(skill, caster.Stats, target.Stats);
            if (hit.hit)
            {
                StartCoroutine(DisplayText(pos, hit.damage.ToString(), hit.isWeak, hit.isTechnical, hit.isCrit));
                if (isPlayer)
                {
                    log.Add("You hit " + target.NameStr + " with " + skill.NameStr + " for " + hit.damage);
                    UpdateAnalysis(target, skill.DamageType);
                } else
                {
                    log.Add(caster.NameStr + " hit you with " + skill.NameStr + " for " + hit.damage);
                }
            }
            else
            {
                if (isPlayer)
                {
                    StartCoroutine(DisplayText(pos, "Miss", hit.isWeak, hit.isTechnical, hit.isCrit));
                    log.Add("You missed " + target.NameStr + " with " + skill.NameStr);
                } else
                {
                    StartCoroutine(DisplayText(pos, "Dodge", hit.isWeak, hit.isTechnical, hit.isCrit));
                    log.Add("You dodged " + caster.NameStr + "'s " + skill.NameStr);
                }
            }
            if ((hit.isCrit || hit.isWeak) && !target.IsDown)
            {
                ShowOneMore();
                caster.OneMore = true;
                if (isPlayer)
                {
                    log.Add("You knocked " + target.NameStr + " down");
                } else
                {
                    log.Add(caster.NameStr + " knocks you down");
                }
            }
            target.TakeHit(hit);
            if (isPlayer)
            {
                monsterDisplay[pos].UpdateFill(monsters[pos]);
            }
            UpdateGlobes();
        }

        /// <summary>
        /// Displays battle text
        /// </summary>
        /// <param name="pos">Target position</param>
        /// <param name="damage">Damage value</param>
        /// <param name="isWeak">Hit a weak target</param>
        /// <param name="isTechnical">Was a technical</param>
        /// <param name="isCrit">Was a critical hit</param>
        /// <returns></returns>
        private IEnumerator DisplayText(int pos, string damage, bool isWeak = false, bool isTechnical = false, bool isCrit = false)
        {
            HitDisplay display;
            if (pos < Constants.MAX_ENEMIES)
            {
                display = monsterDisplay[pos];
            } else
            {
                display = playerDisplay;
            }
            if (isWeak)
            {
                display.weak.gameObject.SetActive(true);
            }
            if (isTechnical)
            {
                display.technical.gameObject.SetActive(true);
            }
            if (isCrit)
            {
                display.crit.gameObject.SetActive(true);
            }
            display.damage.text = damage;
            display.damage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            display.HideAll();
        }

        /// <summary>
        /// Updates resource globes
        /// </summary>
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
                            IsCasting = false;
                            StartCoroutine(CastSkill(currentSkill, player));
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