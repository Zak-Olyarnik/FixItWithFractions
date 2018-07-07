using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    static private Inventory instance;      // instance of the GameController
    [SerializeField] private Text[] countTexts;
    public Piece[] pieces;
    private int[] counts = new int[9];

    public static Inventory Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        instance = this;
    }

    public void Decrease(Constants.PieceLength piece)
    {
        if (Constants.unlimitedInventory)
            return;

        int index = (int)piece;
        index -= 2; // array offset to 0
        counts[index]--;

        UpdateUI();
    }

    public void Increase(Constants.PieceLength piece, int num)
    {
        if (Constants.unlimitedInventory)
            return;

        int index = (int)piece;
        index -= 2; // array offset to 0
        counts[index] += num;

        UpdateUI();
    }

    public void Set(Constants.PieceLength piece, int num)
    {
        int index = (int)piece;
        index -= 2; // array offset to 0
        counts[index] = num;

        UpdateUI();
    }

    public void SetUnlimited()
    {
        for (int i = 0; i < counts.Length; i++)
        {
            counts[i] = -1;
            countTexts[i].text = "\u221E";   // infinity symbol
            countTexts[i].fontSize = 95;
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < counts.Length; i++)
        {
            countTexts[i].text = counts[i].ToString();
            if (counts[i] == 0)
                pieces[i].SetInteractable(false);
            else
                pieces[i].SetInteractable(true);
        }
    }
}
