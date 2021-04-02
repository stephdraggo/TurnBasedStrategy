using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    /// <summary>
    /// Singleton class that can spawn units on the map
    /// </summary>
    public class UnitSpawner : MonoBehaviour
    {
        #region instance
        public static UnitSpawner instance;
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
        #endregion

        /// <summary>
        /// Spawns a unit at the given position
        /// </summary>
        /// <param name="_unitPrefab">Prefab holding a script that inherits from unit</param>
        /// <param name="_gridPosition">Where on the grid to spawn the unit</param>
        /// <returns> false if the tile was not valid or a unit was already there</returns>
        public bool SpawnUnit(Unit _unit, Vector2Int _gridPosition)
        {
            //check that the tile is valid
            Tile spawnTile = Map.instance.Tiles[_gridPosition.x, _gridPosition.y];
            if (!spawnTile.IsTileWalkable(_unit)) return false;
            if (spawnTile.CurrentUnit != null) return false;

            //instantiate the unit
            Unit newUnit = Instantiate(_unit, transform.position, transform.rotation);

            //setup the unit, includes connecting references and going to the start position
            newUnit.Setup(_gridPosition.x, _gridPosition.y);

            //add the unit to its team
            TurnControl.instance.AddUnit(newUnit, newUnit.GetTeam());

            return true;
        }

    }
}
