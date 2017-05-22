using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;
using Microsoft.Exchange.WebServices.Data;

namespace Leftovers
{
    public sealed partial class MainPage : Page
    {
        private readonly LightController _lights;
        private ExchangeService _exchangeService;
        private StreamingSubscriptionConnection _streamingSubscriptionConnection;

        public MainPage()
        {
            InitializeComponent();
            _lights = new LightController();
            WatchEmail();
        }

        private async void WatchEmail()
        {
            _exchangeService = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
//            _exchangeService.TraceEnabled = true;
            _exchangeService.Credentials = new WebCredentials("carpench", "SOMEPASSWORD", "WWTHC");
            //            _exchangeService.AutodiscoverUrl("chris.carpenter@wwt.com", url => true);
            _exchangeService.Url = new Uri("https://mobile.wwt.com/ews/exchange.asmx");

            var streamingSubscription = _exchangeService.SubscribeToStreamingNotificationsOnAllFolders(EventType.NewMail);
            _streamingSubscriptionConnection = new StreamingSubscriptionConnection(_exchangeService, 30);
            _streamingSubscriptionConnection.AddSubscription(await streamingSubscription);
            _streamingSubscriptionConnection.OnNotificationEvent += EmailRecieved;
            _streamingSubscriptionConnection.OnDisconnect += (sender, eventArgs) => _streamingSubscriptionConnection.Open();
            _streamingSubscriptionConnection.Open();
        }

        private async void EmailRecieved(object sender, NotificationEventArgs notification)
        {
            foreach (var notificationEvent in notification.Events)
            {
                var item = (ItemEvent)notificationEvent;
                var message = await EmailMessage.Bind(_exchangeService, item.ItemId);
                var text = (message.Subject + Regex.Replace(message.Body.Text, "<.*?>", string.Empty)).ToLower();
                if (MessageHelper.IsLeftovers(text))
                {
                    var room = MessageHelper.PickRoom(text);
                    _lights.Blink(room);
                }
            }
        }
    }
}
