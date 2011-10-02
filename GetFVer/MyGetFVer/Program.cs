using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using CommandLine.Utility;

namespace GetFVer
{
    class Program
    {
        private static int iRet;

        private static int Main(string[] args)
        {
                                    
            if ((args.Length < 1))
            {
                Console.Out.WriteLine("Invalid parameters, usage: GetFVer [file name].");
                iRet = -1;
            }
            else
            {
                string fullFileName = args[0];
                Arguments CommandLine = new Arguments(args);
                if (CommandLine["r"] != null)
                {
                    fullFileName = Directory.GetCurrentDirectory() + "\\" + CommandLine["r"];
                } 
                if (CommandLine["relative"] != null)
                {
                    fullFileName = Directory.GetCurrentDirectory() + "\\" + CommandLine["relative"];
                }
                Console.Out.WriteLine(getVersionInfo(fullFileName));
            }
            return iRet;
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
