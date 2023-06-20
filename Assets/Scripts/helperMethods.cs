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
                permutations.Add(new List<string>(currentPermutation));
                return;
            }

            foreach (string color in colors)
            {
                currentPermutation.Add(color);
                GeneratePermutationsRecursive(currentPermutation);
                currentPermutation.RemoveAt(currentPermutation.Count - 1);
            }
        }

        GeneratePermutationsRecursive(new List<string>());
        ShuffleList(permutations);

        helperMethods.GetInstance().ShuffleList(permutations);

        if (permutations.Count > count)
        {
            return permutations.GetRange(0, count);
        }
        else
        {
            return permutations;
        }
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