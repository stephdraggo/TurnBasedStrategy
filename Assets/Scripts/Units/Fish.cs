using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Fish : Unit
    {
        [SerializeField] Tile exitTile;

        public override UnitTeam GetTeam() => UnitTeam.fish;
        public override UnitTeam[] GetOpposingTeams() => new UnitTeam[] { UnitTeam.enemy };

        public void Setup(Tile _startTile, Tile _exitTile)
        {
            startTile = _startTile;
            exitTile = _exitTile;
        }

        protected override IEnumerator TakeTurnRoutine()
        {
            bool leave;

            //if on the exit tile, do not move
            if (currentTile == exitTile) leave = true;
            else
            {
                leave = FishMove();
                yield return new WaitForSeconds(TurnControl.instance.WaitTime);

            }
            
            //If the fish is leaving, destroy it
            if (leave)
            {
                TurnControl.instance.NextUnitMove();
                DestroyUnit();
                yield break;
            }

            TurnControl.instance.NextUnitMove();

        }

        /// <summary>
        /// Moves the unit on a path towards its exit tile, returns true if it reached the exit tile with extra moves left
        /// </summary>
        bool FishMove()
        {
            //Get a path to the exit tile
            List<Tile> path = Pathfinding.FindShortestPath(currentTile, exitTile, this, true);

            //Get a tile as far along the path as possible
            Tile targetTile = MoveAlongPath(path);

            //if the exit tile can be reached with more movement left
            bool reachedWithExtraMoves = Pathfinding.DistanceBetweenTiles(currentTile, targetTile) < movement;

            //Go to the tile
            GoToTile(targetTile);

            //return whether the unit can still move
            return reachedWithExtraMoves;

        }

        


    }
}
