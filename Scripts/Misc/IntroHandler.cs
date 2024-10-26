using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroHandler : MonoBehaviour
{
    public List<Sprite> introSprites;
    public Image introImage;
    public int introImagesNumber;

    public float screenHeight;
    public float scrollSpeed;
    public TextMeshProUGUI introText;

    public AudioSource introMusic;

    // Start is called before the first frame update
    void Start()
    {
        introImagesNumber = 0;
        screenHeight = Screen.height;

        introImage.GetComponent<Image>().sprite = introSprites[0];

        StartCoroutine(FadeIntroImageIn(true));

        Invoke(nameof(ProceedToNextImage), 17.5f);

        introMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        float finalScrollSpeed = screenHeight * scrollSpeed * Time.deltaTime;

        introText.gameObject.transform.position += new Vector3(0, finalScrollSpeed, 0);

        //ends intro with esc clicked
        if (Input.GetKeyDown("escape"))
        {
            //should go to main menu from here?
            Debug.Log("intro ends");
            //GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().SetMenuScene();
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

    void ProceedToNextImage()
    {
        introImagesNumber += 1;

        if (introImagesNumber == 1)
        {
            StartCoroutine(FadeIntroImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (introImagesNumber == 2)
        {
            StartCoroutine(FadeIntroImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (introImagesNumber == 3)
        {
            StartCoroutine(FadeIntroImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }

        //this shouldnt be called tho?
        if (introImagesNumber > 3)
        {
            //fade music & image
            StartCoroutine(FadeIntroImageOut(true));
            StartCoroutine(StartMusicFade(introMusic, 3f, 0));

            Invoke(nameof(GoToMainMenu), 3f);
            return;
        }
        else
        {
            ProceedToNextImage();
            return;
        }
    }

    void SwapNextImage()
    {
        introImage.GetComponent<Image>().sprite = introSprites[introImagesNumber];

        StartCoroutine(FadeIntroImageIn(true));

        //could have different delays for different images (depending on text lenghts?)
        if (introImagesNumber == 1)
        {
            Invoke(nameof(ProceedToNextImage), 13f);
        }

        if (introImagesNumber == 2)
        {
            Invoke(nameof(ProceedToNextImage), 19f);
        }

        if (introImagesNumber == 3)
        {
            Invoke(nameof(ProceedToNextImage), 15f);
        }
    }

    void GoToMainMenu()
    {
        //should go to main menu from here?
        Debug.Log("intro ends");
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
        //GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().SetMenuScene();

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    IEnumerator FadeIntroImageIn(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            introImage.color = new Color(1, 1, 1, 0);

            // loop over 1 second backwards
            for (float i = 0; i <= 1; i += Time.deltaTime * 0.35f)
            {
                // set color with i as alpha
                introImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeIntroImageOut(bool fadeAway)
    {
        Debug.Log("image should fade");
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime * 0.35f)
            {
                // set color with i as alpha
                introImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    IEnumerator StartMusicFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol = audioSource.volume;
        //audioSource.GetFloat(exposedParam, out currentVol);
        //float currentVol = Mathf.Pow(10, audioSource.volume / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioSource.volume = newVol;
            //audioSource.volume = Mathf.Log10(newVol) * 20;
            yield return null;
        }
        yield break;
    }
}
