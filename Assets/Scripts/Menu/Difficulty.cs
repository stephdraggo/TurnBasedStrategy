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
        private Toggle easy, medium, hard, helpAvailable;

        void Start()
        {
            difficulty = DifficultyLevel.Easy;

            easy.isOn = true;
            medium.isOn = false;
            hard.isOn = false;
        }

        #region Canvas Methods
        public void SelectEasy(bool _isOn)
        {
            if (_isOn)
            {
                difficulty = DifficultyLevel.Easy;

                medium.isOn = false;
                hard.isOn = false;
            }
        }
        public void SelectMedium(bool _isOn)
        {
            if (_isOn)
            {
                difficulty = DifficultyLevel.Medium;

                easy.isOn = false;
                hard.isOn = false;
            }
        }
        public void SelectHard(bool _isOn)
        {
            if (_isOn)
            {
                difficulty = DifficultyLevel.Hard;

                easy.isOn = false;
                medium.isOn = false;
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