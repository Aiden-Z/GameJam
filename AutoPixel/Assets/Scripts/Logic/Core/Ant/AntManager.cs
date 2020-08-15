using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Core.Ant
{
    public class AntManager : MonoBehaviour
    {
        public float SendAntInterval;
        public int SendAntPerInterval;
        public int LowestAntsLimited;
        public int MaxAnts;
        public float AntsSurroundRadius;
        
        private Dictionary<Bait.Bait, List<Ant>> m_sentAnts;
        private List<Ant> m_holdingAnts;
        public Transform AntsBornPoint;
        private AntPool m_antPool;

        private void Awake()
        {
            m_antPool = new AntPool(AntsBornPoint);
            m_sentAnts = new Dictionary<Bait.Bait, List<Ant>>();
            m_holdingAnts = new List<Ant>();
            for (int i = 0; i < LowestAntsLimited; i++)
            {
                var ant = m_antPool.Alloc(AntsBornPoint, GetRandomPoint());
                m_holdingAnts.Add(ant);
            }
        }

        private float m_sendingTimer;
        public void PushBait(Bait.Bait bait)
        {
            m_sentAnts.Add(bait, new List<Ant>());
            m_clearMark = false;
        }
        
        public Vector3 GetRandomPoint()
        {
            var curRadius = AntsSurroundRadius;
            var randomRad = Random.Range(0, Mathf.PI * 2);
            var x = Mathf.Cos(randomRad) * curRadius;
            var y = Mathf.Sin(randomRad) * curRadius;
            return new Vector3(x, y) + AntsBornPoint.position;
        }

        private bool m_clearMark = false;
        private void FixedUpdate()
        {
            if (m_sendingTimer >= SendAntInterval)
            {
                m_sendingTimer = 0;
                var deletingBaits = new List<Bait.Bait>();
                foreach (var sentAnt in m_sentAnts)
                {
                    if (sentAnt.Key != null) // 还未消失
                    {
                        var count = SendAntPerInterval;
                        if (count >= m_holdingAnts.Count - LowestAntsLimited) // 不足
                        {
                            count -= (m_holdingAnts.Count - LowestAntsLimited); // 需要alloc的数量
                            var needSendCount = m_holdingAnts.Count - LowestAntsLimited;
                            var needSend = new List<Ant>(needSendCount);
                            for (; needSendCount > 0; needSendCount--)
                            {
                                var ant = m_holdingAnts[m_holdingAnts.Count - 1];
                                ant.TowardsBait(sentAnt.Key);
                                needSend.Add(ant);
                                m_holdingAnts.RemoveAt(m_holdingAnts.Count - 1);
                            }
                            sentAnt.Value.AddRange(needSend);
                        }
                        else // 足够
                        {
                            count = 0;
                            var needSendCount = m_holdingAnts.Count - LowestAntsLimited;
                            var needSend = new List<Ant>(needSendCount);
                            for (; needSendCount > 0; needSendCount--)
                            {
                                var ant = m_holdingAnts[m_holdingAnts.Count - 1];
                                ant.TowardsBait(sentAnt.Key);
                                needSend.Add(ant);
                                m_holdingAnts.RemoveAt(m_holdingAnts.Count - 1);
                            }
                            sentAnt.Value.AddRange(needSend);
                        }
                        List<Ant> ants = new List<Ant>();
                        for (int i = 0; i < count; i++)
                        {
                            var ant = m_antPool.Alloc(AntsBornPoint, GetRandomPoint());
                            ant.TowardsBait(sentAnt.Key);
                            ants.Add(ant);
                        }
                        sentAnt.Value.AddRange(ants);
                    }
                    else // 已消失
                    {
                        m_holdingAnts.AddRange(sentAnt.Value);
                        sentAnt.Value.Clear();
                        deletingBaits.Add(sentAnt.Key);
                    }
                }

                foreach (var deletingBait in deletingBaits)
                {
                    m_sentAnts.Remove(deletingBait);
                }

                if (!m_clearMark)
                {
                    if (m_sentAnts.Count == 0)
                    {
                        var needReleaseAnt = m_holdingAnts.Count - MaxAnts;
                        var needRemoveIndex = new List<int>();
                        var reserveAnts = MaxAnts;
                        for (var i = 0; i < m_holdingAnts.Count; i++)
                        {
                            var holdingAnt = m_holdingAnts[i];
                            if (holdingAnt.State == AntState.AroundingBornPoint)
                            {
                                reserveAnts--;
                                if (reserveAnts < 0)
                                {
                                    needRemoveIndex.Add(i);
                                    needReleaseAnt--;
                                    if (needReleaseAnt < 0)
                                    {
                                        m_clearMark = true;
                                        break;
                                    }
                                }
                            }
                        }

                        for (int i = needRemoveIndex.Count; i > 0; i--)
                        {
                            var ant = m_holdingAnts[needRemoveIndex[i - 1]];
                            m_antPool.Release(ant);
                            m_holdingAnts.RemoveAt(needRemoveIndex[i - 1]);
                        }
                    }
                }
                
            }
            else
            {
                m_sendingTimer += Time.fixedDeltaTime;
            }
        }
    }
}