using System;
using System.Diagnostics;
using NUnit.Framework;
using MPI;

namespace MatrixRankCalculator.Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        public void PerformanceAnalyzer_CalculatesCorrectSpeedupAndEfficiency()
        {
            double sequentialTime = 1000;
            double parallelTime = 250;
            int nodeCount = 4;
            int coresPerNode = 1; 

            var analyzer = new PerformanceAnalyzer();

            var originalOut = Console.Out;
            using (var consoleOutput = new System.IO.StringWriter())
            {
                Console.SetOut(consoleOutput);

                analyzer.AnalyzePerformance(sequentialTime, parallelTime, nodeCount, coresPerNode);

                string output = consoleOutput.ToString();

                // Assert
                Assert.That(output, Contains.Substring("Speedup: 4.00x"), "Speedup should be calculated correctly");
                Assert.That(output, Contains.Substring("Efficiency: 1.00"), "Efficiency should be calculated correctly");
            }
            Console.SetOut(originalOut);
        }

        [Test]
        public void FaultToleranceAnalyzer_CalculatesCorrectReliabilityMetrics()
        {
            // Arrange
            int nodeCount = 4;
            double nodeMTBF = 8760; // 1 year in hours
            double taskDuration = 1; // 1 hour

            var analyzer = new FaultToleranceAnalyzer();

            // Act - we'll capture console output to verify the calculations
            var originalOut = Console.Out;
            using (var consoleOutput = new System.IO.StringWriter())
            {
                Console.SetOut(consoleOutput);

                analyzer.AnalyzeSystemReliability(nodeCount, nodeMTBF, taskDuration);

                string output = consoleOutput.ToString();

                // Assert
                Assert.That(output, Contains.Substring("Node count: 4"), "Node count should be displayed correctly");
                Assert.That(output, Contains.Substring("Node MTBF (hours): 8760"), "MTBF should be displayed correctly");
            }
            Console.SetOut(originalOut);
        }
    }
}

