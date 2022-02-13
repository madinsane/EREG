using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines the player
    /// </summary>
    public class Player : Unit
    {
        private Dictionary<int, ItemStats> items;
        private Gear[] gear;
        private UnitStats playerBase;

        public RewardManager rewards;
        public GearPanel[] gearPanels;

        public Player(UnitStats stats, List<SkillStats> skills) : base(stats, skills)
        {
            IsPlayer = true;
        }

        private void Awake()
        {
            IsPlayer = true;
        }

        /// <summary>
        /// Initialises the player's gear
        /// </summary>
        public void InitGear()
        {
            gear = new Gear[(int)Constants.Slot.Ring + 1];
            gear[(int)Constants.Slot.Helm] = rewards.GetGear(0);
            gear[(int)Constants.Slot.Chest] = rewards.GetGear(9);
            gear[(int)Constants.Slot.Gloves] = rewards.GetGear(18);
            gear[(int)Constants.Slot.Boots] = rewards.GetGear(27);
            gear[(int)Constants.Slot.Weapon] = rewards.GetGear(36);
            gear[(int)Constants.Slot.Shield] = rewards.GetGear(45);
            gear[(int)Constants.Slot.Amulet] = rewards.GetGear(54);
            gear[(int)Constants.Slot.Ring] = rewards.GetGear(63);
            gear[(int)Constants.Slot.Helm] = rewards.CreateGear(Constants.Slot.Helm);
            UpdateGearPanels();
            UpdateGearStats();
        }

        /// <summary>
        /// Adds gear to player
        /// </summary>
        /// <param name="slot">Slot to use</param>
        /// <param name="newGear">Gear item to add</param>
        public void AddGear(Constants.Slot slot, Gear newGear)
        {
            gear[(int)slot] = newGear;
            UpdateGearPanels();
            UpdateGearStats();
        }

        /// <summary>
        /// Adds or replaces a skill
        /// </summary>
        /// <param name="skill">New skill to use</param>
        public void AddSkill(SkillStats skill)
        {
            if (Skills.Count >= Constants.MAX_SKILLS)
            {
                ReplaceSkill(skill);
            }
            else
            {
                Skills.Add(skill);
            }
            unitManager.gameManager.ShowSkills();
        }

        /// <summary>
        /// Handles replacing a skill
        /// </summary>
        /// <param name="skill">New skill to use</param>
        private void ReplaceSkill(SkillStats skill)
        {
            rewards.rewardPanel.SetActive(false);
            unitManager.ChangeDescription("Choose a Skill to replace");
            unitManager.gameManager.ShowSkills();
            unitManager.gameManager.SetReplacing(skill);
        }

        /// <summary>
        /// Swaps a skill for another
        /// </summary>
        /// <param name="pos">Position of skill to be swapped</param>
        /// <param name="newSkill">New skill to use</param>
        public void SwapSkill(int pos, SkillStats newSkill)
        {
            Skills[pos] = newSkill;
        }

        /// <summary>
        /// Adds an item to player
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItem(ItemStats item)
        {
            items[item.Id].Quantity++;
            unitManager.gameManager.ShowItems();
        }

        /// <summary>
        /// Updates stats from gear
        /// </summary>
        public void UpdateGearStats()
        {
            Stats = playerBase.Copy();
            for (int i=0; i<gear.Length; i++)
            {
                if (gear[i].mods == null)
                {
                    continue;
                }
                foreach (Modifier mod in gear[i].mods)
                {
                    RewardManager.SetStat(Stats, mod.Stat, mod.RealValue);
                }
            }
        }

        /// <summary>
        /// Updates panels containing gear
        /// </summary>
        public void UpdateGearPanels()
        {
            for (int i=0; i<gear.Length; i++)
            {
                gearPanels[i].ChangeGear(gear[i], rewards.GetGearSprite(gear[i]));
            }
        }

        /// <summary>
        /// Sets base stats of the player
        /// </summary>
        /// <param name="stats">Stats to copy from</param>
        public void SetBaseStats(UnitStats stats)
        {
            playerBase = stats.Copy();
        }

        /// <summary>
        /// Gets an item from data
        /// </summary>
        /// <param name="id">Id of target item</param>
        /// <returns>Item matching id</returns>
        public ItemStats GetItem(int id)
        {
            if (items == null)
            {
                LoadItems();
            }
            return items[id];
        }

        /// <summary>
        /// Chooses a random item
        /// </summary>
        /// <returns>Randomly chosen item</returns>
        public ItemStats ChooseItem()
        {
            if (items == null)
            {
                LoadItems();
            }
            int roll = Damage.RandomInt(0, items.Count - 1);
            return items[roll];
        }

        /// <summary>
        /// Uses item with matching id
        /// </summary>
        /// <param name="id">Id of item to use</param>
        public void UseItem(int id)
        {
            ItemStats item = GetItem(id);
            if (item.Quantity <= 0)
            {
                return;
            }
            switch (item.Type)
            {
                case Constants.ItemTypes.Health:
                    ChangeHealth((int)((float)item.Value / 100 * Stats.MaxHealth));
                    break;
                case Constants.ItemTypes.Mana:
                    ApplyCost(-(int)((float)item.Value / 100 * Stats.MaxMana), Constants.CostTypes.Spell);
                    break;
                case Constants.ItemTypes.Break:
                    RemoveStatus();
                    break;
            }
            item.Quantity--;
            unitManager.UpdateGlobes();
            unitManager.AdvanceTurn();
        }

        /// <summary>
        /// Loads items from data
        /// </summary>
        private void LoadItems()
        {
            items = new Dictionary<int, ItemStats>();
            using (var reader = new StreamReader(Application.streamingAssetsPath + Constants.DATA_PATH + Constants.ITEM_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = "\t";
                var records = csv.GetRecords<ItemStats>();
                foreach (ItemStats data in records)
                {
                    if (data.Type == Constants.ItemTypes.Health || data.Type == Constants.ItemTypes.Mana)
                    {
                        data.Quantity = Constants.START_POTIONS;
                    } else
                    {
                        data.Quantity = Constants.START_ELIXIR;
                    }
                    items.Add(data.Id, data);
                }
            }
        }

        /// <summary>
        /// Changes player health
        /// </summary>
        /// <param name="value">Amount to change health by</param>
        public override void ChangeHealth(int value)
        {
            if (ExperimentControl.active)
            {
                if (GetBuffs().Find(x => x.BuffType == Constants.BuffTypes.Speed) == null)
                {
                    if (CurrentHealth + value < (Constants.CHEAT_THRESHOLD * Stats.MaxHealth))
                    {
                        Stats.Speed -= Constants.SPEED_REDUCTION;
                    }
                    else if (CurrentHealth + value <= (Constants.CHEAT_THRESHOLD * Stats.MaxHealth))
                    {
                        Stats.Speed += Constants.SPEED_REDUCTION;
                    }
                }
            }
            if (CurrentHealth + value > Stats.MaxHealth)
            {
                CurrentHealth = Stats.MaxHealth;
            }
            else
            {
                CurrentHealth += value;
                if (CurrentHealth <= 0)
                {
                    unitManager.log.Add(NameStr + " die");
                    Die();
                }
            }
        }

        /// <summary>
        /// Kills the player
        /// </summary>
        public override void Die()
        {
            unitManager.Turn = UnitManager.Turns.EndGame;
            base.Die();
        }
    }
}
