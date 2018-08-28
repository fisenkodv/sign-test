using System;
using System.Collections.Generic;
using System.Numerics;

namespace SignTest
{
  internal static class Program
  {
    private static void Main(string[] args)
    {
      Console.WriteLine("Begin Sign Test:");

      var before = new double[] {70, 80, 75, 85, 70, 75, 50, 60};
      var after = new double[] {65, 78, 72, 87, 68, 74, 48, 63};
      Console.WriteLine("The weight data is:");
      ShowVector("Before: ", before, 0, "");
      ShowVector("After : ", after, 0, "\n");

      var counts = DoCounts(before, after);
      Console.WriteLine($"Num success = {counts.success}"); // weight decrease
      Console.WriteLine($"Num failure = {counts.fail}"); // weight increase

      var k = counts.success;
      var n = counts.success + counts.fail;
      Console.WriteLine($"k = {k} n = {n} p = 0.5");

      var probability = BinomRightTail(k, n, 0.5);
      Console.WriteLine($"Probability of \'no effect\' is {probability:F4}");
      Console.WriteLine($"Probability of \'an effect\' is {1 - probability:F4}");

      Console.WriteLine("End Sign Test.");
    }

    private static void ShowVector(string pre, IEnumerable<double> v, int dec, string post)
    {
      Console.Write(pre);
      foreach (var t in v)
        Console.Write($"{t.ToString($"F{dec}")} ");

      Console.WriteLine(post);
    }

    private static (int fail, int success, int neither) DoCounts(
      IReadOnlyList<double> before,
      IReadOnlyList<double> after)
    {
      (int fail, int success, int neither) result = (0, 0, 0);

      for (var i = 0; i < before.Count; ++i)
      {
        if (after[i] > before[i])
          ++result.fail;
        else if (after[i] < before[i])
          ++result.success;
        else
          ++result.neither;
      }

      return result;
    }

    private static BigInteger Choose(int n, int k)
    {
      if (n == k) return 1; // Required special case
      int delta, iMax;
      if (k < n - k)
      {
        // Ex: Choose(100,3)
        delta = n - k;
        iMax = k;
      }
      else
      {
        // Ex: Choose(100,97)
        delta = k;
        iMax = n - k;
      }

      BigInteger ans = delta + 1;
      for (var i = 2; i <= iMax; ++i)
        ans = ans * (delta + i) / i;
      return ans;
    }

    /// <summary>
    /// Probability of k "successes" in n trials
    /// if p is prob of success on a single trial
    /// </summary>
    private static double BinomProb(int k, int n, double p)
    {
      var c = Choose(n, k);
      var left = Math.Pow(p, k);
      var right = Math.Pow(1.0 - p, n - k);
      return (double) c * left * right;
    }

    /// <summary>
    /// Probability of k or more successes in n trials
    /// </summary>
    private static double BinomRightTail(int k, int n, double p)
    {
      var sum = 0.0;
      for (var i = k; i <= n; ++i)
        sum += BinomProb(i, n, p);
      return sum;
    }
  }
}