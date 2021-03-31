using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BigBoi.Menus
{
    [AddComponentMenu("BigBoi/Menu System/Methods/Load Scene")]
    [RequireComponent(typeof(Button))]
    public class LoadScene : MonoBehaviour
    {
        [SerializeField, SceneField, Tooltip("The scene this button will load.")]
        private string scene;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ChangeScene);
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene(scene);
        }
    }
}