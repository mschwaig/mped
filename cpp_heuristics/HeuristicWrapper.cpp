#include "heuristic/HillClimbing.h"
#include "heuristic/SimulatedAnnealing.h"

#include <string>
#include <msclr\marshal_cppstd.h>

using System::Runtime::InteropServices::Marshal;
using msclr::interop::marshal_as;

using at::mschwaig::mped::definitions::AlgorithmType;
using at::mschwaig::mped::persistence::BestSolution;
using at::mschwaig::mped::persistence::Problem;
using at::mschwaig::mped::persistence::Result;
using at::mschwaig::mped::persistence::HeuristicRun;

namespace at {
namespace mschwaig {
namespace mped {
namespace cpp_heuristics {


	template <typename heuristic>
	public ref class HeuristicBase : at::mschwaig::mped::persistence::Heuristic {
	public:
		HeuristicBase(AlgorithmType type) : at::mschwaig::mped::persistence::Heuristic(type) {}

		 virtual Result^ applyTo(Problem^ p, int max_evaluation_number) override {

			std::string s1 = marshal_as<std::string>(p->s1);
			std::string s2 = marshal_as<std::string>(p->s2);
			std::string a = marshal_as<std::string>(gcnew System::String(p->a));
			std::string b = marshal_as<std::string>(gcnew System::String(p->b));

			MPED prob(s1, s2, a, b);
			prob.setAttempts(1);
			prob.setSelfIdentity(false);
			prob.debug();


			heuristic h(prob, max_evaluation_number);
			h.computeAndAlign();

			const unsigned short * sigma1 = h.getComputedSigma1();
			int* sigma1_int = new int[prob.getSgl1()];
			for (int i = 0; i < prob.getSgl1(); i++)
			{
				sigma1_int[i] = sigma1[i];
			}
			cli::array<System::Int32>^ permutation1 = gcnew cli::array<System::Int32>(prob.getSgl1());

			Marshal::Copy(System::IntPtr((void *)sigma1_int), permutation1, 0, prob.getSgl1());

			delete sigma1_int;

			const unsigned short * sigma2 = h.getComputedSigma2();
			int* sigma2_int = new int[prob.getSgl2()];
			for (int i = 0; i < prob.getSgl2(); i++)
			{
				sigma2_int[i] = sigma2[i];
			}

			cli::array<System::Int32>^ permutation2 = gcnew cli::array<System::Int32>(prob.getSgl2());

			Marshal::Copy(System::IntPtr((void *)sigma2_int), permutation2, 0, prob.getSgl2());

			delete sigma2_int;

			persistence::Result^ r = gcnew persistence::Result(p, run);

			r->Solutions->Add(gcnew BestSolution(r, permutation1, permutation2, h.getEvalCount()));

			return r;

		}
	};



	public ref class CPPHillClimbingHeuristic : HeuristicBase<::HillClimbing> {
	public:
		CPPHillClimbingHeuristic() : HeuristicBase(AlgorithmType::CPP_HILLCLIMBING) {}
	};

	public ref class CPPSimulatedAnnealingHeuristic : HeuristicBase<::SimulatedAnnealing> {
	public:
		CPPSimulatedAnnealingHeuristic() : HeuristicBase(AlgorithmType::CPP_SIMULATEDANNEALING) {}
	};
}
}
}
}