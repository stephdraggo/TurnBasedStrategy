using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Enemy : Unit
    {
        //Boat that is using this enemy
        Boat boat;

        public override UnitTeam GetTeam() => UnitTeam.enemy;
        public override UnitTeam[] GetOpposingTeams() => new UnitTeam[] { UnitTeam.player, UnitTeam.fish };

        #region take turn
        protected override IEnumerator TakeTurnRoutine()
        {
            Move();
            yield return new WaitForSeconds(TurnControl.instance.WaitTime);

            if (Attack())
                yield return new WaitForSeconds(TurnControl.instance.WaitTime);

            TurnControl.instance.NextUnitMove();
        }
        #endregion

        #region movement
        private void Move()
        {

            //Get a tile in range with an enemy near it
            Tile tile = GetEnemyAdjacentTile();

            //if there are none, move towards the closest enemy
            if (tile == null) tile = GetClosestTileToEnemy();
            
            //if there are no enemies, get a random tile
            if (tile == null ) tile = GetRandomMoveableTile();

            //move to the tile if one was found
            if (tile != null) MoveToTile(tile);
        }

        /// <summary>
        /// Returns a random tile with an enemy next to it, if there are none returns null
        /// </summary>
        /// <returns>Tile with an enemy adjacent to it, otherwise null</returns>
        Tile GetEnemyAdjacentTile()
        {
            if (EnemiesInRange(CurrentTile).Count > 0) return CurrentTile;

            Tile targetTile = null;

            //get all the mvoeable tiles
            List<Tile> moveableTiles = CalculateMovementTiles();

            //make a list of tiles to remove from moveable tiles
            List<Tile> removeTiles = new List<Tile>();
            foreach (Tile tile in moveableTiles)
            {
                //remove the tile if it has no enemies next to it
                if (EnemiesInRange(tile).Count == 0) removeTiles.Add(tile);
                //remove the tile if it has a unit in it already
                if (tile.CurrentUnit != null) removeTiles.Add(tile);

            }
            //remove all of those tiles from the list
            foreach (Tile tile in removeTiles) moveableTiles.Remove(tile);

            //if there are tiles with enemies next to them
            if (moveableTiles.Count > 0)
            {
                //pick a random one and move to it to attack
                targetTile = moveableTiles[Random.Range(0, moveableTiles.Count)];
            }

            return targetTile;
        }

        /// <summary>
        /// Finds the shortest path to the closest enemy unit and returns a tile along it as far as possible
        /// </summary>
        Tile GetClosestTileToEnemy()
        {
            //Find the closest unit outside of range
            Unit targetUnit = FindClosestUnit(currentTile, GetOpposingTeams());

            //if there are no units to target, return
            if (targetUnit == null) return null;

            //Find the shortest path to the unit
            List<Tile> pathTiles = Pathfinding.FindShortestPath(currentTile, targetUnit.CurrentTile, targetUnit);

            //Return a tile as far along the path as possible
            return MoveAlongPath(pathTiles);

        }

        
        /// <summary>
        /// Returns a random tile to move to
        /// </summary>
        /// <returns></returns>
        Tile GetRandomMoveableTile()
        {
            List<Tile> moveableTiles = CalculateMovementTiles();

            if (moveableTiles.Count == 0) return null;

            Tile targetTile = moveableTiles[Random.Range(0, moveableTiles.Count)];

            return targetTile;
        }
        #endregion

        #region attacking
        private bool Attack()
        {
            List<Tile> enemiesInRange = EnemiesInRange();
            if (enemiesInRange.Count > 0)
            {
                Unit unitToAttack = enemiesInRange[Random.Range(0, enemiesInRange.Count)].CurrentUnit;
                AttackUnit(unitToAttack);
                return true;
            }
            else return false;
        }
        #endregion

        public void SetBoat(Boat _boat) => boat = _boat;

        protected override void DestroyUnit()
        {
            if (boat) boat.RemoveHook();
            base.DestroyUnit();
        }

    }
}
