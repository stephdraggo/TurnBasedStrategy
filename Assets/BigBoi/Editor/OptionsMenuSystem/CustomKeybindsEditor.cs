using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace BigBoi.OptionsSystem
{
    [CustomEditor(typeof(CustomKeybinds))]
    public class CustomKeybindsEditor : Editor
    {
        private SerializedProperty pButtonPrefab, pBaseColour, pSelectedColour, pChangedColour, pIncludeResetButton, pResetButton, pKeybinds;

        private AnimBool resetButtonImplemented = new AnimBool();

        private void OnEnable()
        {
            //attach properties
            pButtonPrefab = serializedObject.FindProperty("buttonPrefab");
            pBaseColour = serializedObject.FindProperty("baseColour");
            pSelectedColour = serializedObject.FindProperty("selectedColour");
            pChangedColour = serializedObject.FindProperty("changedColour");
            pIncludeResetButton = serializedObject.FindProperty("includeResetButton");
            pResetButton = serializedObject.FindProperty("resetButton");
            pKeybinds = serializedObject.FindProperty("keybinds");

            resetButtonImplemented.value = pIncludeResetButton.boolValue; //align bool values
            resetButtonImplemented.valueChanged.AddListener(Repaint); //add repaint method to this bool
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //display box and instructions for button prefab
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                //instructions
                EditorGUILayout.LabelField("The button prefab must follow a specific format:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Root object is a Text object - will display action name", EditorStyles.label);
                EditorGUILayout.LabelField("Child object is a Button - will display key currently bound", EditorStyles.label);
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(pButtonPrefab);
            }
            EditorGUILayout.EndVertical();

            //display box for choosing colours
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Colours to Show Modified Keybinds", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pBaseColour);
                EditorGUILayout.PropertyField(pSelectedColour);
                EditorGUILayout.PropertyField(pChangedColour);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            //optional reset button
            EditorGUILayout.PropertyField(pIncludeResetButton);
            resetButtonImplemented.target = pIncludeResetButton.boolValue;
            if (EditorGUILayout.BeginFadeGroup(resetButtonImplemented.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pResetButton);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            //display box for choosing colours
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(pKeybinds);
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}