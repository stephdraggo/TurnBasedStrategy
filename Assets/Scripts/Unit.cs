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
    public class Unit : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] UnitTeam team = UnitTeam.ally;
        [Header("Movement")]
        [SerializeField] Tile startTile;
        [SerializeField] int movement;
        [SerializeField] List<TileType> walkableTiles = new List<TileType>() { TileType.water };
        Tile currentTile;

        private void Start()
        {
            if (!startTile) throw new Exception("Start tile must be set for units");
            GoToTile(startTile);
        }

        private void GoToTile(Tile _tile)
        {
            if (currentTile) currentTile.RemoveUnit();
            currentTile = _tile;
            transform.position = new Vector3(_tile.transform.position.x, _tile.transform.position.y, transform.position.z);
            _tile.SetUnit(this);
        }

        public void MoveToTile(Tile _tile)
        {
            GoToTile(_tile);
        }

        #region calculate movement
        public List<Tile> CalculateMovementTiles()
        {
            List<Tile> availableTiles = new List<Tile>();

            Map.instance.ResetTileStepChecks();

            AddTile(ref availableTiles, currentTile, movement);

            return availableTiles;
        }

        void AddAdjacentTiles(ref List<Tile> tileList, Tile _tile, int _remainingSteps)
        {
            _remainingSteps -= 1;

            if (_tile.upTile) AddTile(ref tileList, _tile.upTile, _remainingSteps);

            if (_tile.rightTile) AddTile(ref tileList, _tile.rightTile, _remainingSteps);

            if (_tile.downTile) AddTile(ref tileList, _tile.downTile, _remainingSteps);

            if (_tile.leftTile) AddTile(ref tileList, _tile.leftTile, _remainingSteps);

        }

        void AddTile(ref List<Tile> tileList, Tile _tile, int _remainingSteps)
        {
            if (!walkableTiles.Contains(_tile.TileType)) return;

            Unit tileUnit = _tile.CurrentUnit;

            if (tileUnit == null)
            {
                if (!tileList.Contains(_tile)) tileList.Add(_tile);
            }
            else if (tileUnit.team != this.team) return;

            if (_remainingSteps <= _tile.stepCheck) return;

            _tile.stepCheck = _remainingSteps;

            AddAdjacentTiles(ref tileList, _tile, _remainingSteps);
        }

        #endregion
    }
}
