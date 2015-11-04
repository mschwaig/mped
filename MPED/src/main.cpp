#include <iostream>
#include <string>
#include "utility/SeqReader.h"
#include "mped/MPED.h"
#include "heuristic/HillClimbing.h"
#include "heuristic/SimulatedAnnealing.h"
using namespace std;

int main() {

	// SeqReader s("from_this_file.txt");

	MPED mped(string("AAABCCDCADD"), string("BABAEFEAFAD"));
	mped.setAttempts(1);
	mped.setSelfIdentity(false);
	mped.debug();

	//mped.computeIdentityAlignment();
	//Alignment* identity = mped.getIdentityAligment();
	//identity->print_out();

	HillClimbing h(mped);
	h.computeAndAlign();
	Alignment* external = mped.getExternalAlignment();
	external->print_out();
	delete external;

	SimulatedAnnealing s(mped);
	s.computeAndAlign();
	external = mped.getExternalAlignment();
	external->print_out();
	delete external;

	return 0;
}
