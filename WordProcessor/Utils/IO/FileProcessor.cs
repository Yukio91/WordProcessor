using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WordProcessor.Utils.Logger;

namespace WordProcessor.Utils.IO
{
    public abstract class ProcessorBase
    {
        protected readonly ILogger Logger;

        protected ProcessorBase(ILogger logger = null)
        {
            Logger = logger;
        }

        public abstract Dictionary<string, int> Process(string filename);
    }

    public class FileProcessor : ProcessorBase
    {
        private FileProcessorSettings _settings;

        public FileProcessor(FileProcessorSettings settings = null, ILogger logger = null) : base(logger)
        {
            _settings = settings;
        }

        public override Dictionary<string, int> Process(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (!File.Exists(filename))
                throw new FileNotFoundException("Файл не найден!", filename);

            if (_settings == null)
                _settings = new FileProcessorSettings();

            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream, _settings.FileEncoding))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (String.IsNullOrEmpty(line.Trim()))
                            continue;

                        var words = line.ToLower().Split(new[] {" ", ",", "."}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(word => word).Where(w =>
                                !w.All(Char.IsDigit) && w.Length >= _settings.MinWordLength &&
                                w.Length <= _settings.MaxWordLength).GroupBy(w => w)
                            .ToDictionary(w => w.Key, w => w.Count());

                        foreach (var word in words)
                        {
                            int freq = -1;
                            if (dictionary.TryGetValue(word.Key, out freq))
                                dictionary[word.Key] += word.Value;
                            else dictionary.Add(word.Key, word.Value);
                        }
                    }
                }
            }

            return dictionary.Where(kvp => kvp.Value >= _settings.MinFrequency)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }

    public class FileProcessorSettings
    {
        public int MinFrequency { get; } = 3;
        public int MinWordLength { get; } = 3;
        public int MaxWordLength { get; } = 15;
        public Encoding FileEncoding { get; } = Encoding.UTF8;
    }
}
