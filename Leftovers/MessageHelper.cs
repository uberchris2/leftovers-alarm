using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;

namespace Leftovers
{
    public sealed class MessageHelper
    {
        public static bool IsLeftovers(EmailMessage message)
        {
            return message.Subject.ToLower().Contains("leftover");
        }

        public static Rooms PickRoom(EmailMessage message)
        {
            var body = Regex.Replace(message.Body.Text, "<.*?>", string.Empty).ToLower();

            if (body.Contains("downtown")) return Rooms.Downtown;
            if (body.Contains("stadium")) return Rooms.Stadium;
            if ((body.Contains("5") || body.Contains("five")) && body.Contains("kitchen")) return Rooms.FiveKitchen;
            if (body.Contains("forrest")) return Rooms.ForrestPark;
            if ((body.Contains("7") || body.Contains("seven")) && body.Contains("kitchen")) return Rooms.SevenKitchen;
            if ((body.Contains("6") || body.Contains("six")) && body.Contains("kitchen")) return Rooms.SixKitchen;

            return Rooms.Beacon;
        }
    }
}