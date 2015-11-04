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
	MPED* mped;

	unsigned short* computed_sigma1;
	unsigned short* computed_sigma2;

public:
	Heuristic() : result(0), mped(0), computed_sigma1(0), computed_sigma2(0) {};

	// TODO: deconstructor
	// ~Heuristic();

	const unsigned getResult() const { return this->result; }
};

#endif /* HEURISTIC_H_ */
