using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Json;
using LunarLander.Utilities;
using LunarLander.Assets.GameData;

namespace LunarLander.DataManagement
{
    // IO management for persistent storage of high score data.
    public class ScoreData_IO : DataIO_BasePlugin
    {
        // Loaded instance of HighScoresList object.
        public HighScoresList highScoreList_Loaded { get; private set; } = null;

        // File name of where to save and load HighScoresList data.
        private string scoreListFileName = "scores.json";

        // Loads the high scores list.
        public void LoadScoresList()
        {
            Load();
        }

        // Saves the high scores list.
        public void SaveScoresList()
        {
            Save();
        }

        // Loads the HighScoresList.
        protected override void Load()
        {
            lock (this)
            {
                if (!isLoading)
                {
                    isLoading = true;
                    var result = LoadAsync();
                    result.Wait();
                }
            }
        }

        // Saves the HighScoresList.
        protected override void Save()
        {
            lock (this)
            {
                if (!isSaving)
                {
                    isSaving = true;
                    SaveAsync();
                }
            }
        }

        // Async operation for loading.
        protected override void LoadAsyncOperation()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    if (storage.FileExists(scoreListFileName))
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile(scoreListFileName, FileMode.Open))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(HighScoresList));
                                highScoreList_Loaded = (HighScoresList)mySerializer.ReadObject(fs);
                                Managers.gameDataManager.highScoresList = highScoreList_Loaded;
                            }
                        }
                    }
                }
                catch (IsolatedStorageException)
                {
                }
            }
        }

        // Async operation for saving.
        protected override void SaveAsyncOperation()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (IsolatedStorageFileStream fs = storage.OpenFile(scoreListFileName, FileMode.Create))
                    {
                        if (fs != null)
                        {
                            DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(HighScoresList));
                            mySerializer.WriteObject(fs, Managers.gameDataManager.highScoresList);
                        }
                    }
                }
                catch (IsolatedStorageException)
                {
                }
            }
        }
    }
}