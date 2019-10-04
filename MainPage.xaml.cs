using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;

using System.Threading.Tasks;
using System.Threading;

using DJI.WindowsSDK;

using Quobject.EngineIoClientDotNet.Client;
using Quobject.EngineIoClientDotNet.Client.Transports;
using Quobject.EngineIoClientDotNet.ComponentEmitter;
using Quobject.EngineIoClientDotNet.Modules;
using Quobject.EngineIoClientDotNet.Parser;
using Quobject.SocketIoClientDotNet.Client;

using Socket = Quobject.SocketIoClientDotNet.Client.Socket;

using Newtonsoft.Json;

using System.Diagnostics;


// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace Snapdrone_GCS
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public class Location
    {
        [JsonProperty("latitude")]
        private String _latitude;//위도 (-90~90)
        [JsonProperty("longitude")]
        private String _longitude; //경도 (-180~180)

        public String Longitude() => this._longitude;
        public void Longitude(String longitude) => this._longitude = longitude;
        //public String Longitude
        //{
        //    get => _longitude;
        //    set => _longitude = value;
        //}

        public String Latitude() => this._latitude;
        public void Latitude(String latitude) => this._latitude = latitude;
        //public String Latitude
        //{
        //    get => _latitude;
        //    set => _latitude = value;
        //}

        // Constructor
        public Location(String latitude, String longitude)
        {
            Longitude(longitude);
            Latitude(latitude);
        }
    }

    public class DroneData
    {
        [JsonProperty("battery")]
        private int _battery;
        [JsonProperty("location")]
        private Location _location;
        [JsonProperty("altitude")]
        private String _altitude;

        public DroneData()
        {
            // Default Constructor
        }

        public DroneData(int battery, String latitude, String longitude, String altitude)
        {
            // Constructor
            _battery = battery;
            _location = new Location(latitude, longitude);
            _altitude = altitude;
        }

        public int Battery() => this._battery;
        public void Battery(int bat) => this._battery = bat;
        //public int Battery
        //{
        //    get => _battery;
        //    set => _battery = value;
        //}

        public Location Location() => this._location;
        public void Location(Location location) => this._location = location;
        //public Location Location
        //{
        //    get => _location;
        //    set => _location = value;
        //}

        public String Altitude() => this._altitude;
        public void Altitude(String altitude) => this._altitude = altitude;

        public void SetLocation(String latitude, String longitude)
        {
            _location.Longitude(longitude); // = longitude;
            _location.Latitude(latitude); // = latitude;
        }
    }

    public class UserData
    {
        [JsonProperty("location")]
        private Location _location;

        public UserData()
        {
            //Default Constructor
        }

        public UserData(String longitude, String latitude) => _location = new Location(latitude, longitude);

        public Location Location() => this._location;
        public void Location(Location location) => this._location = location;

        public void SetLocation(String latitude, String longitude)
        {
            if (_location == null)
            {
                _location = new Location(latitude, longitude);
            }
            else
            {
                _location.Longitude(longitude); // = longitude;
                _location.Latitude(latitude); // = latitude;
            }
        }
    }
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Socket socket = IO.Socket("https://api.teamhapco.com/");
        DroneData DD = new DroneData(80, "127", "20", "5");
        UserData UD = new UserData();

        public MainPage()
        {
            this.InitializeComponent();
            DJISDKManager.Instance.SDKRegistrationStateChanged += Instance_SDKRegistrationEvent;
            //DJISDKManager.Instance.ComponentManager.GetProductHandler(0).ProductTypeChanged += Get_Aircraft;
            //DJISDKManager.Instance.ComponentManager.GetBatteryHandler(0, 0).ChargeRemainingChanged += Get_Battery;
            //DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AircraftLocationChanged += Get_Location;
            //DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AltitudeChanged += Get_Location;
            //Replace with your registered App Key. Make sure your App Key matched your application's package name on DJI developer center.
            DJISDKManager.Instance.RegisterApp("f93e5575d53bf45b03cd6d64");
        }

        private async void Instance_SDKRegistrationEvent(SDKRegistrationState state, SDKError resultCode)
        {
            if (resultCode == SDKError.NO_ERROR)
            {
                Debug.WriteLine("Register app successfully.");
                DJISDKManager.Instance.ComponentManager.GetProductHandler(0).ProductTypeChanged += Get_Aircraft;
                DJISDKManager.Instance.ComponentManager.GetBatteryHandler(0, 0).ChargeRemainingInPercentChanged += Get_Battery;
                DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AltitudeChanged += Get_Altitude;

                //The product connection state will be updated when it changes here.
                DJISDKManager.Instance.ComponentManager.GetProductHandler(0).ProductTypeChanged += async delegate (object sender, ProductTypeMsg? value)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (value != null && value?.value != ProductType.UNRECOGNIZED)
                        {
                            Debug.WriteLine("The Aircraft is connected now.");
                            //You can load/display your pages according to the aircraft connection state here.
                        }
                        else
                        {
                            Debug.WriteLine("The Aircraft is disconnected now.");
                            //You can hide your pages according to the aircraft connection state here, or show the connection tips to the users.
                        }
                    });
                };

                //If you want to get the latest product connection state manually, you can use the following code
                var productType = (await DJISDKManager.Instance.ComponentManager.GetProductHandler(0).GetProductTypeAsync()).value;
                if (productType != null && productType?.value != ProductType.UNRECOGNIZED)
                {
                    Debug.WriteLine("The Aircraft is connected now.");
                    //You can load/display your pages according to the aircraft connection state here.
                }
            }
            else
            {
                Debug.WriteLine("Register SDK failed, the error is: ");
                Debug.WriteLine(resultCode.ToString());
            }
        }

        private void Socket_init(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Socket Connecting...");
            socket.On(Socket.EVENT_CONNECT, async () =>
            {
                string JsonString = JsonConvert.SerializeObject(DD);
                Debug.WriteLine("Connect Success");
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => init_status.Text = "Connect Success");

                socket.Emit("init_gcs", JsonString);

                socket.On("client_gps", async (data) =>
                {
                    Location location = JsonConvert.DeserializeObject<Location>(data.ToString());
                    UD.SetLocation(location.Latitude(), location.Longitude());
                    //Debug.WriteLine(data.ToString());

                    Debug.WriteLine("User Lat : " + UD.Location().Latitude());
                    Debug.WriteLine("User Long : " + UD.Location().Longitude());
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Client_latitude.Text = UD.Location().Latitude();
                        Client_latitude.Text = UD.Location().Longitude();
                    });

                });

                socket.On("drone_call", async (data) =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => call_status.Text = "'drone_call' Request");
                    Debug.WriteLine("Drone Call Request.");

                    StartTakeOff();

                });

                socket.On("return_to_home", async () =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => rth_status.Text = "'return_to_home' Request");
                    Debug.WriteLine("RTH Request.");

                    StartReturnToHome();
                });

                socket.On("take_picture", async () =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => photo_status.Text = "'take_picture' Request");
                    Debug.WriteLine("Take Picture Request.");

                    Get_Photo();
                });
            });
        }

        private async void StartTakeOff()
        {
            SDKError err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).StartTakeoffAsync();
            Debug.WriteLine("Takeoff err : " + err.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => call_status.Text = "Takeoff err : " + err.ToString());

            if (err == SDKError.NO_ERROR || err == SDKError.REQUEST_TIMEOUT)
            {
                DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(1, 0, 0, 0);
                Thread.Sleep(5000);
                DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(0, 0, 1, 0);
                Thread.Sleep(2500);
                DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(0, 0, 0, 0);
            }
        }

        private async void StartReturnToHome()
        {  
            SDKError err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).StartGoHomeAsync();
            Debug.WriteLine("RTH err : " + err.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => rth_status.Text = "RTH err : " + err.ToString());
            DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).GoHomeStateChanged += Check_RTHsequence;
        }

        private void Check_RTHsequence(object sender, FCGoHomeStateMsg? RTHstate)
        {
            Debug.WriteLine("RTH sequence : " + RTHstate?.value.ToString());
            if (RTHstate?.value == FCGoHomeState.COMPLETED)
            {
                DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AltitudeChanged += RTH_Landing;
                DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).GoHomeStateChanged -= Check_RTHsequence;
            }
        }

        private async void RTH_Landing(object sender, DoubleMsg? alt)
        {
            if (alt?.value <= 0.6)
            {
                Thread.Sleep(1000);
                var landingConfirmErr = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).ConfirmLandingAsync();
                Debug.WriteLine("Landing Confirm Err : " + landingConfirmErr.ToString());
                if(landingConfirmErr == SDKError.NO_ERROR)
                {
                    DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AltitudeChanged -= RTH_Landing;
                }
            }
        }

        private async void Get_Photo()
        {
            Debug.WriteLine((await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).GetSDCardRemainSpaceAsync()).value?.value.ToString() + " MB");

            PhotoStorageFormatMsg format;
            format.value = PhotoStorageFormat.JPEG;
            await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).SetPhotoStorageFormatAsync(format);

            CameraFocusModeMsg setFM;
            setFM.value = CameraFocusMode.AF;
            await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).SetCameraFocusModeAsync(setFM);

            CameraShootPhotoModeMsg setmode;
            setmode.value = CameraShootPhotoMode.NORMAL;
            await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).SetShootPhotoModeAsync(setmode);

            var error = (await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).StartShootPhotoAsync());
            Debug.WriteLine("Take Picture err : " + error.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => photo_status.Text = "Take Picture err : " + error.ToString());
        }

        private async void Get_Aircraft(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var model = (await DJISDKManager.Instance.ComponentManager.GetProductHandler(0).GetProductTypeAsync());

                if (model.value == null)
                {
                    aircraft.Text = model.error.ToString();
                    Debug.WriteLine("Get Aircraft err : " + model.error.ToString());
                }
                else
                {
                    string modelstring = model.value?.value.ToString();
                    aircraft.Text = modelstring;
                    Debug.WriteLine("Aircraft : " + modelstring);
                }
            });
        }

        private async void Get_Aircraft(object sender, ProductTypeMsg? model)
        {
            string modelstring = model?.value.ToString();
            if (modelstring == null) modelstring = "NULL";
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => aircraft.Text = modelstring);
            Debug.WriteLine("Aircraft : " + modelstring);
        }

        private async void Get_Battery(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var bat = await DJISDKManager.Instance.ComponentManager.GetBatteryHandler(0, 0).GetChargeRemainingInPercentAsync();

                if (bat.value == null)
                {
                    battery.Text = bat.error.ToString();
                    Debug.WriteLine("Get Battery err : " + bat.error.ToString());
                }
                else
                {
                    string batstring = bat.value?.value.ToString();
                    battery.Text = batstring;
                    Debug.WriteLine("Battery : " + batstring);
                }
            });
        }

        private async void Get_Battery(object sender, IntMsg? bat)
        {
            string batstring = bat?.value.ToString();
            if (batstring == null) batstring = "NULL";
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => battery.Text = batstring);
            Debug.WriteLine("Battery : " + batstring);
        }

        private async void Get_Location(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var location = (await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).GetAircraftLocationAsync());

                if (location.value == null)
                {
                    aircraft.Text = location.error.ToString();
                    Debug.WriteLine("Get Loaction err : " + location.error.ToString());
                }
                else
                {
                    string latitude = location.value?.latitude.ToString();
                    string longitude = location.value?.longitude.ToString();
                    Drone_latitude.Text = latitude;
                    Drone_longitude.Text = longitude;
                    Debug.WriteLine("Drone : {Lat : " + latitude + ", Long : " + longitude + "}");
                }
            });
        }

        private async void Get_Location(object sender, LocationCoordinate2D? location)
        {
            string latitude = location?.latitude.ToString();
            string longitude = location?.longitude.ToString();
            if (latitude == null) latitude = "NULL";
            if (longitude == null) longitude = "NULL";
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Drone_latitude.Text = latitude;
                Drone_longitude.Text = longitude;
            });
            Debug.WriteLine("Drone : {Lat : " + latitude + ", Long : " + longitude + "}");
        }

        private async void Get_Altitude(object sender, DoubleMsg? altitude)
        {
            string altstring = altitude?.value.ToString();
            DD.Altitude(altstring);
            if (altstring == null) altstring = "NULL";
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Drone_altitude.Text = altstring;
            });
            Debug.WriteLine("Latitude : " + altstring);
        }

        private void startLocationAsync(object sender, RoutedEventArgs e) => DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AircraftLocationChanged += Get_Location;

        private void stopLocationAsync(object sender, RoutedEventArgs e) => DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AircraftLocationChanged -= Get_Location;
    }
}
