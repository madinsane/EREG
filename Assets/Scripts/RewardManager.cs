using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages rewards
    /// </summary>
    public class RewardManager : MonoBehaviour
    {
        public UnitManager unitManager;
        public GameManager gameManager;
        public SpriteAtlas gearAtlas;
        public GameObject rewardPanel;
        public RewardPanel[] rewards;

        private List<Modifier> modifiers;
        private List<Gear> gear;

        /// <summary>
        /// Gets value of a property
        /// </summary>
        /// <param name="src">Source object</param>
        /// <param name="propName">Property name</param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        /// <summary>
        /// Sets value of a property
        /// </summary>
        /// <param name="unit">Unit to modify</param>
        /// <param name="propName">Property name</param>
        /// <param name="increase">Value to add</param>
        public static void SetStat(UnitStats unit, string propName, float increase)
        {
            unit.GetType().GetProperty(propName).SetValue(unit, (float)unit.GetType().GetProperty(propName).GetValue(unit, null) + increase);
        }

        /// <summary>
        /// Loads modifiers from file
        /// </summary>
        private void LoadModifiers()
        {
            modifiers = new List<Modifier>();
            using (var reader = new StreamReader(Application.streamingAssetsPath + Constants.DATA_PATH + Constants.MODIFIER_PATH))
            using (var csvUnit = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvUnit.Configuration.Delimiter = "\t";
                IEnumerable records = csvUnit.GetRecords<Modifier>();
                foreach (Modifier mod in records)
                {
                    modifiers.Add(mod);
                }
            }
        }

        /// <summary>
        /// Loads gear from file
        /// </summary>
        private void LoadGear()
        {
            gear = new List<Gear>();
            using (var reader = new StreamReader(Application.streamingAssetsPath + Constants.DATA_PATH + Constants.GEAR_PATH))
            using (var csvUnit = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvUnit.Configuration.Delimiter = "\t";
                IEnumerable records = csvUnit.GetRecords<Gear>();
                foreach (Gear gear in records)
                {
                    this.gear.Add(gear);
                }
            }
        }

        /// <summary>
        /// Gets gear from data
        /// </summary>
        /// <param name="id">Id to find</param>
        /// <returns>Copy of gear item</returns>
        public Gear GetGear(int id)
        {
            if (gear == null)
            {
                LoadGear();
            }
            return gear[id].Copy();
        }

        /// <summary>
        /// Gets sprite for gear
        /// </summary>
        /// <param name="gear">Gear to get sprite from</param>
        /// <returns>Sprite for the gear</returns>
        public Sprite GetGearSprite(Gear gear)
        {
            return gearAtlas.GetSprite(gear.SpriteName);
        }

        /// <summary>
        /// Creates a gear item in given slot
        /// </summary>
        /// <param name="slot">Slot to add to</param>
        /// <returns>Gear object</returns>
        public Gear CreateGear(Constants.Slot slot)
        {
            if (modifiers == null)
            {
                LoadModifiers();
            }
            if (gear == null)
            {
                LoadGear();
            }
            List<Modifier> chosenMods = new List<Modifier>();
            int level = gameManager.Level;
            Gear newGear = gear.FindLast(x => x.RLvl <= level && x.Slot == slot).Copy();
            for (int i=0; i<newGear.Affixes; i++)
            {
                Modifier newMod = PickMod();
                chosenMods.Add(newMod);
            }
            chosenMods = CombineMods(chosenMods, level);
            newGear.mods = chosenMods;
            return newGear;
        }

        /// <summary>
        /// Chooses a modifier
        /// </summary>
        /// <returns>Chosen modifier</returns>
        private Modifier PickMod()
        {
            int roll = Damage.RandomInt(0, modifiers.Count - 1);
            Modifier newMod = modifiers[roll].Copy();
            return newMod;
        }

        /// <summary>
        /// Combines similar modifiers
        /// </summary>
        /// <param name="mods">Mods list</param>
        /// <param name="level">Current level</param>
        /// <returns>New list with combined mods</returns>
        private List<Modifier> CombineMods(List<Modifier> mods, int level)
        {
            var grouped = mods.GroupBy(i => i.Id);
            List<Modifier> combined = new List<Modifier>();
            foreach (var grp in grouped)
            {
                int count = grp.Count();
                Modifier mod = modifiers[grp.Key];
                mod.RealValue = mod.Value * (level + Constants.AFFIX_BASE_VALUE) * count;
                combined.Add(mod);
            }
            return combined;
        }

        /// <summary>
        /// Generates rewards
        /// </summary>
        public void GenerateRewards()
        {
            SkillStats skill = unitManager.ChooseSkillReward();
            int chosenSlot = Damage.RandomInt(0, (int)Constants.Slot.Ring);
            Gear gear = CreateGear((Constants.Slot)chosenSlot);
            ItemStats item = unitManager.player.ChooseItem();
            rewards[(int)Constants.RewardTypes.Skill].ChangeReward(skill, gameManager.GetSkillSprite(skill));
            rewards[(int)Constants.RewardTypes.Gear].ChangeReward(gear, gearAtlas.GetSprite(gear.SpriteName));
            rewards[(int)Constants.RewardTypes.Item].ChangeReward(item, gearAtlas.GetSprite(item.SpriteName));
            rewardPanel.SetActive(true);
        }

        /// <summary>
        /// Handles reward being selected
        /// </summary>
        /// <param name="type">Type of reward</param>
        /// <param name="reward">Reward object</param>
        public void PickedReward(Constants.RewardTypes type, object reward)
        {
            switch (type)
            {
                case Constants.RewardTypes.Skill:
                    unitManager.player.AddSkill((SkillStats)reward);
                    break;
                case Constants.RewardTypes.Gear:
                    Gear newGear = (Gear)reward;
                    unitManager.player.AddGear(newGear.Slot, newGear);
                    break;
                case Constants.RewardTypes.Item:
                    unitManager.player.AddItem((ItemStats)reward);
                    break;
            }
            rewardPanel.SetActive(false);
            gameManager.StartRound();
        }
    }
}
