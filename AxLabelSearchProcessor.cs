using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FindUnusedLabels
{
    internal class AxLabelSearchProcessor : ILabelSearchProcessor
    {
        int threadCount;
        int filesRemaining;

        private const int TotalFilesPosition = 2;
        private const int TotalLabelsPostion = 3;
        private const int FilesRemainingPosition = 4;
        private const int ThreadCountPosition = 5;
        private const int UnusedLabelCountPosition = 6;
        
        private static object _lock = new object();

        private ReadOnlyCollection<ILabelData> _labelData;

        public AxLabelSearchProcessor(ReadOnlyCollection<ILabelData> labelData)
        {
            _labelData = labelData;
        }

        #region ILabelSearchProcessor Members

        public void Run(string searchPath, string filter)
        {
            if (!Directory.Exists(searchPath))
            {
                throw new DirectoryNotFoundException("The specified directory does not exist.");
            }

            var files = Directory.EnumerateFiles(searchPath, filter, SearchOption.AllDirectories);

            Console.SetCursorPosition(0, TotalFilesPosition);
            Console.Write("Total Files: " + files.Count());

            Console.SetCursorPosition(0, TotalLabelsPostion);
            Console.Write("Total labels found in label file: " + _labelData.Count());

            Console.SetCursorPosition(0, FilesRemainingPosition);
            Console.Write("Files Remaining: ");

            Console.SetCursorPosition(0, ThreadCountPosition);
            Console.Write("Thread count:");

            Console.SetCursorPosition(0, UnusedLabelCountPosition);
            Console.Write("Unused label count: ");

            filesRemaining = files.Count();

            Parallel.ForEach(files, filePath =>
            {
                threadCount++;
                UpdateUI();

                var fileContents = File.ReadAllText(filePath);

                Parallel.ForEach(_labelData, label =>
                {
                    if (!label.IsUsed)
                    {
                        // Label is not used yet.  Check this file.
                        if (fileContents.Contains(label.LabelId))
                        {
                            label.IsUsed = true;
                        }
                    }
                });
                filesRemaining--;
                threadCount--;
                UpdateUI();
            });

            filesRemaining = 0;
            threadCount = 0;
            UpdateUI();

            WriteUnusedLabels();
            WriteUsedLabels();
        }

        private void UpdateUI()
        {
            lock (_lock)
            {
                Console.SetCursorPosition(18, FilesRemainingPosition);
                Console.Write(filesRemaining.ToString().PadRight(10));

                Console.SetCursorPosition(15, ThreadCountPosition);
                Console.Write(threadCount.ToString().PadRight(10));

                Console.SetCursorPosition(21, UnusedLabelCountPosition);
                Console.Write(_labelData.Count(o => o.IsUsed == false).ToString().PadRight(10));
            }
        }

        private void WriteUnusedLabels()
        {
            Console.SetCursorPosition(0, 9);
            Console.WriteLine("Writing unused labels to: unusedLabels.txt...");

            var labels = _labelData.Where(o => o.IsUsed == false);

            Console.WriteLine("Total unused labels: " + labels.Count());

            var sb = new StringBuilder();

            foreach (var label in labels)
            {
                sb.AppendLine(label.Text);

                if (!string.IsNullOrWhiteSpace(label.Description))
                {
                    sb.AppendLine(label.Description);
                }
            }

            File.WriteAllText("unusedLabels.txt", sb.ToString());
        }

        private void WriteUsedLabels()
        {
            Console.SetCursorPosition(0, 13);
            Console.WriteLine("Writing used labels to: cleanedLabelFile.txt...");

            var labels = _labelData.Where(o => o.IsUsed == true);

            Console.WriteLine("Total used labels: " + labels.Count());

            var sb = new StringBuilder();

            foreach (var label in labels)
            {
                sb.AppendLine(label.Text);

                if (!string.IsNullOrWhiteSpace(label.Description))
                {
                    sb.AppendLine(label.Description);
                }
            }

            File.WriteAllText("cleanedLabelFile.txt", sb.ToString());
        }

        #endregion
    }
}
