/*
 * Heuristic.h
 *
 *  Created on: 20/ott/2015
 *      Author: finalfire
 */

#ifndef HEURISTIC_H_
#define HEURISTIC_H_

#include "../mped/MPED.h"

class Heuristic {
protected:
	unsigned result;
	unsigned eval_count;
	int max_eval_count;
	MPED* mped;

	unsigned short* computed_sigma1;
	unsigned short* computed_sigma2;

public:
	Heuristic(int max_evaluation_number) : result(0), eval_count(0), mped(0), computed_sigma1(0), computed_sigma2(0) {
		max_eval_count = max_evaluation_number;
	};

	// TODO: deconstructor
	// ~Heuristic();

	const unsigned getResult() const { return this->result; }

	const unsigned getEvalCount() const { return this->eval_count; }

	const unsigned short* getComputedSigma1() const { return this->computed_sigma1; }

	const unsigned short* getComputedSigma2() const { return this->computed_sigma2; }
};

#endif /* HEURISTIC_H_ */
