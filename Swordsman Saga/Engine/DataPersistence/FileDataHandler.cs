using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.DataPersistence
{
    internal class FileDataHandler
    {
        private string mDataDirPath = "";
        private string mDataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            mDataDirPath = dataDirPath;
            mDataFileName = dataFileName;
        }

        public Dictionary<string, GameData> LoadAllProfiles()
        {
            Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

            foreach (DirectoryInfo dirInfo in new DirectoryInfo(mDataDirPath).EnumerateDirectories())
            {
                string profileId = dirInfo.Name;
                string fullPath = Path.Combine(mDataDirPath, profileId, mDataFileName);

                if (!File.Exists(fullPath))
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("Skipping directory when loading all profiles because it does not contain data: " + profileId);
                    }
                    continue;
                }

                GameData profileData = Load(profileId);
                profileDictionary.Add(profileId, profileData);
            }

            return profileDictionary;
        }

        public GameData Load(string profileId)
        {
            string fullPath = Path.Combine(mDataDirPath, profileId, mDataFileName);
            GameData loadedData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = File.ReadAllText(fullPath);

                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new EventConverter() }
                    };

                    loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad, settings);
                }
                catch (Exception e)
                {
                    // Consider logging the error
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
                    }
                }
            }

            return loadedData;
        }

        public void Save(GameData gameData, string profileId)
        {
            string fullPath = Path.Combine(mDataDirPath, profileId, mDataFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                if (gameData != null)
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new EventConverter() },
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Add this line
                    };

                    string dataToStore = JsonConvert.SerializeObject(gameData, typeof(GameData), settings);
                    File.WriteAllText(fullPath, dataToStore);
                }
            }
            catch (Exception e)
            {
                // Consider logging the error
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
                }
            }
        }

        public void Save<T>(T data)
        {
            string fullPath = Path.Combine(mDataDirPath, typeof(T).ToString(), mDataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = { new EventConverter() },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Add this line
                };

                string dataToStore = JsonConvert.SerializeObject(data, typeof(T), settings);
                File.WriteAllText(fullPath, dataToStore);
            }
            catch (Exception e)
            {
                // Consider logging the error
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
                }
            }
        }

        public T Load<T>()
        {
            string fullPath = Path.Combine(mDataDirPath, typeof(T).ToString(),mDataFileName);
            T loadedData = default(T);

            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = File.ReadAllText(fullPath);

                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new EventConverter() }
                    };
                    loadedData = JsonConvert.DeserializeObject<T>(dataToLoad, settings);
                }
                catch (Exception e)
                {
                    // Consider logging the error
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
                    }
                }

            }
            return loadedData;
        }
    }
}
