using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace Lab_9_Sort
{
    public partial class MainWindow : Window
    {
        private Context _context = new Context();

        public MainWindow()
        {
            InitializeComponent();
            SortMetrics.OutputBox = RtbSteps;
        }

        private void TxtSize_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(TxtSize.Text, out int val) && val >= 5 && val <= 200)
                SliderSize.Value = val;
        }

        private void SliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtSize != null)
                TxtSize.Text = ((int)SliderSize.Value).ToString();
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            int size = (int)SliderSize.Value;
            var rnd = new Random();
            Context.array = Enumerable.Range(0, size).Select(_ => rnd.Next(-500, 500)).ToArray();
            TxtArray.Text = string.Join(" ", Context.array);
            RtbSteps.Document.Blocks.Clear();
            ResetStats();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(ofd.FileName);

                    string[] parts = content.Split(new[] { ' ', '\t', '\r', '\n', ';', ',' },
                                                   StringSplitOptions.RemoveEmptyEntries);

                    Context.array = parts.Select(int.Parse).ToArray();

                    if (Context.array.Length > 0)
                    {
                        int size = Context.array.Length;
                        if (size > 200) size = 200;
                        if (size < 5) size = 5;

                        SliderSize.Value = size;
                        TxtSize.Text = size.ToString();

                        TxtArray.Text = string.Join(" ", Context.array);

                        RtbSteps.Document.Blocks.Clear();
                        ResetStats();

                        MessageBox.Show($"Успешно загружено {Context.array.Length} элементов.", "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка чтения файла: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSort_Click(object sender, RoutedEventArgs e)
        {
            if (Context.array == null || Context.array.Length < 2)
            {
                MessageBox.Show("Массив пуст или состоит из 1 элемента!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RbIntro.IsChecked == true)
                _context = new Context(new IntroSort());
            else
                _context = new Context(new PigeonholeSort());

            TxtStepTitle.Text = $"{(RbIntro.IsChecked == true ? "IntroSort" : "PigeonholeSort")} (по убыванию):";
            SortMetrics.Reset();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            _context.ExecuteAlgorithm(true);
            sw.Stop();

            TxtResult.Text = string.Join(" ", Context.array);
            UpdateStats(SortMetrics.ComparisonCount, SortMetrics.PermutationCount, sw.ElapsedMilliseconds);
            RtbSteps.AppendText($"\n✅ Завершено. Время: {sw.ElapsedMilliseconds} мс\n");
            RtbSteps.ScrollToEnd();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Context.array = null;
            TxtArray.Text = "";
            TxtResult.Text = "";
            RtbSteps.Document.Blocks.Clear();
            TxtStepTitle.Text = "Сортировка (по убыванию):";
            ResetStats();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Context.array == null) return;

            var sfd = new SaveFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt" };
            if (sfd.ShowDialog() == true)
            {
                string finalContent = SortMetrics.FileLog +
                    $"\nОтсортированный массив:\n{string.Join(" ", Context.array)}\n" +
                    $"Количество сравнений: {SortMetrics.ComparisonCount}\n" +
                    $"Количество перестановок: {SortMetrics.PermutationCount}";

                File.WriteAllText(sfd.FileName, finalContent);
                MessageBox.Show("Файл сохранен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAnalysis_Click(object sender, RoutedEventArgs e) =>
            new AnalysisWindow().ShowDialog();

        private void UpdateStats(int c, int p, long t)
        {
            TxtComp.Text = $"Количество сравнений: {c}";
            TxtPerm.Text = $"Количество перестановок: {p}";
            TxtTime.Text = $"Время сортировки: {t} мс";
        }

        private void ResetStats() => UpdateStats(0, 0, 0);
    }
}