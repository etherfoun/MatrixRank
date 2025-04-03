using System;

namespace MatrixRankCalculator
{
    public class FaultToleranceAnalyzer
    {
        public void AnalyzeSystemReliability(int nodeCount, double nodeMTBF, double taskDuration)
        {
            double failureProbPerNode = 1 - Math.Exp(-taskDuration / nodeMTBF);

            double systemFailureProb = 1 - Math.Pow(1 - failureProbPerNode, nodeCount);

            Console.WriteLine("\nFault Tolerance Analysis:");
            Console.WriteLine($"Node count: {nodeCount}");
            Console.WriteLine($"Node MTBF (hours): {nodeMTBF}");
            Console.WriteLine($"Task duration (hours): {taskDuration}");
            Console.WriteLine($"Probability of node failure during task: {failureProbPerNode:P4}");
            Console.WriteLine($"Probability of system experiencing at least one node failure: {systemFailureProb:P4}");
        }
    }
}

