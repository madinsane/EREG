using System.Linq;
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
        private MonsterData[] monsterData;

        void Start()
        {
            turn = Turn.Player;
            monsterData = new MonsterData[Constants.MAX_ENEMIES];
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

            //Place monsters
            for (int i=0; i<monsterData.Length; i++)
            {
                monsterData[i] = unitManager.GetMonsterData(0);
                if (monsterData[i].SkillTypeFull == null)
                {
                    Constants.SkillTypes[] temp = new Constants.SkillTypes[Constants.ENEMY_SKILL_TYPE_MAX];
                    int counter = 0;
                    if (monsterData[i].SkillType1 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[0] = monsterData[i].SkillType1;
                    }
                    if (monsterData[i].SkillType2 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[1] = monsterData[i].SkillType2;
                    }
                    if (monsterData[i].SkillType3 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[2] = monsterData[i].SkillType3;
                    }
                    if (monsterData[i].SkillType4 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[3] = monsterData[i].SkillType4;
                    }
                    if (monsterData[i].SkillType5 != Constants.SkillTypes.None)
                    {
                        counter++;
                        temp[4] = monsterData[i].SkillType5;
                    }
                    monsterData[i].SkillTypeFull = new Constants.SkillTypes[counter];
                    int tempCount = 0;
                    for (int j=0; j<temp.Length; j++)
                    {
                        if (temp[j] != Constants.SkillTypes.None)
                        {
                            monsterData[i].SkillTypeFull[tempCount] = temp[j];
                            tempCount++;
                        }
                    }
                }
                //Select skills
                unitManager.MakeMonster(i, monsterData[i], unitManager.ChooseSkills(monsterData[i]).ToList());
            }
            //Pass to Turn manager

        }

        void Update()
        {
               
        }
    }
}
