using System.Collections.Generic;
using UnityEngine;

namespace BigBoi.AI
{
    /// <summary>
    /// Generates random targets within a range determined by mesh renderer.
    /// Tested with plane and cube, rounded objects will most likely be treated as cubes when calculating area bounds.
    /// Tickbox to allow vertical movement or keep entities grounded.
    /// Mesh renderer must exist but can be disabled.
    /// Limitations: area bounds are set at start and cannot be modified in runtime.
    /// </summary>
    [AddComponentMenu("BigBoi/AI/Movement/Targetting/Random Movement Free Range")]
    [RequireComponent(typeof(MeshRenderer))]
    public class RandomMovementFreeRange : MonoBehaviour
    {
        [SerializeField, Tooltip("The entities that will move around in this space.")]
        protected List<FreeMovement> entities;

        [SerializeField, Min(0.01f), Tooltip("The distance where the entity is 'close enough' to its target that it should choose a new target.\nIf the entites appear to jitter at points, increase this value or decrease the speed of the entities.")]
        protected float distanceRange;

        [SerializeField, Tooltip("Is vertical movement allowed? If not, the entities will keep their current y coordinate.")]
        protected bool yMovement;

        protected MeshRenderer mesh;
        protected Vector2 xBound, yBound, zBound;

        protected virtual void Start()
        {
            //get the bounds from the shape
            mesh = GetComponent<MeshRenderer>();

            Vector3 offset = mesh.bounds.center;

            float xSize = mesh.bounds.size.x * 0.5f;
            xBound = new Vector2(offset.x - xSize, offset.x + xSize);

            float ySize = mesh.bounds.size.y * 0.5f;
            yBound = new Vector2(offset.y - ySize, offset.y + ySize);

            float zSize = mesh.bounds.size.z * 0.5f;
            zBound = new Vector2(offset.z - zSize, offset.z + zSize);

            if (xBound.x == xBound.y && yBound.x == yBound.y && zBound.x == zBound.y) //check bounds are not non-existant
            {
                Debug.LogError("Mesh renderer lacks size. Min and max bounds values should not be zero.");
                enabled = false;
                return;
            }

            if (entities.Count < 1) //warn if no entities to control
            {
                Debug.LogWarning("There are no entities attached to this movement area.");
            }
            else
            {
                foreach (FreeMovement _entity in entities)
                {
                    if (_entity != null)
                    {
                        _entity.ChangeTarget(_entity.transform.position);
                    }
                    else Debug.LogError("This entity is null.");
                }
            }
        }

        protected virtual void Update()
        {
            //go through each entity and check if close enough to target
            //generate new target if needed
            foreach (FreeMovement _entity in entities)
            {
                float distance = _entity.transform.position.x - _entity.Target.x;
                distance += _entity.transform.position.z - _entity.Target.z;
                if (yMovement)
                {
                    distance += _entity.transform.position.y - _entity.Target.y;
                }
                if (Mathf.Abs(distance) < distanceRange) //is close enough to target?
                {
                    //generate new random target within area bounds
                    
                    _entity.ChangeTarget(GenerateTarget(_entity));
                }
            }
        }

        protected virtual Vector3 GenerateTarget(FreeMovement _entity)
        {
            float xTarget = xBound.RanFloat();
            float yTarget;
            float zTarget = zBound.RanFloat();
            if (yMovement)
            {
                yTarget = yBound.RanFloat();
            }
            else
            {
                yTarget = _entity.Target.y;
            }

            Vector3 generatedTarget= new Vector3(xTarget, yTarget, zTarget);

            if (CheckTarget(generatedTarget))
            {
                return generatedTarget;
            }

            return _entity.transform.position;
        }

        /// <summary>
        /// Do more to the vector3 before returning.
        /// For use in child scripts.
        /// </summary>
        protected virtual bool CheckTarget(Vector3 _checkThis)
        {
            return true;
        }

        /// <summary>
        /// Dynamically remove an entity from random movement (for example to switch state to chasing something)
        /// </summary>
        public void RemoveFromList(FreeMovement _entity)
        {
            entities.Remove(_entity);
        }

        /// <summary>
        /// Dynamically add an entity to random movement (for example to switch state to wandering)
        /// </summary>
        public void AddToList(FreeMovement _entity)
        {
            entities.Add(_entity);
        }
    }
}