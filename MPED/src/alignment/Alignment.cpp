#include "Alignment.h"

void Alignment::print_identity_alignment() {
	cout << ">> IDENTITY ALIGNMENT" << endl;

	// print first string
	for (size_t i = 0; i < this->size; i++)
		if (s1[i] != -1)
			cout << (*_sigma1)[s1[i]];
		else
			cout << "-";
	cout << endl;

	// print second string
	for (size_t i = 0; i < this->size; i++)
		if (s2[i] != -1)
			cout << (*_sigma2)[s2[i]];
		else
			cout << "-";
	cout << endl;

	// print sharp for each identity match
	int sharps = 0;
	for (size_t i = 0; i < this->size; i++)
		if ((*_sigma1)[s1[i]] == (*_sigma2)[s2[i]] && s1[i] != -1) {
			cout << "*";
			sharps++;
		} else
			cout << " ";
	cout << endl;

	float perc = sharps / (float) this->size;
	cout << "d-min: " << this->size - sharps << endl;
	cout << "% OF IDENTITY (ALL):\t" << sharps << " / " << this->size << " = " << perc * 100 << "%" << endl;
	perc = sharps / (float) this->original_length_s1;
	cout << "% OF IDENTITY (s1):\t" << sharps << " / " << this->original_length_s1 << " = " << perc * 100 << "%" << endl;
	perc = sharps / (float) this->original_length_s2;
	cout << "% OF IDENTITY (s2):\t" << sharps << " / " << this->original_length_s2 << " = " << perc * 100 << "%" << endl;
}

void Alignment::print_external_alignment() {
	cout << ">> ALIGNMENT BY HEURISTIC" << endl;

	// print first string
	for (size_t i = 0; i < this->size; i++)
		if (s1[i] != -1)
			cout << (*_sigma1)[s1[i]];
		else
			cout << "-";
	cout << endl;

	// print second string
	for (size_t i = 0; i < this->size; i++)
		if (s2[i] != -1)
			cout << (*_sigma2)[s2[i]];
		else
			cout << "-";
	cout << endl;

	// print sharp for each identity match
	int stars = 0;

	int pos_of_s1 = -1;
	int pos_of_s2 = -1;

	// this routine just print out the alignment in the following syntax:
	// x1 x2 x1 x1 - x3 x4 ...
	// y2 y1 y2 -  - y1 y1 ...
	// *     *       *  *  ...
	// where a star indicated that the two symbols match
	for (size_t i = 0; i < this->size; i++) {
		if (s1[i] != -1 && s2[i] != -1) {
			pos_of_s1 = -1;
			pos_of_s2 = -1;

			// search for s1[i] in _sigma1
			for (size_t inx = 0; inx < _sigma1->size(); inx++) if (s1[i] == this->computed_sigma1[inx]) pos_of_s1 = inx;
			// search for s2[i]  in _sigma1
			for (size_t inx = 0; inx < _sigma2->size(); inx++) if (s2[i] == this->computed_sigma2[inx]) pos_of_s2 = inx;

			if (!this->weights[pos_of_s1][pos_of_s2] || (this->has_identity && (*_sigma1)[s1[i]] == (*_sigma2)[s2[i]])) {
				stars++;
				cout << "*";
			} else {
				cout << " ";
			}

		} else {
			cout << " ";
		}
	}
	cout << endl;

	float perc = stars / (float) this->size;
	cout << "d-min: " << this->size - stars << endl;
	cout << "% OF IDENTITY (ALL):\t" << stars << " / " << this->size << " = " << perc * 100 << "%" << endl;
	perc = stars / (float) this->original_length_s1;
	cout << "% OF IDENTITY (s1):\t" << stars << " / " << this->original_length_s1 << " = " << perc * 100 << "%" << endl;
	perc = stars / (float) this->original_length_s2;
	cout << "% OF IDENTITY (s2):\t" << stars << " / " << this->original_length_s2 << " = " << perc * 100 << "%" << endl;
}

