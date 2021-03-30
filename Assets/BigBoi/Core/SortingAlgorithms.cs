using System;
using System.Collections.Generic;

namespace BigBoi
{
    public static class SortingAlgorithms
    {
        #region Counting Sort
        /// <summary>
        /// Uses maths to efficiently sort objects with integer values.
        /// Works best for groups of values close in range.
        /// Requires IValue instead of IComparable since Counting sort does not do any comparisons for the sorting and instead relies on just counting numbers.
        /// IValue contains the method GetValue() which returns an int.
        /// </summary>
        public static List<T> CountingSort<T>(this List<T> _list) where T : IValue
        {
            //temporary storage list
            List<T> newList = new List<T>();

            //set min and max values to the first integer
            int min = _list[0].GetValue();
            int max = _list[0].GetValue();

            //find actual min and max values
            for (int i = 0; i < _list.Count; i++) //go through given array
            {
                newList.Add(default);

                if (_list[i].GetValue() < min) //if this integer is smaller than min
                {
                    min = _list[i].GetValue(); //set min to this integer
                }
                else if (_list[i].GetValue() > max) //if this integer is bigger than max
                {
                    max = _list[i].GetValue(); //set max to this integer
                }
            }

            max++; //this prevents index out of bounds on the "counting" for loop

            //new empty list
            List<int> valueCount = new List<int>();

            //correct length
            for (int i = 0; i < (max - min); i++)
            {
                valueCount.Add(0);
            }

            //count how many of each value
            for (int i = 0; i < _list.Count; i++)
            {
                valueCount[_list[i].GetValue() - min]++;
            }

            //add values together
            for (int i = 1; i < valueCount.Count; i++)
            {
                valueCount[i] += valueCount[i - 1];
            }

            //assign to new list
            foreach (T item in _list)
            {
                int valueCountIndex = item.GetValue() - min;
                int newListIndex = valueCount[valueCountIndex] - 1;

                newList[newListIndex] = item;
                valueCount[valueCountIndex]--;
            }

            //give list pls
            return newList;
        }
        #endregion

        #region Shell Sort
        /// <summary>
        /// Compare and sort in increments that increase in range using bit-shifting.
        /// Type must extend IComparable for the CompareTo method.
        /// </summary>
        public static List<T> ShellSort<T>(this List<T> _list) where T : IComparable
        {
            int count = _list.Count;
            int subCount = 1;

            while (subCount < (count >> 1)) //while subCount is less than half the count (rounded down)
            {
                subCount = (subCount << 1) + 1; //double the subCount and add 1, ie 1->21, 21->421 (binary: 1->11, 11->111)
            }
            //now subCount >= count (rounded down)

            while (subCount >= 1) //while subCount has value
            {
                //first iteration will not enter
                //first iteration to enter will loop about count/2 times
                for (int i = subCount; i < count; i++) //loop difference between subCount and count
                {
                    int compareIndex = i - subCount; //get the value you would from a generic for loop, ie i=0,1,2,3,etc.

                    //loop from i when j >= subCount AND
                    //comparison between items at j and compareIndex < 0 (namely compareIndex still goes before j)
                    //decrease compareIndex by subCount
                    //will not enter on first iteration
                    //on second iteration it will run once
                    //third will run twice, etc.
                    for (int j = i; j >= subCount && _list[j].CompareTo(_list[compareIndex]) < 0; compareIndex -= subCount)
                    {
                        //do the ol' switcheroo
                        T temp = _list[j];
                        _list[j] = _list[compareIndex];
                        _list[compareIndex] = temp;

                        j = compareIndex; //new j is compareIndex
                        //^ this prevents index out of bounds in the CompareTo argument
                        //since when compareIndex < 0, j is less than subCount

                    } //compareIndex decreases here
                }

                subCount >>= 1; //halve subCount and round down
            }

            return _list;
        }
        #endregion
    }
}