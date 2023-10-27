using MPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPILaba
{
    public class MPIClient
    {
        private Action<string> _logAction;
        private Action<string> _outputLine/* = Console.WriteLine*/;
        private Action<string> _output/* = Console.Write*/;

        public int[,] ResultMatrix = new int[3, 4];
        private Stopwatch _stopwatch = new Stopwatch();
        private MatrixGenerator _matrixGenerator = new MatrixGenerator();
        public MPIClient()
        {

        }
        public MPIClient(Action<string> log)
        {
            _logAction = log;
        }
        private void Log(string message) => _logAction?.Invoke(message);
        private void Output(string message) => _output?.Invoke(message);
        private void OutputLine(string message) => _outputLine?.Invoke(message);
        public void CalculateMatrix(ref string[] args, int countMatrixes)
        {
            using (new MPI.Environment(ref args))
            {
                Intracommunicator comm = Communicator.world;
                int rank = comm.Rank;
                int size = comm.Size;
                int countThreads = 4;

                if (rank >= countThreads)
                {
                    return;
                }

                //if (size < 2)
                //{
                //    Log("At least two processes are required for the program to work");
                //    return;
                //}
                try
                {
                    if (rank == 0)
                    {
                        var firstMatrixes = _matrixGenerator.Generate(countMatrixes, 3, 3).ToArray();
                        var secondMatrixes = _matrixGenerator.Generate(countMatrixes, 3, 4).ToArray();
                        _stopwatch.Start();
                        for (int c = 0; c < countMatrixes; c++)
                        {
                            for (int i = 1; i < countThreads; i++)
                            {
                                int[] dataToSend = MatrixToArray(i - 1, firstMatrixes[c]);
                                if (size > 1)
                                {
                                    comm.Send(new Payload
                                    {
                                        Vector = dataToSend,
                                        Matrix = secondMatrixes[c],
                                    }, i, 0);
                                    Log($"Process 0 sends data {string.Join(',', dataToSend)} to process {i}");
                                }
                                else
                                {
                                    var changedData = MatrixConverter.Multiply(dataToSend, secondMatrixes[c]);
                                    for (int j = 0; j < changedData.Length; j++)
                                    {
                                        ResultMatrix[i - 1, j] = changedData[j];
                                    }
                                }
                            }

                            if (size > 1)
                            {
                                for (int i = 1; i < countThreads; i++)
                                {
                                    int[] receivedData = new int[4];
                                    comm.Receive(i, 1, ref receivedData);
                                    Log($"Process 0 received values: {string.Join(',', receivedData)}");
                                    for (int j = 0; j < receivedData.Length; j++)
                                    {
                                        ResultMatrix[i - 1, j] = receivedData[j];
                                    }
                                }
                            }
                            OutputResult(ResultMatrix);
                            ResultMatrix = new int[3, 4];
                        }
                        _stopwatch.Stop();
                        Console.WriteLine($"Processed {countMatrixes} processes in {_stopwatch.ElapsedMilliseconds} ms");
                    }
                    else
                    {
                        if (size == 1)
                            return;
                        for (int c = 0; c < countMatrixes; c++)
                        {
                            comm.Receive(0, 0, out Payload receivedData);
                            Log($"Process {rank} received values: {string.Join(',', receivedData)}");

                            var changedData = MatrixConverter.Multiply(receivedData.Vector, receivedData.Matrix);
                            Log($"Process {rank} change values: {string.Join(',', changedData)}");

                            comm.Send(changedData, 0, 1);
                            Log($"Process {rank} send values: {string.Join(',', changedData)}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private int[] MatrixToArray(int row, int[,] matrix)
        {
            int[] result = new int[3];
            for (int i = 0; i < 3; i++)
            {
                result[i] = matrix[row, i];
            }
            return result;
        }
        private void OutputResult(int[,] matrix)
        {
            var rowsCount = matrix.GetLength(0);
            var colsCount = matrix.GetLength(1);
            OutputLine("==================");
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < colsCount; j++)
                {
                    Output(matrix[i, j] + " ");
                }
                OutputLine(string.Empty);
            }
            OutputLine("==================");
        }
    }
}
