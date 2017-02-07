namespace Schedule.ChatBot.Models
{
    public class LuisResponse
    {
        public string query { get; set; }
        public LuisIntent[] intents { get; set; }
        public LuisEntity[] entities { get; set; }
    }
}