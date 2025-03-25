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
    [SerializeField] GameObject pointsObject;
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI pointsText;
    private TextMeshProUGUI fuelText;

    [SerializeField] private Slider fuelBar;
    [SerializeField] private float maxFuel = 1000f; 
    private float currentFuel;
    
    
    private void Start()
    {
        fuelText = fuelObject.GetComponent<TextMeshProUGUI>();
        pointsText = pointsObject.GetComponent<TextMeshProUGUI>();
        currentFuel = maxFuel; // Inizia con il serbatoio pieno
        UpdateFuelBar();
    }

    private void UpdateFuelBar()
    {
        fuelBar.value = currentFuel / maxFuel; // Normalizza il valore tra 0 e 1
    }
    
    public void UpdateFuelText(int fuel)
    {
        fuelText.text = fuel.ToString();
        currentFuel = Mathf.Clamp(fuel, 0, maxFuel);
        UpdateFuelBar();
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
        pointsObject.SetActive(true);
        pointsText.text = "time: " + Time.time + " s";
        gameOverText.alpha = 0f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, targetColor.a, elapsedTime / fadeDuration);
            blackScreenImage.color = new Color(0f, 0f, 0f, alpha);

            
            Color newColor = gameOverText.color;
            newColor.a = alpha;
            gameOverText.color = newColor;

            yield return null;
        }
    }
}
