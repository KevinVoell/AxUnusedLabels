using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace FindUnusedLabels
{
    internal class AxLabelFileLoader : ILabelFileLoader
    {
        #region ILabelFileLoader Members

        public ReadOnlyCollection<ILabelData> Labels { get; private set; }

        public AxLabelFileLoader()
        {

        }

        public void LoadLabels(string path)
        {
            var labels = new List<ILabelData>();
            Labels = new ReadOnlyCollection<ILabelData>(labels);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified label file does not exist.");
            }

            var fileLines = File.ReadAllLines(path);

            Console.WriteLine("Parsing label file...");

            //foreach (var line in fileLines)
            for (int i = 0; i < fileLines.Length; i++)
            {
                var line = fileLines[i];
                if (Regex.IsMatch(line, "@Prefix(?:[0-9]*)"))
                {
                    var labelMatch = Regex.Match(line, "@Prefix(?:[0-9]*)");
                    var labelData = new AxLabelData
                    {
                        LabelId = labelMatch.Groups[0].Value,
                        Text = line
                    };

                    if (!fileLines[i + 1].StartsWith("@"))
                    {
                        // Next line is a description, grab it.
                        i++;
                        labelData.Description = fileLines[i];
                    }

                    labels.Add(labelData);
                }
            }

            Labels = new ReadOnlyCollection<ILabelData>(labels);
        }

        #endregion
    }
}
