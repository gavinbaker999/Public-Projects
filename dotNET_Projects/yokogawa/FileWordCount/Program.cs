using System;
using System.IO;
using System.Collections;

namespace FileWordCount {
    class Program {
        // create a master list of fileInfoBlocks that hold the word stats for each file
        static List<fileInfoBlock> fileInfoBlocks = new List<fileInfoBlock>();
        static bool ignoreCase = true;

        static void Main(string[] args) {
            // set default values
            SearchOption directorySearchOption = SearchOption.TopDirectoryOnly;

            // process the command line arguments
            int validCmdLineArgs = 1; // assuming the first argument is the directory name
            foreach (string arg in args) {
                if (arg == "-r") {
                    directorySearchOption = SearchOption.AllDirectories;
                    validCmdLineArgs++;
                }
                if (arg == "-c") {
                    ignoreCase = false;
                    validCmdLineArgs++;
                }
            }

            if (args.Length != validCmdLineArgs) {
                Console.WriteLine("FileWordCount <directory> [-r] [-c]");
                return;
            }
        
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

                // add the fileInfoBlock to the master list
                fileInfoBlocks.Add(processFile(file));

            }

            // count of words across all files
            int totalWordCountInAllFiles = 0;

            // display the word stats for each fileInfoBlock
            foreach (fileInfoBlock fib in fileInfoBlocks) {
                fib.printStats();
                totalWordCountInAllFiles = totalWordCountInAllFiles + fib.getTotalWordCount();
            }

            Console.WriteLine("Total words in all files is {0} words", totalWordCountInAllFiles);

            // release all allocated storage
            fileInfoBlocks.Clear();
        }

        public static fileInfoBlock processFile(string fileName) {
            // create a fileinfoBlock to contain the word stats for file
            fileInfoBlock fib = new fileInfoBlock(fileName,ignoreCase);

            // use a smart pointer to read the file line by line
            using (StreamReader sr = new StreamReader(fileName)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    string[] words = line.Split(' ',StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words) {
                        fib.addWord(word);
                    }
                }
            }

            return fib; 
        }
    }

    public class fileInfoBlock {
        private bool ignoreCase = true;
        private string pathFileName = "";
        private int totalWordCount = 0;
        private Dictionary <string,int> wordStats = new Dictionary<string,int>();

        public fileInfoBlock (string pathFileName,bool ignoreCase) {
            this.pathFileName = pathFileName;
            this.ignoreCase = ignoreCase;
        }
        ~fileInfoBlock() {
            wordStats.Clear();
        }
        public int getTotalWordCount() {
            return totalWordCount;
        }
        // add a word to the wordStats dictionary if it does not exist or increase the count if it does
        public void addWord(string word) {
            // remove any leading and trailing spaces
            word = word.Trim();

            if (ignoreCase) {
                word = word.ToLower(); // convert to lower case so that The and the are treated as the same word
            }

            if (wordStats.ContainsKey(word)) {
                wordStats[word]++;
            } else {
                wordStats.Add(word,1);
            }

            totalWordCount++;
        }

        // display the word stats for this fileInfoBlock
        public void printStats() {

           // use a lambda function to sort the word stats by descending value
           // this means that the longest word with the highest count will be displayed first
           // and the shortest word with the lowest occurrence count will be displayed last
            var orderedWordStats = wordStats.OrderByDescending(x => x.Value);

            Console.WriteLine("File: {0} contains {1} words", pathFileName, totalWordCount);
            foreach (KeyValuePair<string,int> kvp in orderedWordStats) {
                Console.WriteLine("{0} {1}", kvp.Key, kvp.Value);
            }
        }
    }
}
