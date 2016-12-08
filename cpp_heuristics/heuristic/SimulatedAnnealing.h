#ifndef SIMULATEDANNEALING_H_
#define SIMULATEDANNEALING_H_

#include "Heuristic.h"

class SimulatedAnnealing : public Heuristic{
private:

	void randomNext(unsigned short*, unsigned short*);

public:
	SimulatedAnnealing(MPED& mped, int max_evaluation_number) : Heuristic(max_evaluation_number) { this->mped = &mped; }

	SimulatedAnnealing(MPED& mped, int max_evaluation_number, report_cb cb) : Heuristic(max_evaluation_number, cb) { this->mped = &mped; }

	const unsigned getResult() const { return this->result; }

	void compute();
	void computeAndAlign();
};

#endif /* SIMULATEDANNEALING_H_ */
