using UnityEngine;
using BigBoi.Menus;

namespace TurnBasedStrategy.Menus
{
    /// <summary>
    /// Automated, just make sure the toggles are attached in order: easy, medium, hard.
    /// </summary>
    [AddComponentMenu("Turn Based Strategy/Difficulty Selection")]
    public class Difficulty : ToggleSingleSelector
    {
        /// <summary>
        /// Difficulty for current game.
        /// </summary>
        public static DifficultyLevel difficulty;

        /// <summary>
        /// Change difficulty.
        /// </summary>
        public void SelectDifficulty(DifficultyLevel _level)
        {
            difficulty = _level;
        }

        /// <summary>
        /// Add specific difficulty selection functionality and toggle "easy" on.
        /// </summary>
        protected override void ToggleActiveOnStart()
        {
            difficulty = DifficultyLevel.Easy;

            toggles[0].onValueChanged.AddListener(data => SelectDifficulty(DifficultyLevel.Easy));
            toggles[1].onValueChanged.AddListener(data => SelectDifficulty(DifficultyLevel.Medium));
            toggles[2].onValueChanged.AddListener(data => SelectDifficulty(DifficultyLevel.Hard));

            toggles[0].SetIsOnWithoutNotify(true);
        }

        /// <summary>
        /// The level difficulty.
        /// </summary>
        public enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard,
        }
    }
}