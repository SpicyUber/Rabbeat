using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
public class HintPanel : MonoBehaviour
{

    public Sprite[] KeyImages;
    private Coroutine _coroutine;
    public HeadphoneAlertBlink Headphones;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }
    private void Notification(bool on)
    {
       
            Headphones.Alert(on);
        


    }
    public void Show(Hint[] hints)
    {
        Hide();
        Notification(true);
        _coroutine = StartCoroutine(ShowRoutine(hints));
        
    }

    IEnumerator ShowRoutine(Hint[] hints)
    {

        foreach (Hint hint in hints)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = hint.TopText;
            texts[1].text = hint.BottomText;
            GetComponentInChildren<Image>().sprite = KeyImages[hint.KeyImageIndex];
            float t = 0;
            while (t <= 1f) {
                texts[0].alpha = t;
                texts[1].alpha = t;
                GetComponentInChildren<Image>().color = new Color(1, 1, 1, t);

                yield return null; t += Time.deltaTime; }
            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            texts[0].alpha = 1;
            texts[1].alpha = 1;
            yield return new WaitForSeconds(hint.DurationInSeconds);
        }

        Hide();
    }
    public void Hide()
    {
        if(_coroutine != null) { StopCoroutine(_coroutine); }
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = "";
        GetComponentInChildren<Image>().color = new Color(0,0,0,0);
        texts[1].text = "";
        Notification(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
