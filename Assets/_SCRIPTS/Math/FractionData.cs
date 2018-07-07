using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FractionData {

    public FractionTools.Fraction Value;
    public List<FractionTools.Fraction> Components;

    public FractionData()
    {
        Value = FractionTools.Fraction.Zero;
        Components = new List<FractionTools.Fraction>();
    }

    public FractionData(FractionData toCopy)
    {
        Value = new FractionTools.Fraction(toCopy.Value);
        Components = new List<FractionTools.Fraction>(toCopy.Components);
    }

    public override string ToString()
    {
        string result = "\t" + Value.ToString() + "\r\n";

        foreach (FractionTools.Fraction comp in Components)
            result += "\t\t" + comp.ToString() + "\r\n";
        
        return result;
    }
}
