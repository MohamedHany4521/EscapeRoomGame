using UnityEngine;
using UnityEngine.Events;

public class WavePuzzle : MonoBehaviour
{
    public Renderer screen;
    public Transform knobAmp, knobFreq, knobPhase;

    [Header("Control")]
    public bool isActive = false;

    [Header("Knob Degrees")]
    public float turnSpeed = 120f;          // degrees per second
    public float minAngle = -135f;          // amp + freq limits
    public float maxAngle = 135f;
    public bool invertKnob = true;          // flip visual direction
    public Vector3 knobAxis = Vector3.up;   // rotation axis (Y)

    [Header("Ranges")]
    public Vector2 ampRange = new(0.1f, 0.45f);
    public Vector2 freqRange = new(1f, 8f);

    [Header("Target")]
    public bool randomizeTarget = true;
    public float minSeparationPercent = 25f;
    public float targetAmp = 0.3f, targetFreq = 4f, targetPhase = 0.25f;

    [Header("Tolerance")]
    [Range(0.1f, 20f)] public float tolerancePercent = 2f;
    public bool logError = false;

    [Header("Audio")]
    public AudioSource sfx;                 // one-shots (tick, solved)
    public AudioSource hum;                 // looping tone (set Loop on)
    public AudioClip tickClip;
    public AudioClip solvedClip;
    public float tickEveryDegrees = 15f;
    public float humVolume = 0.25f;
    public Vector2 humPitch = new(0.7f, 1.8f);     // frequency knob: min..max pitch
    public Vector2 humVolumeRange = new(0.05f, 0.4f); // amplitude knob: min..max volume

    public UnityEvent onSolved;

    float[] ang = new float[3];             // degrees: amp, freq, phase
    Quaternion[] baseRot = new Quaternion[3];
    Transform[] knobs;
    float tickAcc;
    int selected;
    bool solved;
    MaterialPropertyBlock mpb;

    // values from knob angles
    float Amp => Mathf.Lerp(ampRange.x, ampRange.y, Mathf.InverseLerp(minAngle, maxAngle, ang[0]));
    float Freq => Mathf.Lerp(freqRange.x, freqRange.y, Mathf.InverseLerp(minAngle, maxAngle, ang[1]));
    float Phase => Mathf.Repeat(ang[2], 360f) / 360f;

    // error in % of each knob's range
    float AmpErr => Mathf.Abs(Amp - targetAmp) / (ampRange.y - ampRange.x) * 100f;
    float FreqErr => Mathf.Abs(Freq - targetFreq) / (freqRange.y - freqRange.x) * 100f;
    float PhaseErr => Mathf.Abs(Mathf.DeltaAngle(Phase * 360f, targetPhase * 360f)) / 180f * 100f;

    // overall error = worst of the three
    public float ErrorPercent => Mathf.Max(AmpErr, FreqErr, PhaseErr);
    public float MatchPercent => Mathf.Clamp(100f - ErrorPercent, 0f, 100f);

    void Start()
    {
        mpb = new MaterialPropertyBlock();
        knobs = new[] { knobAmp, knobFreq, knobPhase };
        for (int i = 0; i < 3; i++)
            if (knobs[i]) baseRot[i] = knobs[i].localRotation;

        ang[0] = Random.Range(minAngle, maxAngle);
        ang[1] = Random.Range(minAngle, maxAngle);
        ang[2] = Random.Range(0f, 360f);

        if (randomizeTarget) RandomizeTarget();

        if (hum) { hum.loop = true; if (isActive) hum.Play(); }
        ApplyAll();
    }

    public void SetActive(bool value)
    {
        isActive = value;
        if (hum)
        {
            if (value && !solved) hum.Play();
            else hum.Pause();
        }
    }

    public void RandomizeTarget()
    {
        PickTarget();
        // re-roll if it starts too close to the player's wave
        for (int i = 0; i < 20 && ErrorPercent < minSeparationPercent; i++)
            PickTarget();
    }

