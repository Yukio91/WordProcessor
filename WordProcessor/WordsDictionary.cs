using System.Collections.Generic;
using WordProcessor.Db;
using WordProcessor.Utils.IO;
using WordProcessor.Utils.Logger;

namespace WordProcessor
{
    internal class WordsDictionary
    {
        private readonly ILogger _logger;
        private readonly FileProcessor _processor;
        private readonly DbManager _dbmanager;

        public WordsDictionary(ILogger logger = null)
        {
            _logger = logger;

            _processor = new FileProcessor();
            _dbmanager = new DbManager();
        }

        public bool Create(string filename)
        {
            _dbmanager.CreateDatabase();

            Update(filename);

            return true;
        }

        public bool Update(string filename)
        {
            var words = _processor.Process(filename);
            _dbmanager.Update(words);

            return true;
        }

        public bool Clear()
        {
            _dbmanager.Clear();

            return true;
        }

        public IEnumerable<string> Find(string prefix)
        {
            return _dbmanager.FindWordPrefix(prefix.ToLower(), 5);
        }
    }
}