using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Image blackScreenImage; 
    private float fadeDuration = 1f; 
    [SerializeField] GameObject gameOverObject;
    [SerializeField] GameObject fuelObject;
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI fuelText;

    private void Start()
    {
        fuelText = fuelObject.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateFuelText(int fuel)
    {
        fuelText.text = fuel.ToString();
    }
    public void GameOverCamera()
    {
        gameOverText = gameOverObject.GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadeInCoroutine());
        
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        Color startColor = blackScreenImage.color;
        Color targetColor = new Color(0f, 0f, 0f, 1f); // black alpha 1 (opaco)

        
        gameOverObject.SetActive(true);
        gameOverText.alpha = 0f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, targetColor.a, elapsedTime / fadeDuration);
            blackScreenImage.color = new Color(0f, 0f, 0f, alpha);

            
            gameOverText.alpha = alpha;

            yield return null;
        }
    }
}
