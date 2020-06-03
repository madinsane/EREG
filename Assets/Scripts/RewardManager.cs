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
    public class RewardManager : MonoBehaviour
    {
        public UnitManager unitManager;
        public GameManager gameManager;
        public SpriteAtlas gearAtlas;
        public GameObject rewardPanel;
        public RewardPanel[] rewards;

        private List<Modifier> modifiers;
        private List<Gear> gear;

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetStat(UnitStats unit, string propName, float increase)
        {
            unit.GetType().GetProperty(propName).SetValue(unit, (float)unit.GetType().GetProperty(propName).GetValue(unit, null) + increase);
        }

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

        public Gear GetGear(int id)
        {
            if (gear == null)
            {
                LoadGear();
            }
            return gear[id].Copy();
        }

        public Sprite GetGearSprite(Gear gear)
        {
            return gearAtlas.GetSprite(gear.SpriteName);
        }

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

        private Modifier PickMod()
        {
            int roll = Damage.RandomInt(0, modifiers.Count - 1);
            Modifier newMod = modifiers[roll].Copy();
            return newMod;
        }

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
