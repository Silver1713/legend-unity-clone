using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // Prevent this GameObject from being destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
