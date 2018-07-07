using UnityEngine;
using UnityEditor;
using Fraction = FractionTools.Fraction;
using System.Collections.Generic;
using System.Linq;

public class GenerateFractionDatabase {

    /// <summary>
    /// Creates a "FractionDatabase.bytes" & "FractionDatabase.txt" in the current folder.
    /// </summary>
    /// <remarks>
    /// These files *must* be generated (or moved) under a "Resources" folder to be loaded
    /// </remarks>
    [MenuItem("Assets/Create/Fraction Database", priority = 21)]
    private static void CreateFractionDatabaseAsset()
    {
        Object selectedAsset = Selection.activeObject;
        if (selectedAsset != null)
        {
            string directoryPath = AssetDatabase.GetAssetPath(selectedAsset);
            if (System.IO.Directory.Exists(directoryPath))
            {
                string filePath = directoryPath + "/" + Constants.dictionaryFileName + Constants.dictionaryFileExtension;

                /* Delete the old database file if one exists */
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                /* Generate the FractionDatabase */
                FractionDatabase database = BuildFractionDatabase();

                /* TESTING: Save the database as a text file for viewing */
                IOHelper<FractionDatabase>.ToTextFile(database, directoryPath + "/database.txt");

                /* Save the database */
                IOHelper<FractionDatabase>.SerializeObject(database, filePath);

                /* Refresh the asset database to show the Readme file */
                AssetDatabase.Refresh();
            }
        }
    }

    /// <summary>
    /// Creates a dictionary of keys 1-5
    /// For each level, takes every fraction combination from the previous level and adds an atomic fraction to it to generate the next level
    /// </summary>
    /// <returns>
    /// FractionDatabase consisting of a Dictionary with 5 levels of fraction combinations
    ///     Level 1: All fractions made up of adding 1 atomic fraction from 1/2 to 1/10
    ///     Level 2: All fractions made up of adding 2 atomic fraction
    /// </returns>
    private static FractionDatabase BuildFractionDatabase()
    {
        FractionDatabase database = new FractionDatabase();

        /* Create every combination of single pieces, two pieces, ..., up to a limit of 5 (for now?) */
        /* For each difficulty level (number of pieces) */
        List<FractionData> atomicList = new List<FractionData>();
        /* Build the atomicList of fractions with one component (atomic) */
        /* For every possible atomic fraction */
        for (int denominator = 2; denominator <= 10; denominator++)
        {
            FractionData data = new FractionData();
            Fraction fraction = new Fraction(1, denominator);
            data.Value += fraction;
            data.Components.Add(fraction);

            /* Add it to the atomicList */
            atomicList.Add(data);
        }
        database.Data.Add(1, atomicList);

        /* If more than one component needs to be generated, combine every item in atomicList with every possible atomic fraction */
        for (int numComponents = 2; numComponents <= 5; numComponents++)
        {
            List<FractionData> fractionList = new List<FractionData>();
            /* Loop through the previous difficulty level, adding every combination of atomic fraction */
            foreach (FractionData fractionFromPreviousLevel in database.Data[numComponents - 1])
            {
                /* For every possible atomic fraction */
                for (int denominator = 2; denominator <= 10; denominator++)
                {
                    Fraction fraction = new Fraction(1, denominator);

                    /* Only add the new fraction if the base is within 2-10 */
                    Fraction sum = fractionFromPreviousLevel.Value + fraction;
                    if (sum.denominator <= 10)
                    {
                        FractionData data = new FractionData(fractionFromPreviousLevel)
                        {
                            Value = sum
                        };
                        data.Components.Add(fraction);

                        /* Ensure this new item is unique (1/2 + 1/4 === 1/4 + 1/2) */
                        if (IsDistinct(data, fractionList))
                        {
                            /* Add this fraction data to the list */
                            fractionList.Add(data);
                        }
                    }
                }
            }
            /* Add the list of fractionData to the database */
            database.Data.Add(numComponents, fractionList);
        }

        return database;
    }

    private static bool IsDistinct(FractionData item, List<FractionData> list)
    {
        bool result = true;
        
        List<Fraction> a = item.Components;
        a.Sort();
        foreach(FractionData toCheck in list)
        {
            if (item.Value == toCheck.Value && item.Components.Count == toCheck.Components.Count)
            {
                List<Fraction> b = toCheck.Components;
                b.Sort();

                /* Iterate over both lists. If we reach the end of both, then the lists are the same */
                int counter;
                for (counter = 0; counter < item.Components.Count; counter++)
                {
                    if (item.Components[counter] != toCheck.Components[counter])
                        break;
                }

                /* If the counter reached the end of the array, these two lists were equal */
                if (counter == item.Components.Count)
                {
                    Debug.Log("Duplicate found!");
                    Debug.Log("    " + item.Value + ": " + item.Components.ToDelimitedString());
                    Debug.Log("    " + toCheck.Value + ": " + toCheck.Components.ToDelimitedString());
                    return false;
                }
            }
        }

        return result;
    }
}
