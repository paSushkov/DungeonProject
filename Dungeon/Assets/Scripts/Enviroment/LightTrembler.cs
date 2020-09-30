using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightTrembler : MonoBehaviour
{
    [SerializeField] private float defaultRange = 1;
    [SerializeField] private float defaultIntensity = 1;
    [SerializeField] [Range(0, 1f)] private float rangeMaxRandom = 1;
    [SerializeField] [Range(0, 1f)] private float intensityMaxRandom = 1;
    [SerializeField] [Range(0, 1f)] private float lerpSpeed = 0.2f;
    private Light myLight;
    private float targetRange;
    private float targetIntensity;


    private void Awake()
    {
        TryGetComponent(out myLight);
        targetIntensity = defaultIntensity + defaultIntensity * Random.Range(-intensityMaxRandom, intensityMaxRandom);
        targetRange = defaultRange + defaultRange * Random.Range(-rangeMaxRandom, rangeMaxRandom);
    }

    void LateUpdate()
    {
        if (Mathf.Abs(myLight.intensity - targetIntensity) < 0.01)
        {
            targetIntensity = defaultIntensity +
                              defaultIntensity * Random.Range(-intensityMaxRandom, intensityMaxRandom);
        }

        if (Mathf.Abs(myLight.range - targetRange) < 0.1)
        {
            targetRange = defaultRange + defaultRange * Random.Range(-rangeMaxRandom, rangeMaxRandom);
        }

        myLight.intensity = Mathf.Lerp(myLight.intensity, targetIntensity, lerpSpeed*Time.deltaTime*100);
        myLight.range = Mathf.Lerp(myLight.range, targetRange, lerpSpeed*Time.deltaTime*100);
    }
}