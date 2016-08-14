/*
 * SeqReader.cpp
 *
 *  Created on: 25/set/2014
 *      Author: finalfire
 */

#include "SeqReader.h"
#include <sstream>

const std::string& SeqReader::getInputFile() const {
	return inputFile;
}

void SeqReader::setInputFile(const std::string& inputFile) {
	this->inputFile = inputFile;
}

const std::string& SeqReader::getSeq1() const {
	return seq1;
}

const std::string& SeqReader::getSeq2() const {
	return seq2;
}

SeqReader::SeqReader(std::string input) {
	this->inputFile = input;
	this->reader.open(this->inputFile);

	if (this->reader.is_open()) {
		std::getline(this->reader, this->seq1);
		std::getline(this->reader, this->seq2);
	}
	this->reader.close();
}

