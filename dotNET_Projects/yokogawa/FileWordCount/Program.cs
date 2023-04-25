using System;
using System.io;
using Systems.Collections;

namespace FileWordCount {
    class Program {
        static void Main(string[] args) {
            if (args.length == 0) {
                Console.WriteLine("FileWordCount <directory> [-s]");
                return;
            }

            List<fileInfoBlock> fieInfoBlocks = new List<fileInfoBlock>();

            string[] files = Directory.GetFiles(args[0]);

            foreach (string file in files) {
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

                fileInfoBlocks.Add(fib);
            }

            foreach (fileInfoBlock fib in fileInfoBlocks) {
                fib.printStats();
            }
        }
    }

    public class fileInfoBlock {
        private string pathFileName {get;set;} = "";
        private Dictionary <string,int> wordStats {get;set;} = new Dictionary<string,int>();

        public fileInfoBlock (string pathFileName) {
            this.pathFileName = pathFileName;
        }
        ~fileInfoBlock() {
            wordStats.Clear();
        }
        public void addWord(string word) {
            word = word.Trim().ToLower();
            if (wordStats.ContainsKey(word)) {
                wordStats[word]++;
            } else {
                wordStats.Add(word,1);
            }
        }
        public void printStats() {
            Console.WriteLine("File: {0}", pathFileName);
            foreach (KeyValuePair<string,int> kvp in wordStats) {
                Console.WriteLine("{0} {1}", kvp.Key, kvp.Value);
            }
        }
    }
}
