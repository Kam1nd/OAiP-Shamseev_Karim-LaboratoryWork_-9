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
        public ObservableCollection<AnalysisResult> Items { get; set; } = new();

        public AnalysisWindow()
        {
            InitializeComponent();
            DgResults.ItemsSource = Items;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            Items.Clear();
            int[] sizes = { 10, 100, 150 };
            var rnd = new Random();

            foreach (int size in sizes)
            {
                int[] baseArray = Enumerable.Range(0, size).Select(_ => rnd.Next(-500, 500)).ToArray();

                int[] copy1 = (int[])baseArray.Clone();
                SortMetrics.Reset();
                var sw1 = Stopwatch.StartNew();
                new IntroSort().Algorithm(copy1);
                sw1.Stop();
                var introData = new Metrics((int)sw1.ElapsedMilliseconds, SortMetrics.ComparisonCount, SortMetrics.PermutationCount);

                int[] copy2 = (int[])baseArray.Clone();
                SortMetrics.Reset();
                var sw2 = Stopwatch.StartNew();
                new PigeonholeSort().Algorithm(copy2);
                sw2.Stop();
                var pigeonData = new Metrics((int)sw2.ElapsedMilliseconds, SortMetrics.ComparisonCount, SortMetrics.PermutationCount);

                Items.Add(new AnalysisResult
                {
                    Size = size,
                    IntroSort = $"С: {introData.Comparisons} | П: {introData.Permutations} | В: {introData.Time} мс",
                    Pigeonhole = $"С: {pigeonData.Comparisons} | П: {pigeonData.Permutations} | В: {pigeonData.Time} мс",
                    IntroData = introData,
                    PigeonData = pigeonData
                });
            }

            var sb = new StringBuilder();
            sb.AppendLine("ПОДРОБНЫЙ СРАВНИТЕЛЬНЫЙ АНАЛИЗ:");
            sb.AppendLine("--------------------------------------------------");

            int introTotalPoints = 0;
            int pigeonTotalPoints = 0;

            foreach (var item in Items)
            {
                sb.AppendLine($" Размер массива: {item.Size} элементов");
                sb.AppendLine();

                if (item.IntroData.Time < item.PigeonData.Time)
                {
                    sb.AppendLine($"   Время: IntroSort быстрее ({item.IntroData.Time} мс < {item.PigeonData.Time} мс).");
                    introTotalPoints++;
                }
                else if (item.PigeonData.Time < item.IntroData.Time)
                {
                    sb.AppendLine($"   Время: PigeonholeSort быстрее ({item.PigeonData.Time} мс < {item.IntroData.Time} мс).");
                    pigeonTotalPoints++;
                }
                else
                {
                    sb.AppendLine($"   Время: Одинаковое ({item.IntroData.Time} мс).");
                    introTotalPoints++;
                    pigeonTotalPoints++;
                }

                if (item.IntroData.Comparisons < item.PigeonData.Comparisons)
                {
                    sb.AppendLine($"   Сравнений: IntroSort эффективнее ({item.IntroData.Comparisons}).");
                    introTotalPoints++;
                }
                else if (item.PigeonData.Comparisons < item.IntroData.Comparisons)
                {
                    sb.AppendLine($"   Сравнений: PigeonholeSort эффективнее ({item.PigeonData.Comparisons}).");
                    pigeonTotalPoints++;
                }
                else
                {
                    sb.AppendLine($"   Сравнений: Одинаково ({item.IntroData.Comparisons}).");
                }

                if (item.IntroData.Permutations < item.PigeonData.Permutations)
                {
                    sb.AppendLine($"   Перестановок: IntroSort эффективнее ({item.IntroData.Permutations}).");
                    introTotalPoints++;
                }
                else if (item.PigeonData.Permutations < item.IntroData.Permutations)
                {
                    sb.AppendLine($"   Перестановок: PigeonholeSort эффективнее ({item.PigeonData.Permutations}).");
                    pigeonTotalPoints++;
                }
                else
                {
                    sb.AppendLine($"   Перестановок: Одинаково ({item.IntroData.Permutations}).");
                }

                sb.AppendLine("--------------------------------------------------");
            }

            sb.AppendLine("ОБЩИЙ ИТОГ:");
            if (introTotalPoints > pigeonTotalPoints)
                sb.AppendLine("Победитель: INTRO SORT");
            else if (pigeonTotalPoints > introTotalPoints)
                sb.AppendLine("Победитель: PIGEONHOLE SORT");
            else
                sb.AppendLine("Ничья! Методы показали равную эффективность.");

            TxtAnalysis.Text = sb.ToString();
        }
    }

    public class Metrics
    {
        public int Time { get; }
        public int Comparisons { get; }
        public int Permutations { get; }
        public Metrics(int t, int c, int p) { Time = t; Comparisons = c; Permutations = p; }
    }

    public class AnalysisResult
    {
        public int Size { get; set; }
        public string IntroSort { get; set; }
        public string Pigeonhole { get; set; }
        public Metrics IntroData { get; set; }
        public Metrics PigeonData { get; set; }
    }
}