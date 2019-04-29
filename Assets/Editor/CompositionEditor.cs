using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CompositionEditor : EditorWindow
{
    public AudioClip song;
    private AudioClip songBuffer;
    private float[] songData;
    private Texture2D waveformTexture;

    private int selectorLocation;

    private Rect waveformToolsRect;
    private Rect waveformRect;

    private int viewportOffset;
    private float zoomLevel;
    private float bpm;
    private float beatOffset;
    private float[] beats;
    private List<Rect> beatMarkerRects;
    private Texture2D beatMarkerTexture;

    [MenuItem("Window/Composition")]
    public static void ShowWindow()
    {
        CompositionEditor window = GetWindow<CompositionEditor>("Composition");
        window.minSize = new Vector2(400, 400);
    }
     
    private void OnEnable()
    {
        viewportOffset = 0;
        zoomLevel = 1f;

        InitTextures();
    }

    /// <summary>
    /// Analagous to Update()
    /// </summary>
    private void OnGUI()
    {
        songBuffer = (AudioClip)EditorGUILayout.ObjectField(song, typeof(AudioClip), false);
        if (songBuffer != null && (song == null || song != songBuffer))
        {
            song = songBuffer;
            songData = null;
            beats = null;
        }

        if (GUILayout.Button("Generate Waveform"))
        {
            InitTextures();
        }

        if (waveformTexture != null)
        {
            DrawLayouts();
        }
    }

    private void DrawLayouts()
    {
        DrawWaveformArea();
        DrawWaveformToolsArea();
        DrawBeatMarkers();
    }

    private void DrawWaveformToolsArea()
    {
        waveformToolsRect.x = 0;
        waveformToolsRect.y = waveformRect.y - 75;
        waveformToolsRect.width = Screen.width;
        waveformToolsRect.height = 50;

        GUILayout.BeginArea(waveformToolsRect);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Zoom In"))
        {
            zoomLevel *= 2;
            waveformTexture = GetZoomWaveform(song, (int)waveformRect.height - 25);
        }

        else if (GUILayout.Button("Zoom Out"))
        {
            zoomLevel /= 2;
            waveformTexture = GetZoomWaveform(song, (int)waveformRect.height - 25);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("BPM: ");
        bpm = EditorGUILayout.FloatField(bpm);
        GUILayout.Label("Beat Offset: ");
        beatOffset = EditorGUILayout.FloatField(beatOffset);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Populate Beats"))
        {
            GetBeats();
            beatMarkerRects = GetBeatMarkerRects();
        }

        GUILayout.EndArea();
    }

    private void DrawWaveformArea()
    {
        int h = Mathf.Max(175, Screen.height / 4);
        waveformRect.x = 0;
        waveformRect.y = Screen.height - (h + 25);
        waveformRect.width = Screen.width;
        waveformRect.height = h;

        GUI.DrawTexture(waveformRect, waveformTexture);
    }

    private void DrawBeatMarkers()
    {
        if (beats == null || beatMarkerRects == null)
            return;

        GUILayout.BeginArea(waveformRect);

        foreach (Rect r in beatMarkerRects)
            GUI.DrawTexture(r, beatMarkerTexture);

        GUILayout.EndArea();
    }

    private void InitTextures()
    {
        if (song != null)
        {
            int h = Mathf.Max(175, Screen.height / 4);
            waveformRect.x = 0;
            waveformRect.y = Screen.height - (h + 25);
            waveformRect.width = Screen.width;
            waveformRect.height = h;
            waveformTexture = GetZoomWaveform(song, (int)waveformRect.height - 25);

            beatMarkerTexture = new Texture2D(1, (int)waveformRect.height);
            for (int x = 0; x < beatMarkerTexture.width; x++)
            {
                for (int y = 0; y < beatMarkerTexture.height; y++)
                {
                    beatMarkerTexture.SetPixel(x, y, Color.green);
                }
            }
        }
    }

    private Texture2D GetWaveform(AudioClip clip, int height)
    {
        int width = Screen.width;
        int channels = clip.channels;
        int size = clip.samples * channels;
        if (songData == null)
        {
            songData = new float[size];
            clip.GetData(songData, 0);
        }
        float[] waveformData = new float[width * channels];
        Texture2D waveformTexture = new Texture2D(width, height);

        // Populate the waveformData array
        float partialPack = (float)clip.samples / width;
        float packSize = partialPack * channels;
        int d = 0;
        for (int k = 0; k < channels; k++)
        {
            for (float i = k; i < size; i += packSize)
            {
                float sum = 0;
                int bottomClamp = Clamp(i, true, channels, k);
                int topClamp = Mathf.Min(Clamp(i + packSize, false, channels, k), songData.Length);
                for (int j = bottomClamp; j < topClamp; j += channels)
                {
                    sum += Mathf.Abs(songData[j]);
                }
                waveformData[d] = sum / partialPack;
                d++;
            }
        }

        // Make the texture black;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                waveformTexture.SetPixel(x, y, Color.black);
            }
        }

        // Translate the waveformData array to a texture
        int maxWfHeight = (int)((height / channels) * 0.9f);
        for (int z = 0; z < channels; z++)
        {
            int yOffset = (height / (channels * 2)) * ((2 * z) + 1);
            for (int x = 0; x < width; x++)
            {
                int wfHeight = (int)(waveformData[x] * maxWfHeight);
                for (int y = 0; y <= wfHeight; y++)
                {
                    waveformTexture.SetPixel(x, yOffset + y, Color.magenta);
                    waveformTexture.SetPixel(x, yOffset - y, Color.magenta);
                }
            }
        }
        waveformTexture.Apply();
        return waveformTexture;
    }

    private int Clamp(float f, bool ceiling, int modulo, int position)
    {
        int ret = 0;
        if (ceiling)
        {
            ret = Mathf.CeilToInt(f);
            while (ret % modulo != position)
                ret++;
        }
        else
        {
            ret = Mathf.FloorToInt(f);
            while (ret % modulo != position)
                ret--;
        }

        return ret;
    }

    private Texture2D GetZoomWaveform(AudioClip clip, int height)
    {
        int widthInPixels = Screen.width;
        int channels = clip.channels;
        int totalDatapointCount = clip.samples * channels;
        int viewportDatapointCount = (int)(totalDatapointCount / zoomLevel);
        if (songData == null)
        {
            songData = new float[totalDatapointCount];
            clip.GetData(songData, 0);
        }
        float[] waveformData = new float[widthInPixels * channels];
        Texture2D waveformTexture = new Texture2D(widthInPixels, height);

        // Populate the waveformData array
        // Note: A "Pack" is the number of data points used to draw one pixel on the screen
        float singleChannelPack = clip.samples / (widthInPixels * zoomLevel);
        float multiChannelPack = singleChannelPack * channels;
        int d = 0;
        for (int k = 0; k < channels; k++)
        {
            for (float i = k + viewportOffset; i < viewportDatapointCount + viewportOffset; i += multiChannelPack)
            {
                float sum = 0;
                int bottomClamp = Clamp(i, true, channels, k);
                int topClamp = Mathf.Min(Clamp(i + multiChannelPack, false, channels, k), songData.Length);
                for (int j = bottomClamp; j < topClamp; j += channels)
                {
                    sum += Mathf.Abs(songData[j]);
                }
                waveformData[d] = sum / singleChannelPack;
                d++;
            }
        }

        // Make the texture black;
        for (int x = 0; x < widthInPixels; x++)
        {
            for (int y = 0; y < height; y++)
            {
                waveformTexture.SetPixel(x, y, Color.black);
            }
        }

        // Translate the waveformData array to a texture
        int maxWfHeight = (int)((height / channels) * 0.9f);
        for (int z = 0; z < channels; z++)
        {
            int yOffset = (height / (channels * 2)) * ((2 * z) + 1);
            for (int x = 0; x < widthInPixels; x++)
            {
                int wfHeight = (int)(waveformData[x] * maxWfHeight);
                for (int y = 0; y <= wfHeight; y++)
                {
                    waveformTexture.SetPixel(x, yOffset + y, Color.magenta);
                    waveformTexture.SetPixel(x, yOffset - y, Color.magenta);
                }
            }
        }
        waveformTexture.Apply();
        return waveformTexture;
    }

    private void GetBeats()
    {
        float songLength = song.length - beatOffset;
        float bps = bpm / 60f;
        float spb = 1 / bps;
        beats = new float[(int)(songLength * bps) + 1];

        int i = 0;
        for (float b = beatOffset; b < songLength; b += spb)
        {
            beats[i] = b;
            i++;
        }
    }

    private List<Rect> GetBeatMarkerRects()
    {
        if (beats == null)
            return null;

        int maxMarkers = Screen.width / 5;
        int totalDatapointCount = song.samples * song.channels;
        float viewportStartInSeconds = viewportOffset;
        float viewportEndInSeconds = (song.length / zoomLevel) + viewportOffset;

        float singleChannelPack = song.samples / (Screen.width * zoomLevel);
        float multiChannelPack = singleChannelPack * song.channels;

        List<Rect> beatMarkerRects = new List<Rect>();
        

        int i = 0;
        while (beats[i] < viewportStartInSeconds)
            i++;
        while (beats[i] < viewportEndInSeconds && beatMarkerRects.Count < maxMarkers)
        {
            float dataPoint = beats[i] * song.frequency;
            float pixel = dataPoint / singleChannelPack;
            beatMarkerRects.Add(new Rect(pixel - 1, 0f, 3f, waveformRect.height));
            i++;
        }

        if (beatMarkerRects.Count < maxMarkers)
            return beatMarkerRects;

        return null;
    }
}
