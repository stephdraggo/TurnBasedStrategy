using UnityEngine;

namespace BigBoi.Menus
{
    [AddComponentMenu("BigBoi/Menu System/Methods/Press Any Key")]
    public class PressAnyKey : MonoBehaviour
    {
        void Update()
        {
            if (Input.anyKeyDown)
            {
                gameObject.SetActive(false);
            }
        }
    }
}