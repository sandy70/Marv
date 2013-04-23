using System;
using System.Collections.Generic;
using System.IO;

namespace LibBn
{
    public class NetworkHeaderReader
    {
        public List<string> Read(string inputFileName)
        {
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(inputFileName))
            {
                String line;
                bool readingLine = false;

                while ((line = sr.ReadLine()) != null)
                {
                    // Check if the first four letters are "node"
                    if (line.Trim().Equals("net"))
                    {
                        readingLine = true;
                    }

                    if (readingLine)
                    {
                        lines.Add(line);

                        if (line.Trim().Equals("}"))
                        {
                            break;
                        }
                    }
                }
            }

            return lines;
        }
    }
}