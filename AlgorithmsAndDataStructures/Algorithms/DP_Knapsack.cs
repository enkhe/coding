// DP_Knapsack.cs - 0/1 and unbounded knapsack.
// 0/1: each item used at most once. O(n * W) time, O(W) space (rolling array).
// Unbounded: each item used unlimited times. Same complexity, forward update.

using System;

namespace AlgorithmsAndDataStructures.Algorithms;

public static class Knapsack
{
    // 0/1 knapsack: maximize total value with weight <= capacity.
    // Iterate weights in reverse to avoid double-counting an item.
    public static int ZeroOne(int[] weights, int[] values, int capacity)
    {
        if (weights.Length != values.Length) throw new ArgumentException();
        var dp = new int[capacity + 1];
        for (int i = 0; i < weights.Length; i++)
        {
            int w = weights[i], v = values[i];
            for (int c = capacity; c >= w; c--)
                dp[c] = Math.Max(dp[c], dp[c - w] + v);
        }
        return dp[capacity];
    }

    // Unbounded knapsack: items may be picked any number of times.
    // Iterate weights forward.
    public static int Unbounded(int[] weights, int[] values, int capacity)
    {
        if (weights.Length != values.Length) throw new ArgumentException();
        var dp = new int[capacity + 1];
        for (int c = 1; c <= capacity; c++)
            for (int i = 0; i < weights.Length; i++)
                if (weights[i] <= c)
                    dp[c] = Math.Max(dp[c], dp[c - weights[i]] + values[i]);
        return dp[capacity];
    }
}
