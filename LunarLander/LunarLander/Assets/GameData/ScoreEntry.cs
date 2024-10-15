using System.Runtime.Serialization;

namespace LunarLander.Assets.GameData
{
    // This is the data container to hold an entry in the high scores list.
    [DataContract(Name = "ScoreEntry")]
    public class ScoreEntry
    {
        // Constructor.
        public ScoreEntry(int score, string name)
        {
            this.score = score;
            this.name = name;
        }

        // The score.
        [DataMember()]
        public int score;

        // Player name.
        [DataMember()]
        public string name;
    }
}
