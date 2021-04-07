using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy.Menus
{
    [AddComponentMenu("Turn Based Strategy/Show Help UI")]
    [RequireComponent(typeof(Toggle))]
    public class HelpUI : MonoBehaviour
    {
        public static bool showHelpfulUI;

        private Toggle toggle;
        void Start()
        {
            toggle = GetComponent<Toggle>();

            toggle.isOn = showHelpfulUI;
        }

        public void ShowHelpfulUI(bool _show)
        {
            showHelpfulUI = _show;
        }
    }
}