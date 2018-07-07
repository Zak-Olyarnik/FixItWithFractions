using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[Serializable]
public class FractionDatabase {

    public Dictionary<int, List<FractionData>> Data;

    public FractionDatabase()
    {
        Data = new Dictionary<int, List<FractionData>>();
    }

    public FractionData GetRandomByDifficulty(Constants.Difficulty difficulty, bool forceProper = false, bool forceAlwaysOne = false, bool forceNumeratorOne = false)
    {
        Debug.Log("forceProper: " + forceProper + ", forceAlwaysOne: " + forceAlwaysOne + ", forceNumeratorOne: " + forceNumeratorOne);
        //if (difficulty == Constants.Difficulty.EASY)
        //    throw new ArgumentException("Easy difficulty can't have improper fractions!");

        List<FractionData> fractionData = Data[(int)difficulty];

        if (forceAlwaysOne)
            fractionData = fractionData.Where(fd => fd.Value.numerator == fd.Value.denominator).ToList();
        else if (forceNumeratorOne)
            fractionData = fractionData.Where(fd => fd.Value.numerator == 1).ToList();
        else if (forceProper)
            fractionData = fractionData.Where(fd => fd.Value.numerator < fd.Value.denominator).ToList();

        /* Choose a random bit of data from the list */
        FractionData choice = fractionData[UnityEngine.Random.Range(0, fractionData.Count)];

        return choice;
    }

    public override string ToString()
    {
        string result = "";
        foreach(int key in Data.Keys)
            result += "Difficulty Level " + key + ":\r\n" + Data[key].ToDelimitedString() + "\r\n";

        return result;
    }
}
