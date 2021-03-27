using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Enemy : Unit
    {
        public override UnitTeam GetTeam() => UnitTeam.enemy;

        public override List<Tile> EnemiesInRange() => UnitsInRange(UnitTeam.player);

        private new void Start()
        {
            GameControl.instance.AddUnit(this, UnitTeam.enemy);
            base.Start();
        }
    }
}
