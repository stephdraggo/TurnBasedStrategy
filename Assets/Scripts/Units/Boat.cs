using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] int hookCount = 3;
        [SerializeField] int hooksAtATime = 1;
        int hooksInMap = 0;

        [SerializeField] Transform lineStartPoint;
        public Transform LineStartPoint => lineStartPoint;

        int positionX;
        public int PositionX => positionX;

        Enemy hookPrefab;
        Tile leftTile, rightTile;
        /// <summary>
        /// Enters the map at a given x position
        /// </summary>
        /// <param name="_positionX">Left Tile for this boat to spawn in</param>
        public void Setup(int _positionX, Enemy _hookPrefab)
        {
            //Make sure the position is valid
            if (_positionX < 0 || _positionX > Map.instance.GridSize.x - 2) throw new System.Exception("Boat cannot be spawned outisde of map");

            positionX = _positionX;

            //Set the tiles the boat will be in
            leftTile = Map.instance.Tiles[_positionX, Map.instance.GridSize.y - 1];
            rightTile = Map.instance.Tiles[_positionX + 1, Map.instance.GridSize.y - 1];

            //get the hook prefab
            hookPrefab = _hookPrefab;

            //Go to the tile
            transform.position = new Vector3(leftTile.transform.position.x, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Spawns a hook in a tile beneath this boat, if one is available
        /// </summary>
        public void SpawnHook()
        {
            //if there are no hooks available return
            if (hooksInMap >= hooksAtATime || hookCount <= 0) return;

            Tile spawnTile;

            //Check whether a hook can be spawned in the left and right tiles
            bool leftAvailable = leftTile.CurrentUnit == null && leftTile.IsTileWalkable(hookPrefab);
            bool rightAvailable = rightTile.CurrentUnit == null && rightTile.IsTileWalkable(hookPrefab);

            //Set the spawn tile to either the left or right tiles if they are available, otherwise return
            if (leftAvailable)
            {
                if (rightAvailable) spawnTile = Random.Range(0f, 1f) <= 0.5f ? leftTile : rightTile;
                else spawnTile = leftTile;
            }
            else if (rightAvailable) spawnTile = rightTile;
            else return;

            //Move a hook from the onHand count to the map count
            hookCount--;
            hooksInMap++;

            //Spawn the hook
            Enemy newhook = UnitSpawner.instance.SpawnUnit(hookPrefab, spawnTile.GridPosition) as Enemy;

            newhook.SetBoat(this);

        }

        public void RetrieveHook()
        {
            //Move a hook from the map count to the onHand count
            hooksInMap--;
            hookCount++;
        }

        public void RemoveHook()
        {
            hooksInMap--;
            if (hooksInMap == 0 && hookCount == 0) LeaveMap();
        }

        void LeaveMap()
        {
            GameControl.instance.RemoveBoat(this);
            Destroy(gameObject);
        }
    }
}
