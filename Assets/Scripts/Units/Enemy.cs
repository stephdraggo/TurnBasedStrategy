using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Enemy : Unit
    {
        public override UnitTeam GetTeam() => UnitTeam.enemy;

        public override List<Tile> EnemiesInRange(Tile _tile) => UnitsInRange(UnitTeam.player, _tile);
        public override List<Tile> EnemiesInRange() => UnitsInRange(UnitTeam.player, currentTile);

        private new void Start()
        {
            TurnControl.instance.AddUnit(this, UnitTeam.enemy);
            base.Start();
        }

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
            //Get a tile with an enemy near it
            Tile tile = GetEnemyAdjacentTile();

            //if there are none, get a random tile
            if (tile == null ) tile = GetRandomMoveableTile();

            //move to the tile if one was found
            if (tile != null) GoToTile(tile);
        }

        /// <summary>
        /// Returns a random tile with an enemy next to it, if there are none returns null
        /// </summary>
        /// <returns>Tile with an enemy adjacent to it, otherwise null</returns>
        Tile GetEnemyAdjacentTile()
        {
            Tile targetTile = null;
            //get all tiles that can be moved to
            List<Tile> moveableTiles = CalculateMovementTiles();
            //get all the moveable tiles that have no enemies next to them
            List<Tile> removeTiles = new List<Tile>();
            foreach (Tile tile in moveableTiles)
            {
                if (EnemiesInRange(tile).Count == 0)
                {
                    removeTiles.Add(tile);
                }
            }
            //remove all of those tiles from the list
            foreach (Tile tile in removeTiles) moveableTiles.Remove(tile);

            //if there are tiles with enemies next to them
            if (moveableTiles.Count > 0)
            {
                //return a random one
                targetTile = moveableTiles[Random.Range(0, moveableTiles.Count)];
            }

            return targetTile;
        }

        /// <summary>
        /// Returns a random tile to move to
        /// </summary>
        /// <returns></returns>
        Tile GetRandomMoveableTile()
        {
            List<Tile> moveableTiles = CalculateMovementTiles();
            if (moveableTiles.Count == 0) return null;

            Tile tile = moveableTiles[Random.Range(0, moveableTiles.Count)];
            return tile;
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



    }
}
