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

        [Header("UI References")]
        [SerializeField] Button endTurnButton;
        [SerializeField] Text turnNumberText;
        [SerializeField] GameObject loadingPanel;

        List<Unit> playerTeam = new List<Unit>();
        List<Unit> enemyTeam = new List<Unit>();
        List<Unit> fishTeam = new List<Unit>();
        
        UnitTeam currentTurn = UnitTeam.player;

        int turnNumber = 1;
        public int TurnNumber => turnNumber;

        [Header("Time to wait between ai actions")]
        //Time to wait between each unit moving in the enemy turn
        [SerializeField] float waitTime = 0.5f;
        public float WaitTime => waitTime;

        bool waitingForMove;

        #region team control
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

        #endregion

        #region turn control
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
                //stop all player units from moving any more
                foreach (Unit unit in playerTeam)
                {
                    unit.EndAction();
                }

                //disable end turn button
                SetEndTurnButtonInteractable(false);

                //set turn to fish and start fish coroutine
                currentTurn = UnitTeam.fish;
                StartCoroutine(AITurn(UnitTeam.fish));
            }
            //if fish turn, start enemy turn
            else if (currentTurn == UnitTeam.fish)
            {
                //Spawn fish
                GameControl.instance.SpawnFish();

                GameControl.instance.StartEnemyTurn();

                currentTurn = UnitTeam.enemy;
                StartCoroutine(AITurn(UnitTeam.enemy));
            }
            //if enemy turn, start player turn
            else if (currentTurn == UnitTeam.enemy)
            {
                GameControl.instance.StartPlayerTurn();

                //enable end turn button
                SetEndTurnButtonInteractable(true);

                //increase the turn number
                turnNumber++;
                SetTurnNumberText();

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
            SetLoadingPanelVisibility(true);
        }

        /// <summary>
        /// Call when the units are spawned in to start the game
        /// </summary>
        public void StartGame()
        {
            SetLoadingPanelVisibility(false);

            GameControl.instance.StartPlayerTurn();

            SetTurnNumberText();

            foreach (Unit unit in playerTeam)
            {
                unit.StartTurn();
            }
        }
        #endregion

        #region ai turns

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
            yield return null;
            yield return new WaitForSeconds(waitTime);

            List<Unit> unitsInTeam = new List<Unit>();
            unitsInTeam.AddRange(GetAllUnitsInTeam(_team));
            int unitsInTeamCount = unitsInTeam.Count;

            for (int i = 0; i < unitsInTeamCount; i++)
            {
                waitingForMove = true;
                unitsInTeam[i].TakeTurn();
                yield return new WaitUntil(() => !waitingForMove);
            }

            EndTurn();
        }
        #endregion

        #region UI

        void SetLoadingPanelVisibility(bool _visible) => loadingPanel.SetActive(_visible);

        void SetEndTurnButtonInteractable(bool _interactable) => endTurnButton.interactable = _interactable;

        void SetTurnNumberText() => turnNumberText.text = "Turn " + turnNumber;

        #endregion

    }
}
