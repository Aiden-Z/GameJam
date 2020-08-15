using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Ground
{
    public class Ground : MonoBehaviour
    {
        public int Life = 4;
        public GroundType Type;

        public int X, Y;

        public bool TimeDamage()
        {
            Life -= 2;
            if (Life > 0)
            {
                return false;
            }
            else
            {
                gameObject.SetActive(false);
                return true;
            }
        }

        public void OnCorrode()
        {

        }

        private void OnTriggerEnter2D(Collider2D other)
        {

        }

        public enum GroundType
        {
            Edge,
            Inner
        }
    }
}