using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoi
{
    /// <summary>
    /// Some extensions and conversions I want.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Array.ToList
        /// <summary>
        /// Convert array to list.
        /// </summary>
        public static List<T> ToList<T>(this T[] _array)
        {
            return new List<T>(_array);
        }
        #endregion

        #region Random Float from Vector2
        /// <summary>
        /// Generate random float from a vector2.
        /// x does not have to be less than y.
        /// </summary>
        public static float RanFloat(this Vector2 _vector2)
        {
            if (_vector2.x < _vector2.y)
            {
                return Random.Range(_vector2.x, _vector2.y);
            }
            else if (_vector2.x > _vector2.y)
            {
                return Random.Range(_vector2.y, _vector2.x);
            }
            else return _vector2.x;
        }
        #endregion

        #region Float within range of vector2
        /// <summary>
        /// Return true if the float in within the passed range (inclusive).
        /// </summary>
        public static bool InRange(this float _float, Vector2 _range)
        {
            if (_range.x > _range.y) //if wrong way
            {
                //flip
                float temp = _range.x;
                _range.x = _range.y;
                _range.y = temp;
            }

            if (_float >= _range.x && _float <= _range.y)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}