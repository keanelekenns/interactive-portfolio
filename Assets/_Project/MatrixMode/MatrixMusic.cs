using UnityEngine;

// Placeholder soundtrack for matrix mode, synthesized at runtime so the
// project does not have to bundle a music file. The actual Matrix soundtrack
// is copyrighted, so if you want something closer to the movies, assign a
// track you are licensed to use (e.g. a royalty-free dark techno loop) to
// MatrixModeController.matrixMusic instead.
public static class MatrixMusic
{
    private const int SampleRate = 44100;
    private const float StepDuration = 0.3f; // Eighth notes at 100 BPM
    private const int Steps = 32; // Two bars; the clip loops seamlessly

    public static AudioClip CreateLoop()
    {
        int totalSamples = (int)(Steps * StepDuration * SampleRate);
        float[] samples = new float[totalSamples];

        AddDrone(samples);
        AddArpeggio(samples);
        AddEcho(samples);
        Normalize(samples, 0.6f);

        AudioClip clip = AudioClip.Create("Matrix Mode Loop", totalSamples, 1, SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // Low A drone. 55 Hz divides the loop length exactly, so there is no
    // audible seam at the loop point.
    private static void AddDrone(float[] samples)
    {
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / SampleRate;
            samples[i] += 0.16f * Mathf.Sin(2f * Mathf.PI * 55f * t);
            samples[i] += 0.05f * Mathf.Sin(2f * Mathf.PI * 82.5f * t);
        }
    }

    private static void AddArpeggio(float[] samples)
    {
        float[] barA = { 220.00f, 261.63f, 329.63f, 440.00f }; // A minor
        float[] barB = { 174.61f, 220.00f, 261.63f, 349.23f }; // F major
        int stepSamples = (int)(StepDuration * SampleRate);
        int noteSamples = SampleRate; // Let each note ring for a second

        for (int step = 0; step < Steps; step++)
        {
            float[] chord = step < Steps / 2 ? barA : barB;
            float frequency = chord[step % chord.Length];
            int start = step * stepSamples;
            for (int j = 0; j < noteSamples; j++)
            {
                float t = (float)j / SampleRate;
                float envelope = Mathf.Min(t / 0.005f, 1f) * Mathf.Exp(-t / 0.13f);
                float wave = Mathf.Sin(2f * Mathf.PI * frequency * t)
                    + 0.35f * Mathf.Sin(4f * Mathf.PI * frequency * t)
                    + 0.12f * Mathf.Sin(6f * Mathf.PI * frequency * t);
                // Wrap tails around the end so the loop point stays seamless
                samples[(start + j) % samples.Length] += 0.14f * envelope * wave;
            }
        }
    }

    private static void AddEcho(float[] samples)
    {
        int delaySamples = (int)(1.5f * StepDuration * SampleRate); // Dotted eighth
        for (int pass = 0; pass < 2; pass++)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] += 0.25f * samples[(i - delaySamples + samples.Length) % samples.Length];
            }
        }
    }

    private static void Normalize(float[] samples, float peak)
    {
        float max = 0f;
        foreach (float sample in samples)
        {
            max = Mathf.Max(max, Mathf.Abs(sample));
        }
        if (max < 0.0001f) return;

        float scale = peak / max;
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] *= scale;
        }
    }
}
