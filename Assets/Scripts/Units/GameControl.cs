using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    /// <summary>
    /// Singleton class that controls teams and turns
    /// </summary>
    public class GameControl : MonoBehaviour
    {
        #region instance
        public static GameControl instance;

        private void Awake()
        {
            if (!instance) instance = this;
            else Destroy(gameObject);
        }
        #endregion

        List<Unit> playerTeam = new List<Unit>();
        List<Unit> enemyTeam = new List<Unit>();

        UnitTeam currentTurn = UnitTeam.player;

        /// <summary>
        /// Adds a unit to a team, call when initialising units
        /// </summary>
        /// <param name="_unit">Unit to add to team</param>
        /// <param name="_team">Team to add unit to</param>
        public void AddUnit(Unit _unit, UnitTeam _team)
        {
            switch (_team)
            {
                case UnitTeam.player:
                    playerTeam.Add(_unit);
                    break;
                case UnitTeam.enemy:
                    enemyTeam.Add(_unit);
                    break;
            }
        }

        /// <summary>
        /// Starts the turn for the next team
        /// </summary>
        public void EndTurn()
        {
            //Deselect tile
            Map.instance.Deselect();

            //lets all players move again, replace when adding enemy turn
            foreach(Unit unit in playerTeam)
            {
                unit.StartTurn();
            }
        }

        private void Start()
        {
            EndTurn();
        }

    }
}
