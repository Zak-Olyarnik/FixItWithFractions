using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour {
#region Variables
    private GameController gc;
    private Inventory inv;

    #region Serialized Privates
    [SerializeField] private Constants.PieceLength length;
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject newPiece;
    #endregion
#endregion

#region Draggable Functions
#endregion

#region Unity Overrides
    void Start () {
        gc = GameController.Instance;
        inv = Inventory.Instance;
    }

    // Mouse OVER
    private void OnMouseOver()
    {
        // ignore hover if a menu is active
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (gc.ActiveCursor == Constants.CursorType.HAND)
        {
            highlight.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                gc.ActiveCursor = Constants.CursorType.DRAG;
                highlight.SetActive(false);
                Instantiate(newPiece, transform.position, Quaternion.identity);
                inv.Decrease(length);
            }
            if (Input.GetMouseButtonDown(1))
            {
                highlight.SetActive(false);
            }
        }
    }

    // Mouse EXIT
    private void OnMouseExit()
    {
       if (gc.ActiveCursor == Constants.CursorType.HAND)
        {
            highlight.SetActive(false);
        }
    }
#endregion
}
