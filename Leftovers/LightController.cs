using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Leftovers
{
    public class LightController
    {
        private int blinkTime = 10000;
        private readonly Dictionary<Rooms, GpioPin> _pins = new Dictionary<Rooms, GpioPin>();
        private GpioController _gpioController;

        public LightController()
        {
            _gpioController = GpioController.GetDefault();

            InitPin(Rooms.Downtown, 2);
            InitPin(Rooms.SevenKitchen, 3);
            InitPin(Rooms.SixKitchen, 4);
            InitPin(Rooms.ForrestPark, 5);
            InitPin(Rooms.Stadium, 6);
            InitPin(Rooms.FiveKitchen, 7);
            InitPin(Rooms.Beacon, 9);
            InitPin(Rooms.Status, 21);
        }

        public void Blink(Rooms room)
        {
            _pins[room].Write(GpioPinValue.Low);
            _pins[Rooms.Beacon].Write(GpioPinValue.Low);
            Task.Delay(blinkTime)
                .ContinueWith(task =>
                {
                    _pins[room].Write(GpioPinValue.High);
                    _pins[Rooms.Beacon].Write(GpioPinValue.High);
                });
        }

        private void InitPin(Rooms room, int pinNum)
        {
            var pin = _gpioController.OpenPin(pinNum);
            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            _pins.Add(room, pin);
        }
    }
}