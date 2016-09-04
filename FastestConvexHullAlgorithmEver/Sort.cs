
#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace ConvexHull
{
    #region Parallel Sort

    public static class Sort
    {
        public static int Threshold = 150; // array length to use InsertionSort instead of SequentialQuickSort

        public static void InsertionSort<T>(T[] array, int from, int to) where T : IComparable<T>
        {
            for (int i = from + 1; i < to; i++)
            {
                var a = array[i];
                int j = i - 1;

                //while (j >= from && array[j] > a)
                while (j >= from && array[j].CompareTo(a) == -1)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = a;
            }
        }

        static void Swap<T>(T[] array, int i, int j) where T : IComparable<T>
        {
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        static int Partition<T>(T[] array, int from, int to, int pivot) where T : IComparable<T>
        {
            // Pre: from <= pivot < to (other than that, pivot is arbitrary)
            var arrayPivot = array[pivot];  // pivot value
            Swap(array, pivot, to - 1); // move pivot value to end for now, after this pivot not used
            var newPivot = from; // new pivot 
            for (int i = from; i < to - 1; i++) // be careful to leave pivot value at the end
            {
                // Invariant: from <= newpivot <= i < to - 1 && 
                // forall from <= j <= newpivot, array[j] <= arrayPivot && forall newpivot < j <= i, array[j] > arrayPivot
                //if (array[i] <= arrayPivot)
                if (array[i].CompareTo(arrayPivot) != -1)
                {
                    Swap(array, newPivot, i);  // move value smaller than arrayPivot down to newpivot
                    newPivot++;
                }
            }
            Swap(array, newPivot, to - 1); // move pivot value to its final place
            return newPivot; // new pivot
            // Post: forall i <= newpivot, array[i] <= array[newpivot]  && forall i > ...
        }

        public static void SequentialQuickSort<T>(T[] array) where T : IComparable<T>
        {
            SequentialQuickSort(array, 0, array.Length);
        }

        static void SequentialQuickSort<T>(T[] array, int from, int to) where T : IComparable<T>
        {
            if (to - from <= Threshold)
            {
                InsertionSort<T>(array, from, to);
            }
            else
            {
                int pivot = from + (to - from) / 2; // could be anything, use middle
                pivot = Partition<T>(array, from, to, pivot);
                // Assert: forall i < pivot, array[i] <= array[pivot]  && forall i > ...
                SequentialQuickSort(array, from, pivot);
                SequentialQuickSort(array, pivot + 1, to);
            }
        }

        public static void ParallelQuickSort<T>(T[] array) where T : IComparable<T>
        {
            ParallelQuickSort(array, 0, array.Length,
                 (int)Math.Log(Environment.ProcessorCount, 2) + 4);
        }

        static void ParallelQuickSort<T>(T[] array, int from, int to, int depthRemaining) where T : IComparable<T>
        {
            if (to - from <= Threshold)
            {
                InsertionSort<T>(array, from, to);
            }
            else
            {
                int pivot = from + (to - from) / 2; // could be anything, use middle
                pivot = Partition<T>(array, from, to, pivot);
                if (depthRemaining > 0)
                {
                    Parallel.Invoke(
                        () => ParallelQuickSort(array, from, pivot, depthRemaining - 1),
                        () => ParallelQuickSort(array, pivot + 1, to, depthRemaining - 1));
                }
                else
                {
                    ParallelQuickSort(array, from, pivot, 0);
                    ParallelQuickSort(array, pivot + 1, to, 0);
                }
            }
        }
    }

    #endregion
}
