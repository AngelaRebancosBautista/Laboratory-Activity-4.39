using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratory_Activity_39
{
    internal class Program
    {
        public class SeatGrid
        {
            private readonly int _rows;
            private readonly int _cols;
            private readonly Random _random;
            private readonly List<string> _students;
            private readonly HashSet<Tuple<string, string>> _friendships;
            private readonly HashSet<Tuple<string, string>> _flaggedPairs;

            public SeatGrid(List<string> students, List<Tuple<string, string>> friendships,
                           List<Tuple<string, string>> flaggedPairs, int rows, int cols, int? seed = null)
            {
                _rows = rows;
                _cols = cols;
                _students = students;
                _friendships = new HashSet<Tuple<string, string>>(friendships.Select(p =>
                    p.Item1.CompareTo(p.Item2) < 0 ? Tuple.Create(p.Item1, p.Item2) : Tuple.Create(p.Item2, p.Item1)));
                _flaggedPairs = new HashSet<Tuple<string, string>>(flaggedPairs.Select(p =>
                    p.Item1.CompareTo(p.Item2) < 0 ? Tuple.Create(p.Item1, p.Item2) : Tuple.Create(p.Item2, p.Item1)));

                _random = seed.HasValue ? new Random(seed.Value) : new Random();
            }

            private List<string> GetRandomOrder()
            {
                return _students.OrderBy(s => _random.Next()).ToList();
            }

            private bool AreAdjacent(int index1, int index2)
            {
                var row1 = index1 / _cols;
                var col1 = index1 % _cols;
                var row2 = index2 / _cols;
                var col2 = index2 % _cols;

                return Math.Abs(row1 - row2) <= 1 && Math.Abs(col1 - col2) <= 1 && !(row1 == row2 && col1 == col2);
            }

            private int CountViolations(List<string> seating)
            {
                int violations = 0;

                for (int i = 0; i < seating.Count; i++)
                {
                    for (int j = i + 1; j < seating.Count; j++)
                    {
                        if (!AreAdjacent(i, j)) continue;

                        var pair = seating[i].CompareTo(seating[j]) < 0 ?
                            Tuple.Create(seating[i], seating[j]) :
                            Tuple.Create(seating[j], seating[i]);

                        if (_flaggedPairs.Contains(pair)) violations++;
                    }
                }

                return violations;
            }

            private int FriendClusteringScore(List<string> seating)
            {
                int score = 0;

                for (int i = 0; i < seating.Count; i++)
                {
                    for (int j = i + 1; j < seating.Count; j++)
                    {
                        if (!AreAdjacent(i, j)) continue;

                        var pair = seating[i].CompareTo(seating[j]) < 0 ?
                            Tuple.Create(seating[i], seating[j]) :
                            Tuple.Create(seating[j], seating[i]);

                        if (_friendships.Contains(pair)) score++;
                    }
                }

                return score;
            }

            public Tuple<List<string>, int> GenerateSeating(int maxAttempts = 1000)
            {
                List<string> bestSeating = null;
                int minViolations = int.MaxValue;
                int bestFriendScore = -1;
                int? stableSeed = null;

                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    var seating = GetRandomOrder();
                    var violations = CountViolations(seating);
                    var friendScore = FriendClusteringScore(seating);

                    if (violations == 0 && (bestSeating == null || friendScore > bestFriendScore))
                    {
                        bestSeating = seating;
                        minViolations = violations;
                        bestFriendScore = friendScore;
                        stableSeed = _random.Next();
                    }
                }

                if (bestSeating == null)
                {
                    bestSeating = GetRandomOrder();
                    minViolations = CountViolations(bestSeating);
                    stableSeed = _random.Next();
                }

                Console.WriteLine($"Using seed: {stableSeed}");
                return Tuple.Create(bestSeating, minViolations);
            }
            public void PrintSeating(List<string> seating)
            {
                Console.WriteLine("Seating Arrangement:");
                for (int row = 0; row < _rows; row++)
                {
                    for (int col = 0; col < _cols; col++)
                    {
                        int index = row * _cols + col;
                        if (index < seating.Count)
                            Console.Write($"{seating[index],-15}");
                        else
                            Console.Write($"{new string(' ', 15)}");
                    }
                    Console.WriteLine();
                }
            }
        }
        class Prog
        {
            static void Main(string[] args)
            {
                var students = new List<string> {
            "Angela", "Bobby", "Char", "Dane", "Eve",
            "Frank", "Grace", "Heidi", "Ivan", "Judy"
        };

                var friendships = new List<Tuple<string, string>> {
            Tuple.Create("Angela", "Bobby"),
            Tuple.Create("Char", "Dane"),
            Tuple.Create("Eve", "Frank")
        };

                var flaggedPairs = new List<Tuple<string, string>> {
            Tuple.Create("Grace", "Heidi"),
            Tuple.Create("Ivan", "Judy")
        };

                int rows = 3;
                int cols = 4;

                var seatGrid = new SeatGrid(students, friendships, flaggedPairs, rows, cols);
                var result = seatGrid.GenerateSeating();

                seatGrid.PrintSeating(result.Item1);
                Console.WriteLine($"Violations: {result.Item2}");
            }
        }
    }
}
        
