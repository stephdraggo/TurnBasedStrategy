using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoi
{
    public static class SearchingAlgorithms
    {
        #region Linear Search
        /// <summary>
        /// Find the index of an object in a given list
        /// </summary>
        /// <returns>index of target in array</returns>
        public static int LinearSearch<T>(this T _target, List<T> _list) where T : IComparable
        {
            for (int i = 0; i < _list.Count; i++) //go through list
            {
                if ((_list[i].CompareTo(_target)) == 0) //check each item if the target item
                {
                    return i; //end method and return index
                }
            }

            //if target object does not exist in this array
            Debug.LogError("Target not found in the passed list. Returning -1.");
            return -1;
        }
        #endregion

        #region Binary Search
        /// <summary>
        /// Searches through list of objects for a specific object and returns index
        /// T must extend IComparable.
        /// </summary>
        public static int BinarySearch<T>(this T _target, List<T> _list) where T : IComparable
        {
            //declare variables
            int lowIndex = 0;
            int highIndex = _list.Count;
            int midPoint;

            while (lowIndex <= highIndex) //while within bounds of high and low indices
            {
                midPoint = (lowIndex + highIndex) / 2; //set/reset midpoint

                int comparison = _list[midPoint].CompareTo(_target); //comparing time

                if (comparison > 0) //if comparison is positive
                {
                    highIndex = midPoint - 1; //decrease high index
                }
                else if (comparison < 0) //if comparison is negative
                {
                    lowIndex = midPoint + 1; //increase low index
                }
                else //if comparison is 0
                {
                    return midPoint; //target found
                }
            }

            //if target object does not exist in this array
            Debug.LogError("Target not found in the passed list. Returning -1.");
            return -1;
        }
        #endregion
    }
}