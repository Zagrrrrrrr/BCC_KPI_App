using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BCC_KPI_App.Services
{
    public static class ExcelExportService
    {
        public static void ExportToExcel(DataTable data, string fileName, FrameworkElement chart1, FrameworkElement chart2, string title)
        {
            if (data == null || data.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv";
                saveDialog.FileName = $"{fileName}";
                saveDialog.Title = "Сохранить отчет";

                if (saveDialog.ShowDialog() != true) return;

                string folder = Path.GetDirectoryName(saveDialog.FileName);
                string baseName = Path.GetFileNameWithoutExtension(saveDialog.FileName);

                // Сохраняем CSV
                string csvFile = Path.Combine(folder, $"{baseName}.csv");
                var sb = new StringBuilder();

                // Заголовки
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    sb.Append(data.Columns[i].ColumnName);
                    if (i < data.Columns.Count - 1) sb.Append(";");
                }
                sb.AppendLine();

                // Данные
                foreach (DataRow row in data.Rows)
                {
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        sb.Append(row[i].ToString());
                        if (i < data.Columns.Count - 1) sb.Append(";");
                    }
                    sb.AppendLine();
                }
                File.WriteAllText(csvFile, sb.ToString(), Encoding.UTF8);

                // Сохраняем картинки
                int picCount = 0;
                if (chart1 != null)
                {
                    SaveChartAsImage(chart1, Path.Combine(folder, $"{baseName}_график1.png"));
                    picCount++;
                }
                if (chart2 != null)
                {
                    SaveChartAsImage(chart2, Path.Combine(folder, $"{baseName}_график2.png"));
                    picCount++;
                }

                string message = $"Отчет сохранён!\n\n📊 Данные: {csvFile}";
                if (picCount > 0) message += $"\n📸 Графики: {picCount} файла(ов) в той же папке";
                message += "\n\nОткройте CSV в Excel (Данные → Из текстового файла)";

                MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void SaveChartAsImage(FrameworkElement element, string filePath)
        {
            try
            {
                if (element == null) return;

                var width = (int)element.ActualWidth;
                var height = (int)element.ActualHeight;

                if (width <= 0 || height <= 0) return;

                element.UpdateLayout();

                var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(element);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
            catch { }
        }
    }
}