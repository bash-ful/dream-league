using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;

public class StringGenerator : MonoBehaviour
{
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private StringBuilder GenerateRandomString(string answer)
    {
        int randomStringLength = 18 - answer.Length;
        StringBuilder generatedString = new();
        System.Random random = new();
        for (int i = 0; i < randomStringLength; i++)
        {
            generatedString.Append(chars[random.Next(chars.Length)]);
        }
        generatedString.Append(answer);
        return ScrambleString(generatedString);
    }

    private StringBuilder ScrambleString(StringBuilder text)
    {
        StringBuilder jumble = text;
        int length = jumble.Length;
        System.Random random = new System.Random();
        for (int i = length - 1; i > 0; i--)
        {
            int j = random.Next(i);
            (jumble[i], jumble[j]) = (jumble[j], jumble[i]);
        }

        return jumble;
    }

    public void ApplyGeneratedStringToButtons(String answer)
    {
        StringBuilder Letters = GenerateRandomString(answer);
        for (int i = 0; i <= 17; i++)
        {
            GameObject.Find(i.ToString()).GetComponentInChildren<Text>().text = Letters[i].ToString();
        }
    }
}