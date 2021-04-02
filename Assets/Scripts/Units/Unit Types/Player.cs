using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class Player : Unit
    {
        public override UnitTeam GetTeam() => UnitTeam.player;
        public override UnitTeam[] GetOpposingTeams() => new UnitTeam[] { UnitTeam.enemy };
    }
}
