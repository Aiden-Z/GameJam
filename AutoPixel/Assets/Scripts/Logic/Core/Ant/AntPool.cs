using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core.Ant
{
    public class AntPool
    {
        public Transform Root;
        private Queue<Ant> m_waitingAnts;
        private HashSet<Ant> m_usingAnts;

        public AntPool(Transform root)
        {
            Root = root;
            m_waitingAnts = new Queue<Ant>();
            m_usingAnts = new HashSet<Ant>();
        }

        public Ant Alloc(Transform parent, Vector3 pos)
        {
            if (m_waitingAnts.Count != 0)
            {
                var ant = m_waitingAnts.Dequeue();
                ant.gameObject.SetActive(true);
                ant.transform.SetParent(parent);
                m_usingAnts.Add(ant);
                return ant;
            }
            else
            {
                var ant = Object.Instantiate(GameSceneManager.Instance.AntTemplate, pos,
                    Quaternion.identity, parent);
                ant.gameObject.SetActive(true);
                ant.transform.SetParent(parent);
                m_usingAnts.Add(ant);
                return ant;
            }
        }

        public void Release(Ant ant)
        {
            m_usingAnts.Remove(ant);
            m_waitingAnts.Enqueue(ant);
            ant.ResetAnt();
            ant.gameObject.SetActive(false);
        }
    }
}