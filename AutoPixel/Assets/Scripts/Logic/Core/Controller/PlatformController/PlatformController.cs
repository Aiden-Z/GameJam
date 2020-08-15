using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Core.Controller.PlatformController
{
    public class PlatformController : MonoBehaviour
    {
        public HashSet<Transform> Targets = new HashSet<Transform>();
        public float ForceFactor;
        public float FloatingFactor;
        public Rigidbody2D Rigidbody2D;
        public int XCount;
        public int YCount;
        private Ground.Ground[][] Grounds;

        private void Awake()
        {
            var grounds = GetComponentsInChildren<Ground.Ground>();
            Grounds = new Ground.Ground[XCount][];
            for (int i = 0; i < XCount; i++)
            {
                Grounds[i] = new Ground.Ground[YCount];
            }

            foreach (var ground in grounds)
            {
                Grounds[ground.X][ground.Y] = ground;
            }
        }

        public bool IsEdge(int x, int y)
        {
            if (x <= 0 || x >= XCount - 1 || y <= 0 || y >= YCount - 1)
            {
                return true;
            }

            if (!IsValid(Grounds[x + 1][y]) || !IsValid(Grounds[x + 1][y + 1]) || !IsValid(Grounds[x - 1][y]) ||
                !IsValid(Grounds[x - 1][y - 1]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Ground.Ground GetRandomGround()
        {
            while (true)
            {
                var x = Random.Range(0, XCount);
                var y = Random.Range(0, YCount);
                if (Grounds[x][y] != null && Grounds[x][y].IsAlive)
                {
                    return Grounds[x][y];
                }
            }
        }

        public int GetConnectCount(Ground.Ground ground)
        {
            var ret = 0;
            ret += IsValid(ground.X + 1, ground.Y) ? 1 : 0;
            ret += IsValid(ground.X + 1, ground.Y + 1) ? 1 : 0;
            ret += IsValid(ground.X - 1, ground.Y) ? 1 : 0;
            ret += IsValid(ground.X - 1, ground.Y - 1) ? 1 : 0;
            return ret;
        }

        public Ground.Ground GetCanBeDestroyGround()
        {
            var list = new List<Ground.Ground>();
            foreach (var ground in Grounds)
            {
                foreach (var ground1 in ground)
                {
                    if (IsEdge(ground1.X, ground1.Y) && IsValid(ground1))
                    {
                        list.Add(ground1);
                    }
                }
            }

            var maxCount = int.MaxValue;
            Ground.Ground ret = null;
            foreach (var ground in list)
            {
                var count = GetConnectCount(ground);
                if (count < maxCount)
                {
                    ret = ground;
                    maxCount = count;
                }
            }

            return ret;
        }

        public bool IsValid(Ground.Ground ground)
        {
            if (!ground) return false;
            return ground.IsAlive;
        }

        public bool IsValid(int x, int y)
        {
            if (x <= 0 || x >= XCount || y <= 0 || y >= YCount)
            {
                return false;
            }

            return Grounds[x][y] != null && Grounds[x][y].IsAlive;
        }

        public void SetTarget(Transform target)
        {
            Targets.Add(target);
            m_isMoving = true;
        }

        public void RemoveTarget(Transform target)
        {
            Targets.Remove(target);
        }

        private bool m_isMoving = false;
        private void FixedUpdate()
        {
            if (Targets.Count == 0)
            {
                if (m_isMoving)
                {
                    Rigidbody2D.velocity = Rigidbody2D.velocity.normalized * FloatingFactor;
                    m_isMoving = false;
                }
            }
            else
            {
                //Vector2 direction = Vector2.zero;
                foreach (var target in Targets)
                {
                    //direction += (Vector2)(target.position - transform.position);
                    Rigidbody2D.AddForceAtPosition((target.position - transform.position).normalized * ForceFactor, target.position);
                }

                //direction = direction.normalized * ForceFactor;
                
                
            }
        }
    }
}