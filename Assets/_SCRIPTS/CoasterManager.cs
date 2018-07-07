using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoasterManager : MonoBehaviour {

	public enum SectionTriggers /* Names of triggers on Animator */
    {
        PlaySectionA,
        PlaySectionB,
        PlaySectionC,
        PlayFullSection,
        PlayEnter,
        PlayExit
    }
    private static string PlaySpeedMultipier = "PlaySpeedMultipier"; /* Float parameter name on Animator */
    private Animator animator;

    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private LineRenderer[] cartConnectors;
    public SpriteRenderer[] decalSprites;
    public Sprite[] decals;
    public Sprite[] frontDecals;
    [SerializeField] private AudioSource trackAudio;
    private Vector3 startPosition;
    [SerializeField] private SplineFollower[] splineFollowers;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float slowedSpeed;
    [SerializeField] private float splineFollowFirstCartStartDelay;
    [SerializeField] private float splineFollowOtherCartStartDelay;

    public static CoasterManager Instance { get; private set; }

    public void Awake()
    {
        /* Since the coaster is destroyed onLoad, always update the instance */
        Instance = this;
        animator = GetComponent<Animator>();
        startPosition = this.transform.position;
    }

    public void ChangeColor(Color c)
    {
        foreach (SpriteRenderer sp in sprites)
            sp.color = c;
        foreach (LineRenderer lr in cartConnectors)
        {
            lr.startColor = c;
            lr.endColor = c;
        }
        decalSprites[0].sprite = frontDecals[Constants.decalIndex];
        for (int i = 1; i < decalSprites.Length; i++)
            decalSprites[i].sprite = decals[Constants.decalIndex];


        //foreach (SpriteRenderer sp in decalSprites)
        //{
        //    sp.sprite = decals[Constants.decalIndex];
        //    //sp.color = new Color(1 - c.r, 1 - c.g, 1 - c.b);
        //}
    }

    private void Start()
    {
        ChangeColor(Constants.trackColor);
    }

    public void PlaySection(SectionTriggers st)
    {
        animator.SetTrigger(st.ToString());
    }

    public void StartCoasterAlongSpline(SplineComputer sc)
    {
        StartCoroutine(SplineFollowWithDelay(sc));
    }

    private IEnumerator SplineFollowWithDelay(SplineComputer sc)
    {
        float delay = splineFollowFirstCartStartDelay;
        foreach (SplineFollower sf in splineFollowers)
        {
            /* Set the spline computer to the new spline */
            sf.computer = sc;
            /* Turn autoFollow back on and restart the follow */
            sf.autoFollow = true;
            sf.Restart();
            /* Wait before starting the next cart */
            yield return new WaitForSeconds(delay / sf.followSpeed);
            delay = splineFollowOtherCartStartDelay;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch(collider.tag)
        {
            case "decrease":
                //animator.SetFloat(PlaySpeedMultipier, Constants.slowCoasterSpeed);
                foreach (SplineFollower sf in splineFollowers)
                    sf.followSpeed = slowedSpeed;
                break;
            case "increase":
                SpeedUp();
                break;
            case "lose":
                /* Stop the coaster animation */
                foreach (SplineFollower sf in splineFollowers)
                {
                    sf.followSpeed = 0f;
                }
                /* Tell GameController to end the game with a lose state */
                GameController.Instance.EndGame(false);
                break;
            default:

                break;
        }
    }

    public void SpeedUp()
    {
        foreach (SplineFollower sf in splineFollowers)
            sf.followSpeed = normalSpeed;
    }

    public void StopTrackAudio()
    {
        trackAudio.Stop();
    }

    public void ExitScreenFinished()
    {
        trackAudio.Stop();
        SceneManager.LoadScene("Main");
    }

    public void PlayTrackAudio()
    {
        trackAudio.Play();
    }

    public void ResetToStartPosition()
    {
        /* Stop the animation and audio */
        trackAudio.Stop();
        animator.Rebind();

        this.transform.position = startPosition;
    }
}
