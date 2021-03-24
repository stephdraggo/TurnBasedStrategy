using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Exception = System.Exception;

namespace TurnBasedStrategy.Gameplay
{
    public enum UnitTeam
    {
        ally,
        enemy
    }
    /// <summary>
    /// Class for each ally or enemy unit on the map
    /// </summary>
    public class Unit : MonoBehaviour
    {
        [Header("Data")]
        //which team the unit is on, units can only walk through other units on the same team
        [SerializeField] UnitTeam team = UnitTeam.ally;
        [Header("Movement")]
        //tile this unit will go to when it is created
        [SerializeField] Tile startTile;
        //how many tiles this unit can move in a turn
        [SerializeField] int movement;
        //List of all tiles this unit can walk on
        [SerializeField] List<TileType> walkableTiles = new List<TileType>() { TileType.water };
        //tile the unit is currently on
        Tile currentTile;

        private void Start()
        {
            //go to the starting tile
            if (!startTile) throw new Exception("Start tile must be set for units");
            GoToTile(startTile);
        }

        /// <summary>
        /// remove the unit from the current tile and teleport it to the new one
        /// </summary>
        /// <param name="_tile">Tile to move to</param>
        private void GoToTile(Tile _tile)
        {
            if (currentTile) currentTile.RemoveUnit();
            currentTile = _tile;
            transform.position = new Vector3(_tile.transform.position.x, _tile.transform.position.y, transform.position.z);
            _tile.SetUnit(this);
        }

        /// <summary>
        /// for now just calls go to tile, will be used for limiting how many times you can move in a turn
        /// </summary>
        /// <param name="_tile">Tile to move to</param>
        public void MoveToTile(Tile _tile)
        {
            GoToTile(_tile);
        }



        #region calculate movement
        /// <summary>
        /// finds every tile this unit can move to and returns it as a list of tiles
        /// </summary>
        /// <returns>All tiles the unit can move to</returns>
        public List<Tile> CalculateMovementTiles()
        {
            //create a new empty list for adding tiles to
            List<Tile> availableTiles = new List<Tile>();

            //reset all tiles step check to 0
            Map.instance.ResetTileStepChecks();

            //Get every tile this unit can move to from the current tile
            AddTile(ref availableTiles, currentTile, movement);

            //return the list of tiles
            return availableTiles;
        }

        /// <summary>
        /// Adds the current tile to the list if it is walkable, and then checks all adjacent tiles if the unit can still move more
        /// </summary>
        /// <param name="tileList">reference to list to add tiles to</param>
        /// <param name="_tile">Tile to check and add to the list</param>
        /// <param name="_remainingSteps">How many more tiles the unit can move</param>
        void AddTile(ref List<Tile> tileList, Tile _tile, int _remainingSteps)
        {
            //if the tile isnt walkable for this unit, return
            if (!walkableTiles.Contains(_tile.TileType)) return;

            //get the unit on the tile
            Unit tileUnit = _tile.CurrentUnit;

            //if there is no unit on this tile
            if (tileUnit == null)
            {
                //and if the tile isnt already in the list, add it
                if (!tileList.Contains(_tile)) tileList.Add(_tile);
            }
            //if there is a unit on this tile not on the same team as this one, return
            else if (tileUnit.team != this.team) return;
            //(if the tile contains an ally unit, the tile won't be added to the list but the checker will continue for tiles past the ally)

            //the stepCheck of a tile defaults at 0, so if there are not enough steps left return and dont check any further tiles
            if (_remainingSteps <= _tile.stepCheck) return;

            //set the stepCheck to how many steps are left - this way if the tile is reached again by a different path with less steps remaining
            //the neighbours dont have to be checked again, since they will be already added
            _tile.stepCheck = _remainingSteps;

            //Since there are still steps left, continue the checker for each tile adjacent to this one
            AddAdjacentTiles(ref tileList, _tile, _remainingSteps);
        }

        /// <summary>
        /// calls AddTile for each tile around this one if they exist, with 1 less step
        /// </summary>
        /// <param name="tileList">reference to list to add tiles</param>
        /// <param name="_tile">tile to check the neighbours of</param>
        /// <param name="_remainingSteps">How many more tiles the unit can move</param>
        void AddAdjacentTiles(ref List<Tile> tileList, Tile _tile, int _remainingSteps)
        {
            //decrease remainingSteps by 1, since the unit is moving a tile
            _remainingSteps -= 1;

            //if the neighbour exists on each side, add it to the list with the new value of remainingSteps
            //each neighbour tile will then call this function for it again if there are still steps remaining
            if (_tile.upTile) AddTile(ref tileList, _tile.upTile, _remainingSteps);

            if (_tile.rightTile) AddTile(ref tileList, _tile.rightTile, _remainingSteps);

            if (_tile.downTile) AddTile(ref tileList, _tile.downTile, _remainingSteps);

            if (_tile.leftTile) AddTile(ref tileList, _tile.leftTile, _remainingSteps);

        }

        

        #endregion
    }
}
