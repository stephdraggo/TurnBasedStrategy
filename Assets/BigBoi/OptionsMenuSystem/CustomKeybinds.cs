using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BigBoi.OptionsSystem
{
    [AddComponentMenu("BigBoi/Options Menu System/Custom Keybinds")]
    public class CustomKeybinds : MonoBehaviour
    {
        [Serializable]
        public struct KeyBind
        {
            public string actionName;
            public string saveName => actionName + "KeyBind";
            public KeyCode key;
            public string keyName => key.ToString();

            //do not want these to display on inspector
            public GameObject KeySet { get => keySet; set => keySet = value; }
            private GameObject keySet;
            public Button KeyButton { get => keyButton; set => keyButton = value; }
            private Button keyButton;
            public Text DisplayText { get => displayText; set => displayText = value; }
            public Text ButtonText { get => buttonText; set => buttonText = value; }
            private Text displayText, buttonText;
            public Image KeyImage { get => keyImage; set => keyImage = value; }
            private Image keyImage;

        }

        [SerializeField, Tooltip("This prefab must follow a specific format:\n\nRoot object is a Text object - displays action name.\n\nChild object is a Button - displays key currently bound to the action.")]
        private GameObject buttonPrefab;

        [SerializeField, Tooltip("Colours to display ")]
        private Color32 baseColour, selectedColour, changedColour;

        [SerializeField, Tooltip("Implement reset button for keys?")]
        private bool includeResetButton = false;

        [SerializeField, Tooltip("Attach the button to reset the keys to their original configuration here.")]
        private Button resetButton;

        public KeyBind[] keybinds;


        private int keyCount;

        private KeyCode[] resetToTheseKeys;

        private bool waitingForInput;
        private KeyBind selectedKey;

        void Start()
        {
            keyCount = keybinds.Length;
            waitingForInput = false;
            selectedKey = keybinds[0]; //set to first keybind by default bc keybinds are not nullable, should not be accessible when not relevant

            if (TryGetComponent(out LayoutGroup _group)) //if there is a layout group attached to this object
            {
                if (!_group.isActiveAndEnabled) //if not enabled
                {
                    _group.enabled = true; //enable it

                }
            }
            else gameObject.AddComponent<VerticalLayoutGroup>(); //add a vertical layout group


            if (resetButton != null) //if reset button attached
            {
                resetButton.onClick.AddListener(ResetKeys); //add method to the button
                resetButton.GetComponentInChildren<Text>().text = "Reset Keybinds";
            }
            resetToTheseKeys = new KeyCode[keyCount]; //make array for default keys
            for (int i = 0; i < keyCount; i++) //loop through original keys (before accessing playerprefs)
            {
                resetToTheseKeys[i] = keybinds[i].key; //place default keys into array
            }


            for (int i = 0; i < keyCount; i++) //for each configurable keybind
            {
                GameObject newButton = Instantiate(buttonPrefab, transform); //generate new button

                //assign keybind struct object references
                keybinds[i].KeySet = newButton;
                keybinds[i].DisplayText = keybinds[i].KeySet.GetComponent<Text>();
                keybinds[i].KeyButton = keybinds[i].KeySet.GetComponentInChildren<Button>();
                keybinds[i].ButtonText = keybinds[i].KeyButton.GetComponentInChildren<Text>();
                keybinds[i].KeyImage = keybinds[i].KeyButton.GetComponent<Image>();

                keybinds[i].DisplayText.text = keybinds[i].actionName; //display action name
                keybinds[i].KeyImage.color = baseColour; //make sure button is base colour

                KeyBind newKeyBind = keybinds[i]; //separate parameter (this fixes index out of bounds)
                keybinds[i].KeyButton.onClick.AddListener(() => SelectKey(newKeyBind)); //add method with parameter to button



                if (PlayerPrefs.HasKey(keybinds[i].saveName)) //if this key has a saved value
                {
                    keybinds[i].key = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keybinds[i].saveName)); //change bound key to match saved key string
                }
                else
                {
                    PlayerPrefs.SetString(keybinds[i].saveName, keybinds[i].keyName); //save key as string
                }

                keybinds[i].ButtonText.text = keybinds[i].keyName; //update display
                newButton.name = keybinds[i].actionName + " Key Configure Button"; //name object in hierarchy

            }
        }

        private void OnGUI()
        {
            if (waitingForInput) //if input for key bind configuring needed
            {
                Event e = Event.current; //define event
                if (e != null) //if e is not null
                {
                    if (Input.GetMouseButton(0))
                    {
                        selectedKey.KeyImage.color = baseColour;
                        waitingForInput = false;
                        return;
                    }
                    if (e.isKey) //if event is a key press
                    {
                        ChangeKey(selectedKey, e.keyCode); //call change key with selected key and e's keycode
                    }
                }
            }
        }



        //apparently this method gets index out of bounds
        void SelectKey(KeyBind _keybind)
        {
            _keybind.KeyImage.color = selectedColour; //change colour to "selected"

            selectedKey = _keybind; //assign currently selected

            waitingForInput = true; //tell update to wait for input
        }

        void ChangeKey(KeyBind _keybind, KeyCode _newCode)
        {
            _keybind.key = _newCode; //change key

            _keybind.ButtonText.text = _keybind.keyName; //update display
            _keybind.KeyImage.color = changedColour; //change colour of button to "changed"

            PlayerPrefs.SetString(_keybind.saveName, _keybind.keyName);

            waitingForInput = false; //tell update to stop waiting for input
        }

        void ResetKeys()
        {
            for (int i = 0; i < keyCount; i++) //for every key
            {
                ChangeKey(keybinds[i], resetToTheseKeys[i]); //call change keys for the key, feeding the default keys

                keybinds[i].KeyImage.color = baseColour; //reset colour of button as well
            }
        }
    }
}