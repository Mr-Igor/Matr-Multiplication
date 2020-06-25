using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp12
{
    class Program
    {
        static async Task<double> MulAsync(string inputLeft, string inputRight, int row, int col)
        {
            double[] rowData, colData;

            rowData = await ReadRow(inputLeft, row);


            colData = await ReadCol(inputRight, col);

            return await Task.Run<double>(Mul(rowData, colData));
        }

        private static Func<double> Mul(double[] rowData, double[] colData)
        {
            Func<double> meth = delegate ()
            {
                double result = 0;
                for (int i = 0; i < rowData.Length; i++)
                {
                    result += rowData[i] * colData[i];
                }
                return result;
            };
            return meth;
        }

        private static Task<double[]> ReadCol(string inputRight, int col)
        {
            StreamReader sr = new StreamReader(inputRight);
            int lenght = File.ReadAllLines(inputRight).Length - 1;
            double[] colData = new double[lenght];
            string line;
            int i = -1;
            Task<double[]> t = Task<double[]>.Factory.StartNew(() =>
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (i == -1) { i++; continue; }
                    colData[i] = double.Parse(line.Split(' ')[col]);
                    i++;
                }
                return colData;
            });

            return t;
        }

        private static Task<double[]> ReadRow(string inputLeft, int row)
        {


            string input1 = File.ReadLines(inputLeft).Skip(1 + row).First();
            string[] input = input1.Split(' ');
            double[] rowData = new double[input.Length - 1];



            Task<double[]> t = Task<double[]>.Factory.StartNew(() =>
            {

                for (int i = 0; i < input.Length - 1; i++)
                {
                    rowData[i] = int.Parse(input[i]);


                }
                return rowData;
            });

            return t;
        }

        public static async void Solve(string inputLeft, string inputRight, string output)
        {
            StreamReader sr = File.OpenText(inputLeft);
            string[] input = sr.ReadLine().Split(' ');
            int rowsLeft = int.Parse(input[0]);
            int colsLeft = int.Parse(input[1]);
            sr.Close();


            sr = File.OpenText(inputRight);
            input = sr.ReadLine().Split(' ');
            int rowsRight = int.Parse(input[0]);
            int colsRight = int.Parse(input[1]);


            sr.Close();

            if (colsLeft != rowsRight)
            {
                Console.WriteLine("Матрицы не могут быть умножены");
                return;
            }
            StreamWriter sw = new StreamWriter(output);
            for (int i = 0; i < rowsLeft; i++)
            {
                Task<double>[] mulTasks = new Task<double>[colsRight];
                for (int j = 0; j < colsRight; j++)
                {
                    mulTasks[j] = MulAsync(inputLeft, inputRight, i, j);
                }
                double[] results = await Task.WhenAll<double>(mulTasks);
                sw.WriteLine(String.Join(" ", results));
            }
            Console.WriteLine("конец");
            sw.Close();
        }

        static void Input_Matrix(string inputLeft, int row, int col)
        {

            Random rnd = new Random();
            StreamWriter sw = new StreamWriter(inputLeft);
            sw.WriteLine($"{row} {col}");
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    sw.Write($"{rnd.Next(0, 10)} ");
                }
                sw.WriteLine();
            }
            sw.Close();

        }


        static void Main(string[] args)
        {
            Console.WriteLine("Выполнить программу?");

            Input_Matrix("Матрица1.txt", 100, 99);
            Input_Matrix("Матрица2.txt", 99, 100);

            Solve("Матрица1.txt", "Матрица2.txt", "Итог.txt");
            Console.ReadKey();
        }
    }
}