using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using NullReEx = System.NullReferenceException;

namespace BigBoi.OptionsSystem
{
    [AddComponentMenu("BigBoi/Options Menu System/Mute Toggle")]
    [RequireComponent(typeof(Toggle))]
    public class MuteToggle : MonoBehaviour
    {
        private Toggle toggle;
        private string saveName;
        private float previousVolume;

        [SerializeField]
        private AudioMixer mixer;

        [SerializeField, Tooltip("Type the name of the exposed volume parameter exactly as written.")]
        private string exposedParamName;

        void Start()
        {
            if (mixer == null) throw new NullReEx("No audio mixer attached."); //if no slider
            if (string.IsNullOrEmpty(exposedParamName)) throw new NullReEx("No exposed volume parameter named."); //if no exposed parameter named

            saveName = exposedParamName + "Toggle"; //generate save name

            toggle = GetComponent<Toggle>(); //connect to own toggle

            toggle.onValueChanged.AddListener(Mute); //add method to event group

            if (PlayerPrefs.HasKey(saveName)) //if saved key
            {
                if (PlayerPrefs.GetInt(saveName) == 0) //if key muted
                {
                    toggle.isOn = true; //update display
                    Mute(true); //apply mute
                }
                else //if key not muted
                {
                    toggle.isOn = false; //update display
                    Mute(false); //apply unmute
                }
            }
        }

        /// <summary>
        /// Mutes and un-mutes
        /// Saves automatically
        /// </summary>
        void Mute(bool _muted)
        {
            if (_muted)
            {
                mixer.GetFloat(exposedParamName, out previousVolume); //save previous volume for un-muting
                mixer.SetFloat(exposedParamName, -80); //mute sound
                PlayerPrefs.SetInt(saveName, 0); //save mute
            }
            else
            {
                mixer.SetFloat(exposedParamName, previousVolume); //get previous volume
                PlayerPrefs.SetInt(saveName, 1); //save unmuted
            }
        }
    }
}