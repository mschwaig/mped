#ifndef ALIGNMENT_H_
#define ALIGNMENT_H_

#include <iostream>
using namespace std;

class Alignment {
private:
	int* s1;
	int* s2;

	unsigned start, end, size;
	unsigned dist;

	size_t original_length_s1;
	size_t original_length_s2;

	string* _sigma1;
	string* _sigma2;

	bool is_identity;
	bool has_identity;

	unsigned short** weights;
	unsigned short* computed_sigma1;
	unsigned short* computed_sigma2;

	void print_identity_alignment();
	void print_external_alignment();

public:

	Alignment(int* a, int* b, unsigned s, unsigned e, unsigned d, string& sg1, string& sg2, bool identity, bool h_identity,
			unsigned ls1, unsigned ls2, unsigned short** w, unsigned short* cs1, unsigned short* cs2) :
								start(s), end(e), dist(d), original_length_s1(ls1), original_length_s2(ls2),
								is_identity(identity), has_identity(h_identity) {
		this->size = end - start;
		s1 = new int[this->size];
		s2 = new int[this->size];

		// copying s1 and s2 in integer representation
		for (size_t i = 0; i < this->size; i++) {
			s1[i] = a[start + i];
			s2[i] = b[start + i];
		}

		_sigma1 = &sg1;
		_sigma2 = &sg2;

		// allocating weights
		weights = new unsigned short*[_sigma1->size()];
		for (size_t i = 0; i < _sigma1->size(); i++)
			weights[i] = new unsigned short[_sigma2->size()];
		// copying them
		for (size_t i = 0; i < _sigma1->size(); i++)
			for (size_t j = 0; j < _sigma2->size(); j++)
				this->weights[i][j] = w[i][j];

		// allocating computed_sigma 1 and 2
		computed_sigma1 = new unsigned short[_sigma1->size()];
		computed_sigma2 = new unsigned short[_sigma2->size()];
		// copying them
		copy(cs1, cs1 + _sigma1->size(), computed_sigma1);
		copy(cs2, cs2 + _sigma2->size(), computed_sigma2);
	}

	~Alignment() {
		delete[] s1;
		delete[] s2;

		if (weights != 0) {
			for (size_t i = 0; i < _sigma1->size(); i++)
				delete[] weights[i];
			delete[] weights;
		}

		if (computed_sigma1 != 0 && computed_sigma2 != 0)
			delete[] computed_sigma1;
			delete[] computed_sigma2;
	}

	inline void print_out() { is_identity ? this->print_identity_alignment() : this->print_external_alignment(); }

	const unsigned getDistance() const { return this->dist; }
};

#endif /* ALIGNMENT_H_ */
