using System;
using System.IO;
using System.Collections;

namespace FileWordCount {
    class Program {
        static void Main(string[] args) {

            // set default values
            SearchOption directorySearchOption = SearchOption.TopDirectoryOnly;

            // process the command line arguments
            if (args.Length == 1 || args.Length == 2) {
                if (args[1] == "-s") {
                    directorySearchOption = SearchOption.AllDirectories;
                }
            } else {
                Console.WriteLine("FileWordCount <directory> [-s]");
                return;
            }
        
            // create a master list of fileInfoBlocks that hold the word stats for each file
            List<fileInfoBlock> fileInfoBlocks = new List<fileInfoBlock>();

            // check specified directory exists
            if (!Directory.Exists(args[0])) {
                Console.WriteLine("Directory does not exist");
                return;
            }

            // display processing status message
            Console.WriteLine("Processing files in {0}", args[0]);

            // get all the files in the current directory
            string[] files = Directory.GetFiles(args[0],"*.txt",directorySearchOption);

            foreach (string file in files) {
                // create a fileinfoBlock to contain the word stats for a file
                fileInfoBlock fib = new fileInfoBlock(file);

                // use a smart pointer to read the file line by line
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

            // release all allocated storage
            fileInfoBlocks.Clear();
        }
    }

    public class fileInfoBlock {
        private string pathFileName = "";
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

           // use a lambda function to sort the word stats by descending value
           // this means that the longest word with the highest count will be displayed first
           // and the shortest word with the lowest occurrence count will be displayed last
            var orderedWordStats = wordStats.OrderByDescending(x => x.Value);

            Console.WriteLine("File: {0}", pathFileName);
            foreach (KeyValuePair<string,int> kvp in orderedWordStats) {
                Console.WriteLine("{0} {1}", kvp.Key, kvp.Value);
            }
        }
    }
}
