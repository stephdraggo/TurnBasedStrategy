using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Exception = System.Exception;

namespace TurnBasedStrategy.Gameplay
{
    public class Fish : Unit
    {
        Tile exitTile;

        bool caught = false;
        public bool IsCaught() => caught;
        Enemy hook;

        public override UnitTeam GetTeam() => UnitTeam.fish;
        public override UnitTeam[] GetOpposingTeams() => new UnitTeam[] { UnitTeam.enemy };

        #region setup
        public override void Setup(int _startTileX, int _startTileY)
        {
            //set exit tile and direction facing based on which side of the map the fish starts on
            int exitTileX;
            if (_startTileX < Map.instance.GridSize.x / 2)
            {
                exitTileX = Map.instance.GridSize.x - 1;
                model.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                //if moving left, flip the model
                exitTileX = 0;
                model.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
            int exitTileY = _startTileY;

            exitTile = Map.instance.Tiles[exitTileX, exitTileY];

            //If the exit tile is in a wall, find one that isn't
            if (!exitTile.IsTileWalkable(this))
            {
                int searchDirY = -1;
               
                while (exitTileY > 0)
                {
                    exitTileY += searchDirY;
                    exitTile = Map.instance.Tiles[exitTileX, exitTileY];
                    if (exitTile.IsTileWalkable(this)) break;
                }
                if (!exitTile.IsTileWalkable(this))
                {
                    searchDirY = 1;
                    while (exitTileY < Map.instance.GridSize.y - 1)
                    {
                        exitTileY += searchDirY;
                        exitTile = Map.instance.Tiles[exitTileX, exitTileY];
                        if (exitTile.IsTileWalkable(this)) break;
                    }
                }
                if (!exitTile.IsTileWalkable(this)) throw new Exception("No exit found for fish!");
            }

            base.Setup(_startTileX, _startTileY);
        }

        #endregion

        #region take turn
        protected override IEnumerator TakeTurnRoutine()
        {
            if (caught)
            {
                TurnControl.instance.NextUnitMove();
            }
            else
            {
                bool leave;

                //if on the exit tile, do not move
                if (currentTile.GridPosition.x == exitTile.GridPosition.x) leave = true;
                else
                {
                    leave = FishMove();
                    yield return new WaitForSeconds(TurnControl.instance.WaitTime);

                }

                //If the fish is leaving, destroy it
                if (leave)
                {
                    HideUnit();
                    yield return new WaitForSeconds(TurnControl.instance.WaitTime);
                    TurnControl.instance.NextUnitMove();
                    DestroyUnit();
                }
                else TurnControl.instance.NextUnitMove();
            }
           
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

            //if there is no tile that can be moved to, try moving the exit tile up or down by 1 and moving
            if (targetTile == null)
            {
                //pick between 1 and -1 based on map position
                int newDirection;
                if (exitTile.GridPosition.y == Map.instance.GridSize.y - 1) newDirection = -1;
                else if (exitTile.GridPosition.y == 0) newDirection = 1;
                else newDirection = Random.Range(0, 1) * 2 - 1;

                //move the exit tile up or down
                exitTile = Map.instance.Tiles[exitTile.GridPosition.x, exitTile.GridPosition.y + newDirection];
                //Find a new path
                path = Pathfinding.FindShortestPath(currentTile, exitTile, this, true);
                targetTile = MoveAlongPath(path);
            }

            //if there is still no tiles that can be moved to, return
            if (targetTile == null) return false;

            //if the exit tile can be reached with more movement left
            bool reachedWithExtraMoves = false;
            if (targetTile.GridPosition.x == exitTile.GridPosition.x)
            {
                reachedWithExtraMoves = Pathfinding.DistanceBetweenTiles(currentTile, targetTile) < movement;
            }
                
            //Go to the tile
            MoveToTile(targetTile);

            //return whether the unit can still move
            return reachedWithExtraMoves;

        }

        #endregion

        #region getting caught
        
        public bool Catch(Enemy _hook)
        {
            if (caught) return false;

            hook = _hook;
            caught = true;
            freeMovement.enabled = false;

            TurnControl.instance.RemoveUnit(this, UnitTeam.fish);

            return true;
        }

        public void Release(Tile _tile)
        {
            freeMovement.enabled = true;
            hook = null;
            caught = false;

            GoToTile(_tile);

            TurnControl.instance.AddUnit(this, UnitTeam.fish);
        }




        #endregion


    }
}
