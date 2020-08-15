using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Core.Ant
{
    public class Ant : MonoBehaviour
    {
        public Bait.Bait Bait;
        public float SinglePathDuration;
        public float MaxAntsSpeed;
        public float MinAntsSpeed;
        public AntState State
        {
            private set
            {
                switch (value)
                {
                    case AntState.AroundingBait:
                        m_aroundTimer = 0;
                        break;
                    case AntState.TowardsBait:
                        m_aroundTimer = 0;
                        break;
                }
                m_antState = value;
            }
            get => m_antState;
        }

        private AntState m_antState = AntState.AroundingBornPoint;
        public void ResetAnt()
        {
            Bait = null;
            State = AntState.AroundingBornPoint;
        }

        private float GetRandomSpeed()
        {
            return Random.Range(MinAntsSpeed, MaxAntsSpeed);
        }

        private float m_aroundTimer;
        private Vector2 m_arroundDirection;
        private float m_forwardTimer;
        public Vector2 m_forwardDirection;
        private float m_curVelocity;

        public void TowardsBait(Bait.Bait bait)
        {
            Bait = bait;
            State = AntState.TowardsBait;
            var randomPos = bait.GetRandomPoint();
            m_forwardDirection = (randomPos - transform.position).normalized;
            m_curVelocity = GetRandomSpeed();
            SinglePathDuration = Vector3.Distance(randomPos, transform.position) / m_curVelocity;
            m_forwardTimer = 0;
        }

        private void FixedUpdate()
        {
            switch (State)
            {
                case AntState.AroundingBait:
                    m_aroundTimer += Time.fixedDeltaTime;
                    if (m_aroundTimer >= SinglePathDuration)
                    {
                        m_aroundTimer = 0;
                        Vector3 tempPos;
                        if (Bait != null)
                        {
                            tempPos = Bait.GetRandomPoint();
                        }
                        else
                        {
                            tempPos = GameSceneManager.Instance.AntManager.GetRandomPoint();
                            State = AntState.TowardsBornPoint;
                        }

                        m_arroundDirection = (tempPos - transform.position).normalized;
                        m_curVelocity = GetRandomSpeed();
                        SinglePathDuration = Vector3.Distance(tempPos, transform.position) / m_curVelocity;
                    }
                    else
                    {
                        transform.position += (Vector3)m_arroundDirection * (m_curVelocity * Time.fixedDeltaTime);
                    }
                    break;
                case AntState.TowardsBait:
                    m_forwardTimer += Time.fixedDeltaTime;
                    if (Bait != null)
                    {
                        if (m_forwardTimer >= SinglePathDuration)
                        {
                            State = AntState.AroundingBait;
                            m_aroundTimer = float.MaxValue;
                        }
                        else
                        {
                            transform.position += (Vector3) m_forwardDirection * (m_curVelocity * Time.fixedDeltaTime);
                        }
                    }
                    else
                    {
                        State = AntState.TowardsBornPoint;
                        m_forwardTimer = 0;
                        var pos = GameSceneManager.Instance.AntManager.GetRandomPoint();
                        m_forwardDirection = (pos - transform.position).normalized;
                        m_curVelocity = GetRandomSpeed();
                        SinglePathDuration = Vector3.Distance(pos, transform.position) / m_curVelocity;
                    }
                    break;
                case AntState.TowardsBornPoint:
                    m_forwardTimer += Time.fixedDeltaTime;
                    if (m_forwardTimer >= SinglePathDuration)
                    {
                        State = AntState.AroundingBornPoint;
                    }
                    else
                    {
                        transform.position += (Vector3) m_forwardDirection * (m_curVelocity * Time.fixedDeltaTime);
                    }
                    break;
                case AntState.AroundingBornPoint:
                    m_aroundTimer += Time.fixedDeltaTime;
                    if (m_aroundTimer >= SinglePathDuration)
                    {
                        m_aroundTimer = 0;
                        var tempPos = GameSceneManager.Instance.AntManager.GetRandomPoint();

                        m_arroundDirection = (tempPos - transform.position).normalized;
                        m_curVelocity = GetRandomSpeed();
                        SinglePathDuration = Vector3.Distance(tempPos, transform.position) / m_curVelocity;
                    }
                    else
                    {
                        transform.position += (Vector3)m_arroundDirection * (m_curVelocity * Time.fixedDeltaTime);
                    }
                    break;
            }
        }
    }

    public enum AntState
    {
        /// <summary>
        /// 围绕着目标
        /// </summary>
        AroundingBait,
        /// <summary>
        /// 围绕出生点
        /// </summary>
        AroundingBornPoint,
        /// <summary>
        /// 向着诱饵前进
        /// </summary>
        TowardsBait,
        /// <summary>
        /// 向着出生点
        /// </summary>
        TowardsBornPoint,
    }
}