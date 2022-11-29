using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Microsoft.Extensions.Logging;
using System.Configuration;
using MRS.AudioPlayer;

var loggerFactory = LoggerFactory.Create(
    static builder =>
    {
        builder.AddConsole();
    }
);

var library = new AudioLibrary("..\\..\\signalling-media\\", loggerFactory.CreateLogger<AudioLibrary>());


var logger = loggerFactory.CreateLogger<Program>();

// See https://aka.ms/new-console-template for more information
logger.LogInformation("Hello, World!");

while (true)
{
    var _inputReader = library.GetNextAudioOfType("remainseated");
    var stereo = new MonoToStereoSampleProvider(_inputReader);
    stereo.LeftVolume = 0.0f;
    stereo.RightVolume = 1.0f;

    var _waveOut = new WaveOutEvent();
    _waveOut.Init(stereo);
    _waveOut.Play();
    while (_waveOut.PlaybackState == PlaybackState.Playing)
    {
    }
}