using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core.Ground;
using static Logic.Core.Ground.Ground;

namespace Logic.Core.Ground
{
    public class GroundDestory : MonoBehaviour
    {
        public float CheckTime;
        private readonly List<int> Num
            = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        public List<GameObject> objects;

        void Start()
        {
            InvokeRepeating("TimeDestory", 0, CheckTime);
        }

        private void TimeDestory()
        {
            int rn;
            do
            {
                rn = Random.Range(0, Num.Count - 1);
            } while (!(objects[Num[rn]].gameObject.activeSelf
                    && objects[Num[rn]].GetComponent<Ground>().Type == GroundType.Edge));

            if (objects[Num[rn]].GetComponent<Ground>().TimeDamage())
            {
                for (int i = 1; i <= 4; i += 3)
                {
                    if (Num[rn] + i >= 0 && Num[rn] + i <= 15)
                    {
                        objects[Num[rn] + i].GetComponent<Ground>().Type = GroundType.Edge;
                    }
                    if (Num[rn] - i >= 0 && Num[rn] - i <= 15)
                    {
                        objects[Num[rn] - i].GetComponent<Ground>().Type = GroundType.Edge;
                    }
                }
                Num.RemoveAt(rn);
            }
        }
    }
}


