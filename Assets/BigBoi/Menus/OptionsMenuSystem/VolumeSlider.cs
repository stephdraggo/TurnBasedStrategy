using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using NullReEx = System.NullReferenceException;

namespace BigBoi.OptionsSystem
{
    [AddComponentMenu("BigBoi/Options Menu System/Volume Slider")]
    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(EventTrigger))]
    public class VolumeSlider : MonoBehaviour
    {
        private Slider slider;
        private string saveName;

        [SerializeField]
        private AudioMixer mixer;

        [SerializeField, Tooltip("Volume range for this slider, recommended values are -20 & 0")]
        private Vector2 range = new Vector2(-20, 0);

        [SerializeField, Tooltip("Type the name of the exposed volume parameter exactly as written.")]
        private string exposedParamName;

        void Start()
        {
            if (mixer == null) throw new NullReEx("No audio mixer attached."); //if no slider
            if (string.IsNullOrEmpty(exposedParamName)) throw new NullReEx("No exposed volume parameter named."); //if no exposed parameter named

            saveName = exposedParamName + "Slider"; //generate save name

            slider = GetComponent<Slider>(); //connect to own slider

            slider.minValue = range.x; //set max
            slider.maxValue = range.y; //set min

            slider.onValueChanged.AddListener(ChangeValue); //add method to event group

            //setup on pointer up method
            EventTrigger trigger = GetComponent<EventTrigger>(); //get event trigger
            EventTrigger.Entry entry = new EventTrigger.Entry(); //create new entry
            entry.eventID = EventTriggerType.PointerUp; //assign trigger type to entry
            entry.callback.AddListener(data => SaveValue()); //add method to entry listener
            trigger.triggers.Add(entry); //attach entry to event trigger

            if (PlayerPrefs.HasKey(saveName)) //if saved key
            {
                slider.value = PlayerPrefs.GetFloat(saveName); //load key
            }
            else ChangeValue(range.y); //else set volume to max
        }

        /// <summary>
        /// Dynamically changes volume according to slider input.
        /// </summary>
        void ChangeValue(float _value)
        {
            mixer.SetFloat(exposedParamName, _value);
        }

        /// <summary>
        /// Saves volume value to playerprefs on pointer up.
        /// </summary>
        void SaveValue()
        {
            PlayerPrefs.SetFloat(saveName, slider.value);
        }
    }
}