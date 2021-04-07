using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy.Menus
{
    [AddComponentMenu("Turn Based Strategy/Difficulty Selection")]
    public class Difficulty : MonoBehaviour
    {
        /// <summary>
        /// Difficulty for current game.
        /// </summary>
        public static DifficultyLevel difficulty;


        [SerializeField, Tooltip("Difficulty selection toggle.")]
        private Toggle easy, medium, hard;

        void Start()
        {
            difficulty = DifficultyLevel.Easy;

            SelectEasy();
        }

        #region Canvas Methods
        public void SelectEasy()
        {
            {
                difficulty = DifficultyLevel.Easy;

                easy.isOn = true;
                medium.isOn = false;
                hard.isOn = false;
            }
        }
        public void SelectMedium()
        {
            {
                difficulty = DifficultyLevel.Medium;

                easy.isOn = false;
                medium.isOn = true;
                hard.isOn = false;
            }
        }
        public void SelectHard()
        {
            {
                difficulty = DifficultyLevel.Hard;

                easy.isOn = false;
                medium.isOn = false;
                hard.isOn = true;
            }
        }
        #endregion

        public enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard,
        }
    }
}