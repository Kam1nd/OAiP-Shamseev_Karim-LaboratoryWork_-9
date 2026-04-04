using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace Lab_9_Sort
{
    public partial class AnalysisWindow : Window
    {
        public ObservableCollection<AnalysisResult> Results { get; set; } = new();

        public AnalysisWindow()
        {
            InitializeComponent();
            DgResults.ItemsSource = Results;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            Results.Clear();
            int[] sizes = { 10, 100, 150 };
            var rnd = new Random();
            var sb = new StringBuilder();

            foreach (int size in sizes)
            {
                int[] baseArray = Enumerable.Range(0, size).Select(_ => rnd.Next(-500, 500)).ToArray();

                int[] copy1 = (int[])baseArray.Clone();
                SortMetrics.Reset();
                var sw1 = Stopwatch.StartNew();
                new IntroSort().Algorithm(copy1);
                sw1.Stop();
                string introResult = $"С: {SortMetrics.ComparisonCount} П: {SortMetrics.PermutationCount} " +
                                    $"В: {sw1.ElapsedMilliseconds} мс";

                int[] copy2 = (int[])baseArray.Clone();
                SortMetrics.Reset();
                var sw2 = Stopwatch.StartNew();
                new PigeonholeSort().Algorithm(copy2);
                sw2.Stop();
                string pigeonResult = $"С: {SortMetrics.ComparisonCount} П: {SortMetrics.PermutationCount} " +
                                     $"В: {sw2.ElapsedMilliseconds} мс";

                Results.Add(new AnalysisResult
                {
                    Size = size,
                    IntroSort = introResult,
                    Pigeonhole = pigeonResult
                });
            }

            sb.AppendLine("• IntroSort эффективен для больших массивов (O(n log n)).");
            sb.AppendLine("• PigeonholeSort не использует сравнения, эффективен при малом диапазоне значений.");
            sb.AppendLine("• IntroSort требует меньше памяти, чем PigeonholeSort.");
            sb.AppendLine("• PigeonholeSort работает за O(n + range), где range - диапазон значений.");

            TxtAnalysis.Text = sb.ToString();
        }
    }

    public class AnalysisResult
    {
        public int Size { get; set; }
        public string IntroSort { get; set; }
        public string Pigeonhole { get; set; }
    }
}