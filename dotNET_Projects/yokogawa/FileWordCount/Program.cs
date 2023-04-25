using System;
using System.io;
using Systems.Collections;

namespace FileWordCount {
    class Program {
        static void Main(string[] args) {

            // set default values
            int directorySearchOption = SearchOption.AllDirectories;

            // process the command line arguments
            if (args.length == 0) {
                Console.WriteLine("FileWordCount <directory> [-s]");
                return;
            }
            if (args.length == 0) {
                Console.WriteLine("FileWordCount <directory> [-s]");
                return;
            }

            // create a master list of fileInfoBlocks that hold the word stats for each file
            List<fileInfoBlock> fieInfoBlocks = new List<fileInfoBlock>();

            // get all the files in the current directory
            string[] files = Directory.GetFiles(args[0],directorySearchOption);

            foreach (string file in files) {
                // create a fileinfoBlock to contain the word stats for a file
                fileInfoBlock fib = new fileInfoBlock(file);

                using (StreamReader sr = new StreamReader(file)) {
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        string[] words = line.Split(' ',StringSplitOptions.RemoveEmptyEntries);
                        foreach (string word in words) {
                            fib.addWord(word);
                        }
                    }
                }

                // add the fileInfoBlock to the master list
                fileInfoBlocks.Add(fib);
            }

            // display the word stats for each fileInfoBlock
            foreach (fileInfoBlock fib in fileInfoBlocks) {
                fib.printStats();
            }
        }
    }

    public class fileInfoBlock {
        private string pathFileName {get;set;} = "";
        private Dictionary <string,int> wordStats = new Dictionary<string,int>();

        public fileInfoBlock (string pathFileName) {
            this.pathFileName = pathFileName;
        }
        ~fileInfoBlock() {
            wordStats.Clear();
        }
        // add a word to the wordStats dictionary if it does not exist or increase the count if it does
        public void addWord(string word) {
            word = word.Trim().ToLower();
            if (wordStats.ContainsKey(word)) {
                wordStats[word]++;
            } else {
                wordStats.Add(word,1);
            }
        }
        // display the word stats for this fileInfoBlock
        public void printStats() {
            Console.WriteLine("File: {0}", pathFileName);
            foreach (KeyValuePair<string,int> kvp in wordStats) {
                Console.WriteLine("{0} {1}", kvp.Key, kvp.Value);
            }
        }
    }
}
