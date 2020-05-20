using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Log log;
        public UnitManager unitManager;
        enum Turn
        {
            Player, Monster1, Monster2, Monster3, Monster4, Monster5
        }
        private Turn turn = Turn.Player;
        void Start()
        {
            turn = Turn.Player;
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

        void Update()
        {
               
        }
    }
}
