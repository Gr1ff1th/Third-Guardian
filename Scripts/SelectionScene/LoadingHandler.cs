using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//handles the loading screen from selection scene to main scene
public class LoadingHandler : MonoBehaviour
{
    public GameObject loadOverlay;
    public GameObject loadingIcon;
    public Image loadingImage;

    public TextMeshProUGUI tipHolder;

    public List<Sprite> loadingImageList;

    [TextArea(5, 20)]
    public List<string> loadingTips;

    // Start is called before the first frame update
    void Start()
    {
        loadOverlay.SetActive(false);

        int randomNumber = Random.Range(0, loadingImageList.Count);

        loadingImage.sprite = loadingImageList[randomNumber];

        int randomNumber2 = Random.Range(0, loadingTips.Count);

        tipHolder.text = loadingTips[randomNumber2];
    }

    // Update is called once per frame
    void Update()
    {
        if (loadOverlay.activeSelf)
        {
            loadingIcon.transform.Rotate(0, 0, -2f);
        }
    }

    public void ActivateLoadingScreen()
    {
        loadOverlay.SetActive(true);
    }
}
