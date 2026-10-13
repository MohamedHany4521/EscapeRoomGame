using UnityEngine;

public class Sinewave : MonoBehaviour
{

    public LineRenderer WaveLine;
    public int points;

    public float amplitude = 1;
    public float frequncey = 1;
    public float speed = 1;
    public Vector2 xLimts = new Vector2(0, 1);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaveLine = GetComponent<LineRenderer>();
    }

    void Draw()
    {
        float xStart = xLimts.x;
        float tau = 2 * Mathf.PI;
        float xFinish = xLimts.y;

        WaveLine.positionCount = points;

        for(int currentPoints = 0; currentPoints<points;currentPoints++)
        {
            float progress = (float)currentPoints / (points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = amplitude * Mathf.Sin((tau*frequncey*x)+(Time.timeSinceLevelLoad*speed));
            WaveLine.SetPosition(currentPoints, new Vector3(x, y, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }
}
