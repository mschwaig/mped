#include <iostream>
#include <string>
#include "mped/MPED.h"
#include "heuristic/HillClimbing.h"
using namespace std;

int main() {

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

	return 0;
}
