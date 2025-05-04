using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Arithmic;
using NAudio.Wave;
using Tiger;
using Tiger.Schema.Audio;

namespace Charm;

public partial class MusicPlayerControl : UserControl
{
    private WaveOut _output;
    private Wem _wem;
    private WwiseSound _sound;
    private WaveChannel32 _waveProvider;

    public bool CanPlay { get; set; } = false;

    public MusicPlayerControl()
    {
        InitializeComponent();
        SetVolume(VolumeBar.Value);
    }

    private void MakeOutput()
    {
        _output = new WaveOut();
        _output.PlaybackStopped += (sender, args) =>
        {
            _output.Stop();
            _waveProvider.Position = 0;
            SetSliderPosition(0, true);
            (PlayPause.Content as TextBlock).Text = "PLAY";
        };
    }

    public void SetPlayingText(string name)
    {
        PlayingText.Text = $"PLAYING: {name}";
    }

    public FileHash GetWem()
    {
        if (_wem != null)
            return _wem.Hash;
        else
            return null;
    }

    public bool SetWem(Wem wem)
    {
        _output?.Dispose();

        if (wem is null)
            return false;

        _wem = wem;
        _waveProvider = wem.MakeWaveChannel();

        if (_waveProvider == null)
        {
            CanPlay = false;
            Log.Error("WaveProvider is null");
            MessageBox.Show("Error: WaveProvider is null");
            return false;
        }

        try
        {
            MakeOutput();
            _output.Init(_waveProvider);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to initialize audio output: {ex.Message}");
            MessageBox.Show("Error initializing audio playback.");
            CanPlay = false;
            return false;
        }

        SetVolume(VolumeBar.Value);
        CanPlay = true;

        // Ensure duration UI is updated and safe
        var totalTime = _waveProvider.TotalTime;
        CurrentDuration.Text = Wem.GetDurationString((float)totalTime.TotalSeconds); // Could be 0 initially
        TotalDuration.Text = Wem.GetDurationString((float)totalTime.TotalSeconds);

        ProgressBar.Value = 0;
        SetPlayingText(wem.Hash);

        return true;
    }

    public async Task SetSound(WwiseSound sound)
    {
        if (_output != null)
            _output.Dispose();
        _sound = sound;
        if (sound.TagData.Wems.Count > 10)
        {
            MainWindow.Progress.SetProgressStages(new List<string>
            {
                $"Loading Sound {sound.Hash}",
            });
            await Task.Run(() =>
            {
                _waveProvider = sound.MakeWaveChannel();
            });
            MainWindow.Progress.CompleteStage();
        }
        else
        {
            _waveProvider = sound.MakeWaveChannel();
        }

        try
        {
            MakeOutput();
            _output.Init(_waveProvider);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to initialize audio output: {ex.Message}");
            MessageBox.Show("Error initializing audio playback.");
            CanPlay = false;
            return;
        }

        SetVolume(VolumeBar.Value);
        CanPlay = true;

        // Ensure duration UI is updated and safe
        var totalTime = _waveProvider.TotalTime;
        CurrentDuration.Text = Wem.GetDurationString((float)totalTime.TotalSeconds); // Could be 0 initially
        TotalDuration.Text = sound.Duration ?? Wem.GetDurationString((float)totalTime.TotalSeconds);

        ProgressBar.Value = 0;
        SetPlayingText(sound.Hash);
    }

    public void Play()
    {
        if (_output == null)
        {
            Log.Error("Output is null");
            return;
        }

        string name = _wem == null ? _sound.Hash : _wem.Hash;
        Log.Info($"Playing {name}");
        (PlayPause.Content as TextBlock).Text = "PAUSE";

        Task.Run(() =>
        {
            try
            {
                _output.Play();  // can sometimes break if its still ending the playback
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
                return;
            }
            StartPositionHandlerAsync();
        });
    }

    public void StartPositionHandlerAsync()
    {
        while (IsPlaying() && CanPlay)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsPlaying())
                    SetSliderPosition(_waveProvider.Position);
            });
            System.Threading.Thread.Sleep(100);
        }
    }

    public bool IsPlaying()
    {
        if (_output is null)
            return false;

        return _output.PlaybackState == PlaybackState.Playing;
    }

    public void Pause()
    {
        _output?.Pause();
        (PlayPause.Content as TextBlock).Text = "PLAY";
        string name = _wem == null ? _sound.Hash : _wem.Hash;
        Log.Verbose($"Paused {name}");
    }

    public void SetVolume(double volume)
    {
        if (_waveProvider != null)
            _waveProvider.Volume = (float)volume;
    }

    private void VolumeBar_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var s = sender as Slider;
        SetVolume(s.Value);
    }

    private void PlayPause_OnClick(object sender, RoutedEventArgs e)
    {
        if (_wem == null && _sound == null)
            return;

        if (IsPlaying())
        {
            Pause();
        }
        else
        {
            Play();
        }
    }

    private void ProgressBar_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        SetPosition(sender as Slider);
    }

    private void ProgressBar_OnDragCompleted(object sender, DragCompletedEventArgs e)
    {
        SetPosition(sender as Slider);
    }

    private void SetPosition(Slider slider)
    {
        if (_wem == null && _sound == null)
            return;

        Pause();

        double timeInSeconds = slider.Value * _waveProvider.TotalTime.TotalSeconds;
        long targetPosition = (long)(timeInSeconds * _waveProvider.WaveFormat.AverageBytesPerSecond);

        // Clamp to valid byte range
        targetPosition = Math.Min(targetPosition, _waveProvider.Length - _waveProvider.WaveFormat.BlockAlign);
        targetPosition = Math.Max(targetPosition, 0);

        if (targetPosition >= _waveProvider.Length - _waveProvider.WaveFormat.BlockAlign)
        {
            // don't Play() past end
            SetSliderPosition(targetPosition, true);
            return;
        }

        long alignedPosition = (targetPosition / _waveProvider.WaveFormat.BlockAlign) * _waveProvider.WaveFormat.BlockAlign;
        _waveProvider.Position = alignedPosition;

        SetSliderPosition(targetPosition);
        Play();
    }

    private void SetSliderPosition(long bytePosition, bool forceUpdate = false)
    {
        if (_waveProvider == null)
            return;

        var waveFormat = _waveProvider.WaveFormat;
        var totalSeconds = _waveProvider.TotalTime.TotalSeconds;
        var bytesPerSecond = waveFormat.AverageBytesPerSecond;

        if (bytesPerSecond <= 0 || totalSeconds <= 0)
            return;

        double proportion = bytePosition / (totalSeconds * bytesPerSecond);

        double progressMilliseconds = proportion * _waveProvider.TotalTime.TotalMilliseconds;
        double deltaMilliseconds = Math.Abs(ProgressBar.Value - proportion) * _waveProvider.TotalTime.TotalMilliseconds;

        if (deltaMilliseconds < 500 || forceUpdate)
        {
            TimeSpan currentTime = TimeSpan.FromMilliseconds(progressMilliseconds);
            CurrentDuration.Text = Wem.GetDurationString((float)currentTime.TotalSeconds);
            ProgressBar.Value = proportion;
        }
    }

    public void Dispose()
    {
        CanPlay = false;
        _output?.Stop();
        _waveProvider?.Dispose();
        _output?.Dispose();
        _wem?.Dispose();
    }
}
