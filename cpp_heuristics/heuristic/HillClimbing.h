#ifndef HILLCLIMBING_H_
#define HILLCLIMBING_H_

#include "Heuristic.h"

class HillClimbing : public Heuristic {
private:

	bool isValid(unsigned short*, size_t, unsigned short);
	void shuffle(unsigned short*, size_t);

	static const size_t _SHUFFLE_TRIES = 30;

public:
	HillClimbing(MPED& mped, int max_evaluation_number) : Heuristic(max_evaluation_number) { this->mped = &mped; }
	const unsigned getResult() const { return this->result; }

	void compute();
	void computeAndAlign();
};

#endif /* HILLCLIMBING_H_ */
