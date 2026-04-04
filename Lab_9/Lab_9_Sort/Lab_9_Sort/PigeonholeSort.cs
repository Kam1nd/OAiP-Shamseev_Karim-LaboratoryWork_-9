namespace Lab_9_Sort
{
    public class PigeonholeSort : IStrategy
    {
        public int[] Algorithm(int[] mas, bool flag = true)
        {
            SortMetrics.Reset();
            if (mas == null || mas.Length < 2) return mas;

            int min = mas[0], max = mas[0];
            for (int i = 1; i < mas.Length; i++)
            {
                if (mas[i] < min) min = mas[i];
                if (mas[i] > max) max = mas[i];
                SortMetrics.ComparisonCount += 2;
            }

            int range = max - min + 1;
            int[] holes = new int[range];

            for (int i = 0; i < mas.Length; i++)
            {
                holes[mas[i] - min]++;
                SortMetrics.PermutationCount++;
            }

            int idx = 0;
            for (int i = range - 1; i >= 0; i--) 
            {
                while (holes[i] > 0)
                {
                    int val = i + min;

                    SortMetrics.LogIteration();

                    SortMetrics.LogComparison(val, i);

                    SortMetrics.LogPermutation(val, idx);

                    mas[idx] = val;

                    SortMetrics.ShowArray(mas, idx, idx);
                    SortMetrics.LogArray(mas); 

                    holes[i]--;
                    idx++;
                }
            }
            return mas;
        }
    }
}