using Arithmic;
using DataTool.ConvertLogic;
using NAudio.Vorbis;
using NAudio.Wave;
using Tiger.Schema.Audio.ThirdParty;

namespace Tiger.Schema.Audio;

/// <summary>
/// Used for efficient loading of RIFF tags.
/// Only loads the tag when asked and keeps it cached here, it's ofc still in PackageHandler cache
/// but still a bit more efficient.
/// </summary>
[NonSchemaType(TigerStrategy.DESTINY1_RISE_OF_IRON, 8, new[] { 21 })]
[NonSchemaType(TigerStrategy.DESTINY2_SHADOWKEEP_2601, 26, new[] { 6 })]
[NonSchemaType(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, 26, new[] { 7 })]
public class Wem : TigerFile
{
    private MemoryStream _wemStream = null;
    private WaveStream _wemReader = null;
    private bool _bDisposed = false;
    private OWSound.WwiseRIFFVorbis WemData = null;

    private int _channels;
    public int Channels
    {
        get
        {
            GetWEMData();
            return WemData.Channels;
        }
    }

    public Wem(FileHash hash) : base(hash)
    {
    }

    // 85840081 and FBEB1A81 for testing
    public void Load()
    {
        if (GetReferenceHash() is null || GetReferenceHash().IsInvalid())
            return;

        _bDisposed = false;
        _wemStream = GetWemStream();
        _wemReader = new VorbisWaveReader(_wemStream);
        _channels = _wemReader.WaveFormat.Channels;

        try
        {
            if (_wemReader.WaveFormat.Channels > 2) // this sucks I hate this so much
                _wemReader = DownmixToStereo(_wemReader);
        }
        catch (Exception e)
        {
            Log.Error($"{e.Message}: {_wemReader.WaveFormat.ToString()}");
        }
    }

    private void GetWEMData()
    {
        if (WemData is null)
            WemData = WemConverter.GetWwiseRIFFVorbis(GetStream());
    }

    private void CheckLoaded()
    {
        if (_wemStream == null || _bDisposed)
            Load();
    }

    private MemoryStream GetWemStream()
    {
        // Somethings going on in here maybe that's causing some random artifacting
        // on some audio (especially surround sound)
        return WemConverter.ConvertSoundFile(GetStream());
    }

    public WaveChannel32? MakeWaveChannel()
    {
        CheckLoaded();
        try
        {
            var waveChannel = new WaveChannel32(_wemReader);
            waveChannel.PadWithZeroes = false;
            return waveChannel;
        }
        catch (Exception e)
        {
            Log.Error($"{e.Message}: {_wemReader.WaveFormat.ToString()}");
            return null;
        }
    }

    public TimeSpan GetDuration()
    {
        GetWEMData();
        return TimeSpan.FromSeconds((double)GetStream().Length / (double)WemData.AvgBytesPerSecond);
    }

    public static string GetDurationString(TimeSpan timespan)
    {
        return $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
    }

    public string Duration
    {
        get
        {
            GetWEMData();
            var timespan = GetDuration();
            return GetDurationString(timespan);
        }
    }

    public int Seconds
    {
        get
        {
            GetWEMData();
            return GetDuration().Seconds;
        }
    }

    public void Dispose()
    {
        _wemReader?.Dispose();
        _wemStream?.Dispose();
        _bDisposed = true;
    }

    public void SaveToFile(string savePath)
    {
        CheckLoaded();
        _wemReader.Position = 0;

        // Remake the reader so none of the downmix stuff gets exported, though idk if that really matters or not at this point
        if (_wemReader.WaveFormat.Channels > 2)
            _wemReader = new VorbisWaveReader(_wemStream);

        WaveFileWriter.CreateWaveFile(savePath, _wemReader);
    }

    // This is no where near perfect but it's good enough for preview audio...
    public WaveStream DownmixToStereo(WaveStream waveStream)
    {
        var inputFormat = waveStream.WaveFormat;
        //if (inputFormat.Channels != 4) // For testing, C8FC1A81 has 5
        //    throw new ArgumentException($"Input stream {Hash} must have 4 channels. Has {waveStream.WaveFormat.Channels}");

        Log.Info($"Downsampling {this.Hash} to Stereo format ({waveStream.WaveFormat.ToString()})");

        var stereoFormat = WaveFormat.CreateIeeeFloatWaveFormat(inputFormat.SampleRate, 2);
        var output = new MemoryStream();
        var writer = new WaveFileWriter(output, stereoFormat);

        int bytesPerSample = inputFormat.BitsPerSample / 8; // 4 bytes for 32-bit IEEE Float
        int frameSize = inputFormat.Channels * bytesPerSample; // Total size of one frame
        var buffer = new byte[frameSize * 1024]; // Buffer size for reading, can be adjusted
        int read;

        while ((read = waveStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            // Ensure we are processing complete frames, adjust read size if necessary
            int numFrames = read / frameSize;

            if (numFrames == 0)
                continue; // Skip if no frames were read (avoid reading partial frames)

            // Convert byte buffer to float samples
            float[] samples = new float[numFrames * inputFormat.Channels];
            for (int i = 0; i < numFrames; i++)
            {
                for (int channel = 0; channel < inputFormat.Channels; channel++)
                {
                    int byteIndex = i * frameSize + channel * bytesPerSample;
                    samples[i * inputFormat.Channels + channel] = BitConverter.ToSingle(buffer, byteIndex);
                }
            }

            // Downmix 4 channels to 2 channels (stereo)
            float[] stereoBuffer = new float[numFrames * 2]; // 2 channels for stereo output
            for (int i = 0, j = 0; i < samples.Length; i += inputFormat.Channels, j += 2)
            {
                // Downmix channels: Combine the 4 channels into left and right stereo
                // In order: Front Left, Front Right, Back Left, Back Right
                // Adding back left and right seem to cause most of the artifacting
                stereoBuffer[j] = Math.Clamp(samples[i], -1f, 1f); // Left 
                stereoBuffer[j + 1] = Math.Clamp(samples[i + 1], -1f, 1f);// Right 
            }

            // Convert the downmixed stereo floats back to bytes
            byte[] stereoBytes = new byte[stereoBuffer.Length * bytesPerSample];
            for (int i = 0; i < stereoBuffer.Length; i++)
            {
                // Convert float back to 32-bit IEEE float for output
                BitConverter.GetBytes(stereoBuffer[i]).CopyTo(stereoBytes, i * bytesPerSample);
            }

            // Write the stereo bytes to the output stream
            writer.Write(stereoBytes, 0, stereoBytes.Length);
        }

        writer.Flush();
        output.Position = 0;
        return new WaveFileReader(output);
    }
}
