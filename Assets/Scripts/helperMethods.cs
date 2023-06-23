using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class helperMethods
{
    private static helperMethods instance;

    private helperMethods()
    {
    }

    public void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = random.Next(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public Color GetColorFromString(string colorName)
    {
        switch (colorName)
        {
            case "red":
                return Color.red;
            case "green":
                return Color.green;
            case "blue":
                return Color.blue;
            case "white":
                return Color.white;
            default:
                return Color.magenta;
        }
    }

    public void RestartScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public List<List<string>> GeneratePermutations(List<string> colors, int length, int count)
    {
        List<List<string>> permutations = new List<List<string>>();

        void GeneratePermutationsRecursive(List<string> currentPermutation)
        {
            if (currentPermutation.Count == length)
            {
                if (!AreAllColorsSame(currentPermutation)) // Check if all colors in the current permutation are not the same
                {
                    permutations.Add(new List<string>(currentPermutation));
                }
                return;
            }

            foreach (string color in colors)
            {
                if (currentPermutation.Count > 0 && currentPermutation[currentPermutation.Count - 1] == color)
                {
                    continue; // Skip the current color if it is the same as the previous one
                }

                currentPermutation.Add(color);
                GeneratePermutationsRecursive(currentPermutation);
                currentPermutation.RemoveAt(currentPermutation.Count - 1);
            }
        }

        GeneratePermutationsRecursive(new List<string>());
        ShuffleList(permutations);

        if (permutations.Count > count)
        {
            return permutations.GetRange(0, count);
        }
        else
        {
            return permutations;
        }
    }

    private bool AreAllColorsSame(List<string> colors)
    {
        string firstColor = colors[0];
        for (int i = 1; i < colors.Count; i++)
        {
            if (colors[i] != firstColor)
            {
                return false;
            }
        }
        return true;
    }

    public static helperMethods GetInstance()
    {
        if (instance == null)
        {
            instance = new helperMethods();
        }
        return instance;
    }

}