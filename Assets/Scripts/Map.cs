using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Map : MonoBehaviour
    {
        [SerializeField] int gridSizeX, gridSizeY;
        Tile[,] tiles;
        List<Tile> tileList = new List<Tile>();
        

        Tile selectedTile = null;
        List<Tile> movementTiles = new List<Tile>();

        public static Map instance;

        private void Awake()
        {
            if (!instance) instance = this;
            else Destroy(gameObject);
        }

        #region tile setup
        private void Start()
        {
            SetupTiles();
        }
        void SetupTiles()
        {
            tiles = new Tile[gridSizeX, gridSizeY];
            int i = 0;
            foreach (Transform row in transform)
            {
                int j = 0;
                foreach (Transform tile in row)
                {
                    Tile newTile = tile.GetComponent<Tile>();
                    tiles[j, i] = newTile;
                    tileList.Add(newTile);
                    j++;
                }
                i++;
            }

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

        public void OnClickTile(Tile _tile, SelectionState _state)
        {
            switch (_state)
            {
                case SelectionState.none:

                    if (_tile.CurrentUnit)
                    {
                        ClickUnit(_tile);
                    }
                    else ClickEmptyTile(_tile);

                    break;
                case SelectionState.selected:
                    Deselect();
                    break;
                case SelectionState.ShowUnitSelection:
                    Deselect();
                    break;
                case SelectionState.showMovement:
                    ClickMovementTile(_tile);
                    break;
            }
        }

        void ClickEmptyTile(Tile _tile)
        {
            Deselect();

            selectedTile = _tile;
            _tile.SetSelectionState(SelectionState.selected);
        }
        void ClickUnit(Tile _tile)
        {
            Deselect();

            selectedTile = _tile;
            _tile.SetSelectionState(SelectionState.ShowUnitSelection);

            foreach(Tile tile in _tile.CalculateUnitMovementTiles())
            {
                tile.SetSelectionState(SelectionState.showMovement);
                movementTiles.Add(tile);
            }
        }
        void ClickMovementTile(Tile _tile)
        {
            selectedTile.CurrentUnit.MoveToTile(_tile);

            Deselect();
        }

        public void Deselect()
        {
            if (selectedTile) selectedTile.SetSelectionState(SelectionState.none);

            if (movementTiles.Count > 0)
            {
                foreach (Tile tile in movementTiles) tile.SetSelectionState(SelectionState.none);
                movementTiles.Clear();
            }
            selectedTile = null;
        }

        public void ResetTileStepChecks()
        {
            foreach (Tile _tile in tileList) _tile.stepCheck = 0;
        }

        #endregion
    }
}

