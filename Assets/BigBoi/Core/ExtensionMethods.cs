using System.Collections.Generic;

namespace BigBoi
{
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
    }
}