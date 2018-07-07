using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildZone : MonoBehaviour {

    public enum PivotType {Left, Center, Right};
    [SerializeField]
    private PivotType SnapPivot;
    [SerializeField] Transform snapStart;
    [SerializeField] private Text equation;
    [SerializeField] private GameObject slowDownTrigger;
    [SerializeField] private GameObject gapMask;
    [SerializeField] private GameObject interactable;
    [SerializeField] private GameObject sparkle;
    [SerializeField] private GameObject dustCloud;
    [SerializeField] private AudioSource explosion;
    private FractionData fractionData;
	private FractionTools.Fraction gapFilled = FractionTools.Fraction.Zero;
    private List<Placeable> piecesInZone = new List<Placeable>();
//    private Inventory inv;
	
	public bool TryPlacePiece(Placeable p)
	{
		//Debug.Log("Trying to place the piece...");
		//Debug.Log("Gap: " + fractionData.Value + ", piece:" + p.Value + ", filled: " + gapFilled);
		bool successful = false;

		if (p.Value + gapFilled <= fractionData.Value)
		{
			successful = true;
            EffectsManager.Instance.PlayEffect(EffectsManager.Effects.Correct);
            //SnapPiece(p);
            p.gameObject.SetActive(false);
            piecesInZone.Add(p);
			gapFilled += p.Value;
            if(gapMask.transform.localScale.x > 0)
                gapMask.transform.localScale = new Vector3(4 * (1f - (float)(gapFilled / fractionData.Value)), 2, 1);
            else
                gapMask.transform.localScale = new Vector3(-4 * (1f - (float)(gapFilled / fractionData.Value)), 2, 1);
            UpdateEquationUI();
			/* Check if the gap has been filled */
			//Debug.Log("Gap filled: " + gapFilled + ", gap size: " + fractionData.Value);
            if (gapFilled == fractionData.Value)
            {
                /* Disable the slow zone in case the coaster didn't even hit it yet (player was really quick) */
                if (slowDownTrigger != null)
                    slowDownTrigger.SetActive(false);
                /* Notify the GameController that a gap has been filled */
                StartCoroutine(GameController.Instance.OnGapFilled());
            }
		}
		else{
			Debug.Log("Piece doesn't want to take a fit! Gap filled: " + gapFilled + ", piece size: " + p.Value + ", gap size: " + fractionData.Value);
            EffectsManager.Instance.PlayEffect(EffectsManager.Effects.Incorrect);
		}

		return successful;
	}

	public void SetFractionData(FractionData data)
	{
        fractionData = data;

		UpdateEquationUI();
	}

    public void Activate()
    {
        interactable.SetActive(true);
    }

    public void Sparkle()
    {
        Instantiate(sparkle,transform.position, Quaternion.identity);
    }

    public void HideBuildZone()
    {
        interactable.SetActive(false);
    }

	private string GapEquation()
	{
		string result = "";

        if (piecesInZone.Count == 0)
        {
            /* Put a question mark */
            result += "?";
        }
        else
        {
            /* Append each piece's fraction using addition */
            foreach (Placeable p in piecesInZone)
                result += p.Value + " + ";

            /* Remove the last + */
            result = result.Remove(result.Length - 3);

            if (gapFilled != fractionData.Value)
            {
                /* Append the "you aren't done yet" part */
                result += " + ...";
            }
        }

        /* Append the total gap size */
        if (fractionData.Value == FractionTools.Fraction.One)
            result += " = 1";
        else
        {
            if(!Constants.gapAllowImproperFractions)
            {
                // if improper fractions not allowed, must display as mixed number
                FractionTools.MixedNumber gapAsMixedNumber = fractionData.Value.ToMixedNumber();
                result += " = " + gapAsMixedNumber;
            }
            else if (Constants.gapAllowImproperFractions && Constants.gapAllowMixedNumbers)
            {
                // if both improper fractions and mixed numbers are allowed, randomly choose how to display
                if (Random.Range(0f, 1f) <= .5f)
                {
                    FractionTools.MixedNumber gapAsMixedNumber = fractionData.Value.ToMixedNumber();
                    result += " = " + gapAsMixedNumber;
                }
                else
                {
                    result += " = " + fractionData.Value;
                }
            }
            else
            {
                // just display as improper fraction
                result += " = " + fractionData.Value;
            }
        }
        return result;
    }

	private void UpdateEquationUI()
	{
		equation.text = GapEquation();
	}

    public bool IsGapFilled()
    {
        return gapFilled == fractionData.Value;
    }

    public void OnUndoButtonClicked()
    {
        if (piecesInZone.Count > 0 && (gapFilled < fractionData.Value)) {
            int pieceIndex = piecesInZone.Count - 1;

            gapFilled -= piecesInZone[pieceIndex].Value;

            // animation of returning to inventory
            piecesInZone[pieceIndex].AnimatedPiece.transform.parent = null;
            piecesInZone[pieceIndex].AnimatedPiece.SetActive(true);

            // cleanup original object
            Destroy(piecesInZone[pieceIndex].gameObject);
            piecesInZone.RemoveAt(pieceIndex);

            // update visual
            if (gapMask.transform.localScale.x > 0)
                gapMask.transform.localScale = new Vector3(4 * (1f - (float)(gapFilled / fractionData.Value)), 2, 1);
            else
                gapMask.transform.localScale = new Vector3(-4 * (1f - (float)(gapFilled / fractionData.Value)), 2, 1);
            UpdateEquationUI();
        }
    }

    public void OnClearButtonClicked()
    {
        /* Toggle on the dust cloud particle effect */
        dustCloud.SetActive(true);
        explosion.Play();
        while (piecesInZone.Count > 0 && gapFilled < fractionData.Value) {
            OnUndoButtonClicked();
        }
    }

    //private void Awake() {
    //    inv = Inventory.Instance;
    //}

    public FractionTools.Fraction[] GetGapComponents()
    {
        return fractionData.Components.ToArray();
    }
}
