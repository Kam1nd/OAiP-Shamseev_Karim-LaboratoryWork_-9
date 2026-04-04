using System.Windows.Controls;
using System.Windows.Documents;

namespace Lab_9_Sort
{
    public static class SortMetrics
    {
        public static int ComparisonCount { get; set; }
        public static int PermutationCount { get; set; }
        public static RichTextBox OutputBox { get; set; }
        public static string FileLog { get; set; } = "";
        public static int Iteration { get; set; } = 1;

        public static void Reset()
        {
            ComparisonCount = 0;
            PermutationCount = 0;
            FileLog = "";
            Iteration = 1;
            OutputBox?.Document.Blocks.Clear();
        }

        public static void LogIteration()
        {
            FileLog += $"{Iteration} итерация:\n";
            Iteration++;
        }

        public static void LogComparison(int first, int second)
        {
            ComparisonCount++;
            FileLog += $"Сравниваем {first} и {second}\n";
        }

        public static void LogPermutation(int first, int second)
        {
            PermutationCount++;
            FileLog += $"Перестановка {first} и {second}\n";
        }

        public static void LogArray(int[] arr)
        {
            FileLog += string.Join(" ", arr) + "\n";
        }

        public static void ShowArray(int[] arr, int idx1, int idx2)
        {
            if (OutputBox == null) return;
            string line = "";
            for (int i = 0; i < arr.Length; i++)
            {
                if (i == idx1 || i == idx2)
                    line += $"[{arr[i]}] ";
                else
                    line += $"{arr[i]} ";
            }
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(line.TrimEnd()));
            OutputBox.Document.Blocks.Add(paragraph);
            OutputBox.ScrollToEnd();
        }
    }
}