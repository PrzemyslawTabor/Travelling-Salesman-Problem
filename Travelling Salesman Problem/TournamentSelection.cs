using System;

namespace Travelling_Salesman_Problem
{
    /// <summary>
    /// Tournament selection.
    /// </summary>
    class TournamentSelection
    {
        public int[,] crossbreedingTab;
        public int[,] randCities;
        public int? min;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public TournamentSelection(TSPData data)
        {
            randCities = data.randCities;

            run(data.distanceTab, data.population, data.amount);
        }

        /// <summary>
        /// Runs the selection.
        /// </summary>
        /// <param name="distanceTab"></param>
        /// <param name="population"></param>
        /// <param name="amount"></param>
        public void run(int[,] distanceTab, int population, int amount)
        {
            for (int iteracja = 0; iteracja < 1000000; iteracja++)
            {
                int[] ratingBoard = Rating(randCities, distanceTab, population, amount);

                for (int i = 0; i < amount; i++)
                {
                    if (!min.HasValue)
                    {
                        min = ratingBoard[i];
                    }

                    if (min > ratingBoard[i])
                    {
                        min = ratingBoard[i];
                        Console.WriteLine("Iteration no. " + iteracja + " Best score: " + min);
                        for (int j = 0; j < population; j++)
                        {
                            Console.Write(randCities[i, j] + "-");
                        }
                        Console.WriteLine();
                    }
                }

                int[] tournamentScore = Tournament(ratingBoard, amount);
                int[,] newPopulation = GetNewPopulation(randCities, tournamentScore, population, amount);
                crossbreedingTab = PMXCrossbreeding(newPopulation, population, amount);
                randCities = Mutation(crossbreedingTab, population, amount, 9950);
            }
        }

        /// <summary>
        /// Creates rating board.
        /// </summary>
        /// <param name="randCities"></param>
        /// <param name="distanceTab"></param>
        /// <param name="population"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static int[] Rating(int[,] randCities, int[,] distanceTab, int population, int amount)
        {
            int a, b;
            int[] ratingBoard = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                ratingBoard[i] = 0;
                for (int j = 0; j < population; j++)
                {
                    a = randCities[i, j];
                    if (j == randCities.GetLength(1) - 1)
                    {
                        b = randCities[i, 0];
                    }
                    else
                    {
                        b = randCities[i, j + 1];
                    }
                    ratingBoard[i] += distanceTab[a, b];
                }
            }

            return ratingBoard;
        }

        /// <summary>
        /// Selects chromosomes in tournament.
        /// </summary>
        /// <param name="ratingBoard"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static int[] Tournament(int[] ratingBoard, int amount)
        {
            int tournamentSize = 4; // rozmiar turnieju (podobno najlepiej < 5) 
            int minValue = 0;
            int chromosome = -1;
            int[] tournamentScore = new int[amount];
            Random rand = new Random();

            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < tournamentSize; j++)
                {
                    int randChromosome = rand.Next(0, amount); // losuje randomowy chromosom 
                    if (j == 0 || minValue > ratingBoard[randChromosome])  // wybiera najmniejszy z turnieju
                    {
                        minValue = ratingBoard[randChromosome];
                        chromosome = randChromosome;
                    }
                }
                tournamentScore[i] = chromosome; // tabela z wygranymi z turnieju
            }

            return tournamentScore;
        }

        /// <summary>
        /// Creates new population based on tournament score.
        /// </summary>
        /// <param name="randCities"></param>
        /// <param name="tournamentScore"></param>
        /// <param name="population"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static int[,] GetNewPopulation(int[,] randCities, int[] tournamentScore, int population, int amount)
        {
            int[,] newPopulation = new int[amount, population];
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < population; j++)
                {
                    newPopulation[i, j] = randCities[tournamentScore[i], j];
                }
            }
            return newPopulation;
        }

        /// <summary>
        /// PMX crossbreeding
        /// </summary>
        /// <param name="newPopulation"></param>
        /// <param name="population"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static int[,] PMXCrossbreeding(int[,] newPopulation, int population, int amount)
        {
            Random rand = new Random();
            int[,] crossbreedingTab = new int[amount, population];
            int part1, part2, item1, item2;
            int[] row1 = new int[population];
            int[] row2 = new int[population];
            int x;

            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < population; j++)
                {
                    crossbreedingTab[i, j] = -1;
                }
            }

            for (int i = 0; i < amount; i += 2)
            {
                x = rand.Next(0, 10000);
                if (x > amount)
                {
                    part1 = rand.Next(0, population);
                    part2 = rand.Next(0, population);

                    while (part1 == part2)
                    {
                        part2 = rand.Next(0, population);
                    }

                    if (part1 > part2)
                    {
                        (part1, part2) = (part2, part1);
                    }

                    for (int j = part1; j <= part2; j++)
                    {
                        (crossbreedingTab[i, j], crossbreedingTab[i + 1, j]) = (newPopulation[i + 1, j], newPopulation[i, j]);
                    }
                }
                else
                {
                    for (int a = 0; a < population; a++)
                    {
                        (crossbreedingTab[i, a], crossbreedingTab[i + 1, a]) = (newPopulation[i, a], newPopulation[i + 1, a]);
                    }
                }
            }

            for (int i = 0; i < amount; i += 2)
            {
                for (int j = 0; j < population; j++)
                {
                    (row1[j], row2[j]) = (crossbreedingTab[i, j], crossbreedingTab[i + 1, j]);
                }
                for (int j = 0; j < population; j++)
                {
                    if (row1[j] == -1)
                    {
                        (item1, item2) = (newPopulation[i, j], newPopulation[i + 1, j]);

                        while (Array.IndexOf(row1, item1) != -1)
                        {
                            item1 = newPopulation[i, Array.IndexOf(row1, item1)];
                        }

                        while (Array.IndexOf(row2, item2) != -1)
                        {
                            item2 = newPopulation[i + 1, Array.IndexOf(row2, item2)];
                        }

                        (crossbreedingTab[i, j], crossbreedingTab[i + 1, j]) = (item1, item2);
                    }
                }
            }

            return crossbreedingTab;
        }

        /// <summary>
        /// Mutates by switching.
        /// </summary>
        /// <param name="mutationTab"></param>
        /// <param name="population"></param>
        /// <param name="amount"></param>
        /// <param name="pointer"></param>
        /// <returns></returns>
        private static int[,] Mutation(int[,] mutationTab, int population, int amount, int pointer)
        {
            Random rand = new Random();
            int changedIndex; 

            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < population; j++)
                {
                    if (rand.Next(0, 10000) < pointer)
                    {
                        continue;
                    }

                    changedIndex = rand.Next(0, population);
                    (mutationTab[i, j], mutationTab[i, changedIndex]) = (mutationTab[i, changedIndex], mutationTab[i, j]);
                }
            }

            return mutationTab;
        }
    }
}
