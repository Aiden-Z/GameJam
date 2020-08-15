using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Ground
{
    public class Ground : MonoBehaviour
    {
        public bool Life = true;
        
        public GroundType Type;      

        public int X, Y;        

        public bool Damage()
        {
            if (Life)
            {
                Life = false;
                return false;
            }
            else
            {
                gameObject.SetActive(false); 
                return true;
            }
        }

        public enum GroundType
        {
            Edge,
            Inner
        }
    }
}