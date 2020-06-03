using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts
{
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

        public void AddGear(Constants.Slot slot, Gear newGear)
        {
            gear[(int)slot] = newGear;
            UpdateGearPanels();
            UpdateGearStats();
        }

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

        private void ReplaceSkill(SkillStats skill)
        {
            rewards.rewardPanel.SetActive(false);
            unitManager.ChangeDescription("Choose a Skill to replace");
            unitManager.gameManager.ShowSkills();
            unitManager.gameManager.SetReplacing(skill);
        }

        public void SwapSkill(int pos, SkillStats newSkill)
        {
            Skills[pos] = newSkill;
        }

        public void AddItem(ItemStats item)
        {
            items[item.Id].Quantity++;
            unitManager.gameManager.ShowItems();
        }

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

        public void UpdateGearPanels()
        {
            for (int i=0; i<gear.Length; i++)
            {
                gearPanels[i].ChangeGear(gear[i], rewards.GetGearSprite(gear[i]));
            }
        }

        public void SetBaseStats(UnitStats stats)
        {
            playerBase = stats.Copy();
        }

        public ItemStats GetItem(int id)
        {
            if (items == null)
            {
                LoadItems();
            }
            return items[id];
        }

        public ItemStats ChooseItem()
        {
            if (items == null)
            {
                LoadItems();
            }
            int roll = Damage.RandomInt(0, items.Count - 1);
            return items[roll];
        }

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

        public override void Die()
        {
            unitManager.Turn = UnitManager.Turns.EndGame;
            base.Die();
        }
    }
}
