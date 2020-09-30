using UnityEngine;

public class SelfDestructor : MonoBehaviour
{
    public float lifeTime = 3f;
    public bool destroyOnAwake = true;
    public bool destroyOnStart = false;

    private void Awake()
    {
        if (destroyOnAwake)
            Destroy(gameObject, lifeTime);
    }

    private void Start()
    {
    if (destroyOnStart)
            Destroy(gameObject, lifeTime);
    }
}