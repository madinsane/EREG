using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Log log;
        public UnitManager unitManager;
        public GameObject actionBoxText;
        public ActionBox actionBox;
        public GameObject itemPanelParent;
        public ItemPanel[] itemPanels;
        public SpriteAtlas itemAtlas;
        public SpriteAtlas guiAtlas;
        public Image analysisBack;
        public Text tooltipStatic;

        enum Turn
        {
            Player, Monster1, Monster2, Monster3, Monster4, Monster5
        }

        private Turn turn = Turn.Player;
        private MonsterData[] monsterData;
        private List<MonsterTier> monsterTiers;
        public int Level { get; private set; }
        private string previousSpawn;
        public bool AnalysisEnabled { get; set; }
        private StringBuilder tooltip;

        void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            turn = Turn.Player;
            Level = 1;
            previousSpawn = "";
            monsterData = new MonsterData[Constants.MAX_ENEMIES];
            unitManager.ClearMonsters();
            unitManager.InitPlayer();
            StartRound();
        }

        void AdvanceTurn()
        {
            if (turn == Turn.Monster5)
            {
                turn = Turn.Player;
                log.Add("Player Turn");
            } else
            {
                turn++;
            }
        }

        void StartRound()
        {
            //Pick monsters
            List<MonsterData> chosen = PickMonsters();
            //Place monsters
            int pos = 0;
            foreach (MonsterData pick in chosen)
            {
                if (pick.SkillTypeFull == null)
                {
                    Constants.SkillTypes[] temp = new Constants.SkillTypes[Constants.ENEMY_SKILL_TYPE_MAX];
                    int counter = 0;
                    if (pick.SkillType1 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[0] = pick.SkillType1;
                    }
                    if (pick.SkillType2 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[1] = pick.SkillType2;
                    }
                    if (pick.SkillType3 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[2] = pick.SkillType3;
                    }
                    if (pick.SkillType4 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[3] = pick.SkillType4;
                    }
                    if (pick.SkillType5 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[4] = pick.SkillType5;
                    }
                    pick.SkillTypeFull = new Constants.SkillTypes[counter];
                    int tempCount = 0;
                    for (int j=0; j<temp.Length; j++)
                    {
                        if (temp[j] != Constants.SkillTypes.None)
                        {
                            pick.SkillTypeFull[tempCount] = temp[j];
                            tempCount++;
                        }
                    }
                }
                //Select skills
                unitManager.MakeMonster(pos, pick, unitManager.ChooseSkills(pick).ToList());
                pos++;
            }
            //Pass to Turn manager

        }

        internal void LoadMonsterTierData()
        {
            monsterTiers = new List<MonsterTier>();
            using (var reader = new StreamReader(Application.dataPath + Constants.DATA_PATH + Constants.MONSTERTIER_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                var records = csv.GetRecords<MonsterTier>();
                foreach (MonsterTier data in records)
                {
                    //Compile data
                    data.Types = new Constants.TierType[5];
                    data.Types[0] = data.Type1;
                    data.Types[1] = data.Type2;
                    data.Types[2] = data.Type3;
                    data.Types[3] = data.Type4;
                    data.Types[4] = data.Type5;
                    data.MonsterNames = new string[5];
                    data.MonsterNames[0] = data.MonsterName1;
                    data.MonsterNames[1] = data.MonsterName2;
                    data.MonsterNames[2] = data.MonsterName3;
                    data.MonsterNames[3] = data.MonsterName4;
                    data.MonsterNames[4] = data.MonsterName5;
                    data.Weights = new int[5];
                    data.TotalWeight = 0;
                    data.TotalWeight += data.Weight1;
                    data.Weights[0] = data.TotalWeight;
                    data.TotalWeight += data.Weight2;
                    data.Weights[1] = data.TotalWeight;
                    data.TotalWeight += data.Weight3;
                    data.Weights[2] = data.TotalWeight;
                    data.TotalWeight += data.Weight4;
                    data.Weights[3] = data.TotalWeight;
                    data.TotalWeight += data.Weight5;
                    data.Weights[4] = data.TotalWeight;
                    monsterTiers.Add(data);
                }
            }
        }

        List<MonsterData> PickMonsters()
        {
            if (monsterTiers == null)
            {
                LoadMonsterTierData();
            }
            List<MonsterData> chosen = new List<MonsterData>();
            int picks = 1;
            int placed = 0;
            int tier = (int)Math.Floor((Level - 1) / Constants.LEVELS_PER_TIER) % Constants.MAX_TIERS;
            Debug.Log("Base Tier: " + tier);
            MonsterTier currentTier;
            int roll;
            int chosenIndex = 0;
            int previousWeight;
            float statMulti = 1f;
            bool upgraded = false;
            while (picks > 0 && placed < Constants.MAX_ENEMIES)
            {
                currentTier = GetTier(tier);
                roll = Damage.RandomInt(1, currentTier.TotalWeight);
                for (int i=0; i<currentTier.Weights.Length; i++)
                {
                    //Prevent repeat upgrades or too many downgrades
                    if (currentTier.Types[i] == Constants.TierType.Upgrade && upgraded)
                    {
                        roll += currentTier.Weights[i];
                        continue;
                    } else if (currentTier.Types[i] == Constants.TierType.Downgrade && (upgraded || picks + placed == Constants.MAX_ENEMIES))
                    {
                        roll += currentTier.Weights[i];
                        continue;
                    }
                    //Prevent Duplicate results within same tier
                    if ((Level - 1) % Constants.LEVELS_PER_TIER > 0)
                    {
                        if (currentTier.Types[i] == Constants.TierType.Monster && currentTier.MonsterNames[i].Equals(previousSpawn) && placed == 0 && picks == 1)
                        {
                            //Force downgrade
                            if (currentTier.Types[1] == Constants.TierType.Downgrade)
                            {
                                chosenIndex = 1;
                                break;
                            } else
                            {
                                chosenIndex = (i + 1) % currentTier.Types.Length;
                                break;
                            }
                        }
                    }
                    if (i == currentTier.Weights.Length - 1)
                    {
                        chosenIndex = i;
                        break;
                    }
                    else if (i == 0)
                    {
                        previousWeight = 0;   
                    } else
                    {
                        previousWeight = currentTier.Weights[i - 1];
                    }
                    if (roll > previousWeight && roll <= currentTier.Weights[i])
                    {
                        chosenIndex = i;
                        break;
                    }
                }
                switch (currentTier.Types[chosenIndex]) {
                    case Constants.TierType.Monster:
                        chosen.Add(unitManager.GetMonsterByName(currentTier.MonsterNames[chosenIndex]));
                        Debug.Log("Spawning: " + chosen.Last().Name + " with multi: " + statMulti);
                        chosen.Last().StatMulti = statMulti;
                        placed++;
                        picks--;
                        break;
                    case Constants.TierType.Upgrade:
                        if (upgraded)
                        {
                            break;
                        }
                        statMulti += Constants.UPGRADE_BONUS;
                        tier++;
                        break;
                    case Constants.TierType.Downgrade:
                        if (upgraded || picks + placed == Constants.MAX_ENEMIES)
                        {
                            break;
                        }
                        picks++;
                        tier--;
                        statMulti -= Constants.DOWNGRADE_PENALTY;
                        break;
                }
            }
            if (chosen.Count == 1)
            {
                previousSpawn = chosen.Last().Name;
            }
            return chosen;
        }

        MonsterTier GetTier(int tier)
        {
            return monsterTiers.Find(x => x.Tier == tier);
        }

        public void ShowItems()
        {
            actionBoxText.SetActive(false);
            itemPanelParent.SetActive(true);
            for (int i=0; i<itemPanels.Length; i++)
            {
                if (i < Constants.ITEM_TYPES)
                {
                    itemPanels[i].gameObject.SetActive(true);
                    ItemStats item = unitManager.player.GetItem(i);
                    itemPanels[i].ChangeItem(item.Id, itemAtlas.GetSprite(item.SpriteName));
                } else
                {
                    itemPanels[i].gameObject.SetActive(false);
                }
            }          
        }

        public void ShowSkills()
        {
            actionBoxText.SetActive(false);
            itemPanelParent.SetActive(true);
            List<SkillStats> skills = unitManager.player.Skills;
            for (int i = 0; i < itemPanels.Length; i++)
            {
                if (i < skills.Count)
                {
                    itemPanels[i].gameObject.SetActive(true);
                    itemPanels[i].ChangeSkill(skills[i].Id, guiAtlas.GetSprite(skills[i].SpriteName), skills[i].Cost, skills[i].CostType);
                }
                else
                {
                    itemPanels[i].gameObject.SetActive(false);
                }
            }
        }

        public void ShowHelp()
        {
            actionBoxText.SetActive(true);
            itemPanelParent.SetActive(false);
            actionBox.SetStatDisplay(false);
            actionBox.DisplayHelp();
        }

        public void ShowStats()
        {
            actionBoxText.SetActive(true);
            itemPanelParent.SetActive(false);
            actionBox.SetStatDisplay(true);
        }

        public void ToggleAnalysis()
        {
            if (AnalysisEnabled)
            {
                AnalysisEnabled = false;
                analysisBack.sprite = guiAtlas.GetSprite(Constants.DISABLED_GUI_BACK);
                unitManager.ChangeTargeting(false);
            } else
            {
                AnalysisEnabled = true;
                analysisBack.sprite = guiAtlas.GetSprite(Constants.ENABLED_GUI_BACK);
                unitManager.ChangeTargeting(true);
            }
        }

        public void DisplayAnalysis(UnitStats unit, MonsterData monster)
        {
            if (tooltip == null)
            {
                tooltip = new StringBuilder();
            }
            tooltip.Clear();
            if (unit == null)
            {
                return;
            }
            tooltip.Append(monster.NameStr);
            tooltip.Append("\n\nResistances:\n");
            tooltip.Append("Physical:\t" + (monster.CheckedType[0] ? unit.ResistPhysical.ToString() : "?") + "%\n");
            tooltip.Append("Projectile:\t" + (monster.CheckedType[1] ? unit.ResistProjectile.ToString() : "?") + "%\n");
            tooltip.Append("Electric:\t" + (monster.CheckedType[2] ? unit.ResistElectric.ToString() : "?") + "%\n");
            tooltip.Append("Cold:\t\t" + (monster.CheckedType[3] ? unit.ResistCold.ToString() : "?") + "%\n");
            tooltip.Append("Fire:\t\t" + (monster.CheckedType[4] ? unit.ResistFire.ToString() : "?") + "%\n");
            tooltip.Append("Wind:\t\t" + (monster.CheckedType[5] ? unit.ResistWind.ToString() : "?") + "%\n");
            tooltip.Append("Arcane:\t\t" + (monster.CheckedType[6] ? unit.ResistArcane.ToString() : "?") + "%\n");
            tooltip.Append("Psychic:\t" + (monster.CheckedType[7] ? unit.ResistPsychic.ToString() : "?") + "%\n");
            tooltip.Append("Holy:\t\t" + (monster.CheckedType[8] ? unit.ResistLight.ToString() : "?") + "%\n");
            tooltip.Append("Shadow:\t\t" + (monster.CheckedType[9] ? unit.ResistDark.ToString() : "?") + "%");
            tooltipStatic.text = tooltip.ToString();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                unitManager.ClearMonsters();
                Level++;
                Debug.Log(Level);
                StartRound();
            }
        }
    }
}
