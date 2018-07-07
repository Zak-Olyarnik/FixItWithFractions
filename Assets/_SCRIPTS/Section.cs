using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    [SerializeField] private BuildZone[] buildZones;

    public BuildZone[] SetupBuildZones()
    {
        List<FractionData> data = new List<FractionData>();

        foreach (BuildZone bz in buildZones)//activeBuildZones)
        {
            FractionData fractionData = Constants.fractionDatabase.GetRandomByDifficulty(Constants.difficulty
                , (!Constants.gapAllowImproperFractions && !Constants.gapAllowMixedNumbers)
                , Constants.gapAlwaysOne
                , Constants.gapAlwaysAtomic);
            bz.SetFractionData(fractionData);
            data.Add(fractionData);
            bz.gameObject.SetActive(true);
        }

        return buildZones;
    }
}
