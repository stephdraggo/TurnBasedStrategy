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
        //list of all tiles showing where a unit can move
        List<Tile> movementTiles = new List<Tile>();

        //instance
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
                case SelectionState.ShowUnitSelection:
                case SelectionState.showEnemyMovement:
                    Deselect();
                    break;

                //tile showing movement, move the current unit to the tile
                case SelectionState.showMovement:
                    ClickMovementTile(_tile);
                    break;
            }
        }

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
        /// Selects a unit
        /// </summary>
        /// <param name="_tile">Tile containing the unit</param>
        void ClickUnit(Tile _tile)
        {
            //deselect current tile
            Deselect();

            //select tile
            selectedTile = _tile;
            //check if the unit is on the player team
            bool isPlayerUnit = selectedTile.CurrentUnit.GetTeam() == UnitTeam.player;
            //set the tile the unit is on to display based on whether the unit is on the player team
            _tile.SetSelectionState(isPlayerUnit ? SelectionState.ShowUnitSelection : SelectionState.showEnemyMovement);

            //get all tiles the unit can move to, display them as moveable and add them to the movementTiles list
            foreach(Tile tile in _tile.CalculateUnitMovementTiles())
            {
                //if the unit is on the player team, select them as moveable tiles, otherwise just display them
                tile.SetSelectionState(isPlayerUnit ? SelectionState.showMovement : SelectionState.showEnemyMovement);
                movementTiles.Add(tile);
            }
        }
        /// <summary>
        /// Moves the current unit to the selected tile
        /// </summary>
        /// <param name="_tile">Tile to move the unit to</param>
        void ClickMovementTile(Tile _tile)
        {
            //move the unit on the currently selected tile to the tile
            selectedTile.CurrentUnit.MoveToTile(_tile);

            //deselect tile
            Deselect();
        }

        /// <summary>
        /// Deselects all tiles
        /// </summary>
        public void Deselect()
        {
            //deselect the current tile
            if (selectedTile) selectedTile.SetSelectionState(SelectionState.none);

            //if movement is showing, deselect all movement tiles
            if (movementTiles.Count > 0)
            {
                foreach (Tile tile in movementTiles) tile.SetSelectionState(SelectionState.none);
                movementTiles.Clear();
            }
            selectedTile = null;
        }

        /// <summary>
        /// sets the stepCheck variable of each tile to 0 so it can be reused
        /// </summary>
        public void ResetTileStepChecks()
        {
            foreach (Tile _tile in tileList) _tile.stepCheck = 0;
        }

        #endregion
    }
}

