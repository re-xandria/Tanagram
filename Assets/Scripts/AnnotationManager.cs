using UnityEngine;
using UnityEngine.UI;

public class AnnotationManager : MonoBehaviour
{
    public Gemini gemini;
    public Button btn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gemini =  new Gemini();
        btn = GameObject.Find("Button").GetComponent<Button>();
        btn.onClick.AddListener(RequestGeminiData);
    }

    private void RequestGeminiData()
    {
        StartCoroutine(gemini.GetGameData());
    }
}
