using UnityEngine;
using UnityEngine.UI;

namespace BigBoi.Menus
{
    [AddComponentMenu("BigBoi/Menu System/Methods/Enable Panel by Index")]
    [RequireComponent(typeof(Button))]
    public class EnablePanelByIndex : MonoBehaviour
    {
        [SerializeField, Tooltip("The panel with this index in the menu manager will be enabled. Other panels not marked as 'Always Visible' will be disabled.")]
        private int index;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(EnablePanel);
        }

        private void EnablePanel()
        {
            FindObjectOfType<BaseMenuManager>().EnablePanelByIndex(index);
        }
    }
}