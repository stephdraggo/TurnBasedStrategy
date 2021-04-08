using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoi.AI
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("BigBoi/AI/Movement/Targetting/Random Movement With Obstacles")]
    [RequireComponent(typeof(MeshRenderer))]
    public class RandomMovementObstacles : RandomMovementFreeRange
    {
        [SerializeField, Tooltip("Obstacles")]
        protected List<MeshRenderer> obstacleMeshes = new List<MeshRenderer>();

        protected List<ObstacleBounds> obstacleBounds = new List<ObstacleBounds>();

        public struct ObstacleBounds
        {
            public Vector2 xBounds;
            public Vector2 yBounds;
            public Vector2 zBounds;
        }

        protected override void Start()
        {
            base.Start();

            foreach (MeshRenderer _obstacle in obstacleMeshes)
            {
                Vector3 offset = _obstacle.bounds.center;

                ObstacleBounds newObstacleBounds = new ObstacleBounds();

                float xSize = _obstacle.bounds.size.x * 0.5f;
                newObstacleBounds.xBounds = new Vector2(offset.x - xSize, offset.x + xSize);

                float ySize = _obstacle.bounds.size.y * 0.5f;
                newObstacleBounds.yBounds = new Vector2(offset.y - ySize, offset.y + ySize);

                float zSize = _obstacle.bounds.size.z * 0.5f;
                newObstacleBounds.zBounds = new Vector2(offset.z - zSize, offset.z + zSize);

                obstacleBounds.Add(newObstacleBounds);
            }
        }

        protected override bool CheckTarget(Vector3 _checkThis)
        {
            foreach (ObstacleBounds _bounds in obstacleBounds)
            {
                if (_checkThis.x.InRange(_bounds.xBounds) && _checkThis.z.InRange(_bounds.zBounds))
                {
                    if (yMovement)
                    {
                        if (_checkThis.y.InRange(_bounds.yBounds))
                        {
                            return false;
                        }
                        return true;
                    }

                    return false;
                }
            }

            return true;
        }
    }
}