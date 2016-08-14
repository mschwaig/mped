#ifndef SEQREADER_H_
#define SEQREADER_H_

#include <string>
#include <fstream>

class SeqReader {
private:

	std::ifstream reader;

	std::string inputFile;
	std::string seq1;
	std::string seq2;

public:
	SeqReader(std::string input);
	const std::string& getInputFile() const;
	void setInputFile(const std::string& inputFile);
	const std::string& getSeq1() const;
	const std::string& getSeq2() const;
};

#endif /* SEQREADER_H_ */
