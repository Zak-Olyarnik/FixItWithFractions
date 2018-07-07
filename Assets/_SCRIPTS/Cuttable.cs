using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    [SerializeField] private int cutPieces;
    [SerializeField] private Constants.PieceLength cutLength;

    public int CutPieces
    { get { return cutPieces; } }

    public Constants.PieceLength CutLength
    { get { return cutLength; } }
}
