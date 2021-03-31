using UnityEngine;
using UnityEngine.UI;

namespace BigBoi.OptionsSystem
{
    [AddComponentMenu("BigBoi/Options Menu System/Fullscreen Toggle")]
    [RequireComponent(typeof(Toggle))]

    public class FullscreenToggle : MonoBehaviour
    {
        private Toggle toggle;
        private string saveName;

        void Start()
        {
            saveName = "FullscreenToggle"; //generate save name

            toggle = GetComponent<Toggle>(); //connect to own toggle

            toggle.onValueChanged.AddListener(Maximise); //add method to event group

            if (PlayerPrefs.HasKey(saveName)) //if key saved
            {
                if (PlayerPrefs.GetInt(saveName) == 1) //if maximise
                {
                    Screen.fullScreen = true; //fullscreen true
                    toggle.isOn = true; //toggle display is on
                }
                else //if not maximise
                {
                    Screen.fullScreen = false; //fullscreen false
                    toggle.isOn = false; //toggle display is off
                }
            }
            else
            {
                Maximise(true); //if no key saved, save default fullscreen
                toggle.isOn = true; //toggle display is on
            }
        }

        void Maximise(bool _maximise)
        {
            Screen.fullScreen = _maximise; //change fullscreen
            if (_maximise) //if fullscreen
            {
                PlayerPrefs.SetInt(saveName, 1); //save as 1
            }
            else
            {
                PlayerPrefs.SetInt(saveName, 0); //save as 0
            }
        }
    }
}