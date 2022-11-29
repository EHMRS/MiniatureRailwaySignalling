using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace MRS.AudioPlayer
{
    public class AudioLibrary
    {
        private Dictionary<string, List<AudioFileReader>> AudioFiles { get; set; }
        private Dictionary<string, int> AudioPointer { get; set; }

        private readonly ILogger _logger;

        public AudioLibrary(string filePath, ILogger logger)
        {
            AudioFiles = new Dictionary<string, List<AudioFileReader>>();
            AudioPointer = new Dictionary<string, int>();
            _logger = logger;
            var dirinfo = new DirectoryInfo(filePath);
            foreach (var i in dirinfo.GetDirectories())
            {
                _logger.LogDebug($"Scanning folder {i.Name} for audio files");
                var innerDirInfo = new DirectoryInfo($"{filePath}{Path.DirectorySeparatorChar}{i.Name}");
                var files = innerDirInfo.GetFiles("*.wav");
                if (files.Length == 0)
                    continue;

                var list = new List<AudioFileReader>();

                foreach (var j in files)
                {
                    list.Add(new AudioFileReader($"{filePath}{Path.DirectorySeparatorChar}{i.Name}{Path.DirectorySeparatorChar}{j.Name}"));
                    _logger.LogDebug($"Found file: {i.Name}{Path.DirectorySeparatorChar}{j.Name}");
                }

                AudioFiles.Add(i.Name, list);
                AudioPointer.Add(i.Name, 0);
            }
        }

        public AudioFileReader? GetNextAudioOfType(string type)
        {
            _logger.LogDebug($"Getting next audio of type {type}");
            if (!AudioFiles.TryGetValue(type, out var list))
            {
                _logger.LogDebug($"Could not find audio type {type}");
                return null;
            }

            _logger.LogDebug($"Returning index {AudioPointer[type]} of {type}");
            var returnValue = list[AudioPointer[type]];
            returnValue.Seek(0, SeekOrigin.Begin);
            AudioPointer[type] = (AudioPointer[type] + 1) % AudioFiles[type].Count;
            return returnValue;
        }
    }
}
