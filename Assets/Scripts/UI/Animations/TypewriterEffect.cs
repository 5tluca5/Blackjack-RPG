using UnityEngine;
using TMPro; // For TextMeshPro support
using DG.Tweening;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // TextMeshPro component
    public float typeSpeed = 0.05f; // Time delay between characters

    private string fullText; // The full text to display
    private string currentText = ""; // The current text being displayed

    void Start()
    {
        fullText = textComponent.text; // Store the full text
        textComponent.text = ""; // Clear the text
        PlayTypewriterEffect();
    }

    public void PlayTypewriterEffect()
    {
        // Clear any existing text and initialize the typewriter effect
        textComponent.text = "";
        currentText = "";

        // Create a sequence
        Sequence typewriterSequence = DOTween.Sequence();

        for (int i = 0; i < fullText.Length; i++)
        {
            int index = i; // Capture index for closure
            typewriterSequence.AppendCallback(() =>
            {
                char currentChar = fullText[index];

                // Check for newline character
                if (currentChar == '\n')
                {
                    // Optional: Add a slight delay for new lines
                    typewriterSequence.AppendInterval(typeSpeed * 3);
                }

                currentText += fullText[index];
                textComponent.text = currentText; // Update the text component
            });
            typewriterSequence.AppendInterval( typeSpeed);
        }
    }
}
