using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace Lab_9_Sort
{
    public partial class MainWindow : Window
    {
        private Context _context = new Context();
        private bool _isDataReady = false;
        private bool _isSorted = false;
        private bool _isSyncing = false; 
        public MainWindow()
        {
            InitializeComponent();
            SortMetrics.OutputBox = RtbSteps;
        }
        private void TxtSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isSyncing) ValidateSize();
        }

        private void SliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isSyncing || TxtSize == null) return;
            _isSyncing = true;
            TxtSize.Text = ((int)SliderSize.Value).ToString();
            ValidateSize(true);
            _isSyncing = false;
        }

        private void ValidateSize(bool fromSlider = false)
        {
            bool isValid = int.TryParse(TxtSize.Text, out int val) && val >= 5 && val <= 200;

            if (BtnGenerate != null)
                BtnGenerate.IsEnabled = isValid;

            if (TxtSize != null)
            {
                if (!isValid && !string.IsNullOrWhiteSpace(TxtSize.Text))
                {
                    TxtSize.BorderBrush = System.Windows.Media.Brushes.Red;

                    TxtSize.ToolTip = "Допустимый диапазон: 5 – 200";
                }

                else
                {
                    TxtSize.BorderBrush = System.Windows.SystemColors.ControlBrush;
                    TxtSize.ToolTip = null;
                }
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtSize.Text, out int size)) return;

            var rnd = new Random();
            Context.array = Enumerable.Range(0, size).Select(_ => rnd.Next(-500, 500)).ToArray();

            if (TxtArray != null) TxtArray.Text = string.Join(" ", Context.array);
            if (RtbSteps != null) RtbSteps.Document.Blocks.Clear();
            ResetStats();

            _isDataReady = true;
            _isSorted = false;
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(ofd.FileName);
                    string[] parts = content.Split(new[] { ' ', '\t', '\r', '\n', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 0)
                    {
                        MessageBox.Show("Файл пуст или не содержит данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Context.array = new int[parts.Length];
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (!int.TryParse(parts[i], out Context.array[i]))
                        {
                            throw new FormatException($"Элемент '{parts[i]}' не является целым числом.");
                        }
                    }

                    if (Context.array.Length < 2)
                    {
                        MessageBox.Show("Для сортировки нужно минимум 2 элемента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    int size = Context.array.Length;
                    if (size > 200) size = 200; 
                    if (size < 5) size = 5;

                    _isSyncing = true;
                    if (SliderSize != null) SliderSize.Value = size;
                    if (TxtSize != null) TxtSize.Text = size.ToString();
                    _isSyncing = false;

                    if (TxtArray != null) TxtArray.Text = string.Join(" ", Context.array);
                    if (RtbSteps != null) RtbSteps.Document.Blocks.Clear();
                    ResetStats();

                    _isDataReady = true;
                    _isSorted = false;
                    MessageBox.Show($"Успешно загружено {Context.array.Length} элементов.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSort_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDataReady || Context.array == null || Context.array.Length < 2)
            {
                MessageBox.Show("Массив не загружен или состоит из 1 элемента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RbIntro.IsChecked == true) _context = new Context(new IntroSort());
            else _context = new Context(new PigeonholeSort());

            if (TxtStepTitle != null)
                TxtStepTitle.Text = $"{(RbIntro.IsChecked == true ? "IntroSort" : "PigeonholeSort")} (по убыванию):";

            SortMetrics.Reset();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _context.ExecuteAlgorithm(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сортировки: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            sw.Stop();

            if (TxtResult != null) TxtResult.Text = string.Join(" ", Context.array);
            UpdateStats(SortMetrics.ComparisonCount, SortMetrics.PermutationCount, sw.ElapsedMilliseconds);
            if (RtbSteps != null)
            {
                RtbSteps.AppendText($"\n✅ Завершено. Время: {sw.ElapsedMilliseconds} мс\n");
                RtbSteps.ScrollToEnd();
            }

            _isSorted = true;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Context.array = null;
            if (TxtArray != null) TxtArray.Text = "";
            if (TxtResult != null) TxtResult.Text = "";
            if (RtbSteps != null) RtbSteps.Document.Blocks.Clear();
            if (TxtStepTitle != null) TxtStepTitle.Text = "Сортировка (по убыванию):";
            ResetStats();

            _isDataReady = false;
            _isSorted = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!_isSorted)
            {
                MessageBox.Show("Сохранение в файл не выполняется, так как массив еще не отсортирован или не внесен в поле.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var sfd = new SaveFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt" };
            if (sfd.ShowDialog() == true)
            {
                string finalContent = SortMetrics.FileLog +
                    $"\nОтсортированный массив:\n{string.Join(" ", Context.array)}\n" +
                    $"Количество сравнений: {SortMetrics.ComparisonCount}\n" +
                    $"Количество перестановок: {SortMetrics.PermutationCount}";

                File.WriteAllText(sfd.FileName, finalContent);
                MessageBox.Show("Файл успешно сохранен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAnalysis_Click(object sender, RoutedEventArgs e) => new AnalysisWindow().ShowDialog();

        private void UpdateStats(int c, int p, long t)
        {
            if (TxtComp != null) TxtComp.Text = $"Количество сравнений: {c}";
            if (TxtPerm != null) TxtPerm.Text = $"Количество перестановок: {p}";
            if (TxtTime != null) TxtTime.Text = $"Время сортировки: {t} мс";
        }

        private void ResetStats() => UpdateStats(0, 0, 0);
    }
}