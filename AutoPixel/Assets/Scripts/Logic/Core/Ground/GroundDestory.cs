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

        void Start()
        {
            StartCoroutine("TimeDestory");
        }

        private IEnumerator TimeDestory()
        {
            while (true)
            {
                if (_Ground != null)
                {
                    int size = _Ground.transform.childCount;
                    if (size > 0)
                    {
                        int rn;
                        do
                        {
                            rn = Random.Range(0, size - 1);
                        } while (_Ground.GetComponentsInChildren<Ground>()[rn].Type != GroundType.Edge);

                        if (_Ground.GetComponentsInChildren<Ground>()[rn].Damage())
                        {
                            int X = _Ground.GetComponentsInChildren<Ground>()[rn].X,
                            Y = _Ground.GetComponentsInChildren<Ground>()[rn].Y;
                            for (int i = -1; i <= 1; i += 2)
                            {
                                for (int j = -1; j <= 1; j += 2)
                                {
                                    if (X + i >= 0 && X + i < 4 && Y + j >= 0 && Y + j < 4)
                                    {
                                        GameObject.Find($"G{X + i}{Y + j}").GetComponent<Ground>().Type
                                            = GroundType.Inner;
                                    }
                                }
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(CheckTime);
            }
        }
    }
}


