#include <iostream>
#include <string>
#include <map>
#include <list>
#include <set>
#include <filesystem>
#include <algorithm>
#include <fstream>

namespace fs = std::filesystem;
using namespace std;

// create a master list of fileInfoBlocks that hold the word stats for each file
list<fileInfoBlock*> fileInfoBlocks;
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
    private:
        bool ignoreCase = true;
        string pathFileName = "";
        int totalWordCount = 0;
        string shortestWord = "";
        string longestWord = "";
        map<string,int> wordStats;

    public:
        fileInfoBlock (string pathFileNameParam,bool ignoreCaseParam) {
            pathFileName = pathFileNameParam;
            ignoreCase = ignoreCaseParam;
        }

        ~fileInfoBlock() {
            wordStats.clear();
        }

    int getTotalWordCount() {
        return totalWordCount;
    }
    // add a word to the wordStats dictionary if it does not exist or increase the count if it does
    void addWord(string word) {
        // Removing whitespaces from string s.
        word.erase(remove_if(word.begin(), word.end(),
            [](char c) { // a lambda function
                return (c == ' ' || c == '\n' || c == '\r' ||
                    c == '\t' || c == '\v' || c == '\f');
            }),
        word.end());

        if (ignoreCase) {
              transform(word.begin(), word.end(), word.begin(), ::tolower); // convert to lower case so that The and the are treated as the same word
        }

        // check for the shortest word
        if (shortestWord == "") {
            shortestWord = word;
        } else {
            if (word.length() < shortestWord.length()) {
                shortestWord = word;
            }
        }

        // check for the longest word
        if (longestWord == "") {
            longestWord = word;
        } else {
            if (word.length() > longestWord.length()) {
                longestWord = word;
            }
        }

        // if the word does not exist in the map then add it with a count of 1
        if (wordStats.count(word) == 0) {
             wordStats.insert({ word, 1 });
        } else {
            // if the word exists in the map then increment the count
            wordStats[word]++;

        }

        totalWordCount++;
    }

    // display the word stats for this fileInfoBlock
    void printStats() {

        // The shortest word with the lowest occurrence count will be displayed first
        // this means that the longest word with the highest count will be displayed last

        // create an empty vector of pairs
        set<pair<string, int>, comp> orderedWordStats(wordStats.begin(), wordStats.end());
    
        cout << "File: " << pathFileName << " contains " << totalWordCount << " words" << endl;

        cout << "Shortest Word:" << shortestWord << " and longest word:" << longestWord << endl;

        for (auto element = orderedWordStats.begin(); element != orderedWordStats.end(); ++element) {
            cout << element->first << ' ' << element->second << '\n';
        } 
    }
};

int main(int argc, char* argv[]) {
            // set default values
            bool recursiveDirectorySearch = false;

            // process the command line arguments
            int validCmdLineArgs = 2; // assuming the first argument the program name and the second argument is the directory name
            
            for (int i=0; i < argc; i++) {
                if (argv[1] == "-r") {
                    recursiveDirectorySearch = true;
                    validCmdLineArgs++;
                }
                if (argv[i] == "-c") {
                    ignoreCase = false;
                    validCmdLineArgs++;
                }
            }

            if (argc != validCmdLineArgs) {
                cout << "FileWordCount <directory> [-r] [-c]" << endl;
                return 0;
            }
        
            // check specified directory exists
            if (!fs::is_directory(argv[1])) {
                cout << "Directory does not exist" << endl;
                return 0;
            }

            // display processing status message
            cout << "Processing files in " << argv[1] << endl;

            // get all the files in the current directory(s) and process
            if (recursiveDirectorySearch) {
                for (const auto & file : fs::directory_iterator(argv[1])) {
                    fileInfoBlocks.push_front(processFile(file.path().string()));
                }
            } else {
                for (const auto & file : fs::recursive_directory_iterator(argv[1])) {
                    fileInfoBlocks.push_front(processFile(file.path().string()));
                }
            }

            // count of words across all files
            int totalWordCountInAllFiles = 0;

            // display the word stats for each fileInfoBlock
            for(auto fib : fileInfoBlocks) {
                fib->printStats();
                totalWordCountInAllFiles = totalWordCountInAllFiles + fib->getTotalWordCount();
            }

            cout << "Total words in all files is " << totalWordCountInAllFiles << " words." << endl;

            // release all allocated storage
            for(auto fib : fileInfoBlocks) {
                 delete fib;
            }
            fileInfoBlocks.clear();
        }

        fileInfoBlock* processFile(string fileName) {
            // create a fileinfoBlock to contain the word stats for file
            fileInfoBlock* fib = new fileInfoBlock(fileName,ignoreCase);

            // Open the stream to 'lock' the file.
            ifstream f(fileName, ios::in | ios::binary);

            // Obtain the size of the file.
            const auto sz = fs::file_size(fileName);

            // Create a buffer.
            std::string result(sz, '\0');

            // Read the whole file into the buffer.
            f.read(result.data(), sz);

            // Close the file
            f.close();

            // split the file contents into words
            stringstream ss(result);  
            string word;
            while (ss >> word) { // Extract word from the stream.
                fib->addWord(word);
            }

            return fib; 
        }
