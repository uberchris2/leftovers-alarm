using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Leftovers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            InitGpio();

            Task.Run(async () =>
            {
                await WatchEmail();
            }).GetAwaiter().GetResult();
        }

        private async Task WatchEmail()
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            service.TraceEnabled = true;
            service.Credentials = new WebCredentials("carpench", "SOMEPASSWORD", "WWTHC");
            service.AutodiscoverUrl("chris.carpenter@wwt.com", url => true);
            //            service.Url = new Uri("https://mobile.wwt.com/ews/exchange.asmx");

            var streamingSubscription = service.SubscribeToStreamingNotificationsOnAllFolders(EventType.NewMail);
            var streamingConnection = new StreamingSubscriptionConnection(service, 30);
            streamingConnection.AddSubscription(await streamingSubscription);
            streamingConnection.OnNotificationEvent += async (sender, notification) =>
            {
                foreach (var notificationEvent in notification.Events)
                {
                    var item = (ItemEvent)notificationEvent;
                    var message = await EmailMessage.Bind(service, item.ItemId);

                    var fAddress = message.From.Address;
                    var subject = message.Subject;
                    var body = Regex.Replace(message.Body.Text, "<.*?>", string.Empty);

                    if (subject.ToLower().Contains("leftover"))
                    {
                        _gpioPin[Rooms.Beacon].Write(GpioPinValue.High);
                        Thread.Sleep(3000);
                        _gpioPin[Rooms.Beacon].Write(GpioPinValue.Low);
                    }
                }
            };
            streamingConnection.OnDisconnect += (sender, eventArgs) => streamingConnection.Open();
            streamingConnection.Open();

            while(true) Thread.Sleep(60000);
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();
            foreach (var ledPin in _pinNumbers)
            {
                var pin = gpio.OpenPin(ledPin.Value);
                var pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                _gpioPin.Add(ledPin.Key, pin);
            }
        }

        readonly Dictionary<Rooms, int> _pinNumbers = new Dictionary<Rooms, int>
        {
            {Rooms.Downtown, 2},
            {Rooms.SevenKitchen, 3},
            {Rooms.SixKitchen, 4},
            {Rooms.ForrestPark, 5},
            {Rooms.Stadium, 6},
            {Rooms.FiveKitchen, 7},
            {Rooms.Beacon, 9},
        };

        readonly Dictionary<Rooms, GpioPin> _gpioPin = new Dictionary<Rooms, GpioPin>();

        private enum Rooms { Downtown, SevenKitchen, SixKitchen, ForrestPark, Stadium, FiveKitchen,  Beacon };
    }
}
