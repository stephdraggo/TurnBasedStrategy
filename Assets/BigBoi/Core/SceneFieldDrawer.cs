using UnityEngine;
using UnityEditor;

namespace BigBoi
{
    [CustomPropertyDrawer(typeof(SceneFieldAttribute))]
    public class SceneFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, _label, _property);

            //load current scene
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_property.stringValue);

            //check if change in inspector
            EditorGUI.BeginChangeCheck();

            //draw scene field as object field as scene asset
            var newScene = EditorGUI.ObjectField(_position, _label, oldScene, typeof(SceneAsset), false) as SceneAsset;

            //did change??
            if (EditorGUI.EndChangeCheck())
            {
                //sure did, I guess
                //set string to path of scene
                string path = AssetDatabase.GetAssetPath(newScene);
                _property.stringValue = path;

            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label) => EditorGUIUtility.singleLineHeight;
    }
}