using UnityEngine;

namespace BigBoi.Menus
{
    [AddComponentMenu("BigBoi/Menu System/Managers/GameScene Menu Manager")]
    public class GameSceneMenuManager : BaseMenuManager
    {
        [SerializeField, Tooltip("Which key do you want to call up the pause menu.")]
        private KeyCode keyToCallPause;

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
    }
}