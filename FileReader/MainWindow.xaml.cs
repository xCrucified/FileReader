using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class TextAnalysisResult
    {
        public int Words { get; set; }
        public int Lines { get; set; }
        public int Punctuation { get; set; }
    }
    public partial class MainWindow : Window
    {
        private static object lockObj = new object();
        string directoryPath;
        List<Thread> threads = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void findBtn_Click(object sender, RoutedEventArgs e)
        {
            directoryPath = pathTxtBox.Text;

            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("Directory is not avaible.");
                return;
            }

            string[] files = Directory.GetFiles(directoryPath);

            threads = new List<Thread>();

            foreach (var file in files)
            {
                Thread thread = new Thread(() =>
                {
                    var result = AnalyzeFile(file);
                    DisplayResult(file, result);
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
        public static TextAnalysisResult AnalyzeFile(string filePath)
        {
            TextAnalysisResult result = new TextAnalysisResult();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Lines++;
                    result.Words += line.Split(new[] { ' ', '\t' }).Length;
                    result.Punctuation += CountPunctuation(line);
                }
            }

            return result;
        }

        public static int CountPunctuation(string text)
        {
            char[] punctuationChars = { '.', ',', ';', ':', '–', '—', '‒', '…', '!', '?', '"', '\'', '«', '»', '(', ')', '{', '}', '[', ']', '<', '>', '/' };
            return text.Count(c => punctuationChars.Contains(c));
        }

        public static void DisplayResult(string fileName, TextAnalysisResult result)
        {
            lock (lockObj)
            {
                MessageBox.Show($"File: {fileName}\n Words count: {result.Words}\n Rows count: {result.Lines}\n Count of separating signs: {result.Punctuation}");
            }
        }
    }
}
