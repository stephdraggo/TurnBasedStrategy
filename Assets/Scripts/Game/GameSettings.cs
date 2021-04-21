using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay {
    [Serializable]
    public class GameSettings
    {
        [Header("Win Condition")]
        [SerializeField] WinCondition winCondition;
        [SerializeField] int turnSurviveGoal = 15;
        [SerializeField] int boatDefeatGoal = 3;

        [Header("Crabs")]
        [SerializeField, Tooltip("Not including king crab")] int startingCrabs = 1;


        public WinCondition WinCondition => winCondition;
        public int TurnSurviveGoal => turnSurviveGoal;
        public int BoatDefeatGoal => boatDefeatGoal;
        public int StartingCrabs => startingCrabs;


    }
}
