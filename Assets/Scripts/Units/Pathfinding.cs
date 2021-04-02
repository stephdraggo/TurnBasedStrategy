using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace TurnBasedStrategy.Gameplay
{
    public static class Pathfinding
    {
        static Unit unit;
        /// <summary>
        /// Finds the distance between 2 tiles, ignoring walls
        /// </summary>
        public static int DistanceBetweenTiles(Tile _tileA, Tile _tileB)
        {
            //Find the absolute distances in x and y
            int xDistance = Mathf.Abs(_tileA.GridPosition.x - _tileB.GridPosition.x);
            int yDistance = Mathf.Abs(_tileA.GridPosition.y - _tileB.GridPosition.y);

            //add the distances together and return
            int totalDistance = xDistance + yDistance;
            return totalDistance;
        }


        /// <summary>
        /// Finds the shortest path between 2 points
        /// </summary>
        /// <param name="_tileA">Tile to start the path</param>
        /// <param name="_tileB">Goal to reach</param>
        /// <param name="_unit">Unit finding the path for, affects what tiles can be walked on</param>
        /// <returns></returns>
        public static List<Tile> FindShortestPath(Tile _tileA, Tile _tileB, Unit _unit, bool _preferVeritcal = false)
        {
            //Reset the previous tiles stored on each tile for making a new path
            Map.instance.ResetTilePathData();

            //set variables
            unit = _unit;
            Tile startTile = _tileA;
            Tile goalTile = _tileB;

            //make new lists for tiles that need to be checked and tiles that have been checked already
            List<Tile> checkTiles = new List<Tile>();
            List<Tile> reachedTiles = new List<Tile>();

            //Add the starting tile to the lists
            checkTiles.Add(startTile);
            reachedTiles.Add(startTile);

            while(checkTiles.Count > 0)
            {
                //Get the tile with the closest distance to the goal
                Tile currentTile;
                if (checkTiles.Count > 1)
                {
                    //set the first tile in the list as the closest
                    Tile minDistanceTile = checkTiles[0];
                    int minDistance = DistanceBetweenTiles(checkTiles[0], goalTile) + DistanceBetweenTiles(checkTiles[0], startTile);

                    //go through the list to find the closest tile
                    for (int i = 1; i < checkTiles.Count; i++)
                    {
                        int tileDistance = DistanceBetweenTiles(checkTiles[i], goalTile) + DistanceBetweenTiles(checkTiles[i], startTile);
                        if (tileDistance < minDistance)
                        {
                            minDistanceTile = checkTiles[i];
                            minDistance = tileDistance;
                        }
                    }

                    //set the current tile to check as the closest tile
                    currentTile = minDistanceTile;
                }
                //if there is only one tile in the list, set it
                else currentTile = checkTiles[0];

                //if the tile is the goal, end the search
                if (currentTile == goalTile) break;

                //if prefers moving vertical first, check up and down tiles before right and left
                if (_preferVeritcal)
                {
                    if (currentTile.downTile) AddTile(currentTile.downTile, currentTile, ref checkTiles, ref reachedTiles);
                    if (currentTile.upTile) AddTile(currentTile.upTile, currentTile, ref checkTiles, ref reachedTiles);
                    if (currentTile.rightTile) AddTile(currentTile.rightTile, currentTile, ref checkTiles, ref reachedTiles);
                    if (currentTile.leftTile) AddTile(currentTile.leftTile, currentTile, ref checkTiles, ref reachedTiles);
                }
                else
                {
                    if (currentTile.rightTile) AddTile(currentTile.rightTile, currentTile, ref checkTiles, ref reachedTiles);
                    if (currentTile.leftTile) AddTile(currentTile.leftTile, currentTile, ref checkTiles, ref reachedTiles);
                    if (currentTile.downTile) AddTile(currentTile.downTile, currentTile, ref checkTiles, ref reachedTiles);
                    if (currentTile.upTile) AddTile(currentTile.upTile, currentTile, ref checkTiles, ref reachedTiles);
                }
                
                

                checkTiles.Remove(currentTile);
            }

            List<Tile> path = new List<Tile>();

            if (reachedTiles.Contains(goalTile))
            {
                path.Add(_tileB);
                Tile previousTile = _tileB.previousTile;
                while (previousTile != null)
                {
                    path.Add(previousTile);
                    previousTile = previousTile.previousTile;
                }

                
            }
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Adds a tile to the checkTiles and reachedTiles lists, if it is walkable and isnt already in reachedTiles
        /// </summary>
        /// <param name="_tile">Tile to check and add to the lists</param>
        /// <param name = "_checkingFrom">Tile that is checking this tile, i.e. the tile before this one in the list</param>
        static void AddTile(Tile _tile, Tile _checkingFrom, ref List<Tile> _checkTiles, ref List<Tile> _reachedTiles)
        {
            if (_tile == null) return;
            if (_reachedTiles.Contains(_tile)) return;
            if (!_tile.IsTileWalkable(unit)) return;
            if (_tile.CurrentUnit && unit.GetOpposingTeams().Contains(_tile.CurrentUnit.GetTeam())) return;

            _tile.previousTile = _checkingFrom;

            _checkTiles.Add(_tile);
            _reachedTiles.Add(_tile);
        }

    }



}
