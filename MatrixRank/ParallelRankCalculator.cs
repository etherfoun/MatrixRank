using System;
using System.Threading.Tasks;
using MPI;

namespace MatrixRankCalculator
{
    public class ParallelRankCalculator
    {
        private const double EPSILON = 1e-10;

        public int CalculateRank(double[,] globalMatrix, Intracommunicator comm)
        {
            int rank = comm.Rank;
            int size = comm.Size;

            int rows = 0;
            int cols = 0;

            if (rank == 0 && globalMatrix != null)
            {
                rows = globalMatrix.GetLength(0);
                cols = globalMatrix.GetLength(1);
            }

            comm.Broadcast(ref rows, 0);
            comm.Broadcast(ref cols, 0);

            int localRowCount = rows / size;
            int remainder = rows % size;

            if (rank < remainder)
            {
                localRowCount++;
            }

            int[] rowCounts = new int[size];
            int[] rowOffsets = new int[size];

            for (int i = 0; i < size; i++)
            {
                rowCounts[i] = rows / size;
                if (i < remainder)
                {
                    rowCounts[i]++;
                }
            }

            for (int i = 1; i < size; i++)
            {
                rowOffsets[i] = rowOffsets[i - 1] + rowCounts[i - 1];
            }

            double[,] localMatrix = new double[localRowCount, cols];

            if (rank == 0)
            {
                for (int i = 0; i < rowCounts[0]; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        localMatrix[i, j] = globalMatrix[i, j];
                    }
                }

                for (int dest = 1; dest < size; dest++)
                {
                    double[] buffer = new double[rowCounts[dest] * cols];
                    int idx = 0;

                    for (int i = rowOffsets[dest]; i < rowOffsets[dest] + rowCounts[dest]; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            buffer[idx++] = globalMatrix[i, j];
                        }
                    }

                    comm.Send(buffer, dest, 0);
                }
            }
            else
            {
                double[] buffer = new double[localRowCount * cols];
                comm.Receive(0, 0, ref buffer);

                int idx = 0;
                for (int i = 0; i < localRowCount; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        localMatrix[i, j] = buffer[idx++];
                    }
                }
            }

            int matrixRank = PerformParallelGaussianElimination(localMatrix, localRowCount, cols, rowOffsets[rank], comm);

            return matrixRank;
        }

        private int PerformParallelGaussianElimination(double[,] localMatrix, int localRows, int cols,
                                                      int rowOffset, Intracommunicator comm)
        {
            int rank = comm.Rank;
            int size = comm.Size;
            int globalRows = comm.Allreduce(localRows, Operation<int>.Add);

            bool[] localRowUsed = new bool[localRows];

            for (int col = 0; col < cols; col++)
            {
                int localPivotRow = -1;
                double localMaxValue = EPSILON;

                for (int i = 0; i < localRows; i++)
                {
                    if (!localRowUsed[i] && Math.Abs(localMatrix[i, col]) > localMaxValue)
                    {
                        localMaxValue = Math.Abs(localMatrix[i, col]);
                        localPivotRow = i;
                    }
                }

                double[] localPivotInfo = new double[2];
                if (localPivotRow != -1)
                {
                    localPivotInfo[0] = localMaxValue;
                    localPivotInfo[1] = rowOffset + localPivotRow;
                }
                else
                {
                    localPivotInfo[0] = 0;
                    localPivotInfo[1] = -1;
                }

                double[] allPivotInfo = null;
                if (rank == 0)
                {
                    allPivotInfo = new double[2 * size];
                }

                comm.GatherFlattened<double>(localPivotInfo, 0, ref allPivotInfo);

                int globalPivotRow = -1;
                int pivotProcess = -1;
                double globalMaxValue = EPSILON;

                if (rank == 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (allPivotInfo[2 * i] > globalMaxValue)
                        {
                            globalMaxValue = allPivotInfo[2 * i];
                            globalPivotRow = (int)allPivotInfo[2 * i + 1];
                            pivotProcess = i;
                        }
                    }
                }

                comm.Broadcast(ref globalPivotRow, 0);
                comm.Broadcast(ref pivotProcess, 0);

                if (globalPivotRow == -1)
                {
                    continue;
                }

                int localPivotIndex = globalPivotRow - rowOffset;

                double[] pivotRow = new double[cols];

                if (rank == pivotProcess)
                {
                    localRowUsed[localPivotIndex] = true;

                    double pivotValue = localMatrix[localPivotIndex, col];

                    Parallel.For(0, cols, j =>
                    {
                        pivotRow[j] = localMatrix[localPivotIndex, j] / pivotValue;
                        localMatrix[localPivotIndex, j] = pivotRow[j];
                    });
                }

                comm.Broadcast<double[]>(ref pivotRow, pivotProcess);

                Parallel.For(0, localRows, i =>
                {
                    if (rank == pivotProcess && i == localPivotIndex)
                        return;

                    double factor = localMatrix[i, col];
                    if (Math.Abs(factor) > EPSILON)
                    {
                        for (int j = col; j < cols; j++)
                        {
                            localMatrix[i, j] -= factor * pivotRow[j];
                        }
                    }
                });
            }

            int localNonZeroRows = 0;
            for (int i = 0; i < localRows; i++)
            {
                if (localRowUsed[i])
                {
                    localNonZeroRows++;
                }
            }

            int globalRank = comm.Allreduce(localNonZeroRows, Operation<int>.Add);

            return globalRank;
        }
    }
}

