using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour {
#region Variables
    private GameController gc;
    private Inventory inv;
    private Vector3 startPos;
    private Vector3 cursorPos;
    private bool placed = false;
    private bool isPickedUp = true;
    private FractionTools.Fraction value;
    private BuildZone bz;
    [SerializeField] private GameObject cuttableList;
    private GameObject myCuttables;
    private Cuttable cu;
    
    [SerializeField] private Constants.PieceLength length;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject animatedPiece;
    [SerializeField] GameObject[] animatedCuts;

    #region Getters and Setters
    public Constants.PieceLength Length {
        get { return length; }
    }

    public GameObject AnimatedPiece {
        get { return animatedPiece; }
    }
    #endregion
    #endregion

    #region Piece Methods
    public FractionTools.Fraction Value
    {
        get { return value; }
    }

    private void ToggleIsPickedUp() {
        if (isPickedUp) {
            isPickedUp = false;
            gc.ActiveCursor = Constants.CursorType.HAND;
        }
        else {
            isPickedUp = true;
            gc.ActiveCursor = Constants.CursorType.DRAG;
        }
    }

    private bool IsWithin(Vector3 obj1, Vector3 obj2)
    {
        float tolerance = 0.05f;
        if ((Mathf.Abs(obj1.x - obj2.x) < tolerance) && (Mathf.Abs(obj1.y - obj2.y) < tolerance))
            return true;
        else
            return false;
    }
#endregion

#region Unity Overrides

    void Start () {
        gc = GameController.Instance;
        inv = Inventory.Instance;
        value = new FractionTools.Fraction(1, (int)length);
        sprite.color = Constants.trackColor;
        if(cuttableList) myCuttables = Instantiate(cuttableList, inv.transform);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("mouse up");
            Destroy(myCuttables, 1f);   // can't be zero for whatever reason...give it time to finish the animation
            if (!placed && bz != null && bz.TryPlacePiece(this))
            {
                Debug.Log("Piece dropped in zone!");
                placed = true;
                gc.ActiveCursor = Constants.CursorType.HAND;
            }
            if(!placed && cu != null)
            {
                Debug.Log("cutting");
                //foreach(GameObject g in animatedCuts[((int)cu.CutLength) - 1].transform.c)
                animatedCuts[((int)cu.CutLength)-1].SetActive(true);
                animatedCuts[((int)cu.CutLength) - 1].transform.DetachChildren();
                //{
                //    GameObject ap = Instantiate(animatedPiece, transform);
                //    ap.transform.parent = null;
                //    ap.SetActive(true);
                //}
                //inv.Increase(cu.CutLength, cu.CutPieces);
                Destroy(gameObject);
                gc.ActiveCursor = Constants.CursorType.HAND;
            }
        }
        //Zak's return to base method
        if (Input.GetMouseButton(0) && !placed)     // follow mouse
        {
            cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.MoveTowards(transform.position, cursorPos, Time.deltaTime * 50f);
        }
        if (!Input.GetMouseButton(0) && !placed)         // return to start if mouse is released
        {
            startPos = inv.pieces[(int)length - 2].transform.position;
            transform.position = Vector2.MoveTowards(transform.position, startPos, Time.deltaTime * 20f);
            gc.ActiveCursor = Constants.CursorType.HAND;
        }
        if (!Input.GetMouseButton(0) && IsWithin(transform.position, startPos))     // destroy when back to start position
        {
            Destroy(gameObject);
            inv.Increase(length, 1);
        }

        //// Joe's preferred method
        //if (isPickedUp && !placed)
        //{
        //    cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    transform.position = Vector2.Lerp(transform.position, cursorPos, 0.5f);
        //}
    }

    // Case 1: entering a new bz / cuttable's trigger.  Replace any old one indiscriminately
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BuildZone"))
        {
            bz = collision.GetComponentInParent<BuildZone>();
        }
        if (collision.CompareTag("Cuttable"))
        {
            if(cu) cu.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            collision.GetComponent<SpriteRenderer>().enabled = true;
            cu = collision.GetComponent<Cuttable>();
        }
    }

    // Case 2: exiting a trigger.  Unset it
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BuildZone"))
        {
            bz = null;
        }
        if (collision.CompareTag("Cuttable"))
        {
            collision.GetComponent<SpriteRenderer>().enabled = false;
            cu = null;
        }
    }

    // Case 3: if a piece happens to be in multiple triggers, then exits one, reset the reference to the other
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (placed)
            return;

        if (collision.CompareTag("BuildZone"))
        {
            bz = collision.GetComponentInParent<BuildZone>();
        }
        if (collision.CompareTag("Cuttable") && !cu)
        {
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            cu = collision.GetComponent<Cuttable>();
        }
    }

    private void OnMouseDown()
    {
        if(!placed)
        {
            ToggleIsPickedUp();
        }
    }

    // OnMouseUp is not called unless MouseDown was called on the same object first,
        // which is not the case since we're instantiating this placeable on click
    //private void OnMouseUp()
    //{
    //    Debug.Log("mouse up");
    //    if (!placed && bz != null && bz.TryPlacePiece(this))
    //    {
    //        Debug.Log("Piece dropped in zone!");
    //        placed = true;
    //        gc.ActiveCursor = Constants.CursorType.HAND;
    //    }
    //}
    #endregion
}
