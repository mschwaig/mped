/*
 * HillClimbing.cpp
 *
 *  Created on: 21/ott/2015
 *      Author: finalfire
 */

#include "HillClimbing.h"
#include <iostream>
#include <cstdlib>
#include <time.h>

using namespace std;

// Fisher-Yates shuffle
void HillClimbing::shuffle(unsigned short* a, size_t s) {
	size_t j = 0;
	unsigned short t = 0;
	for (size_t i = s-1; i > 0; i--) {
		j = rand() % (i+1);
		t = a[j]; a[j] = a[i]; a[i] = t;
	}

}

bool HillClimbing::isValid(unsigned short* s, size_t s_size, unsigned short p) {
	int currentMin = INT_MAX-1;
	int lastMin = INT_MAX;

	for (int i = 0; i < s_size; i++) {
		if (currentMin > s[i])
			currentMin = s[i];

		if (i >= p)
			if (lastMin > currentMin)
				return false;

		if ((i % p) == (p-1)) {
			lastMin = currentMin;
			currentMin = INT_MAX;
		}
	}
	return true;
}

void HillClimbing::computeAndAlign() {
	this->compute();
	this->mped->computeExternalAlignment(this->computed_sigma1, this->computed_sigma2);
}

// that's the random-restart steepest ascent hill climbing
void HillClimbing::compute() {

	srand(time(NULL));
	this->eval_count = 0;

	unsigned d = this->mped->computeEditDistance();
	unsigned min_dist = d, min_min_dist = d;
	unsigned short k_shuffle = 0, temp = 0, tries = 0;
	unsigned depth = 0;
	bool improved = true;

	// sigma & strings lengths
	size_t sgl1 = this->mped->getSgl1();
	size_t sgl2 = this->mped->getSgl2();

	// for swap
	unsigned short* sigma1_o = new unsigned short[sgl1]; copy(this->mped->getSigma1(), this->mped->getSigma1() + sgl1, sigma1_o);
	unsigned short* sigma2_o = new unsigned short[sgl2]; copy(this->mped->getSigma2(), this->mped->getSigma2() + sgl2, sigma2_o);
	unsigned short* sigma1_t = new unsigned short[sgl1]; copy(this->mped->getSigma1(), this->mped->getSigma1() + sgl1, sigma1_t);
	unsigned short* sigma2_t = new unsigned short[sgl2]; copy(this->mped->getSigma2(), this->mped->getSigma2() + sgl2, sigma2_t);
	// for fixpoint on min
	unsigned short* sigma1_min = new unsigned short[sgl1]; copy(this->mped->getSigma1(), this->mped->getSigma1() + sgl1, sigma1_min);
	unsigned short* sigma2_min = new unsigned short[sgl2]; copy(this->mped->getSigma2(), this->mped->getSigma2() + sgl2, sigma2_min);
	unsigned short* sigma1_min_min = new unsigned short[sgl1]; copy(this->mped->getSigma1(), this->mped->getSigma1() + sgl1, sigma1_min_min);
	unsigned short* sigma2_min_min = new unsigned short[sgl2]; copy(this->mped->getSigma2(), this->mped->getSigma2() + sgl2, sigma2_min_min);

	while (improved) {

		if (!isValid(sigma1_o, sgl1, this->mped->getP1()))
			cout << "! not valid" << endl;

		improved = false;

		for (size_t ip = 0; ip < sgl1; ip++) {
			for (size_t jp = ip; jp < sgl1; jp++) {

				// here comes the swap for sigma1
				copy(sigma1_t, sigma1_t + sgl1, sigma1_o);
				temp = sigma1_o[ip]; sigma1_o[ip] = sigma1_o[jp]; sigma1_o[jp] = temp;

				if (isValid(sigma1_o, sgl1, this->mped->getP1())) {

					for (size_t ipp = 0; ipp < sgl2; ipp++) {
						for (size_t jpp = ipp; jpp < sgl2; jpp++) {

							// here comes the swap for sigma2
							copy(sigma2_t, sigma2_t + sgl2, sigma2_o);
							temp = sigma2_o[ipp]; sigma2_o[ipp] = sigma2_o[jpp]; sigma2_o[jpp] = temp;

							// then we have the distance
							d = this->mped->computeExternalEditDistance(this->mped->get_Sigma1(), this->mped->get_Sigma2(),
									sigma1_o, sigma2_o, mped->getS1(), mped->getS2());
							this->eval_count++;

							if (d < min_dist) {
								min_dist = d;

								improved = true;
								copy(sigma1_o, sigma1_o + sgl1, sigma1_min);
								copy(sigma2_o, sigma2_o + sgl2, sigma2_min);
							}

						}

						copy(sigma2_t, sigma2_t + sgl2, sigma2_o);
					}
				}
			}
		}

		if (improved) {
			// copy sigmaMin to sigmaOrig
			copy(sigma1_min, sigma1_min + sgl1, sigma1_o);
			copy(sigma2_min, sigma2_min + sgl2, sigma2_o);
			// copy sigmaOrig to sigmaTmp
			copy(sigma1_o, sigma1_o + sgl1, sigma1_t);
			copy(sigma2_o, sigma2_o + sgl2, sigma2_t);

			depth++;
		} else {
			if (min_dist < min_min_dist) {
				min_min_dist = min_dist;

				// copy sigmaMin to sigmaMinMin
				copy(sigma1_min, sigma1_min + sgl1, sigma1_min_min);
				copy(sigma2_min, sigma2_min + sgl2, sigma2_min_min);

				improved = true;
				tries = 0;
				depth = 0;
			}

			if (tries < this->mped->getAttempts()) {
				improved = true;
				tries++;

				/*cout << ": before shuffle: ";
				for (int i = 0; i < sgl1; i++) cout << sigma1_t[i]; cout << " - ";
				for (int i = 0; i < sgl2; i++) cout << sigma2_t[i];
				cout << endl;*/

				// random swap
				// for sigma1, we try _SHUFFLE_TRIES times, then if is still not valid, we retry with the original one
				for (k_shuffle = 0; k_shuffle < _SHUFFLE_TRIES && !this->isValid(sigma1_t, sgl1, this->mped->getP1()); ++k_shuffle) this->shuffle(sigma1_t, sgl1);
				if (k_shuffle == _SHUFFLE_TRIES) copy(sigma1_o, sigma1_o + sgl1, sigma1_t);
				// no constraints on the shuffle for sigma2
				this->shuffle(sigma2_t, sgl2);

				/*cout << ": after shuffle: ";
				for (int i = 0; i < sgl1; i++) cout << sigma1_t[i]; cout << " - ";
				for (int i = 0; i < sgl2; i++) cout << sigma2_t[i];
				cout << endl;*/

				copy(sigma1_t, sigma1_t + sgl1, sigma1_o);
				copy(sigma2_t, sigma2_t + sgl2, sigma2_o);

				min_dist = this->mped->computeExternalEditDistance(this->mped->get_Sigma1(), this->mped->get_Sigma2(),
						sigma1_o, sigma2_o, mped->getS1(), mped->getS2());
			} else {
				this->computed_sigma1 = new unsigned short[sgl1];
				copy(sigma1_min_min, sigma1_min_min + sgl1, this->computed_sigma1);
				this->computed_sigma2 = new unsigned short[sgl2];
				copy(sigma2_min_min, sigma2_min_min + sgl2, this->computed_sigma2);

				cout << ">> MPED SCHEMA: ";
				for (size_t i = 0; i < sgl1; i++)
					cout << this->mped->get_Sigma1()[computed_sigma1[i]];
				cout << " - ";
				for (size_t i = 0; i < sgl2; i++)
					cout << this->mped->get_Sigma2()[computed_sigma2[i]];
				cout << endl;

				this->result = min_dist;
			}
		}
	}
}
