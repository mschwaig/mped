#ifndef MPED_H_
#define MPED_H_

#include <iostream>
#include <string>
#include <map>
#include "../alignment/Alignment.h"
#include "../utility/Matrix.h"
using namespace std;

class MPED {
private:

	string _s1, _s2;				// original strings
	string _sigma1, _sigma2;		// original sigmas

	unsigned short p1, p2;			// p1, p2
	unsigned short attempts;		// attempts
	bool self_identity;				// self identity

	unsigned short gap;				// gap for the alignment

	unsigned* s1;					// mapped strings
	unsigned* s2;
	unsigned short* sigma1;			// mapped sigmas
	unsigned short* sigma2;

	size_t _l1;						// length of s1
	size_t _l2;						// length of s2
	size_t _sgl1;					// length of sigma1
	size_t _sgl2;					// length of sigma2

	map<char,int> map_sigma1;		// mapping for sigma1
	map<char,int> map_sigma2;		// mapping for sigma2

	unsigned short** weights;		// weights

	Alignment* identity;			// identity resulting alignment
	Alignment* external;			// resulting alignment by some heuristic


	/* ==================================== */

	// Preliminary utility functions
	string defineSigma(string&);
	unsigned short* initMapping(string&, map<char,int>&);
	unsigned* setMapping(string&, map<char,int>&);

	void allocateWeights();
	void deallocateWeights();
	void initIdentityWeights();
	void initGeneralWeights();

	// DEBUGGING UTILITY
	void printWeights();

public:

	MPED(string a, string b) : MPED (a, b, defineSigma(a), defineSigma(b)) {
	}

	MPED(string s1, string s2, string alphabet1, string alphabet2) : _s1(s1), _s2(s2), _sigma1(alphabet1), _sigma2(alphabet2) {
		p1 = 1, p2 = 1, attempts = 0, gap = 1;
		self_identity = false;

		_l1 = _s1.length(), _l2 = _s2.length();
		_sgl1 = _sigma1.length(), _sgl2 = _sigma2.length();
	}

	// TODO: DESTRUCTOR
	// TODO: deallocate weights
	~MPED() {
		/*delete[] s1;
		delete[] s2;
		delete[] sigma1;
		delete[] sigma2;

		for (size_t i = 0; i < _sgl1; i++)
			delete[] weights[i];
		delete[] weights;

		delete identity;
		delete external;*/
	}

	// Identity alignment
	Alignment* getIdentityAligment() { return this->identity; }
	void computeIdentityAlignment();

	// External alignment, i.e., by meaning of a matching scheme compute by a heuristic
	Alignment* getExternalAlignment() { return this->external; }
	void computeExternalAlignment(unsigned short*, unsigned short*);

	// Compute the classic edit distance (uses this->weights too)
	unsigned computeEditDistance();
	// Compute the edit distance using a permutation of the alphabets
	unsigned computeExternalEditDistance(const string&, const string&, unsigned short*, unsigned short*,
			unsigned*, unsigned*);

	// print functions
	inline void printInfo();

	// debugging function
	void debug();


	// get n set
	size_t getL1() const { return _l1; }
	size_t getL2() const { return _l2; }

	const string& get_S1() const { return _s1; }
	const string& get_S2() const { return _s2; }

	size_t getSgl1() const { return _sgl1; }
	size_t getSgl2() const { return _sgl2; }

	const string& get_Sigma1() { return _sigma1; }
	const string& get_Sigma2() { return _sigma2; }

	unsigned short getAttempts() const { return attempts; }
	void setAttempts(unsigned short attempts) { this->attempts = attempts; }

	unsigned short getGap() const { return gap; }
	void setGap(unsigned short gap) { this->gap = gap; }

	const map<char, int>& getMapSigma1() const { return map_sigma1; }
	const map<char, int>& getMapSigma2() const { return map_sigma2; }

	unsigned short getP1() const { return p1; }
	void setP1(unsigned short p1) { this->p1 = p1; }

	unsigned short getP2() const { return p2; }
	void setP2(unsigned short p2) { this->p2 = p2; }

	unsigned* getS1() { return s1; }
	unsigned* getS2() { return s2; }

	bool isSelfIdentity() const { return self_identity; }
	void setSelfIdentity(bool selfIdentity) { self_identity = selfIdentity; }

	unsigned short* getSigma1() { return sigma1; }
	unsigned short* getSigma2() { return sigma2; }

	unsigned short** getWeights() const { return weights; }
};

#endif /* MPED_H_ */
