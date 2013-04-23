using System.Collections.Generic;
using System.IO;

namespace LibBn
{
    public class NetworkHeaderWriter
    {
        public void Write(string outputFileName, List<string> headerLines)
        {
            // Read the file excluding the header.
            List<string> bodyLines = new List<string>();

            using (StreamReader sr = new StreamReader(outputFileName))
            {
                bool skipping = false;
                string line = "";

                // Skip all
                while ((line = sr.ReadLine()) != null)
                {
                    // Check if the first four letters are "node"
                    if (line.Trim().Equals("net"))
                    {
                        skipping = true;
                    }

                    if (skipping)
                    {
                        if (line.Trim().Equals("}"))
                        {
                            skipping = false;
                        }
                    }
                    else
                    {
                        bodyLines.Add(line);
                    }
                }
            }

            // Write the header and then the body
            using (StreamWriter sw = new StreamWriter(outputFileName, append: false))
            {
                foreach (var line in headerLines)
                {
                    sw.WriteLine(line);
                }

                foreach (var line in bodyLines)
                {
                    sw.WriteLine(line.Replace(@"\", @"\\"));
                }
            }
        }
    }
}