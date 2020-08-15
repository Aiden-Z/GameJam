using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Controller.GroundFinder
{
    public class GroundFinder : MonoBehaviour
    {
        private HashSet<Collider2D> m_baits;

        private void Awake()
        {
            m_baits = new HashSet<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            m_baits.Add(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            m_baits.Remove(other);
        }

        public Ground.Ground GetGround(Vector3 position)
        {
            var distance = float.MaxValue;
            Ground.Ground ret = null;
            foreach (var bait in m_baits)
            {
                var curDis = Vector3.Distance(bait.transform.position, position);
                if (curDis < distance)
                {
                    distance = curDis;
                    ret = bait.GetComponent<Ground.Ground>();
                }
            }

            return ret;
        }
    }
}