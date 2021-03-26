using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Enemy : Unit
    {
        public override UnitTeam GetTeam() => UnitTeam.enemy;
    }
}
