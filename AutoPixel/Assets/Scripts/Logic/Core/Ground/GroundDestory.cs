using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core.Ground;
using static Logic.Core.Ground.Ground;

namespace Logic.Core.Ground
{
    public class GroundDestory : MonoBehaviour
    {

        public GameObject _Ground;
        public float CheckTime;
        private readonly List<int> Num
            = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

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
            } while (!(_Ground.GetComponentsInChildren<Transform>()[Num[rn]].gameObject.activeSelf
                    && _Ground.GetComponentsInChildren<Ground>()[Num[rn]].Type == GroundType.Edge));

            if (_Ground.GetComponentsInChildren<Ground>()[Num[rn]].TimeDamage())
            {
                int X = _Ground.GetComponentsInChildren<Ground>()[Num[rn]].X,
                Y = _Ground.GetComponentsInChildren<Ground>()[Num[rn]].Y;
                for (int i = -1; i <= 1; i += 2)
                {
                    if (X + i >= 0 && X + i <= 3)
                    {
                        GameObject.Find($"G{X + i}{Y}").GetComponent<Ground>().Type
                            = GroundType.Edge;
                    }
                    if (Y + i >= 0 && Y + i <= 3)
                    {
                        GameObject.Find($"G{X}{Y + i}").GetComponent<Ground>().Type
                            = GroundType.Edge;
                    }

                }
                Num.RemoveAt(rn);
            }
        }
    }
}


