namespace LunarLander.Assets.GameData
{
    // Stores data for current round of gameplay.
    public class SessionData
    {
        // The current level index.
        public int currentLevel;

        // Current score achieved.
        public int score;

        // How many safe zones exist for current level.
        public int safeZoneCount;

        // Constructor.
        public SessionData()
        {
            score = 0;
            currentLevel = 1;
            safeZoneCount = 2;
        }

        // Resets session data to default data.
        public void ResetData()
        {
            score = 0;
            currentLevel = 1;
            safeZoneCount = 2;
        }
    }
}
