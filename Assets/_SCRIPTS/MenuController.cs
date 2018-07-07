using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject gameSettingsMenu;
    [SerializeField] private GameObject instructionsMenu;
    [SerializeField] private GameObject endgameMenu;
    [SerializeField] private GameObject wonMenu;
    [SerializeField] private GameObject lostMenu;

    [SerializeField] private Toggle unlimitedInventoryToggle;
    [SerializeField] private Toggle gapWidthAlwaysOneToggle;
    [SerializeField] private Toggle gapWidthAlwaysAtomicToggle;
    [SerializeField] private Toggle gapWidthImproperFractionsToggle;
    [SerializeField] private Toggle gapWidthMixedNumbersToggle;
    [SerializeField] private ToggleGroup colorToggles;
    [SerializeField] private GameObject difficultyNeedle;
    [SerializeField] private Text difficultyText;
    [SerializeField] private Text helpText;

    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Image track;



    private void Start()
    {
        Time.timeScale = 1;

        /* Set all of the settings options using the values in Constants */
        SetUIFromConstants();

        Vector2 cursorHotSpot = new Vector2(cursorTexture.width * 0.25f, cursorTexture.height * 0.25f);
        Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.ForceSoftware);
        /* If coming from a finished game, load straight into the station */
        if (Constants.gameOver)
            GameSettingsClick();
    }

    private void SetUIFromConstants()
    {
        /* Coaster color settings */
        track.color = Constants.trackColor;
        CoasterManager.Instance.ChangeColor(Constants.trackColor);
        colorToggles.SetAllTogglesOff();
        Toggle[] toggles = colorToggles.GetComponentsInChildren<Toggle>();
        foreach (Toggle t in toggles)
        {
            if (t.colors.normalColor == Constants.trackColor)
                t.isOn = true;
        }

        /* Game Settings */
        gapWidthAlwaysOneToggle.isOn = Constants.gapAlwaysOne;
        gapWidthAlwaysAtomicToggle.isOn = Constants.gapAlwaysAtomic;
        gapWidthImproperFractionsToggle.isOn = Constants.gapAllowImproperFractions;
        gapWidthMixedNumbersToggle.isOn = Constants.gapAllowMixedNumbers;
        unlimitedInventoryToggle.isOn = Constants.unlimitedInventory;
        difficultyNeedle.transform.Rotate(new Vector3(0, 0, 130 + (72 * (((int)Constants.difficulty)-1))));
        difficultyText.text = Constants.difficulty.ToString();
    }

    public void PlayClick()
    {
        // save selected settings to Constants
        // TODO: read them out of Constants where relevant
        Constants.gapAlwaysOne = gapWidthAlwaysOneToggle.isOn;
        Constants.gapAlwaysAtomic = gapWidthAlwaysAtomicToggle.isOn;
        Constants.gapAllowImproperFractions = gapWidthImproperFractionsToggle.isOn;
        Constants.gapAllowMixedNumbers = gapWidthMixedNumbersToggle.isOn;
        Constants.unlimitedInventory = unlimitedInventoryToggle.isOn;

        // track color
        Toggle activeToggle = colorToggles.ActiveToggles().FirstOrDefault();
        Constants.trackColor = activeToggle.colors.normalColor;

        StartCoroutine(ExitTheStation());
    }

    public IEnumerator ExitTheStation()
    {
        /* Play the coaster leaving the station */
        CoasterManager.Instance.PlaySection(CoasterManager.SectionTriggers.PlayExit);

        //SceneManager.LoadScene("Main"); //CB: Moved this to be triggered by an animation event on "Coaster@ExitScreen" animation in CoasterManager.cs
        yield return null;
    }

    public void GameSettingsClick()
    {
        gameSettingsMenu.SetActive(true);
        CoasterManager.Instance.PlaySection(CoasterManager.SectionTriggers.PlayEnter);
        startMenu.SetActive(false);
    }

    public void PlayAgainClick()
    {
        gameSettingsMenu.SetActive(true);
        CoasterManager.Instance.PlaySection(CoasterManager.SectionTriggers.PlayEnter);
        endgameMenu.SetActive(false);
    }

    public void TutorialClick()
    {
        instructionsMenu.SetActive(true);
        startMenu.SetActive(false);
    }

    public void OptionsClick()
    {
        optionsMenu.SetActive(true);
        startMenu.SetActive(false);  
    }

    public void BackClick()
    {
        /* Move the coaster back off the screen */
        CoasterManager.Instance.ResetToStartPosition();
        startMenu.SetActive(true);
        optionsMenu.SetActive(false);
        endgameMenu.SetActive(false);
        gameSettingsMenu.SetActive(false);
    }

    public void QuitClick()
    { Application.Quit(); }

    public void AlwaysOneCheck()
    {
        if(gapWidthAlwaysOneToggle.isOn)
        {
            gapWidthAlwaysAtomicToggle.isOn = false;
            gapWidthImproperFractionsToggle.isOn = false;
            gapWidthMixedNumbersToggle.isOn = false;
        }        
    }

    public void AlwaysAtomicCheck()
    {
        if (gapWidthAlwaysAtomicToggle.isOn)
        {
            gapWidthAlwaysOneToggle.isOn = false;
            gapWidthImproperFractionsToggle.isOn = false;
            gapWidthMixedNumbersToggle.isOn = false;
        }
    }

    public void ImproperFractionCheck()
    {
        if (gapWidthImproperFractionsToggle.isOn)
        {
            gapWidthAlwaysOneToggle.isOn = false;
            gapWidthAlwaysAtomicToggle.isOn = false;
        }
    }

    public void MixedNumbersCheck()
    {
        if (gapWidthMixedNumbersToggle.isOn)
        {
            gapWidthAlwaysOneToggle.isOn = false;
            gapWidthAlwaysAtomicToggle.isOn = false;
        }
    }

    public void ChangeCoasterColor(Toggle t)
    {
        if (t.isOn)
        {
            track.color = t.colors.normalColor;
            CoasterManager.Instance.ChangeColor(t.colors.normalColor);
        }
    }

    public void OnDifficultyChanged(Slider s)
    {
        Constants.difficulty = (Constants.Difficulty)s.value;
        difficultyText.text = Constants.difficulty.ToString();
    }

    public void ChangeDifficulty()
    {
        int newDifficulty = ((int)Constants.difficulty) + 1;
        if (newDifficulty == 6) newDifficulty = 1;
        difficultyNeedle.transform.Rotate(new Vector3(0, 0, 72));
        Constants.difficulty = (Constants.Difficulty)newDifficulty;
        difficultyText.text = Constants.difficulty.ToString();
    }

    public void DisplayHelp(int toggleNumber)
    {
        switch(toggleNumber)
        {
            case 0:
                helpText.text = "Allows gaps in the track to be displayed as improper fractions";
                break;
            case 1:
                helpText.text = "Allows gaps in the track to be displayed as mixed numbers";
                break;
            case 2:
                helpText.text = "All gaps in the track will have a length of 1";
                break;
            case 3:
                helpText.text = "All gaps in the track will have a length with 1 in the numerator";
                break;
            case 4:
                helpText.text = "Play with an unlimited number of each fractional track piece";
                break;
            case 5:
                helpText.text = "Higher difficulties require more fractional pieces to fill each gap";
                break;
            case 6:
                helpText.text = "Change the color of your coaster and track";
                break;
            //case 7:
            //    helpText.text = "Click to change your coaster's decals";
            //    break;
        }

    }

    public void ClearHelp()
    {
        helpText.text = "";
    }
}
