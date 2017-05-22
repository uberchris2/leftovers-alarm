namespace Leftovers
{
    public sealed class MessageHelper
    {
        public static bool IsLeftovers(string text)
        {
            return text.Contains("leftover");
        }

        public static Rooms PickRoom(string text)
        {
            if (text.Contains("downtown")) return Rooms.Downtown;
            if (text.Contains("stadium")) return Rooms.Stadium;
            if ((text.Contains("5") || text.Contains("five")) && text.Contains("kitchen")) return Rooms.FiveKitchen;
            if (text.Contains("forrest")) return Rooms.ForrestPark;
            if ((text.Contains("7") || text.Contains("seven")) && text.Contains("kitchen")) return Rooms.SevenKitchen;
            if ((text.Contains("6") || text.Contains("six")) && text.Contains("kitchen")) return Rooms.SixKitchen;

            return Rooms.Beacon;
        }
    }
}