using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace BigBoi.AI
{
    /// <summary>
    /// Easy to read editor for BasicMovement.
    /// </summary>
    [CustomEditor(typeof(FreeMovement))]
    [CanEditMultipleObjects]
    public class BasicMovementEditor : Editor
    {
        protected SerializedProperty pSpeed, pRandomiseSpeed, pRange, pSpeedChange, pInterval;

        protected AnimBool randomiseSpeed = new AnimBool();
        protected AnimBool changeOnTimed = new AnimBool();

        protected void OnEnable()
        {
            pSpeed = serializedObject.FindProperty("speed");
            pRandomiseSpeed = serializedObject.FindProperty("randomiseSpeed");
            pRange = serializedObject.FindProperty("range");
            pSpeedChange = serializedObject.FindProperty("speedChange");
            pInterval = serializedObject.FindProperty("interval");

            randomiseSpeed.value = pRandomiseSpeed.boolValue;
            randomiseSpeed.valueChanged.AddListener(Repaint);

            changeOnTimed.value = ((FreeMovement.SpeedChangeWhen)pSpeedChange.enumValueIndex) == FreeMovement.SpeedChangeWhen.OnTimedInterval;
            changeOnTimed.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //speed
            #region Speed
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Speed Values", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(pSpeed);
                EditorGUILayout.PropertyField(pRandomiseSpeed);
                //random speed
                EditorGUI.indentLevel++;
                randomiseSpeed.target = pRandomiseSpeed.boolValue;
                if (EditorGUILayout.BeginFadeGroup(randomiseSpeed.faded))
                {
                    EditorGUILayout.PropertyField(pRange);
                    EditorGUILayout.PropertyField(pSpeedChange);

                    //random change interval
                    changeOnTimed.target = ((FreeMovement.SpeedChangeWhen)pSpeedChange.enumValueIndex) == FreeMovement.SpeedChangeWhen.OnTimedInterval;
                    if (EditorGUILayout.BeginFadeGroup(changeOnTimed.faded))
                    {
                        EditorGUILayout.PropertyField(pInterval);
                    }
                    EditorGUILayout.EndFadeGroup();

                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}