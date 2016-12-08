/*
 * Heuristic.h
 *
 *  Created on: 20/ott/2015
 *      Author: finalfire
 */

#ifndef HEURISTIC_H_
#define HEURISTIC_H_

#include "../mped/MPED.h"

 // Declare an unmanaged function type that takes two int arguments  
 // Note the use of __stdcall for compatibility with managed code  
typedef void(__stdcall *report_cb)(int, int);


static void(__stdcall empty_report_cb)(int mped, int eval_count) {}

class Heuristic {
protected:
	unsigned result;
	unsigned eval_count;
	int max_eval_count;
	MPED* mped;

	unsigned short* computed_sigma1;
	unsigned short* computed_sigma2;

	report_cb reportMpedAndEvalCount;

public:
	Heuristic(int max_evaluation_number) : Heuristic(max_evaluation_number, empty_report_cb) {};

	Heuristic(int max_evaluation_number, report_cb cb) : result(0), eval_count(0), mped(0), computed_sigma1(0), computed_sigma2(0) {
		max_eval_count = max_evaluation_number;
		reportMpedAndEvalCount = cb;
	};

	// TODO: deconstructor
	// ~Heuristic();

	const unsigned getResult() const { return this->result; }

	const unsigned getEvalCount() const { return this->eval_count; }

	const unsigned short* getComputedSigma1() const { return this->computed_sigma1; }

	const unsigned short* getComputedSigma2() const { return this->computed_sigma2; }
};

#endif /* HEURISTIC_H_ */
