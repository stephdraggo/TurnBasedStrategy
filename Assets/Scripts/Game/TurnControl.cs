using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] Button endTurnButton;

        //Time to wait between each unit moving in the enemy turn
        [SerializeField] float waitTime = 0.5f;
        public float WaitTime => waitTime;

        List<Unit> playerTeam = new List<Unit>();
        List<Unit> enemyTeam = new List<Unit>();
        List<Unit> fishTeam = new List<Unit>();
        
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
                case UnitTeam.fish:
                    fishTeam.Add(_unit);
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
                case UnitTeam.fish:
                    if (fishTeam.Contains(_unit)) fishTeam.Remove(_unit);
                    break;
            }
        }

        public List<Unit> GetAllUnitsInTeam(UnitTeam _team)
        {
            switch (_team)
            {
                case UnitTeam.player: return playerTeam;
                case UnitTeam.enemy: return enemyTeam;
                case UnitTeam.fish: return fishTeam;
                default: return null;
            }
        }

        /// <summary>
        /// Starts the turn for the next team
        /// </summary>
        public void EndTurn()
        {
            //Deselect tile
            Map.instance.Deselect();

            //if player turn, start fish turn
            if (currentTurn == UnitTeam.player)
            {
                foreach (Unit unit in playerTeam)
                {
                    unit.EndAction();
                }
                //disable end turn button
                endTurnButton.interactable = false;

                //set turn to fish and start fish coroutine
                currentTurn = UnitTeam.fish;
                StartCoroutine(AITurn(UnitTeam.fish));
            }
            //if fish turn, start enemy turn
            else if (currentTurn == UnitTeam.fish)
            {
                currentTurn = UnitTeam.enemy;
                StartCoroutine(AITurn(UnitTeam.enemy));
            }
            //if enemy turn, start player turn
            else if (currentTurn == UnitTeam.enemy)
            {
                //enable end turn button
                endTurnButton.interactable = true;
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
        public IEnumerator AITurn(UnitTeam _team)
        {
            yield return new WaitForSeconds(waitTime);

            List<Unit> unitsInTeam = GetAllUnitsInTeam(_team);
            int unitsInTeamCount = GetAllUnitsInTeam(_team).Count;

            for (int i = 0; i < unitsInTeamCount; i++)
            {
                waitingForMove = true;
                unitsInTeam[i].TakeTurn();
                yield return new WaitUntil(() => !waitingForMove);
            }

            EndTurn();
        }
    }
}
