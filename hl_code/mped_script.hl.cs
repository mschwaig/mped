
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using HeuristicLab.Algorithms.SimulatedAnnealing;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Views;
using HeuristicLab.Problems.Binary;


using mped_cs;
//using HeuristicLab.Collections;
//using HeuristicLab.Data;

public class MyScript : HeuristicLab.Scripting.CSharpScriptBase {

    readonly ManualResetEvent mutex = new ManualResetEvent(false);

public override void Main() {
  // type your code here
  var ga = new GeneticAlgorithm {
    MaximumGenerations = { Value = 1 },
    PopulationSize = { Value = 1 },
    Problem = new MpedBasicProblem(AString.create("xyz"), AString.create("abc"))
  };

  var experiment = new Experiment();
  for (int i = 0; i < 1; i++) {
    experiment.Optimizers.Add(new BatchRun() { Optimizer = ga, Repetitions = 1 });
    ga.PopulationSize.Value *= 2;
  }

  experiment.ExecutionStateChanged += OnExecutionStateChanged;
  experiment.Start();
  mutex.WaitOne();

  vars.experiment = experiment;
  MainFormManager.MainForm.ShowContent(experiment);
  var viewHost = (ViewHost)MainFormManager.MainForm.ShowContent(experiment.Runs, typeof(RunCollectionBubbleChartView));
  var bubbleChart = (UserControl)(viewHost.ActiveView);
  bubbleChart.Controls.OfType<ComboBox>().Single(x => x.Name == "yAxisComboBox").SelectedItem = "BestQuality";
  bubbleChart.Controls.OfType<ComboBox>().Single(x => x.Name == "xAxisComboBox").SelectedItem = "PopulationSize";
}

  private void OnExecutionStateChanged(object sender, EventArgs e) {
    if (((IExecutable)sender).ExecutionState == ExecutionState.Stopped)
    {
      mutex.Set();
    }
  }
}
