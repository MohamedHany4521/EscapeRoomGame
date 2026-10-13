using UnityEngine;

public class Freqncey : MonoBehaviour
{
    public Material mat;
    [Range(0.5f, 10f)] public float frequency = 2.84f;
    public float speed = 0.5f;   // offset units per second (negative = other direction)
    public float addValue = 0f;  // extra offset added on top (e.g. knob phase)

    static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
    float offsetX;

    void Update()
    {
        offsetX = Mathf.Repeat(offsetX + speed * Time.deltaTime, 1f);

        mat.SetTextureScale(BaseMap, new Vector2(frequency, 1f));
        mat.SetTextureOffset(BaseMap, new Vector2(offsetX + addValue, 0f));
    }
}
