using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Travelling_Salesman_Problem
{
    /// <summary>
    /// Gathers data for processing.
    /// </summary>
    class TSPData
    {
        public int population;
        public int amount;
        public string[] substrings;
        public string line;
        public int[,] distanceTab;
        public int[,] randCities;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="amount"></param>
        public TSPData(int amount)
        {
            this.amount = amount;

            loadDataFromFile();
            randomizeCities();
        }

        /// <summary>
        /// Loads data from file
        /// </summary>
        private void loadDataFromFile()
        {
            using (StreamReader sr = new StreamReader(@"C:\Users\Lookacz\Desktop\Berlin52.txt"))
            {
                population = Convert.ToInt32(sr.ReadLine());
                distanceTab = new int[population, population];

                for (int i = 0; i < population; i++)
                {
                    line = sr.ReadLine().Trim();
                    substrings = line.Split(' ');
                    for (int j = 0; j < substrings.Length; j++)
                    {
                        distanceTab[i, j] = Convert.ToInt32(substrings[j]);
                        distanceTab[j, i] = Convert.ToInt32(substrings[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Creates 2D list with randomized order of cities.
        /// </summary>
        private void randomizeCities()
        {
            List<List<int>> listNumbers = new List<List<int>>();
            List<int> row = new List<int>();
            int pointer = 0;
            Random rand = new Random();

            for (int i = 0; i < amount; i++)
            {
                List<int> possible = Enumerable.Range(0, population).ToList();

                for (int j = 0; j < population; j++)
                {
                    int index = rand.Next(0, possible.Count);
                    row.Add(possible[index]);
                    possible.RemoveAt(index);

                    if (j == population)
                    {
                        listNumbers.Insert(pointer, row);
                        pointer++;
                    }
                }
            }

            convertListToArray(population, row);
        }

        /// <summary>
        /// Converts List to Array.
        /// </summary>
        /// <param name="population"></param>
        /// <param name="row"></param>
        private void convertListToArray(int population, List<int> row)
        {
            randCities = new int[population, population];
            int set = 0;
            int t = 0;

            foreach (int x in row)
            {
                randCities[t, set] = x;
                set++;

                if (set != population)
                {
                    continue;
                }

                t++;
                set = 0;
            }
        }
    }
}
