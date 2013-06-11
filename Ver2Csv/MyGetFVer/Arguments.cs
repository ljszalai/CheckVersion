/*
 * Command line parser class written by Richard Lopes
 * The original source was found at
 * http://www.codeproject.com/KB/recipes/command_line.aspx#
 * This code is licensed under The MIT License
 * http://www.opensource.org/licenses/mit-license.php
 * Copy-pased and modified by Laszlo Szalai, 2011
 */
using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace CommandLine.Utility
{
    /// <summary>

    /// Arguments class

    /// </summary>

    public class Arguments
    {
        // Variables

        private StringDictionary m_Parameters;
        private string[] m_gotArgs;
        private string[] m_parsedArgs;
        private int argNum;

        public StringDictionary Params
        {
            get { return m_Parameters; }
            set { m_Parameters = value; }
        }

        // Constructor

        public Arguments(string[] Args)
        {
            m_gotArgs = Args;
            argNum = 0;
            m_parsedArgs = new string[Args.Length];
            m_Parameters = new StringDictionary();
            //Regex Spliter = new Regex(@"^-{1,2}|^/|=|:",
            //    RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex Spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex Remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string Parameter = null;
            string[] Parts;

            // Valid parameters forms:

            // {-,/,--}param{ ,=,:}((",')value(",'))

            // Examples: 

            // -param1 value1 --param2 /param3:"Test-:-work" 

            //   /param4=happy -param5 '--=nice=--'

            foreach (string Txt in Args)
            {
                // Look for new parameters (-,/ or --) and a

                // possible enclosed value (=,:)

                Parts = Spliter.Split(Txt, 3);

                switch (Parts.Length)
                {
                    // Found a value (for the last parameter 

                    // found (space separator))

                    case 1:
                        if (Parameter != null)
                        {
                            if (!m_Parameters.ContainsKey(Parameter))
                            {
                                Parts[0] =
                                    Remover.Replace(Parts[0], "$1");

                                m_Parameters.Add(Parameter, Parts[0]);
                            }
                            Parameter = null;
                        }
                        else
                        {
                            // this is not a parameter, this is an argument
                            m_parsedArgs[getNextArgNum()] = Txt;
                        }
                        break;

                    // Found just a parameter

                    case 2:
                        // The last parameter is still waiting. 

                        // With no value, set it to true.

                        if (Parameter != null)
                        {
                            if (!m_Parameters.ContainsKey(Parameter))
                            {
                                m_Parameters.Add(Parameter, "true");
                            }
                        }
                        else
                        {
                            // found something can be splitted by splitter
                            // but anyway: this is not a parameter, this is an argument
                            if (!Parts[0].Equals(""))
                                m_parsedArgs[getNextArgNum()] = Txt;
                        }
                        Parameter = Parts[1];
                        break;

                    // Parameter with enclosed value

                    case 3:
                        // The last parameter is still waiting. 

                        // With no value, set it to true.

                        if (Parameter != null)
                        {
                            if (!m_Parameters.ContainsKey(Parameter))
                            {
                                m_Parameters.Add(Parameter, "true");
                            }
                        }

                        Parameter = Parts[1];

                        // Remove possible enclosing characters (",')

                        if (!m_Parameters.ContainsKey(Parameter))
                        {
                            Parts[2] = Remover.Replace(Parts[2], "$1");
                            m_Parameters.Add(Parameter, Parts[2]);
                        }

                        Parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting

            if (Parameter != null)
            {
                if (!m_Parameters.ContainsKey(Parameter))
                {
                    m_Parameters.Add(Parameter, "true");
                }
            }
        }

        private int getNextArgNum()
        {
            int result = argNum;
            argNum++;
            return result;
        }

        // Retrieve a parameter value if it exists 

        // (overriding C# indexer property)

        public string this[string Param]
        {
            get
            {
                return (m_Parameters[Param]);
            }
        }

        public string this[int num]
        {
            get
            {
                string result = "";
                if (num < m_parsedArgs.Length)
                    result = m_parsedArgs[num];
                return result;
            }
        }
    }
}