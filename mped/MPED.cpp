#include "MPED.h"
#include <algorithm>

inline const int indexof(unsigned x, unsigned short a[], int size) {
	for (int i = 0; i < size; i++)
		if (a[i] == x)
			return i;
	return -1;
}

/**
 * Define sigma from string s.
 */
// TODO: any efficient way of doing it?
string MPED::defineSigma(string& s) {
	bool in = false;
	string temp("");
	char curr;

	for (unsigned i = 0; i < s.length(); i++) {
		curr = s[i];

		for (unsigned j = 0; j < temp.length(); j++)
			if (temp[j] == curr)
				in = true;

		if (!in)
			temp += curr;
		in = false;
	}

	sort(temp.begin(), temp.end());
	return temp;
}

/**
 * Init mapping by filling the map m by sigma a
 */
unsigned short* MPED::initMapping(string& a, map<char,int>& m) {
	unsigned short* temp = new unsigned short[a.length()];
	for (unsigned i = 0; i < a.length(); i++) {
		temp[i] = i;	// i-th character, i-th integer
		m[a[i]] = i;	// character -> int
	}
	return temp;
}

/**
 * Convert the current string s by using the map m
 */
unsigned* MPED::setMapping(string& s, map<char,int>& m) {
	unsigned* temp = new unsigned[s.length()];
	for (unsigned i = 0; i < s.length(); i++)
		temp[i] = m[s[i]];
	return temp;
}

void MPED::allocateWeights() {
	this->weights = new unsigned short*[this->_sgl1];
	for (size_t i = 0; i < this->_sgl1; i++)
		this->weights[i] = new unsigned short[this->_sgl2];
}

void MPED::deallocateWeights() {
	for (size_t i = 0; i < this->_sgl1; i++)
		delete[] this->weights[i];
	delete[] weights;
}

void MPED::initIdentityWeights() {
	this->allocateWeights();

	// init for identity
	for (size_t i = 0; i < this->_sgl1; i++)
		for (size_t j = 0; j < this->_sgl2; j++)
			_sigma1[i] == _sigma2[j] ? this->weights[i][j] = 0 : this->weights[i][j] = 1;
}

void MPED::initGeneralWeights() {
	this->allocateWeights();

	//init for general
	for (int i = 0; i < this->_sgl1; i++)
		for (int j = 0; j < this->_sgl2; j++)
			if (p1 !=0 && p2 !=0 && (i/p2 == j/p1))
				this->weights[i][j] = 0;
			else
				this->weights[i][j] = 1;
}

void MPED::printWeights() {
	cout << "\t";
	for (int j = 0; j < this->_sgl2; j++)
		cout << this->_sigma2[j] << "\t";
	cout << endl;
	for (int i = 0; i < this->_sgl1; i++){
		cout << this->_sigma1[i] <<"\t";
		for (int j = 0; j < this->_sgl2; j++)
			cout << this->weights[i][j] << "\t";
		cout << endl;
	}
}

inline void MPED::printInfo() { cout << ">> P1: " << this->p1 << ", P2: " << this->p2 << ", SelfIdentity: " << this->self_identity << endl; }

void MPED::computeExternalAlignment(unsigned short* n_sigma1, unsigned short* n_sigma2) {
	unsigned weight = 0, tmpWeight;

	Matrix<unsigned> d(_l1 + 1, _l2 + 1);
	Matrix<char> dir(_l1 + 1, _l2 + 1);

	for (size_t i = 0; i < d.r(); i++) { d(i,0) = i; dir(i,0) = 'n'; }
	for (size_t j = 0; j < d.c(); j++) { d(0,j) = j; dir(0,j) = 'o'; }

	// needleman-wuntsch
	for (size_t i = 1; i < d.r(); i++)
		for (size_t j = 1; j < d.c(); j++) {
			if (this->self_identity && (this->_sigma1[s1[i-1]] == this->_sigma2[s2[j-1]]))
				weight = 0;
			else
				weight = this->weights[ indexof(s1[i-1], n_sigma1, _sgl1) ][ indexof(s2[j-1], n_sigma2, _sgl2) ];

			tmpWeight = d(i-1, j-1) + weight;
			d(i,j) = std::min(
					std::min(
							d(i-1,j) + gap,
							d(i,j-1) + gap
					),
					tmpWeight);

			if (d(i,j) == tmpWeight)
				dir(i,j) = 'd';
			else if (d(i,j) == d(i-1,j) + gap)
				dir(i,j) = 'n';
			else
				dir(i,j) = 'o';
		}

	size_t max_length = _l1 > _l2 ? _l1 : _l2;
	int* all1 = new int[2*max_length];
	int* all2 = new int[2*max_length];

	size_t start = (2 * max_length)-1;
	size_t i = _l1;
	size_t j = _l2;

	while (i >= 0 || j >= 0) {
		if (i == 0 && j == 0)
			break;
		else {
			if (dir(i,j) == 'n') {
				all1[start] = s1[i-1];
				all2[start] = -1;
				i--;
			} else if (dir(i,j) == 'd') {
				all1[start] = s1[i-1];
				all2[start] = s2[j-1];
				i--;
				j--;
			} else {
				all1[start] = -1;
				all2[start] = s2[j-1];
				j--;
			}
			start--;
		}
	}


	this->printInfo();
	// so the new strings all1 and all2 start at "start+1" and end at "2*max_length-1"
	this->external = new Alignment(all1, all2, start+1, 2*max_length, d(_l1,_l2), _sigma1, _sigma2, false, self_identity, _l1, _l2, weights, n_sigma1, n_sigma2);

	delete[] all1;
	delete[] all2;
}

