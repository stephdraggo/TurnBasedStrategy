using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public static class Pathfinding
    {
        /// <summary>
        /// Finds the distance between 2 tiles ignoring obstacles
        /// </summary>
        public static void DistanceBetweenTiles(Tile _tileA, Tile _tileB)
        {
            int xDistance = Mathf.Abs(_tileA.GridPosition.x - _tileB.GridPosition.x);
            int yDistance = Mathf.Abs(_tileA.GridPosition.y - _tileB.GridPosition.y);
        }
    }
}
