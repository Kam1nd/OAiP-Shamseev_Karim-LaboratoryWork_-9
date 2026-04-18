using System;

namespace Lab_9_Sort
{
    public class IntroSort : IStrategy
    {
        public int[] Algorithm(int[] mas, bool flag = true)
        {
            SortMetrics.Reset();
            if (mas == null || mas.Length < 2) return mas;

            SortMetrics.LogArray(mas);

            int maxDepth = 2 * (int)Math.Floor(Math.Log(mas.Length, 2));
            IntroSortRecursive(mas, 0, mas.Length - 1, maxDepth);
            return mas;
        }

        private void IntroSortRecursive(int[] arr, int left, int right, int depth)
        {
            if (left >= right) return;
            if (right - left < 16) { InsertionSort(arr, left, right); return; }
            if (depth == 0) { HeapSort(arr, left, right); return; }

            int p = Partition(arr, left, right);
            IntroSortRecursive(arr, left, p - 1, depth - 1);
            IntroSortRecursive(arr, p + 1, right, depth - 1);
        }

        private int Partition(int[] arr, int left, int right)
        {
            int pivot = arr[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                SortMetrics.LogIteration();
                SortMetrics.LogComparison(arr[j], pivot);
                SortMetrics.ShowArray(arr, j, right); 

                if (arr[j] > pivot) 
                {
                    i++;
                    int temp = arr[i]; arr[i] = arr[j]; arr[j] = temp;
                    SortMetrics.LogPermutation(temp, arr[i]);
                    SortMetrics.LogArray(arr); 
                }
            }
            int tempPivot = arr[i + 1]; arr[i + 1] = arr[right]; arr[right] = tempPivot;
            SortMetrics.LogPermutation(tempPivot, arr[i + 1]);
            SortMetrics.LogArray(arr);
            return i + 1;
        }

        private void InsertionSort(int[] arr, int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                int key = arr[i];
                int j = i - 1;

                SortMetrics.LogIteration();

                while (j >= left)
                {
                    SortMetrics.LogComparison(arr[j], key);
                    SortMetrics.ShowArray(arr, j, i); 

                    if (arr[j] < key) 
                    {
                        arr[j + 1] = arr[j];
                        SortMetrics.PermutationCount++;
                        j--;
                    }
                    else break;
                }
                arr[j + 1] = key;
                SortMetrics.PermutationCount++;
                SortMetrics.LogArray(arr); 
            }
        }

        private void HeapSort(int[] arr, int left, int right)
        {
            int n = right - left + 1;
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(arr, n, i, left);

            for (int i = n - 1; i > 0; i--)
            {
                int temp = arr[left];
                arr[left] = arr[left + i];
                arr[left + i] = temp;

                SortMetrics.LogPermutation(temp, arr[left]);
                SortMetrics.LogArray(arr);

                Heapify(arr, i, 0, left);
            }
        }

        private void Heapify(int[] arr, int n, int i, int leftOffset)
        {
            int smallest = i;
            int l = 2 * i + 1;
            int r = 2 * i + 2;

            if (l < n && arr[leftOffset + l] < arr[leftOffset + smallest])
            {
                SortMetrics.LogComparison(arr[leftOffset + l], arr[leftOffset + smallest]);
                smallest = l;
            }
            if (r < n && arr[leftOffset + r] < arr[leftOffset + smallest])
            {
                SortMetrics.LogComparison(arr[leftOffset + r], arr[leftOffset + smallest]);
                smallest = r;
            }

            if (smallest != i)
            {
                int temp = arr[leftOffset + i];
                arr[leftOffset + i] = arr[leftOffset + smallest];
                arr[leftOffset + smallest] = temp;

                SortMetrics.LogPermutation(temp, arr[leftOffset + i]);
                SortMetrics.LogArray(arr); // Файл

                Heapify(arr, n, smallest, leftOffset);
            }
        }
    }
}