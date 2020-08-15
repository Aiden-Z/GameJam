using System;
using UnityEngine;

namespace Logic.Core
{
    public class CameraFollower : MonoBehaviour
    {
        public Transform Target;

        private void Update()
        {
            transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
        }
    }
}