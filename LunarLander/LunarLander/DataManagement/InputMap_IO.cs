using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Json;
using LunarLander.Utilities;
using LunarLander.Input;

namespace LunarLander.DataManagement
{
    // IO management for persistent storage of Input Config.
    public class InputMap_IO : DataIO_BasePlugin
    {
        // Loaded instance of InputMap object.
        public InputMap inputMap_Loaded { get; private set; } = null;

        // File name of where to save and load InputMap data.
        private string inputMapFileName = "inputmap.json";

        // Loads the input map.
        public void LoadInputMap()
        {
            Load();
        }

        // Saves the inptu map.
        public void SaveInputMap()
        {
            Save();
        }

        // Loads the InputMap.
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

        // Saves the InputMap.
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
                    if (storage.FileExists(inputMapFileName))
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile(inputMapFileName, FileMode.Open))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(InputMap));
                                inputMap_Loaded = (InputMap)mySerializer.ReadObject(fs);
                                Managers.inputManager.inputMap = inputMap_Loaded;
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
                    using (IsolatedStorageFileStream fs = storage.OpenFile(inputMapFileName, FileMode.Create))
                    {
                        if (fs != null)
                        {
                            DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(InputMap));
                            mySerializer.WriteObject(fs, Managers.inputManager.inputMap);
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