using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    

    /// <summary>
    /// Singleton class that holds all the tiles and controls selections
    /// </summary>
    public class Map : MonoBehaviour
    {
        //whether the player is selecting units to move or to attack
        enum MapMode { selectMode, attackMode }
        MapMode mapMode = MapMode.selectMode;

        // size of the grid, must be equal to grid of tiles in scene
        [SerializeField] int gridSizeX, gridSizeY;
        public Vector2Int GridSize => new Vector2Int(gridSizeX, gridSizeY);
        //2D array that holds all tiles at their position
        Tile[,] tiles;
        public Tile[,] Tiles => tiles;
        //list that holds every tile
        List<Tile> tileList = new List<Tile>();
        
        //currently selected tile, null means no tile is selected
        Tile selectedTile = null;

        //list of all tiles displaying to deselect with the current tile
        List<Tile> otherDisplayedTiles = new List<Tile>();

        #region instance
        public static Map instance;
        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                SetupTiles();
            }
            else Destroy(gameObject);
        }
        #endregion

        #region tile setup
        /// <summary>
        /// Sets up the tile array based on children of the object this script is on, and initialises the tiles
        /// </summary>
        void SetupTiles()
        {
            //create tile array
            tiles = new Tile[gridSizeX, gridSizeY];
            //loop through each row
            int i = 0;
            foreach (Transform row in transform)
            {
                //loop through each tile in the row
                int j = 0;
                foreach (Transform tile in row)
                {
                    //add the tile to the 2d tile array and the list
                    Tile newTile = tile.GetComponent<Tile>();
                    tiles[j, i] = newTile;
                    tileList.Add(newTile);

                    //set the tiles positions on the grid
                    newTile.SetGridPosition(new Vector2Int(j, i));
                    j++;
                }
                i++;
            }

            //loop through every tile in the array and give it its neighbours if it has them
            for(i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (i > 0) tiles[i, j].leftTile = tiles[i - 1, j];
                    if (i < gridSizeX - 1) tiles[i, j].rightTile = tiles[i + 1, j];
                    if (j > 0) tiles[i, j].downTile = tiles[i, j - 1];
                    if (j < gridSizeY - 1) tiles[i, j].upTile = tiles[i, j + 1];
                }
            }

        }
        #endregion

        #region tile selection

        /// <summary>
        /// Performs an action based on the tile clicked on
        /// </summary>
        /// <param name="_tile">Tile that was clicked on</param>
        /// <param name="_state">Selection state of the tile that was clicked on</param>
        public void OnClickTile(Tile _tile, SelectionState _state)
        {
            //check the selection state of the tile
            switch (_state)
            {
                //not currently selected - select the tile
                case SelectionState.none:
                    //if a unit is in the tile, call ClickUnit, otherwise call ClickEmptyTile
                    if (_tile.CurrentUnit)
                    {
                        ClickUnit(_tile);
                    }
                    else ClickEmptyTile(_tile);
                    break;

                //already selected - deselect the tile
                case SelectionState.selected:
                case SelectionState.showEnemyMovement:
                    Deselect();
                    break;

                //tile showing movement, move the current unit to the tile
                case SelectionState.ShowUnitSelection:
                case SelectionState.showMovement:
                    ClickMovementTile(_tile);
                    break;

                //tile with a unit that can be attacked, attack the unit
                case SelectionState.showAttack:
                    ClickAttackTile(_tile);
                    break;
            }
        }

        #endregion

        #region deselecting tile

        /// <summary>
        /// Selects a tile
        /// </summary>
        /// <param name="_tile">Tile to select</param>
        void ClickEmptyTile(Tile _tile)
        {
            //deselect current tile
            Deselect();

            //select tile
            selectedTile = _tile;
            _tile.SetSelectionState(SelectionState.selected);
        }

        /// <summary>
        /// Deselects all tiles
        /// </summary>
        public void Deselect()
        {
            //deselect the current tile
            if (selectedTile) selectedTile.SetSelectionState(SelectionState.none);

            //if other tiles are showing, deselect them
            if (otherDisplayedTiles.Count > 0)
            {
                foreach (Tile tile in otherDisplayedTiles) tile.SetSelectionState(SelectionState.none);
                otherDisplayedTiles.Clear();
            }

            //if the mode is attack mode, set the mode back and end the current units action
            if (mapMode == MapMode.attackMode)
            {
                mapMode = MapMode.selectMode;

                if (selectedTile.CurrentUnit) selectedTile.CurrentUnit.EndAction();
            }


            selectedTile = null;
        }

        #endregion

        #region select unit
        /// <summary>
        /// Selects a unit
        /// </summary>
        /// <param name="_tile">Tile containing the unit</param>
        void ClickUnit(Tile _tile)
        {
            //deselect current tile
            Deselect();

            //check if the unit is on the player team
            bool isPlayerUnit = _tile.CurrentUnit.GetTeam() == UnitTeam.player;

            //if the unit is on the player team and has already acted, instead just select the tile
            if (isPlayerUnit && _tile.CurrentUnit.Acted)
            {
                ClickEmptyTile(_tile);
                return;
            }

            //select tile
            selectedTile = _tile;

            //set the tile the unit is on to display based on whether the unit is on the player team
            _tile.SetSelectionState(isPlayerUnit ? SelectionState.ShowUnitSelection : SelectionState.showEnemyMovement);

            //get all tiles the unit can move to, display them as moveable and add them to the other displayed tiles list
            foreach (Tile tile in _tile.CalculateUnitMovementTiles())
            {
                //if the unit is on the player team, select them as moveable tiles, otherwise just display them
                tile.SetSelectionState(isPlayerUnit ? SelectionState.showMovement : SelectionState.showEnemyMovement);
                otherDisplayedTiles.Add(tile);

                //if the unit is a player unit, show enemy tiles that can be attacked
                if (isPlayerUnit)
                {
                    //get a list of the enemies next to the tile
                    List<Tile> enemiesFound = _tile.CurrentUnit.EnemiesInRange(tile);
                    if (enemiesFound.Count > 0)
                    {
                        //if there were any, set them as attack tiles
                        foreach (Tile enemyTile in enemiesFound) 
                        { 
                            enemyTile.SetSelectionState(SelectionState.showAttack);
                            otherDisplayedTiles.Add(enemyTile);
                        }
                    }
                }
            }

            //show enemies next to the player unit that can be attacked 
            if (isPlayerUnit)
            {
                List<Tile> enemiesFound = selectedTile.CurrentUnit.EnemiesInRange();
                if (enemiesFound.Count > 0)
                {
                    foreach (Tile enemyTile in enemiesFound)
                    {
                        enemyTile.SetSelectionState(SelectionState.showAttack);
                        otherDisplayedTiles.Add(enemyTile);
                    }
                }
            }

            //fix the selection boxes
            selectedTile.FixSelectionBox();
            foreach (Tile tile in otherDisplayedTiles) tile.FixSelectionBox();

        }

        #endregion

        #region selecting movement/attack

        /// <summary>
        /// Moves the current unit to the selected tile
        /// </summary>
        /// <param name="_tile">Tile to move the unit to</param>
        void ClickMovementTile(Tile _tile)
        {
            //get the unit being moved
            Unit unit = selectedTile.CurrentUnit;

            //deselect tile
            Deselect();

            //move the unit on the currently selected tile to the tile
            unit.GoToTile(_tile);

            //get the enemies adjacent to this unit
            List<Tile> rangeTiles = unit.EnemiesInRange();
            if (rangeTiles.Count > 0)
            {
                //select the new tile the unit is at
                selectedTile = _tile;

                //set the mode to attack mode, which will mean the units turn will end when deselected
                mapMode = MapMode.attackMode;

                //there are enemies in range, so display the tiles that can be attacked
                foreach (Tile tile in rangeTiles)
                {
                    tile.SetSelectionState(SelectionState.showAttack);
                    otherDisplayedTiles.Add(tile);
                }
            }
            //there are no enemies in range, end this units action
            else unit.EndAction();
        }

        /// <summary>
        /// Attacks the unit on the given tile with the unit on the current tile
        /// </summary>
        /// <param name="_tile">Tile to attack</param>
        void ClickAttackTile(Tile _tile)
        {
            //if there is not a unit on the current tile or selected tile, deselect the tile and return
            if (!selectedTile.CurrentUnit || !_tile.CurrentUnit) 
            {
                Deselect();
                return;
            }

            //get the unit doing the attack
            Unit unit = selectedTile.CurrentUnit;

            //if the unit is not next to the enemy, move before attacking
            if (Pathfinding.DistanceBetweenTiles(selectedTile, _tile) > 1)
            {
                //Get all the tiles the unit can move to
                List<Tile> movementTiles = unit.CalculateMovementTiles();
                List<Tile> possibleAttackTiles = new List<Tile>();
                
                //Find which ones are next to the enemy
                if (movementTiles.Contains(_tile.upTile)) possibleAttackTiles.Add(_tile.upTile);
                if (movementTiles.Contains(_tile.downTile)) possibleAttackTiles.Add(_tile.downTile);
                if (movementTiles.Contains(_tile.leftTile)) possibleAttackTiles.Add(_tile.leftTile);
                if (movementTiles.Contains(_tile.rightTile)) possibleAttackTiles.Add(_tile.rightTile);

               
                if (possibleAttackTiles.Count > 0)
                {
                    Tile targetTile = possibleAttackTiles[0];

                    //Find the closest tile to the unit
                    if (possibleAttackTiles.Count > 1)
                    {
                        int minDistance = Pathfinding.DistanceBetweenTiles(selectedTile, possibleAttackTiles[0]);
                        for(int i = 1; i < possibleAttackTiles.Count; i++)
                        {
                            int tileDistance = Pathfinding.DistanceBetweenTiles(selectedTile, possibleAttackTiles[i]);
                            if (tileDistance < minDistance)
                            {
                                targetTile = possibleAttackTiles[i];
                                minDistance = tileDistance;
                            }
                        }
                    }

                    //move to it
                    unit.GoToTile(targetTile);
                } 
            }

            //attack the unit on the selected tile
            unit.AttackUnit(_tile.CurrentUnit);

            //set the mode back to select mode
            mapMode = MapMode.selectMode;

            //end the turn of the unit that attacked
            unit.EndAction();

            //deselect the tile
            Deselect();
        }

        #endregion

        #region misc tile functions

        /// <summary>
        /// Returns a random empty tile walkable on by the given unit between the given min and max positions
        /// </summary>
        public Tile GetRandomFreeTile(Unit _unit, Vector2Int _minPosition, Vector2Int _maxPosition)
        {
            if (_minPosition.x > _maxPosition.x || _minPosition.y > _maxPosition.y) return null;

            List<Tile> possibleTiles = new List<Tile>();
            for(int i = Mathf.Max(_minPosition.x, 0); i <= Mathf.Min(gridSizeX - 1, _maxPosition.x); i++)
            {
                for (int j = Mathf.Max(_minPosition.y, 0); j <= Mathf.Min(gridSizeY - 1, _maxPosition.y); j++)
                {
                    if (tiles[i, j].CurrentUnit == null && tiles[i, j].IsTileWalkable(_unit))
                        possibleTiles.Add(tiles[i, j]);
                }
            }
            if (possibleTiles.Count == 0) return null;
            return possibleTiles[Random.Range(0, possibleTiles.Count)];
        }
        /// <summary>
        /// Resets the previous tile and step check so the tile can be used for a new path
        /// </summary>
        public void ResetTilePathData()
        {
            foreach (Tile _tile in tileList) 
            { 
                _tile.stepCheck = 0;
                _tile.previousTile = null;
            }
        }

        #endregion
    }
}

