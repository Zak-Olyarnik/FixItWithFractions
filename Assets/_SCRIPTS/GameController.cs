using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Texture2D[] cursorTextures;    // custom cursor sprites
    private Constants.CursorType activeCursor;
    [SerializeField] private Piece[] pieces;
    [SerializeField] private GameObject[] sections; // list of all spawnable sections, with the end section always position 0
    [SerializeField] private SplineComputer masterSpline;
    [SerializeField] private GameObject cam;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    [SerializeField] private float delayOnWin; // How long to wait after clearing a gap before moving on
    [SerializeField] private Text winText; /* Reference to "you win" text */
    [SerializeField] private GameObject winSparkle; /* Reference to sparkle effect */
    [SerializeField] private GameObject[] confettiCannons;
    [SerializeField] private Text loseText; /* Reference to "you lose" text */

    private List<BuildZone> activeBuildZones;
    private BuildZone lastInteractedBuildZone;
    public int numBuildZones = 0;
    public int clearedBuildZones = 0;

    private Inventory inv;

    public static GameController Instance { get; private set; }

    public Constants.CursorType ActiveCursor
    {
        get { return activeCursor; }
        set
        {
            activeCursor = value;
            int index = 0;
            float offset = 0.5f;
            switch (activeCursor)
            {
                case Constants.CursorType.HAND:
                    index = 0;
                    foreach (Piece piece in pieces)
                        if (piece.Interactable) piece.EnableDraggable(); // only set if interactable
                    break;
                case Constants.CursorType.DRAG:
                    index = 1;
                    break;
            }
            Vector2 cursorHotSpot = new Vector2(cursorTextures[index].width * offset, cursorTextures[index].height * offset);
            Cursor.SetCursor(cursorTextures[index], cursorHotSpot, CursorMode.ForceSoftware);
        }
    }

    public BuildZone LastInteractedBuildZone
    {
        set { lastInteractedBuildZone = value; }
    }

    void Awake()
    {
        Instance = this;
        ActiveCursor = Constants.CursorType.HAND;

        /* Load the FractionDatabase */
        Constants.fractionDatabase = IOHelper<FractionDatabase>.LoadFromResources(Constants.dictionaryFileName);
    }

    void Start()
    {
        Constants.gameOver = false;
        if (Constants.difficulty == Constants.Difficulty.HARD || Constants.difficulty == Constants.Difficulty.IMPOSSIBLE)
            Constants.numSections = 3;
        else
            Constants.numSections = 2;
        Time.timeScale = 1;
        inv = Inventory.Instance;
        activeBuildZones = new List<BuildZone>();
        List<SplinePoint> splinePoints = new List<SplinePoint>();
        splinePoints.AddRange(masterSpline.GetPoints());

        for (int i = 0; i <= Constants.numSections; i++)
        {
            GameObject current = null;
            if (i == Constants.numSections)
            {
                current = Instantiate(sections[0], new Vector3(19.2f + (38.4f * i), sections[0].transform.position.y, 0), Quaternion.identity);
            }
            else
            {
                int ind = Random.Range(1, sections.Length);
                current = Instantiate(sections[ind], new Vector3(28.8f + (38.4f * i), sections[ind].transform.position.y, 0), Quaternion.identity);
                Section currentSection = current.GetComponent<Section>();
                activeBuildZones.AddRange(currentSection.SetupBuildZones());
            }

            SplineComputer currentSpline = current.GetComponentInChildren<SplineComputer>();
            splinePoints.AddRange(currentSpline.GetPoints().Skip(1).ToArray());
            currentSpline.gameObject.SetActive(false);
        }

        masterSpline.SetPoints(splinePoints.ToArray());
        masterSpline.Rebuild();
        masterSpline.GetComponent<SplineRenderer>().color = Constants.trackColor;


        if (Constants.unlimitedInventory)    // set inventory unlimited
        {
            inv.SetUnlimited();
        }
        else    // distribute inventory
        {
            foreach(BuildZone bz in activeBuildZones)
            {
                foreach (FractionTools.Fraction f in bz.GetGapComponents())
                    inv.Increase((Constants.PieceLength)f.denominator, 1);
            }
        }

        numBuildZones = activeBuildZones.Count;
        if (numBuildZones != 0)
        {
            lastInteractedBuildZone = activeBuildZones[0];
            lastInteractedBuildZone.Activate();
        }

        /* Start the coaster */
        CoasterManager.Instance.StartCoasterAlongSpline(masterSpline);
    }

    public IEnumerator OnGapFilled()
    {
        EffectsManager.Instance.PlayEffect(EffectsManager.Effects.Construction);
        lastInteractedBuildZone.Sparkle();

        /* Speed up the coaster */
        CoasterManager.Instance.SpeedUp();

        yield return new WaitForSeconds(delayOnWin);

        /* Hide the old build zone */
        lastInteractedBuildZone.HideBuildZone();

        // set the new build zone
        lastInteractedBuildZone = null;
        clearedBuildZones++;
        if (clearedBuildZones == numBuildZones)
        {
            //EndGame(true); /* CMB: EndGame is now triggered by a game object in the scene */
        }
        else
        {
            lastInteractedBuildZone = activeBuildZones[clearedBuildZones];
            lastInteractedBuildZone.Activate();
        }

        yield return null;
    }

    public void EndGame(bool won)
    {
        Constants.gameWon = won;
        Constants.gameOver = true;

        if (won)
            StartCoroutine(VictoryLap());
        else
            StartCoroutine(CrashAndBurn());
    }

    private IEnumerator VictoryLap()
    {
        StartCoroutine(FadeInText(Time.realtimeSinceStartup, winText));
        winSparkle.SetActive(true);
        foreach (GameObject go in confettiCannons)
            go.SetActive(true);
        //EffectsManager.Instance.PlayEffect(EffectsManager.Effects.Confetti);
        EffectsManager.Instance.PlayEffect(EffectsManager.Effects.Yay);
        yield return new WaitForSeconds(Constants.endGameWaitDelay - 7.0f);
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator CrashAndBurn()
    {
        /* Stop the coaster audio */
        CoasterManager.Instance.StopTrackAudio();

        /* Play a crashing sound effect */
        EffectsManager.Instance.PlayEffect(EffectsManager.Effects.Incorrect); /* TODO: replace this with car crash sound effect */

        /* Pop up you lose text */
        StartCoroutine(FadeInText(Time.realtimeSinceStartup, loseText));

        /* Spawn a dust cloud on the coaster */
        EffectsManager.Instance.PlayEffect(EffectsManager.Effects.DustCloud, CoasterManager.Instance.transform);

        /* Wait a bit, then load the menu scene */
        yield return new WaitForSeconds(Constants.endGameWaitDelay);
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator FadeInText(float startTime, Text t)
    {
        float timer = (Time.realtimeSinceStartup - startTime);
        float fracJourney = timer / 30f;
        t.color = Color.Lerp(t.color, new Color32(186, 188, 190, 255), fracJourney);
        if (timer > 3.5f)
        {
            StopCoroutine(FadeInText(startTime, t));
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.05f);
        StartCoroutine(FadeInText(startTime, t));
    }

    public void MenuCursorSet()
    {
        Vector2 cursorHotSpot = new Vector2(cursorTextures[3].width * 0.25f, cursorTextures[3].height * 0.25f);
        Cursor.SetCursor(cursorTextures[3], cursorHotSpot, CursorMode.ForceSoftware);
    }

    public void MenuCursorUnSet()
    {
        if (pauseMenu.activeSelf)   // don't unset if the PointerExit was caused by the menu blocking the button
        {
            Debug.Log("joke's on you, unity!");
            return;
        }
        ActiveCursor = activeCursor;
    }

    public void PauseClick()
    {
        Time.timeScale = 0; /* Manipulate timeScale to pause the game */
        CoasterManager.Instance.StopTrackAudio();
        pauseMenu.SetActive(true);
        MenuCursorSet();
    }

    public void PlayClick()
    {
        Time.timeScale = 1f; /* Change timeScale back from 0 */
        CoasterManager.Instance.PlayTrackAudio();
        pauseMenu.SetActive(false);
        MenuCursorUnSet();
    }

    public void OptionsClick()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void BackClick()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void QuitClick()
    { SceneManager.LoadScene("Menu"); }
}
