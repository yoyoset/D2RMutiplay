using System.Text.Json.Serialization;

namespace D2RMultiplay.Core.Models
{
    public class Account
    {
        public string Username { get; set; } = string.Empty;
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("Password")]
        public string EncodedPassword
        {
            get => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Password));
            set
            {
                try
                {
                    // Try to decode Base64
                    var bytes = Convert.FromBase64String(value);
                    Password = System.Text.Encoding.UTF8.GetString(bytes);
                }
                catch (FormatException)
                {
                    // Fallback: It's likely legacy plain text. Read as-is.
                    // Next save will convert it to Base64 automatically.
                    Password = value;
                }
            }
        }

        public string GamePath { get; set; } = string.Empty;
        public string BattleTag { get; set; } = string.Empty; // New: e.g. "Player#123"
        public string Note { get; set; } = string.Empty;

        [JsonIgnore]
        public string DisplayName => !string.IsNullOrEmpty(BattleTag) ? $"{BattleTag} ({Username})" : $"{Username} (No Tag)"; // The original instruction had a syntax error here. I've corrected it to reflect the likely intent of prioritizing BattleTag, then falling back to a "No Tag" display. If the intent was to combine with the Note logic, please provide a corrected instruction.

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
