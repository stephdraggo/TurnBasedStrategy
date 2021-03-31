using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public class UnitHUD : MonoBehaviour
    {
        [Header("Visible elements")]
        [SerializeField] bool showHealthBar = true;
        [SerializeField] bool showActIcon = true;

        [Header("References")]
        [SerializeField] SpriteRenderer healthBar;
        [SerializeField] GameObject actIcon;

        private void Start()
        {
            if (!showHealthBar) healthBar.gameObject.SetActive(false);
            if (!showActIcon) actIcon.SetActive(false);
        }

        /// <summary>
        /// Sets how much of the health bar is filled, from 0 to 1
        /// </summary>
        /// <param name="_fillAmount">0 = empty, 1 = full</param>
        public void SetHealthBarFillAmount(float _fillAmount) => healthBar.size = new Vector2(_fillAmount, healthBar.size.y);

        /// <summary>
        /// sets the visibility of the act icon
        /// </summary>
        public void ShowActIcon(bool _show) 
        { 
            if (showActIcon) actIcon.SetActive(_show); 
        }

    }
}