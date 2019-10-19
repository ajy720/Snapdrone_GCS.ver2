using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;

using System.Threading.Tasks;
using System.Threading;

using DJI.WindowsSDK;
using DJI.WindowsSDK.Mission.Waypoint;

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
        
        public String Latitude() => this._latitude;
        public void Latitude(String latitude) => this._latitude = latitude;
       
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
        
        public Location Location() => this._location;
        public void Location(Location location) => this._location = location;
        
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
   
    public class Joystick
    {
        [JsonProperty("throttle")]
        private float _throttle;
        [JsonProperty("roll")]
        private float _roll;
        [JsonProperty("pitch")]
        private float _pitch;
        [JsonProperty("yaw")]
        private float _yaw;

        public Joystick()
        {
            //Default Constructor
        }

        public Joystick(float t, float r, float p, float y)
        {
            _throttle = t;
            _roll = r;
            _pitch = p;
            _yaw = y;
        }

        public float Throt() => this._throttle;
        public float Roll() => this._roll;
        public float Pitch() => this._pitch;
        public float Yaw() => this._yaw;
    }

    public class UserId
    {
        [JsonProperty("ID")]
        private string _id;

        public UserId()
        {
            //Default Constructor
        }

        public string ID() => this._id;
        public void ID(string id) => this._id = id;
    }

    public sealed partial class MainPage : Page
    {
        DroneData DD = new DroneData(80, "127", "20", "5");
        UserData UD = new UserData();
        Joystick JS = new Joystick();
        UserId UI = new UserId();
        Socket socket;
        WaypointMission WaypointMission;

        public MainPage()
        {
            this.InitializeComponent();
            DJISDKManager.Instance.SDKRegistrationStateChanged += Instance_SDKRegistrationEvent;
            DJISDKManager.Instance.RegisterApp("f93e5575d53bf45b03cd6d64");
            socket = IO.Socket("https://api.teamhapco.com/");
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

        private async void UI_output(TextBlock tb, string text)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => tb.Text = text);
        }

        private void Socket_init(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Socket Connecting...");
            UI_output(init_status, "Connect Success");

            string JsonString = JsonConvert.SerializeObject(DD);
            socket.Emit("init_gcs", JsonString);
            socket.On("client_gps", (data) =>
            {
                Location location = JsonConvert.DeserializeObject<Location>(data.ToString());
                UD.SetLocation(location.Latitude(), location.Longitude());

                Debug.WriteLine("User Lat : " + UD.Location().Latitude());
                Debug.WriteLine("User Long : " + UD.Location().Longitude());
                UI_output(Client_latitude, UD.Location().Latitude());
                UI_output(Client_longitude, UD.Location().Longitude());

            });

            socket.On("drone_call", () =>
            {
                UI_output(call_status, "'drone_call' Request");
                Debug.WriteLine("Drone Call Request.");

                StartTakeOff();
            });

            socket.On("return_to_home", () =>
            {
                UI_output(rth_status, "'return_to_home' Request");
                Debug.WriteLine("RTH Request.");

                StartReturnToHome();
            });

            socket.On("take_picture", () =>
            {
                UI_output(photo_status, "'take_picture' Request");
                Debug.WriteLine("Take Picture Request.");

                Get_Photo();
            });

            socket.On("drone_control", (data) =>
            {
                Debug.WriteLine("Drone_Control Request.");

                Set_Joystick(data.ToString());
            });

            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                Debug.WriteLine("Socket Disconnect.");
                JsonString = JsonConvert.SerializeObject(DD);
                socket.Emit("init_gcs", JsonString);
            });

            socket.On("init_gcs_success", (data) =>
            {
                Debug.WriteLine("init GCS Success");
                UI = JsonConvert.DeserializeObject<UserId>(data.ToString());
                Debug.WriteLine("ID : " + UI.ID());
            });

            socket.On("init_client_success", (data) =>
            {
                Debug.WriteLine("Init Client Success");
                UI = JsonConvert.DeserializeObject<UserId>(data.ToString());
                Debug.WriteLine("ID : " + UI.ID());
            });
        }

        private void Set_Joystick(string data)
        { 
            JS = JsonConvert.DeserializeObject<Joystick>(data);
            Debug.WriteLine("Throttle : " + JS.Throt() + ", Roll : " + JS.Roll() + ", Pitch : " + JS.Pitch() + ", Yaw : " + JS.Yaw());
            DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(JS.Throt(), JS.Roll(), JS.Pitch(), JS.Yaw());
        }

        private async void StartTakeOff()
        {
            SDKError err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).StartTakeoffAsync();
            Debug.WriteLine("Takeoff err : " + err.ToString());
            UI_output(call_status, "Takeoff err : " + err.ToString());
            /*
            if (err == SDKError.NO_ERROR || err == SDKError.REQUEST_TIMEOUT)
            {
                DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(1, 0, 0, 0);
                Thread.Sleep(5000);
                DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(0, 0, 1, 0);
                Thread.Sleep(2500);
                DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(0, 0, 0, 0);
            }*/
        }

        private async void StartReturnToHome()
        {  
            SDKError err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).StartGoHomeAsync();
            Debug.WriteLine("RTH err : " + err.ToString());
            UI_output(rth_status, "RTH err : " + err.ToString());
            DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).GoHomeStateChanged += Check_RTHsequence;

            var RTHstate = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).GetGoHomeStateAsync();
            Debug.WriteLine("RTH sequence : " + RTHstate.value?.value.ToString());
            if (RTHstate.value?.value == FCGoHomeState.COMPLETED)
            {
                DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AltitudeChanged += RTH_Landing;
                DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).GoHomeStateChanged -= Check_RTHsequence;
            }
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
            UI_output(photo_status, "Take Picture err : " + error.ToString());
        }

        private async void Get_Aircraft(object sender, RoutedEventArgs e)
        {
            var model = (await DJISDKManager.Instance.ComponentManager.GetProductHandler(0).GetProductTypeAsync());

            if (model.value == null)
            {
                UI_output(aircraft, model.error.ToString());
                Debug.WriteLine("Get Aircraft err : " + model.error.ToString());
            }
            else
            {
                string modelstring = model.value?.value.ToString();
                UI_output(aircraft, modelstring);
                Debug.WriteLine("Aircraft : " + modelstring);
            }
        }

        private void Get_Aircraft(object sender, ProductTypeMsg? model)
        {
            string modelstring = model?.value.ToString();
            if (modelstring == null) modelstring = "NULL";
            UI_output(aircraft, modelstring);
            Debug.WriteLine("Aircraft : " + modelstring);
        }

        private async void Get_Battery(object sender, RoutedEventArgs e)
        {
            var bat = await DJISDKManager.Instance.ComponentManager.GetBatteryHandler(0, 0).GetChargeRemainingInPercentAsync();

            if (bat.value == null)
            {
                UI_output(battery, bat.error.ToString());
                Debug.WriteLine("Get Battery err : " + bat.error.ToString());
            }
            else
            {
                string batstring = bat.value?.value.ToString();
                UI_output(battery, batstring);
                Debug.WriteLine("Battery : " + batstring);
            }
        }

        private void Get_Battery(object sender, IntMsg? bat)
        {
            string batstring = bat?.value.ToString();
            if (batstring == null) batstring = "NULL";
            UI_output(battery, batstring);
            Debug.WriteLine("Battery : " + batstring);
        }

        private async void Get_Location(object sender, RoutedEventArgs e)
        {
            var location = (await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).GetAircraftLocationAsync());

            if (location.value == null)
            {
                UI_output(aircraft, location.error.ToString());
                Debug.WriteLine("Get Loaction err : " + location.error.ToString());
            }
            else
            {
                string latitude = location.value?.latitude.ToString();
                string longitude = location.value?.longitude.ToString();
                UI_output(Drone_latitude, latitude);
                UI_output(Drone_longitude, longitude);
                Debug.WriteLine("Drone : {Lat : " + latitude + ", Long : " + longitude + "}");
            }
        }

        private void Get_Location(object sender, LocationCoordinate2D? location)
        {
            string latitude = location?.latitude.ToString();
            string longitude = location?.longitude.ToString();
            if (latitude == null) latitude = "NULL";
            if (longitude == null) longitude = "NULL";
            UI_output(Drone_latitude, latitude);
            UI_output(Drone_longitude, longitude);
            Debug.WriteLine("Drone : {Lat : " + latitude + ", Long : " + longitude + "}");
        }

        private void Get_Altitude(object sender, DoubleMsg? altitude)
        {
            string altstring = altitude?.value.ToString();
            DD.Altitude(altstring);
            if (altstring == null) altstring = "NULL";
            UI_output(Drone_altitude, altstring);
            Debug.WriteLine("Latitude : " + altstring);
        }

        private void startLocationAsync(object sender, RoutedEventArgs e) => DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AircraftLocationChanged += Get_Location;

        private void stopLocationAsync(object sender, RoutedEventArgs e) => DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AircraftLocationChanged -= Get_Location;

        private void Throttle_Up(object sender, RoutedEventArgs e)
        {
            DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(1, 0, 0, 0);
        }

        private void Throttle_Down(object sender, RoutedEventArgs e)
        {
            DJISDKManager.Instance.VirtualRemoteController.UpdateJoystickValue(-1, 0, 0, 0);
        }

        private async void Set_GroundStationMode(object sender, RoutedEventArgs e)
        {
            BoolMsg yn = new BoolMsg();
            yn.value = true;
            var err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).SetGroundStationModeEnabledAsync(yn);
            Debug.WriteLine(err.ToString());
            DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).StateChanged += Waypoint_Mission_State;
        }

        private void Init_Mission(object sender, RoutedEventArgs e)
        {
            double nowLat = Convert.ToDouble(DD.Location().Latitude());
            double nowLng = Convert.ToDouble(DD.Location().Longitude());

            WaypointMission = new WaypointMission()
            {
                waypointCount = 4,
                maxFlightSpeed = 15,
                autoFlightSpeed = 10,
                finishedAction = WaypointMissionFinishedAction.NO_ACTION,
                headingMode = WaypointMissionHeadingMode.AUTO,
                flightPathMode = WaypointMissionFlightPathMode.NORMAL,
                gotoFirstWaypointMode = WaypointMissionGotoFirstWaypointMode.SAFELY,
                exitMissionOnRCSignalLostEnabled = false,
                pointOfInterest = new LocationCoordinate2D()
                {
                    latitude = 0,
                    longitude = 0
                },
                gimbalPitchRotationEnabled = true,
                repeatTimes = 0,
                missionID = 0,
                waypoints = new List<Waypoint>()
                            {
                                InitDumpWaypoint(nowLat+0.001, nowLng+0.0015),
                                InitDumpWaypoint(nowLat+0.001, nowLng-0.0015),
                                InitDumpWaypoint(nowLat-0.001, nowLng-0.0015),
                                InitDumpWaypoint(nowLat-0.001, nowLng+0.0015),
                            }
            };
        }

        private void Load_Mission(object sender, RoutedEventArgs e)
        {
            var state = DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).GetCurrentState();
            Debug.WriteLine(state.ToString());
            //WaypointMissionState.Text = state.ToString();
            SDKError err = DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).LoadMission(WaypointMission);

            Debug.WriteLine("SDK load mission : " + err.ToString());
            //LoadMissionError.Text = "SDK load mission : " + err.ToString();
        }

        private void Get_Loaded_Mission(object sender, RoutedEventArgs e)
        {
            var WaypointMission = DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).GetLoadedMission();

            //if (err != null); //WaypointMission = DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).GetLoadedMission().Value;
            //else System.Diagnostics.Debug.WriteLine("get load null");
        }

        private async void Upload_Mission(object sender, RoutedEventArgs e)
        {
            //DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).GetLoadedMission();
            SDKError err = await DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).UploadMission();
            Debug.WriteLine("Upload mission to aircraft : " + err.ToString());
            DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).UploadStateChanged += Upload_State;
            //UploadMissionError.Text = "Upload mission to aircraft : " + err.ToString();
        }

        private void Upload_State(object sender, WaypointMissionUploadState? state)
        {
            Debug.WriteLine("Upload State : " + JsonConvert.SerializeObject(state.Value));
        }

        private async void Retry_Upload_Mission(object sender, RoutedEventArgs e)
        {
            var err = await DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).RetryUploadMission();
            Debug.WriteLine("RetryUpload err : " + err.ToString());
        }

            private void Waypoint_Mission_State(object sender, WaypointMissionStateTransition? state)
        {
            Debug.WriteLine("Current State : " + state.Value.current.ToString());
            Debug.WriteLine("Previous State : " + state.Value.previous.ToString());
        }

        private async void Start_Mission(object sender, RoutedEventArgs e)
        {
            var err = await DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).StartMission();
            Debug.WriteLine("Start mission : " + err.ToString());
            //ExecuteMissionError.Text = "Start mission : " + err.ToString();
        }

        private Waypoint InitDumpWaypoint(double latitude, double longitude)
        {
            Waypoint waypoint = new Waypoint()
            {
                location = new LocationCoordinate2D() { latitude = latitude, longitude = longitude },
                altitude = 20,
                turnMode = WaypointTurnMode.CLOCKWISE,
                heading = 0,
                gimbalPitch = -30,
                actionRepeatTimes = 1,
                actionTimeoutInSeconds = 60,
                cornerRadiusInMeters = 0.2,
                speed = 0,
                shootPhotoTimeInterval = -1,
                shootPhotoDistanceInterval = -1,
                waypointActions = new List<WaypointAction>()
                {
                    InitDumpWaypointAction(1000, WaypointActionType.STAY),
                }
            };
            return waypoint;
        }

        private WaypointAction InitDumpWaypointAction(int Param, WaypointActionType actiontype)
        {
            WaypointAction action = new WaypointAction()
            {
                actionType = actiontype,
                actionParam = Param
            };
            return action;
        }


    }
}
