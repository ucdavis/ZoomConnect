using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace SecretJsonConfig
{
    public class SecretConfigManager<TSecret> where TSecret : new()
    {
        private IFileInfo _file;
        private TSecret _secret;
        private JsonSerializerOptions _jsonOptions;
        private Crypt _crypt;

        public SecretConfigManager(Crypt crypt)
        {
            _crypt = crypt;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            _jsonOptions.Converters.Add(new SecretStructConverter(_jsonOptions, _crypt));
        }

        public SecretConfigManager(IFileInfo file)
        {
            _file = file;

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            _jsonOptions.Converters.Add(new SecretStructConverter(_jsonOptions, _crypt));
        }

        public IFileInfo SecretFile
        {
            set
            {
                if (_file != null) { throw new Exception("File already set."); }

                _file = value;
            }
        }

        public async Task<TSecret> GetValue()
        {
            if (_secret == null)
            {
                await Load();
            }

            return _secret;
        }

        public async Task Load()
        {
            if (_file == null) { throw new Exception("No file provided."); }

            if (_file.Exists)
            {
                using (FileStream fs = File.OpenRead(_file.PhysicalPath))
                {
                    _secret = await JsonSerializer.DeserializeAsync<TSecret>(fs, _jsonOptions);
                }
            }
            else
            {
                _secret = new TSecret();
            }
        }

        public async void Save()
        {
            using (FileStream fs = new FileStream(_file.PhysicalPath, FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync(fs, _secret, _jsonOptions);
            }
        }
    }
}
