#include <iostream>
#include <string>
#include <map>
#include <list>
#include <set>
#include <filesystem>

namespace fs = std::filesystem;
using namespace std;

// create a master list of fileInfoBlocks that hold the word stats for each file
list<fileInfoBlock> fileInfoBlocks;
bool ignoreCase = true;

// The comparison function for sorting the set by increasing order of its pair's
// second value. If the second value is equal, order by the pair's first value
struct comp
{
    template<typename T>
    bool operator()(const T &l, const T &r) const
    {
        if (l.second != r.second) {
            return l.second < r.second;
        }
 
        return l.first < r.first;
    }
};

class fileInfoBlock {
    private bool ignoreCase = true;
    private string pathFileName = "";
    private int totalWordCount = 0;
    private map<string,int> wordStats;

    fileInfoBlock (string pathFileNameParam,bool ignoreCaseParam) {
        pathFileName = pathFileNameParam;
        ignoreCase = ignoreCaseParam;
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

        wordStats[word] = wordStats.ContainsKey(word) ? wordStats[word] + 1 : 1;

        totalWordCount++;
    }

    // display the word stats for this fileInfoBlock
    public void printStats() {

        // The shortest word with the lowest occurrence count will be displayed first
        // this means that the longest word with the highest count will be displayed last

        // create an empty vector of pairs
        set<pair<string, int>, comp> orderedWordStats(wordStats.begin(), wordStats.end());
    
        cout << "File: " << pathFileName << " contains " << totalWordCount << " words" << endl;

        for(auto element : orderedWordStats) {
            cout << element.Key << " " << element.Value << endl;
        }
    }
}

int main(int argc, char* argv[]) {
            // set default values
            bool recursiveDirectorySearch = false;

            // process the command line arguments
            int validCmdLineArgs = 2; // assuming the first argument the program name and the second argument is the directory name
            for(auto arg in args) {
                if (arg == "-r") {
                    recursiveDirectorySearch = true;
                    validCmdLineArgs++;
                }
                if (arg == "-c") {
                    ignoreCase = false;
                    validCmdLineArgs++;
                }
            }

            if (args.Length != validCmdLineArgs) {
                cout << "FileWordCount <directory> [-r] [-c]" << endl;
                return 0;
            }
        
            // check specified directory exists
            if (!fs::is_directory(args[0])) {
                count << "Directory does not exist" << endl;
                return 0;
            }

            // display processing status message
            cout << "Processing files in " << args[0] << endl;

            // get all the files in the current directory
            string[] files = Directory.GetFiles(args[0],"*.txt",directorySearchOption);

            for(auto file in files) {

                // add the fileInfoBlock to the master list
                fileInfoBlocks.Add(processFile(file));

            }

            // count of words across all files
            int totalWordCountInAllFiles = 0;

            // display the word stats for each fileInfoBlock
            for(auto fib in fileInfoBlocks) {
                fib.printStats();
                totalWordCountInAllFiles = totalWordCountInAllFiles + fib.getTotalWordCount();
            }

            cout << "Total words in all files is " << totalWordCountInAllFiles << " words." << endl;

            // release all allocated storage
            fileInfoBlocks.Clear();
        }

        public fileInfoBlock processFile(string fileName) {
            // create a fileinfoBlock to contain the word stats for file
            fileInfoBlock fib = new fileInfoBlock(fileName,ignoreCase);

            // Open the stream to 'lock' the file.
            std::ifstream f(fileName, std::ios::in | std::ios::binary);

            // Obtain the size of the file.
            const auto sz = fs::file_size(fileName);

            // Create a buffer.
            std::string result(sz, '\0');

            // Read the whole file into the buffer.
            f.read(result.data(), sz);

            // Close the file
            f.close();

            // split the file contents into words
            string[] words = result.Split(' ',StringSplitOptions.RemoveEmptyEntries);

            // add each word to the fileInfoBlock
            for(auto word in words) {
                fib.addWord(word);
            }

            return fib; 
        }