    void PickTarget()
    {
        targetAmp = Random.Range(ampRange.x, ampRange.y);
        targetFreq = Mathf.Round(Random.Range(freqRange.x, freqRange.y) * 2f) / 2f;  // steps of 0.5
        targetFreq = Mathf.Clamp(targetFreq, freqRange.x, freqRange.y);
        targetPhase = Random.value;
    }

    // reset after solving
    public void ResetPuzzle()
    {
        solved = false;
        ang[0] = Random.Range(minAngle, maxAngle);
        ang[1] = Random.Range(minAngle, maxAngle);
        ang[2] = Random.Range(0f, 360f);
        if (randomizeTarget) RandomizeTarget();
        ApplyAll();
        if (hum && isActive) hum.Play();
    }

    void Update()
    {
        if (solved || !isActive) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) selected = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selected = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selected = 2;
        if (Input.GetKeyDown(KeyCode.Tab)) selected = (selected + 1) % 3;

        float input = Input.GetAxis("Horizontal") + Input.GetAxis("Mouse ScrollWheel") * 5f;
        if (Mathf.Abs(input) < 0.001f) return;

        float delta = input * turnSpeed * Time.deltaTime;   // degrees this frame
        float before = ang[selected];

        if (selected == 2) ang[selected] = Mathf.Repeat(ang[selected] + delta, 360f);
        else ang[selected] = Mathf.Clamp(ang[selected] + delta, minAngle, maxAngle);

        float moved = Mathf.Abs(ang[selected] - before);
        if (selected == 2 && moved > 180f) moved = 360f - moved;   // ignore wrap jump

        tickAcc += moved;
        if (tickAcc >= tickEveryDegrees)
        {
            tickAcc = 0;
            if (sfx && tickClip)
            {
                sfx.pitch = Random.Range(0.95f, 1.05f);
                sfx.PlayOneShot(tickClip);
            }
        }

        ApplyAll();
        CheckSolved();

        if (logError)
            Debug.Log($"Error: {ErrorPercent:F1}%  (A {AmpErr:F1}  F {FreqErr:F1}  P {PhaseErr:F1})");
    }

    void ApplyAll()
    {
        // shader
        screen.GetPropertyBlock(mpb);
        mpb.SetFloat("_AmpA", Amp); mpb.SetFloat("_FreqA", Freq); mpb.SetFloat("_PhaseA", Phase);
        mpb.SetFloat("_AmpB", targetAmp); mpb.SetFloat("_FreqB", targetFreq); mpb.SetFloat("_PhaseB", targetPhase);
        screen.SetPropertyBlock(mpb);

        // knobs
        for (int i = 0; i < 3; i++)
        {
            if (!knobs[i]) continue;
            float a = invertKnob ? -ang[i] : ang[i];
            knobs[i].localRotation = baseRot[i] * Quaternion.AngleAxis(a, knobAxis);
        }

        // hum: pitch follows frequency, volume rises as error drops
        if (hum)
        {
            float aT = Mathf.InverseLerp(ampRange.x, ampRange.y, Amp);
            float fT = Mathf.InverseLerp(freqRange.x, freqRange.y, Freq);
            hum.volume = Mathf.Lerp(humVolumeRange.x, humVolumeRange.y, aT);
            hum.pitch = Mathf.Lerp(humPitch.x, humPitch.y, fT);
        }
    }

    void CheckSolved()
    {
        if (ErrorPercent > tolerancePercent) return;

        solved = true;

        // snap knobs to exact target angles
        ang[0] = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(ampRange.x, ampRange.y, targetAmp));
        ang[1] = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(freqRange.x, freqRange.y, targetFreq));
        ang[2] = targetPhase * 360f;
        ApplyAll();

        if (hum) hum.Stop();
        if (sfx && solvedClip) { sfx.pitch = 1f; sfx.PlayOneShot(solvedClip); }
        onSolved.Invoke();
    }
}