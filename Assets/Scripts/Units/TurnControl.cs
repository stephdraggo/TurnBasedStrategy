using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    /// <summary>
    /// Singleton class that controls teams and turns
    /// </summary>
    public class TurnControl : MonoBehaviour
    {
        #region instance
        public static TurnControl instance;

        private void Awake()
        {
            if (!instance) instance = this;
            else Destroy(gameObject);
        }
        #endregion

        //Time to wait between each unit moving in the enemy turn
        [SerializeField] float waitTime = 0.5f;
        public float WaitTime => waitTime;

        List<Unit> playerTeam = new List<Unit>();
        List<Unit> enemyTeam = new List<Unit>();

        UnitTeam currentTurn = UnitTeam.player;

        bool waitingForMove;

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

        public void RemoveUnit(Unit _unit, UnitTeam _team)
        {
            switch (_team)
            {
                case UnitTeam.player:
                    if (playerTeam.Contains(_unit)) playerTeam.Remove(_unit);
                    break;
                case UnitTeam.enemy:
                    if (enemyTeam.Contains(_unit)) enemyTeam.Remove(_unit);
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

            if (currentTurn == UnitTeam.player)
            {
                foreach (Unit unit in playerTeam)
                {
                    unit.EndAction();
                }
                //set turn to enemy and start enemy coroutine
                currentTurn = UnitTeam.enemy;
                StartCoroutine(EnemyTurn());
            }
            else if (currentTurn == UnitTeam.enemy)
            {
                //set turn to player and reload all units
                currentTurn = UnitTeam.player;
                foreach (Unit unit in playerTeam)
                {
                    unit.StartTurn();
                }
            }
        }

        private void Start()
        {
            foreach (Unit unit in playerTeam)
            {
                unit.StartTurn();
            }
        }

        /// <summary>
        /// Call from an ai unit when it has finished moving to tell the next unit to move
        /// </summary>
        public void NextUnitMove() => waitingForMove = false;

        /// <summary>
        /// Goes through and moves every enemy and then ends the enemy turn
        /// </summary>
        /// <returns></returns>
        public IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(waitTime);

            foreach(Unit unit in enemyTeam)
            {
                waitingForMove = true;
                unit.TakeTurn();
                yield return new WaitUntil(() => !waitingForMove);
            }

            EndTurn();
        }
    }
}
