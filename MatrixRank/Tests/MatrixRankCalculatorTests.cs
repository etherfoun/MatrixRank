using NUnit.Framework;

namespace MatrixRankCalculator.Tests
{
    [TestFixture]
    public class MatrixRankCalculatorTests
    {
        [Test]
        public void SequentialCalculator_IdentityMatrix_ReturnsFullRank()
        {
            int size = 10;
            double[,] identityMatrix = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                identityMatrix[i, i] = 1;
            }
            
            var calculator = new SequentialRankCalculator();
            
            int rank = calculator.CalculateRank(identityMatrix);
            
            Assert.That(rank, Is.EqualTo(size), "Identity matrix should have full rank");
        }
        
        [Test]
        public void SequentialCalculator_ZeroMatrix_ReturnsZeroRank()
        {
            int size = 10;
            double[,] zeroMatrix = new double[size, size];
            var calculator = new SequentialRankCalculator();
            
            int rank = calculator.CalculateRank(zeroMatrix);
            
            Assert.That(rank, Is.EqualTo(0), "Zero matrix should have rank 0");
        }
        
        [Test]
        public void SequentialCalculator_KnownRankMatrix_ReturnsCorrectRank()
        {
            int size = 10;
            int expectedRank = 5;
            double[,] matrix = MatrixGenerator.GenerateMatrixWithKnownRank(size, expectedRank);
            var calculator = new SequentialRankCalculator();
            
            int rank = calculator.CalculateRank(matrix);
            
            Assert.That(rank, Is.EqualTo(expectedRank), "Matrix should have the expected rank");
        }
        
        [Test]
        public void SequentialCalculator_NonSquareMatrix_ReturnsCorrectRank()
        {
            int rows = 10;
            int cols = 5;
            double[,] matrix = new double[rows, cols];
            
            for (int i = 0; i < cols; i++)
            {
                matrix[i, i] = 1;
            }
            
            var calculator = new SequentialRankCalculator();
            
            int rank = calculator.CalculateRank(matrix);
            
            Assert.That(rank, Is.EqualTo(cols), "Non-square matrix should have correct rank");
        }
    }
}

