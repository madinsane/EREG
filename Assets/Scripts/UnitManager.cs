using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class UnitManager : MonoBehaviour
    {
        private Dictionary<int, UnitStats> stats;
        // Start is called before the first frame update
        void Start()
        {

        }

        public UnitStats LoadStats(int id)
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
            IEnumerable<UnitStats> statEnumerable = (IEnumerable<UnitStats>)DataManager.ReadUnits(Application.dataPath + Constants.DATA_PATH + Constants.UNIT_PATH);
            foreach (UnitStats stat in statEnumerable)
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
            return stats[id];
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}