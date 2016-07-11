using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace FindUnusedLabels
{
    class Program
    {
        static DateTime startTime;

        static string labelFilePath;

        static string searchPath;

        static string filter;

        static bool debug;

        static void Main(string[] args)
        {
            startTime = DateTime.Now;
            Console.Clear();
            Console.WriteLine("Starting...");

            try
            {
                ProcessArgs(args);

                var labelLoader = new AxLabelFileLoader();
                labelLoader.LoadLabels(labelFilePath);

                var labelProcessor = new AxLabelSearchProcessor(labelLoader.Labels);
                labelProcessor.Run(searchPath, filter);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Finished successfully!");
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Syntax:");
                Console.WriteLine("FindUnusedLabels.exe -l <Label File> -s <Path> -f <Filter>");
                Console.WriteLine("-l = Location of label file to process. ex: C:\\temp\\axen-us.ald");
                Console.WriteLine("-s = File path to search. ex: C:\\temp\\");
                Console.WriteLine("-f = file filter. ex: *.xpo");

                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed with error: " + ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Total Time: " + (DateTime.Now - startTime));
            Console.WriteLine("Press any key to exit");

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.ReadLine();
        }

        private static void ProcessArgs(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Required parameters are not specfied.");
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-l":
                        i++;
                        labelFilePath = args[i];
                        break;

                    case "-s":
                        i++;
                        searchPath = args[i];
                        break;

                    case "-f":
                        i++;
                        filter = args[i];
                        break;

                    case "-debug":
                        debug = true;
                        break;

                    default:
                        throw new ArgumentException("Unknown or invalid argument.", args[i]);
                }
            }
        }
    }
}
