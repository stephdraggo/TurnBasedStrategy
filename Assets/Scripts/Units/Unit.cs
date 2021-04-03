using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

using Exception = System.Exception;

namespace TurnBasedStrategy.Gameplay
{
    public enum UnitTeam
    {
        player,
        enemy,
        fish
    }
    /// <summary>
    /// Class for each ally or enemy unit on the map
    /// </summary>
    public abstract class Unit : MonoBehaviour
    {
        #region variables
        public abstract UnitTeam GetTeam();
        public abstract UnitTeam[] GetOpposingTeams();

        [Header("Stats")]
        [SerializeField] float health = 5;
        [SerializeField] int attackDamage = 3;
        //how many tiles this unit can move in a turn
        [SerializeField] protected int movement = 2;

        float currentHealth;

        [Header("Walkable Tiles")]
        //List of all tiles this unit can walk on
        [SerializeField] List<TileType> walkableTiles = new List<TileType>() { TileType.water };
        public List<TileType> WalkableTiles => walkableTiles;
        //tile the unit is currently on
        protected Tile currentTile;
        public Tile CurrentTile => currentTile;

        UnitHUD unitHUD;

        public bool Acted { get; private set; }

        #endregion

        #region setup
        /// <summary>
        /// Initiates the unit and puts it in the start position
        /// </summary>
        public virtual void Setup(int _startTileX, int _startTileY)
        {
            currentHealth = health;

            unitHUD = GetComponentInChildren<UnitHUD>();
            unitHUD.SetHealthBarFillAmount(1);

            GoToStartTile(_startTileX, _startTileY);
        }

        /// <summary>
        /// Moves the unit to its starting position
        /// </summary>
        void GoToStartTile(int _startTileX, int _startTileY)
        {
           
            //if the starting point is off the map, put it back on
            if (_startTileX < 0) _startTileX = 0;
            else if (_startTileX >= Map.instance.GridSize.x) _startTileX = Map.instance.GridSize.x - 1;
            if (_startTileY < 0) _startTileY = 0;
            else if (_startTileY >= Map.instance.GridSize.y) _startTileY = Map.instance.GridSize.y - 1;

            //set the starting tile
            Tile startTile = Map.instance.Tiles[_startTileX, _startTileY];

            //go to starting tile
            GoToTile(startTile);
        }
        #endregion

        #region action

        /// <summary>
        /// Allows this unit to act
        /// </summary>
        public void StartTurn()
        {
            Acted = false;
            unitHUD.ShowActIcon(true);
        }

        /// <summary>
        /// Stops this unit from acting
        /// </summary>
        public void EndAction()
        {
            Acted = true;
            unitHUD.ShowActIcon(false);
        }
        #endregion

        #region movement

        /// <summary>
        /// remove the unit from the current tile and teleport it to the new one
        /// </summary>
        /// <param name="_tile">Tile to move to</param>
        public void GoToTile(Tile _tile)
        {
            if (currentTile) currentTile.RemoveUnit();
            currentTile = _tile;
            transform.position = new Vector3(_tile.transform.position.x, _tile.transform.position.y, transform.position.z);
            _tile.SetUnit(this);
        }


        #endregion

