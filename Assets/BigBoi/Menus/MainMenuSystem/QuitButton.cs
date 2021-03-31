using UnityEngine;
using UnityEngine.UI;

namespace BigBoi.Menus
{
    [AddComponentMenu("BigBoi/Menu System/Methods/Quit")]
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(Quit);
        }

        public void Quit()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }
    }
}