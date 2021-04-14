using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Enemy : Unit
    {
        //Boat that is using this enemy
        Boat boat;
        Tile exitTile;

        //Fish that is on this hook
        Fish fish;
        bool hasFish;

        public override UnitTeam GetTeam() => UnitTeam.enemy;
        public override UnitTeam[] GetOpposingTeams() => new UnitTeam[] { UnitTeam.player, UnitTeam.fish };

        Line line;

        #region take turn
        protected override IEnumerator TakeTurnRoutine()
        {
            bool leave = false;
            if (!hasFish)
            {
                Move();
                yield return new WaitForSeconds(TurnControl.instance.WaitTime);
            }
            else
            {
                //if on the exit tile, do not move
                if (currentTile == exitTile) leave = true;
                else
                {
                    leave = MoveTowardsBoat();
                    yield return new WaitForSeconds(TurnControl.instance.WaitTime);

                }

                //If the hook is leaving
                if (leave)
                {
                    HideUnit();
                    yield return new WaitForSeconds(TurnControl.instance.WaitTime);
                    TurnControl.instance.NextUnitMove();
                    ReturnToBoat();
                }
            }

            if (!leave)
                if (Attack()) yield return new WaitForSeconds(TurnControl.instance.WaitTime);

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
                if (tile == null) tile = GetRandomMoveableTile();

                //move to the tile if one was found
                if (tile != null) MoveToTile(tile);
        }
        #endregion

        #region Seek enemy

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

        #region move towards boat
        bool MoveTowardsBoat()
        {
            //Get a path to the exit tile
            List<Tile> path = Pathfinding.FindShortestPath(currentTile, exitTile, this, true);

            //Get a tile as far along the path as possible
            Tile targetTile = MoveAlongPath(path);

            //if there is still no tiles that can be moved to, return
            if (targetTile == null) return false;

            //if the exit tile can be reached with more movement left
            bool reachedWithExtraMoves = false;
            if (targetTile == exitTile)
            {
                reachedWithExtraMoves = Pathfinding.DistanceBetweenTiles(currentTile, targetTile) < movement;
            }

            //Go to the tile
            MoveToTile(targetTile);

            //return whether the unit can still move
            return reachedWithExtraMoves;
        }
        #endregion

        #region attacking
        private bool Attack()
        {
            List<Tile> enemiesInRange;
            if (hasFish) enemiesInRange = UnitsInRange(new UnitTeam[] { UnitTeam.player }, CurrentTile);
            else enemiesInRange = EnemiesInRange();
            
            if (enemiesInRange.Count > 0)
            {
                Unit unitToAttack = enemiesInRange[Random.Range(0, enemiesInRange.Count)].CurrentUnit;
                if (unitToAttack.GetTeam() == UnitTeam.fish && hasFish == false) CatchFish(unitToAttack as Fish);
                else AttackUnit(unitToAttack);
                return true;
            }
            else return false;
        }
        #endregion

        #region boat/fish
        public void SetBoat(Boat _boat)
        {
            boat = _boat;
            exitTile = currentTile;

            //Line connecting
            if (GetComponentInChildren<Line>())
            {
                line = GetComponentInChildren<Line>();
                line.ConnectLine(boat.LineStartPoint);
            }
        }

        void CatchFish(Fish _fish)
        {
            if (_fish.Catch(this))
            {
                hasFish = true;
                fish = _fish;

                fish.transform.SetParent(transform);

                fish.GoToTile(currentTile, true);
            }
        }

        void ReleaseFish()
        {
            if (!hasFish) return;

            fish.transform.SetParent(null);

            fish.Release(CurrentTile);

            CurrentTile.SetUnit(fish);

            hasFish = false;
            fish = null;
        }

        void ReturnToBoat()
        {
            boat.RetrieveHook();
            if (fish) fish.DestroyUnit();
            base.DestroyUnit();
        }

        public override void DestroyUnit()
        {
            if (boat) boat.RemoveHook();
            if (fish) ReleaseFish();
            base.DestroyUnit();
        }
        #endregion

    }
}
