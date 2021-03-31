using System;
using UnityEngine;

namespace BigBoi
{
    [Serializable]
    public class SceneFieldAttribute : PropertyAttribute
    {
        /// <summary>
        /// Converts a full path to a scene manager friendly path for loading scene
        /// </summary>
        /// <param name="_path">original path Assets/.unity</param>
        /// <returns>friendlier path</returns>
        public static string LoadableName(string _path)
        {
            //remove these from string
            string start = "Assets/";
            string end = ".unity";

            //figure out if path has start and or end and remove them if present
            if (_path.StartsWith(start))
            {
                _path = _path.Substring(start.Length);
            }
            if (_path.EndsWith(end))
            {
                _path = _path.Substring(0, _path.LastIndexOf(end));
            }

            //return modified string
            return _path;
        }
    }
}