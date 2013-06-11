using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using CommandLine.Utility;

namespace Ver2Csv   
{
    class Program
    {
        private static int iRet;
        private static string fullPath;
        private static bool cutBase = false;
        private static bool pause = false;
        private static bool printHeader = true;
        private static bool printHelp = false;
        private static string helpString = "";   

        private static int Main(string[] args)
        {
            Arguments CommandLine = new Arguments(args);
            if ((args.Length < 1))
            {
                fullPath = Directory.GetCurrentDirectory();
            }
            else
            {
                configureThisRun(CommandLine);
            }
            if (printHelp)
            {
                printFriendlyHelp();
            }
            else
            {
                printCsvHeader();
                traverseFiles(fullPath);
                execPause();
            }
            return iRet;
        }

        private static void printFriendlyHelp()
        {
            Console.Out.WriteLine(helpString);
        }

        private static void printCsvHeader()
        {
            if (printHeader)
            {
                Console.Out.WriteLine("FileName;FileVersion;ProductVersion");
            }
        }

        private static void execPause()
        {
            if (pause)
            {
                Console.Out.WriteLine("Press a ENTER if you have any...");
                Console.In.ReadLine();
            }
        }

        private static void configureThisRun(Arguments CommandLine)
        {
            string enl = Environment.NewLine;
            string enlpad = enl + "   ";
            helpString =
                "Ver2Csv -- File version informations to comma separated values" + enl +
                "(C) 2013, Laszlo Szalai and Lyland Kft." + enl +
                "Usage:" + enlpad +
                "ver2csv [options] [baseDir]" + enl +
                "Where:" + enlpad +
                "baseDir - folder where version dump starts. Uses current dir by default." + enlpad +
                "options - one or more of the followings:" + enl;
            fullPath = Directory.GetCurrentDirectory();
            if (CommandLine[0] != null)
            {
                fullPath = (CommandLine[0].Length != 0) ? CommandLine[0] : Directory.GetCurrentDirectory();
            }
            helpString +=
                "-r -- baseDir is given relative to current folder" + enl;
            if (CommandLine["r"] != null)
            {
                fullPath = Directory.GetCurrentDirectory() + "\\" + CommandLine["r"];
            }
            helpString +=
                "-relative -- same as -r" + enl;
            if (CommandLine["relative"] != null)
            {
                fullPath = Directory.GetCurrentDirectory() + "\\" + CommandLine["relative"];
            }
            helpString +=
                "-s, --cutbasedir -- the given (or implicit) base folder will not" + enlpad +
                "be printed for each file in the list" + enl;
            if ((CommandLine["s"] != null) || (CommandLine["cutbasedir"] != null))
            {
                cutBase = true;
            }
            helpString +=
                "-p, --pause -- waits for pressing ENTER after print" + enl;
            if ((CommandLine["p"] != null) || (CommandLine["pause"] != null))
            {
                pause = true;
            }
            helpString +=
                "-o, --omitheader -- omits printing nice header for output" + enl;
            if ((CommandLine["o"] != null) || (CommandLine["omitheader"] != null))
            {
                printHeader = false;
            }
            helpString +=
                "-h, -? --help -- prints this message instead of anything else" + enl;
            if ((CommandLine["h"] != null) || (CommandLine["help"] != null) || (CommandLine["?"] != null))
            {
                printHelp = true;
            }
        }

        private static void traverseFiles(string fileName) 
        {

            if (File.Exists(fileName))
            {
                getVersionInfo(fileName);
            }
            else if (Directory.Exists(fileName))
            {
                foreach (string f in Directory.GetFiles(fileName))
                {
                    string ext = Path.GetExtension(f).ToLower();
                    if (ext.Equals(".exe") || ext.Equals(".dll"))
                    {
                        Console.Out.WriteLine(renderVersionInfo(FileVersionInfo.GetVersionInfo(f)));
                    }

                }
                foreach (string d in Directory.GetDirectories(fileName))
                {
                    traverseFiles(d);
                }

            }
            else
            {
                printFriendlyHelp();
                iRet = -1;
                Environment.Exit(iRet);
            }
        }

        private static string renderVersionInfo(FileVersionInfo fvi)
        {
            string fileName = fvi.FileName;
            if (cutBase)
            {
                fileName = fileName.Substring(fullPath.Length, (fileName.Length - fullPath.Length));
                if (fileName.StartsWith("\\"))
                    fileName = fileName.Substring(1, fileName.Length - 1);
            }
            return string.Format(
                "{0};{1}.{2}.{3}.{4};{5}.{6}.{7}.{8}",
                fileName,
                fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart,
                fvi.ProductMajorPart, fvi.ProductMinorPart, fvi.ProductBuildPart, fvi.ProductPrivatePart
            );
        }

        private static string getVersionInfo(string fileName)
        {
            string result = "";
            FileVersionInfo fileVersionInfo;
            try
            {
                fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
                result = fileVersionInfo.ToString();
            }
            catch (FileNotFoundException)
            {
                result = string.Format("{0}; file not found.", fileName);
                iRet = -1;
            }
            catch (ArgumentException aex)
            {
                result = string.Format("{0}; {1}", fileName, aex.Message);
                iRet = -1;
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                iRet = -1;
            }
            iRet = 0;
            return result;
        }

    }
}