void MPED::computeIdentityAlignment() {
	unsigned weight = 0, tmpWeight;

	Matrix<unsigned> d(_l1 + 1, _l2 + 1);
	Matrix<char> dir(_l1 + 1, _l2 + 1);

	for (size_t i = 0; i < d.r(); i++) { d(i,0) = i; dir(i,0) = 'n'; }
	for (size_t j = 0; j < d.c(); j++) { d(0,j) = j; dir(0,j) = 'o'; }

	// needleman-wuntsch
	for (size_t i = 1; i < d.r(); i++)
		for (size_t j = 1; j < d.c(); j++) {
			if (this->self_identity && (this->_sigma1[s1[i-1]] == this->_sigma2[s2[j-1]]))
				weight = 0;
			else
				weight = this->weights[ sigma1[s1[i-1]] ][ sigma2[s2[j-1]] ];

			tmpWeight = d(i-1, j-1) + weight;
			d(i,j) = std::min(
					std::min(
							d(i-1,j) + gap,
							d(i,j-1) + gap
							),
					tmpWeight);

			if (d(i,j) == tmpWeight)
				dir(i,j) = 'd';
			else if (d(i,j) == d(i-1,j) + gap)
				dir(i,j) = 'n';
			else
				dir(i,j) = 'o';
		}

	size_t max_length = _l1 > _l2 ? _l1 : _l2;
	int* all1 = new int[2*max_length];
	int* all2 = new int[2*max_length];

	size_t start = (2 * max_length)-1;
	size_t i = _l1;
	size_t j = _l2;

	while (i > 0 || j > 0) {
		if (i == 1 && j == 1)
			break;
		else {
			if (dir(i,j) == 'n') {
				all1[start] = s1[i-1];
				all2[start] = -1;
				i--;
			} else if (dir(i,j) == 'd') {
				all1[start] = s1[i-1];
				all2[start] = s2[j-1];
				i--;
				j--;
			} else {
				all1[start] = -1;
				all2[start] = s2[j-1];
				j--;
			}
			start--;
		}
	}

	this->printInfo();
	// so the new strings all1 and all2 start at "start+1" and end at "2*max_length-1"
	this->identity = new Alignment(all1, all2, start, 2*max_length, d(_l1,_l2), _sigma1, _sigma2, true, true, _l1, _l2, 0, 0, 0);

	delete[] all1;
	delete[] all2;
}

unsigned MPED::computeEditDistance() {

	unsigned weight = 0;
	Matrix<unsigned> d(_l1 + 1, _l2 + 1);

	for (size_t i = 0; i < d.r(); i++) { d(i,0) = i; }
	for (size_t j = 0; j < d.c(); j++) { d(0,j) = j; }

	for (size_t i = 1; i < d.r(); i++)
		for (size_t j = 1; j < d.c(); j++) {
			if (this->self_identity && (_sigma1.at(s1[i-1]) == _sigma2.at(s2[j-1])))
				weight = 0;
			else
				weight = this->weights[ sigma1[s1[i-1]] ][ sigma2[s2[j-1]] ];

			d(i,j) = std::min(
						std::min(
							d(i-1,j) + gap,
							d(i,j-1) + gap),
						d(i-1,j-1) + weight
					);

		}

	return d(_l1,_l2);
}

/**
 * @param _sigma1 original sigma1
 * @param _sigma2 original sigma2
 * @param sigma1 integer representation of sigma1
 * @param sigma2 integer representation of sigma2
 * @param s1 integer representation of s1
 * @param s2 integer representation of s2
 * @param _l1 length of s1
 * @param _l2 length of s2
 * @param weights matching schema
 * @param self_identity boolean self_identity
 * @param gap gap value
 */
unsigned MPED::computeExternalEditDistance(const string& _sigma1, const string& _sigma2,
		unsigned short* sigma1, unsigned short* sigma2, unsigned* s1, unsigned* s2) {

	unsigned weight = 0;
	Matrix<unsigned> d(_l1 + 1, _l2 + 1);

	for (size_t i = 0; i < d.r(); i++) { d(i,0) = i; }
	for (size_t j = 0; j < d.c(); j++) { d(0,j) = j; }

	for (size_t i = 1; i < d.r(); i++)
		for (size_t j = 1; j < d.c(); j++) {
			if (self_identity && (this->_sigma1[s1[i-1]] == this->_sigma2[s2[j-1]]))
				weight = 0;
			else
				weight = this->weights[ indexof(s1[i-1], sigma1, _sgl1) ][ indexof(s2[j-1], sigma2, _sgl2) ];

			d(i,j) = std::min( std::min(
					d(i-1,j) + gap,
					d(i,j-1) + gap),
					d(i-1,j-1) + weight );
		}
	return d(_l1,_l2);
}

/* =================== */

void MPED::debug() {
	sigma1 = initMapping(_sigma1, map_sigma1);
	sigma2 = initMapping(_sigma2, map_sigma2);

	/*for (int i = 0; i < _sigma1.length(); i++)
		cout << sigma1[i] << " ";
	cout << endl;

	for (int i = 0; i < _sigma2.length(); i++)
		cout << sigma2[i] << " ";
	cout << endl;*/

	s1 = setMapping(_s1, map_sigma1);
	s2 = setMapping(_s2, map_sigma2);

	/*for (int i = 0; i < _s1.length(); i++)
			cout << s1[i] << " ";
		cout << endl;

		for (int i = 0; i < _s2.length(); i++)
			cout << s2[i] << " ";
		cout << endl;*/

	this->initGeneralWeights();

	/*for (int i = 0; i < _sigma1.length(); i++) {
		for (int j = 0; j < _sigma2.length(); j++)
			cout << this->weights[i][j] << " ";
		cout << endl;
	}*/
}
