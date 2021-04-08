using UnityEngine;
using UnityEngine.Events;

namespace BigBoi.Menus
{
    [AddComponentMenu("BigBoi/Menu System/Managers/GameScene Menu Manager")]
    public class GameSceneMenuManager : BaseMenuManager
    {
        [SerializeField, Tooltip("Which key do you want to call up the pause menu.")]
        private KeyCode keyToCallPause;

        [SerializeField, Tooltip("Pause/Resume methods should be dragged here as prefab objects.")]
        private UnityEvent disableGame, enableGame;

        protected override void Start()
        {
            base.Start();
        }

        void Update()
        {
            if (Input.GetKeyDown(keyToCallPause))
            {
                EnablePanelByType(PanelTypes.Pause);
            }
        }

        /// <summary>
        /// Extra functionality added to the disable all panels method: re-enable gameplay if setup
        /// </summary>
        protected override void ExtraDisableFunctionality()
        {
            DisableGame(false);
        }

        /// <summary>
        /// Extra functionality added to the enable panel methods: disable gameplay if the passed panel requires gameplay disabled or paused (pause panel).
        /// Otherwise enables gameplay (HUD panel).
        /// </summary>
        protected override void ExtraEnableFunctionality(PanelDetails _panel)
        {
            if (_panel.disableGame)
            {
                DisableGame(true);
            }
            else DisableGame(false);
        }

        /// <summary>
        /// Method for holding pause/disable methods.
        /// </summary>
        public void DisableGame(bool _disable)
        {
            if (_disable)
            {
                disableGame.Invoke();
            }
            else
            {
                enableGame.Invoke();
            }
        }
    }
}