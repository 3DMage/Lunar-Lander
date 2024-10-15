using System.Threading.Tasks;

namespace LunarLander.DataManagement
{
    // Base class for DataIO_BasePlugin objects.
    public abstract class DataIO_BasePlugin
    {
        // Flag indicating if saving is currently occurring.
        public bool isSaving = false;

        // Flag indicating if loading is currently occurring.
        public bool isLoading = false;

        // Abstract method for saving.
        protected abstract void Save();

        // Abstract method for loading.
        protected abstract void Load();

        // Abstract method for saving during async operation in SaveAsync().
        protected abstract void SaveAsyncOperation();

        // Abstract method for loading during async operation in LoadAsync().
        protected abstract void LoadAsyncOperation();

        // Called to ensure no blocking occurs on main thread when saving.
        protected async Task SaveAsync()
        {
            await Task.Run(() =>
            {
                SaveAsyncOperation();

                isSaving = false;
            });
        }

        // Called to ensure no blocking occurs on main thread when loading.
        protected async Task LoadAsync()
        {
            await Task.Run(() =>
            {
                LoadAsyncOperation();

                isLoading = false;
            });
        }
    }
}