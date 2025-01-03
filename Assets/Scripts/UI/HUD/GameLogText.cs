using UnityEngine;

public class GameLogText : TypewriterEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        textComponent.text = "";
        currentText = "";
    }
}
