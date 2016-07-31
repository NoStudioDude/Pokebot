using System.IO;
using System.Security.AccessControl;
using Newtonsoft.Json;
using PokemonGo.RocketAPI;

namespace PokeGoBot.WPF.Handlers
{
    public static class JsonSerialization
    {
        private static string ConfigFilePath => Configuration.ConfigFilePath();

        public static void WriteToJsonFile<T>(T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(ConfigFilePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                writer?.Close();
            }
        }
        
        public static T ReadFromJsonFile<T>() where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(ConfigFilePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