        #region calculate moveable tiles
        /// <summary>
        /// finds every tile this unit can move to and returns it as a list of tiles
        /// </summary>
        /// <returns>All tiles the unit can move to</returns>
        public List<Tile> CalculateMovementTiles()
        {
            //create a new empty list for adding tiles to
            List<Tile> availableTiles = new List<Tile>();

            //reset all tiles step check to 0
            Map.instance.ResetTilePathData();

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
            if (!_tile.IsTileWalkable(this)) return;

            //get the unit on the tile
            Unit tileUnit = _tile.CurrentUnit;

            //if there is no unit on this tile
            if (tileUnit == null)
            {
                //and if the tile isnt already in the list, add it
                if (!tileList.Contains(_tile)) tileList.Add(_tile);
            }
            //if there is a unit on this tile on an enemy team to this one, return
            else if (GetOpposingTeams().Contains(tileUnit.GetTeam())) return;

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

        #region damage
        public void AttackUnit(Unit _unit)
        {
            _unit.TakeDamage(attackDamage);
        }

        public void TakeDamage(float _damage)
        {
            //lose health
            currentHealth -= _damage;

            //update health bar
            unitHUD.SetHealthBarFillAmount(currentHealth / health);

            //if on 0 health, die
            if (currentHealth <= 0) DestroyUnit();
        }

        protected virtual void DestroyUnit()
        {
            currentTile.RemoveUnit();
            TurnControl.instance.RemoveUnit(this, GetTeam());
            Destroy(gameObject);
        }

        protected void HideUnit() { foreach (Transform child in transform) child.gameObject.SetActive(false); }

        #endregion

        #region find units

        /// <summary>
        /// Finds all enemies in range from the passed tile
        /// </summary>
        public List<Tile> EnemiesInRange(Tile _tile) => UnitsInRange(GetOpposingTeams(), _tile);
        /// <summary>
        /// Finds all enemies in range from the current tile
        /// </summary>
        public List<Tile> EnemiesInRange() => UnitsInRange(GetOpposingTeams(), currentTile);

        /// <summary>
        /// Get all the units in a given team that are adjacent to this unit
        /// </summary>
        /// <param name="_team">Team to check for</param>
        /// <param name = "_tile">Tile to check from, use CurrentTile if checking from current tile</param>
        /// <returns>List of units found</returns>
        protected List<Tile> UnitsInRange(UnitTeam[] _teams, Tile _tile)
        {
            List<Tile> unitTiles = new List<Tile>();

            if (_tile.upTile)
                if (CheckTargetTile(_tile.upTile, _teams)) 
                    unitTiles.Add(_tile.upTile);

            if (_tile.rightTile) 
                if (CheckTargetTile(_tile.rightTile, _teams)) 
                    unitTiles.Add(_tile.rightTile);

            if (_tile.downTile) 
                if (CheckTargetTile(_tile.downTile, _teams)) 
                    unitTiles.Add(_tile.downTile);

            if (_tile.leftTile) 
                if (CheckTargetTile(_tile.leftTile, _teams)) 
                    unitTiles.Add(_tile.leftTile);

            return unitTiles;
        }

        /// <summary>
        /// Returns true if a unit of the specified team is on the tile
        /// </summary>
        /// <param name="_tile">Tile to check</param>
        /// <param name="_team">Team of units to check for</param>
        bool CheckTargetTile(Tile _tile, UnitTeam[] _teams)
        {
            if (_tile.CurrentUnit == null) return false;

            bool teamInTile = false;

            foreach(UnitTeam team in _teams)
            {
                if (_tile.CurrentUnit.GetTeam() == team) teamInTile = true;
            }

            return teamInTile;
        }

        /// <summary>
        /// Returns the closest unit on a given team to a tile
        /// </summary>
        /// <param name="_tile">Tile to search from</param>
        /// <param name="_team">Team to look for units of</param>
        /// <returns></returns>
        public Unit FindClosestUnit(Tile _tile, UnitTeam[] _teams)
        {
            //Get all the units that can be targeted
            List<Unit> possibleUnits = new List<Unit>();
            foreach (UnitTeam team in _teams) possibleUnits.AddRange(TurnControl.instance.GetAllUnitsInTeam(team));


            if (possibleUnits.Count == 0) return null;
            if (possibleUnits.Count == 1) return possibleUnits[0];

            //set the default target to the first in the team
            Unit targetUnit = possibleUnits[0];
            int minDistance = Pathfinding.DistanceBetweenTiles(_tile, targetUnit.currentTile);

            //for each unit in the team, set the target to them if they are closer
            for(int i = 1; i < possibleUnits.Count; i++)
            {
                int targetDistance = Pathfinding.DistanceBetweenTiles(_tile, possibleUnits[i].currentTile);
                if (targetDistance < minDistance)
                {
                    targetUnit = possibleUnits[i];
                    minDistance = targetDistance;
                }
            }
            return targetUnit;
        }
        #endregion

        #region ai take turn
        public void TakeTurn()
        {
            StartCoroutine(TakeTurnRoutine());
        }

        protected virtual IEnumerator TakeTurnRoutine()
        {
            TurnControl.instance.NextUnitMove();
            yield return null;
        }

        /// <summary>
        /// Gets a tile as far along a given path as possible
        /// </summary>
        /// <param name="_path">Path to try and move along, starting at this unit</param>
        protected Tile MoveAlongPath(List<Tile> _path)
        {
            Tile targetTile = null;

            int moveTiles = movement;
            //movement should not go further than the path length
            if (moveTiles > _path.Count - 1) moveTiles = _path.Count - 1;

            //move along the path according to how far the unit can move
            for (int i = moveTiles; i >= 0; i--)
            {
                if (_path[i].CurrentUnit == null)
                {
                    targetTile = _path[i];
                    break;
                }
            }
            return targetTile;
        }
        #endregion

    }
}
