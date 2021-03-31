using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Player : Unit
    {
        public override UnitTeam GetTeam() => UnitTeam.player;
        public override List<Tile> EnemiesInRange(Tile _tile) =>  UnitsInRange(UnitTeam.enemy, _tile);
        public override List<Tile> EnemiesInRange() => UnitsInRange(UnitTeam.enemy, currentTile);

        private new void Start()
        {
            TurnControl.instance.AddUnit(this, UnitTeam.player);
            base.Start();
        }
    }
}
