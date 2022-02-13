using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Controls the UI boxes
    /// </summary>
    public class ActionBox: MonoBehaviour
    {
        public Text text;
        public UnitManager unitManager;
        private string helpText;
        private StringBuilder sb;
        private bool isDisplayingStats;
        private UnitStats player;

        private void Awake()
        {
            helpText = "";
            isDisplayingStats = false;
        }

        /// <summary>
        /// Sets the help text to be displayed
        /// </summary>
        public void DisplayHelp()
        {
            if (helpText == "" || helpText == null)
            {
                LoadHelp();
            }
            text.text = helpText;
            isDisplayingStats = false;
        }

        /// <summary>
        /// Sets the stats display to be set to given param
        /// </summary>
        /// <param name="enabled">Whether to enable or disable stat display</param>
        public void SetStatDisplay(bool enabled)
        {
            isDisplayingStats = enabled;
        }

        /// <summary>
        /// Builds and displays the text of the stats
        /// </summary>
        public void DisplayStats()
        {
            if (player == null)
            {
                player = unitManager.GetPlayerStats();
            }
            if (sb == null)
            {
                sb = new StringBuilder();
            }
            sb.Clear();
            sb.Append("Resources:\n");
            sb.Append("Max Health: " + player.MaxHealth + "\n");
            sb.Append("Max Mana: " + player.MaxMana + "\n");
            sb.Append("\nOffense:\n");
            sb.Append("Attack Power: " + player.AttackPower + "\n");
            sb.Append("Magic Power: " + player.MagicPower + "\n");
            sb.Append("Accuracy: " + player.Accuracy + "\n");
            sb.Append("\nDefense:\n");
            sb.Append("Defense vs Attacks: " + player.AttackDefense + "\n");
            sb.Append("Defense vs Magic: " + player.MagicDefense + "\n");
            sb.Append("Evasion: " + player.Evasion + "\n");
            sb.Append("\nCritical Strikes:\n");
            sb.Append("Critical Strike Chance: " + player.CritChance + "%\n");
            sb.Append("Critical Strike Multiplier: " + player.CritMulti + "%\n");
            sb.Append("Chance to avoid Critical Strikes: " + (100 - player.IncCritChance) + "%\n");
            sb.Append("\nStatuses:\n");
            sb.Append("Increased Chance to inflict Elemental Statuses and Curse: " + (100 - player.TypeStatusChance) + "%\n");
            sb.Append("Increased Chance to inflict Mental Statuses: " + (100 - player.MentalStatusChance) + "%\n");
            sb.Append("Status Effect: " + player.StatusPower + "\n");
            sb.Append("Chance to avoid Elemental Statuses and Curse: " + (100 - player.IncTypeStatus) + "%\n");
            sb.Append("Chance to avoid Mental Statuses: " + (100 - player.IncMentalStatus) + "%\n");
            sb.Append("\nResistances:\n");
            sb.Append("Physical Resistance: " + player.ResistPhysical + "%\n");
            sb.Append("Projectile Resistance: " + player.ResistProjectile + "%\n");
            sb.Append("Electric Resistance: " + player.ResistElectric + "%\n");
            sb.Append("Cold Resistance: " + player.ResistCold + "%\n");
            sb.Append("Fire Resistance: " + player.ResistFire + "%\n");
            sb.Append("Wind Resistance: " + player.ResistWind + "%\n");
            sb.Append("Arcane Resistance: " + player.ResistDark + "%\n");
            sb.Append("Psychic Resistance: " + player.ResistPsychic + "%\n");
            sb.Append("Holy Resistance: " + player.ResistLight + "%\n");
            sb.Append("Shadow Resistance: " + player.ResistDark + "%");
            text.text = sb.ToString();
        }

        /// <summary>
        /// Loads help text from file
        /// </summary>
        private void LoadHelp()
        {
            helpText = File.ReadAllText(Application.streamingAssetsPath + Constants.DATA_PATH + Constants.HELP_PATH);
        }

        private void Update()
        {
            if (isDisplayingStats)
            {
                DisplayStats();
            }
        }
    }
}
