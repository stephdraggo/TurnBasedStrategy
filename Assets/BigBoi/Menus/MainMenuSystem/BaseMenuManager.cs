using System;
using UnityEngine;


namespace BigBoi.Menus
{
    public abstract class BaseMenuManager : MonoBehaviour
    {
        //for press any key, refer to specific script

        [SerializeField, Tooltip("Fill in the details of every panel in this scene that will be affected by this menu manager.")]
        protected PanelDetails[] panels;

        [Serializable]
        public class PanelDetails
        {
            [Tooltip("The panel object which can be enabled or disabled.")]
            public GameObject panelObject;

            [Tooltip("Should this panel start active in the hierarchy? Example: press any key screen.")]
            public bool activeOnStart;

            [Tooltip("Should this panel always be visible? Example: HUD.")]
            public bool alwaysVisible; //always active means active on start also

            [Tooltip("Which predefined type is this panel? To add or remove types, change the 'PanelTypes' enum in BaseMenuManager.cs.")]
            public PanelTypes type;
        }

        protected virtual void Start()
        {
            //enable any panels that are set to active on start
            foreach (PanelDetails _panel in panels)
            {
                if (_panel.alwaysVisible && !_panel.activeOnStart) //if a panel is set to always visible but not active on start this will fix that
                {
                    _panel.activeOnStart = true;
                }
                if (_panel.activeOnStart)
                {
                    _panel.panelObject.SetActive(true);
                }
                else
                {
                    _panel.panelObject.SetActive(false);
                }
            }
        }


        public void EnablePanelByIndex(int _index)
        {
            if (_index < panels.Length) //if index within bounds of array
            {
                DisablePanels();

                panels[_index].panelObject.SetActive(true);
            }
            else
            {
                Debug.LogError("The passed index was out of bounds of the panels array, leaving panels unchanged.");
            }
        }

        public void DisablePanels()
        {
            foreach (PanelDetails _panel in panels)
            {
                if (!_panel.alwaysVisible) //if not set to always visible, disable
                {
                    _panel.panelObject.SetActive(false);
                }
            }
        }



        public void EnablePanelByType(PanelTypes _type)
        {
            foreach (PanelDetails _panel in panels)
            {
                if (_panel.type == _type)
                {
                    DisablePanels();

                    _panel.panelObject.SetActive(true);
                    return; //if more than one of this type, only enable first one
                }
            }
        }

    }

    
    public enum PanelTypes
    {
        //press any key panel has separate script and does not get a type here
        HUD,
        Main,
        Options,
        Pause,
        Other,
    }
}