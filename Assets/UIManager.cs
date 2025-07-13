using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject DamageIndicatorPrefab; // Prefab for the damage indicator UI
    public TextMeshProUGUI DisplayHealth;
    private static UIManager _instance;

    public Canvas mainCanvas;
   public static UIManager Instance
    {
        get {
            if (_instance is null)
            {
               Debug.LogError("UIManager instance is null. Ensure it is initialized before accessing.");
            }
            return _instance;
            
        }
    }

    void Awake()
    {
        if (_instance is null)
        {
            _instance = this;
        }
        else Destroy(this);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDamageIndicator(Transform at, string damageText, Color textColor)
    {
        // Instantiate the damage indicator prefab at the specified position
        GameObject damageIndicator = Instantiate(DamageIndicatorPrefab, mainCanvas.transform);
        
        // Set the text of the damage indicator
        DamageIndicatorUI indicatorUI = damageIndicator.GetComponent<DamageIndicatorUI>();
        if (indicatorUI != null)
        {
            indicatorUI.at = at;
            indicatorUI.Text.text = damageText;
            indicatorUI.Text.color = textColor;
            indicatorUI.FloatSpeed =  Random.Range(0.5f, 1.5f); // Randomize float speed
            indicatorUI.FloatHeight = Random.Range(0.5f, 1.5f); // Randomize float height
            
        }
    }


    public void UpdateHealthDisplay(float currentHealth, float maxHealth)
    {
        if (DisplayHealth != null)
        {
            if (currentHealth < 0)
            {
                DisplayHealth.text = $"Health: Died";
            }
            else
            {
                DisplayHealth.text = $"Health: {currentHealth}/{maxHealth}";
            }
        }
        else
        {
            Debug.LogWarning("DisplayHealth TextMeshProUGUI is not assigned in UIManager.");
        }
    }
}
