using NAudio.Wave;
using NAudio.Dsp;
using System;
using System.Diagnostics;
using System.Linq;

namespace ccc.Services.Audio
{
    public class AudioVisualizerService : IDisposable
    {
        private WasapiLoopbackCapture? _capture;
        private readonly int _fftSize = 2048;
        private readonly int _barCount = 113;
        private readonly float _minFreq = 60;
        private readonly float _maxFreq = 11000;
        
        // Tuning parameters from legacy project
        private readonly float _smoothing = 0.75f; 
        private readonly float _preAmp = 4.0f; 
        
        private float[] _currentBars;
        public float[] CurrentBars => _currentBars;

        public event Action<float[]>? AudioDataUpdated;
        
        // FFT Buffer
        private float[] _sampleBuffer;
        private int _sampleBufferIndex;
        private Complex[] _fftBuffer; // NAudio.Dsp.Complex

        private bool _isCapturing;
        private object _lock = new object();

        public AudioVisualizerService()
        {
            _currentBars = new float[_barCount];
            _sampleBuffer = new float[_fftSize];
            _fftBuffer = new Complex[_fftSize];
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_isCapturing) return;

                try 
                {
                    _capture = new WasapiLoopbackCapture();
                    Console.WriteLine($"[AudioVisualizer] Device: {_capture.WaveFormat}");
                    _capture.DataAvailable += OnDataAvailable;
                    _capture.RecordingStopped += OnRecordingStopped;
                    _capture.StartRecording();
                    _isCapturing = true;
                    Console.WriteLine("[AudioVisualizer] Started Capture successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AudioVisualizer] Failed to start: {ex.Message}");
                    System.Windows.MessageBox.Show($"Audio Visualizer Error: {ex.Message}", "Atlas Error");
                }
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isCapturing || _capture == null) return;
                
                try 
                {
                    _capture.StopRecording();
                    _capture.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AudioVisualizer] Error stopping: {ex.Message}");
                }
                finally
                {
                    _capture = null;
                    _isCapturing = false;
                    _hasLoggedFirstPacket = false;
                }
                
                // Clear bars
                Array.Clear(_currentBars, 0, _barCount);
                AudioDataUpdated?.Invoke(_currentBars);
                 Debug.WriteLine("[AudioVisualizer] Stopped Capture.");
            }
        }

        private bool _hasLoggedFirstPacket = false;
        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (!_hasLoggedFirstPacket) { Console.WriteLine($"[AudioVisualizer] First Packet: {e.BytesRecorded} bytes."); _hasLoggedFirstPacket = true; }
            if (_capture == null) return;
            
            int bytesPerSample = _capture.WaveFormat.BitsPerSample / 8;
            int channels = _capture.WaveFormat.Channels;
            int samplesRead = e.BytesRecorded / bytesPerSample;
            
            // If we don't have enough bytes, skip
            if (samplesRead == 0) return;

            for (int i = 0; i < samplesRead; i += channels)
            {
                // Parse L
                float sample = 0;
                
                // Wasapi Loopback is typically IEEE Float (32 bit)
                if (_capture.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat) 
                {
                    sample = BitConverter.ToSingle(e.Buffer, i * bytesPerSample);
                    // Average with R if Stereo
                    if (channels > 1 && (i + 1) < samplesRead)
                    {
                        float right = BitConverter.ToSingle(e.Buffer, (i + 1) * bytesPerSample);
                        sample = (sample + right) / 2f;
                    }
                }
                // Fallback for PCM (16 bit) - unlikely for Loopback but safe to have
                else if (_capture.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
                {
                    short s = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
                    sample = s / 32768f;
                     if (channels > 1 && (i + 1) < samplesRead)
                    {
                        short r = BitConverter.ToInt16(e.Buffer, (i + 1) * bytesPerSample);
                        sample = (sample + (r/32768f)) / 2f;
                    }
                }

                _sampleBuffer[_sampleBufferIndex] = sample;
                _sampleBufferIndex++;
                
                if (_sampleBufferIndex >= _fftSize)
                {
                    ProcessFFT();
                    _sampleBufferIndex = 0;
                }
            }
        }
        
        private void ProcessFFT()
        {
            // 1. Windowing (Hanning)
            for (int i = 0; i < _fftSize; i++)
            {
                double window = 0.5 * (1.0 - Math.Cos(2.0 * Math.PI * i / (_fftSize - 1)));
                _fftBuffer[i].X = (float)(_sampleBuffer[i] * window);
                _fftBuffer[i].Y = 0;
            }
            
            // 2. FFT
            FastFourierTransform.FFT(true, (int)Math.Log(_fftSize, 2.0), _fftBuffer);
            
            // 3. Map to Bars
            double sampleRate = _capture?.WaveFormat.SampleRate ?? 48000;
            // Half sampling rate is Nyquist
            double dataRange = sampleRate / 2.0; 
            // Only look up to dataRange
             
            // Calculate Magnitudes first for valid bins (0 to fftSize/2)
            // Optimization: Compute on demand in the loop?
             
            double binSize = sampleRate / _fftSize;
             
             // Prepare new values
             float[] newValues = new float[_barCount];

             for (int i = 0; i < _barCount; i++)
             {
                 // Logarithmic Mapping: f = min * (max/min)^(i/N)
                 double logMin = Math.Pow(_maxFreq / _minFreq, (double)i / _barCount);
                 double logMax = Math.Pow(_maxFreq / _minFreq, (double)(i + 1) / _barCount);
                 
                 double startFreq = _minFreq * logMin;
                 double endFreq = _minFreq * logMax;
                 
                 int startBin = (int)(startFreq / binSize);
                 int endBin = (int)(endFreq / binSize);
                 
                 // Clamping
                 if (endBin <= startBin) endBin = startBin + 1;
                 if (startBin < 0) startBin = 0;
                 if (endBin > _fftSize / 2) endBin = _fftSize / 2;
                 
                 float sum = 0;
                 int count = 0;
                 
                 for (int bin = startBin; bin < endBin; bin++)
                 {
                     // Magnitude
                     float mag = (float)Math.Sqrt(_fftBuffer[bin].X * _fftBuffer[bin].X + _fftBuffer[bin].Y * _fftBuffer[bin].Y);
                     sum += mag;
                     count++;
                 }
                 
                 float avg = count > 0 ? sum / count : 0;
                 
                 // Gain/Scaling
                 // Empirically scaled. JS used preAmp * 25 on raw mag. 
                 // Let's assume input samples are 0-1.
                 float scaled = avg * _preAmp * 25.0f; // Matching JS logic
                 
                 newValues[i] = scaled; 
             }
             
             // 4. Smoothing and Update
             for(int i=0; i<_barCount; i++)
             {
                 float target = newValues[i];
                 
                 // Apply sensitivity cutoff?
                 // JS: normalized = Math.min(255, round(...))
                 // JS: adjusted = min(255, smoothed * sensitivity/64)
                 
                 // Apply smoothing
                 float prev = _currentBars[i];
                 float val = (target * (1 - _smoothing)) + (prev * _smoothing);
                 
                 _currentBars[i] = Math.Max(0, val);
             }
             
             AudioDataUpdated?.Invoke(_currentBars);
        }

        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            _isCapturing = false;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
