using UnityEngine;
using UnityEngine.UI;

namespace BigBoi.Menus
{
    /// <summary>
    /// For when you want the functionality of a dropdown in toggle format.
    /// Can start with no toggles active, however once toggled on, a toggle can only be toggled off when another toggle is toggled on.
    /// </summary>
    [AddComponentMenu("BigBoi/Menu System/Methods/Group Toggle Single Select")]
    public class ToggleSingleSelector : MonoBehaviour
    {
        [SerializeField, Tooltip("Toggles to affect.")]
        protected Toggle[] toggles;

        /// <summary>
        /// Store last toggled toggle.
        /// </summary>
        protected Toggle lastToggled;


        protected virtual void Start()
        {
            foreach (Toggle _toggle in toggles)
            {
                _toggle.SetIsOnWithoutNotify(false);

                _toggle.onValueChanged.AddListener(data => LastToggled(_toggle)); //each toggle must report back here when toggled
            }

            ToggleActiveOnStart();
        }

        /// <summary>
        /// Store which toggle was just toggled.
        /// Un-toggle off all toggles then toggle on the toggled toggle without notifying.
        /// Notifying would cause stack overflow here.
        /// </summary>
        /// <param name="_toggled"></param>
        protected void LastToggled(Toggle _toggled)
        {
            lastToggled = _toggled;

            foreach (Toggle _toggle in toggles)
            {
                _toggle.SetIsOnWithoutNotify(false);
            }

            _toggled.SetIsOnWithoutNotify(true);
        }

        /// <summary>
        /// Empty method for logic determining which (if any) toggle should start toggled on.
        /// </summary>
        protected virtual void ToggleActiveOnStart() { }
    }
}