using System.Collections;
using TMPro;
using UnityEngine;

public class DamageIndicatorUI : MonoBehaviour
{

    [Header("Position Settings")] 
    public Transform at;
    public Vector3 WorldVector3;

    [Header("Float Settings")]
    public float FloatHeight = 1f;      // Height at which the damage indicator floats above the target
    public float FloatSpeed = 1f;       // Speed at which the damage indicator floats upwards

    [Header("Fade & Scale Settings")]
    public float FadeDuration = 1f;     // Duration over which the indicator fades out
    public float MaxScale = 1.5f;       // Maximum scale of the damage indicator

    [Header("References")]
    public TextMeshProUGUI Text;            // Reference to the TextMeshPro component for displaying damage text

    private Color _originalColor;
    private Vector3 _startPos;

    void Start()
    {
        // Cache start position and color
        _startPos = transform.position;
        _originalColor = Text.color;

        // Raise initial position
        WorldVector3 = at.position;
        transform.position = Camera.main.WorldToScreenPoint(WorldVector3) + Vector3.up * FloatHeight;

        // Randomize starting scale
        float randomScale = Random.Range(1.0f, MaxScale);
        transform.localScale = Vector3.one * randomScale;


        // Kick off the fade & float
        StartCoroutine(FadeAndFloat());
    }

    private IEnumerator FadeAndFloat()
    {
        float elapsed = 0f;
        Color c = _originalColor;

        while (elapsed < FadeDuration)
        {
            // Move upward
            transform.position += Vector3.up * FloatSpeed * Time.deltaTime;

            // Fade out (alpha from 1 to 0)
            float alpha = Mathf.Lerp(1f, 0f, elapsed / FadeDuration);
            c.a = alpha;
            Text.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully transparent
        c.a = 0f;
        Text.color = c;

        // Destroy this indicator
        Destroy(gameObject);
    }

    public void SetDamage(int amount)
    {
        Text.text = amount.ToString();
        Text.color = _originalColor; // reset alpha in case reused
    }
}
