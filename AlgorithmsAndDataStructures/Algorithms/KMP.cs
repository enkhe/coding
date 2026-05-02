// KMP.cs - Knuth-Morris-Pratt string matching.
// Build failure (LPS) function in O(m), then search in O(n). Total O(n + m).
// LPS[i] = length of longest proper prefix of pattern[0..i] that is also a suffix.

using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.Algorithms;

public static class Kmp
{
    public static int[] BuildLps(string pattern)
    {
        var lps = new int[pattern.Length];
        int len = 0;
        for (int i = 1; i < pattern.Length;)
        {
            if (pattern[i] == pattern[len])
            {
                lps[i++] = ++len;
            }
            else if (len > 0)
            {
                len = lps[len - 1];
            }
            else
            {
                lps[i++] = 0;
            }
        }
        return lps;
    }

    // Returns all start indices of `pattern` in `text`.
    public static List<int> FindAll(string text, string pattern)
    {
        var result = new List<int>();
        if (pattern.Length == 0) return result;
        int[] lps = BuildLps(pattern);

        int i = 0, j = 0;
        while (i < text.Length)
        {
            if (text[i] == pattern[j])
            {
                i++; j++;
                if (j == pattern.Length)
                {
                    result.Add(i - j);
                    j = lps[j - 1];
                }
            }
            else if (j > 0)
            {
                j = lps[j - 1];
            }
            else
            {
                i++;
            }
        }
        return result;
    }
}
