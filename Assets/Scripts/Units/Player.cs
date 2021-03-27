using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Player : Unit
    {
        public override UnitTeam GetTeam() => UnitTeam.player;
        public override List<Tile> EnemiesInRange() =>  UnitsInRange(UnitTeam.enemy);

        private new void Start()
        {
            GameControl.instance.AddUnit(this, UnitTeam.player);
            base.Start();
        }
    }
}
