using UnityEngine;

public class DebugTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public KeyCode SpawnIndicator;
    public GameObject canvasObject;
    public GameObject indicator;
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        bool spawn = Input.GetKeyDown(SpawnIndicator);
        if (spawn)
        {
            GameObject damageIndicator = Instantiate(this.indicator, canvasObject.transform);
            DamageIndicatorUI indicatorClass = damageIndicator.GetComponent<DamageIndicatorUI>();
            indicatorClass.WorldVector3 = transform.position;
            indicatorClass.at = transform;
            indicatorClass.Text.text = "100"; // Example damage value
            indicatorClass.Text.color = Color.red; // Example color for the damage text
            indicatorClass.MaxScale = 1;

        }
    }
}
