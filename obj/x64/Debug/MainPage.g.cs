﻿#pragma checksum "C:\Users\현스기\Desktop\Snapdrone_GCS.ver2\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0778CD94EC40967E1D531F329CC90833"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Snapdrone_GCS
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainPage.xaml line 13
                {
                    this.GetAircraft = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.GetAircraft).Click += this.Get_Aircraft;
                }
                break;
            case 3: // MainPage.xaml line 15
                {
                    this.aircraft = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4: // MainPage.xaml line 17
                {
                    this.GetBattery = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.GetBattery).Click += this.Get_Battery;
                }
                break;
            case 5: // MainPage.xaml line 19
                {
                    this.battery = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 6: // MainPage.xaml line 22
                {
                    this.GetLocation = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.GetLocation).Click += this.Get_Location;
                }
                break;
            case 7: // MainPage.xaml line 23
                {
                    this.Startasync = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.Startasync).Click += this.startLocationAsync;
                }
                break;
            case 8: // MainPage.xaml line 24
                {
                    this.Stopasync = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.Stopasync).Click += this.stopLocationAsync;
                }
                break;
            case 9: // MainPage.xaml line 26
                {
                    this.Drone_latitude = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10: // MainPage.xaml line 28
                {
                    this.Drone_longitude = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11: // MainPage.xaml line 30
                {
                    this.Drone_altitude = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 12: // MainPage.xaml line 32
                {
                    this.socket_init = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.socket_init).Click += this.Socket_init;
                }
                break;
            case 13: // MainPage.xaml line 33
                {
                    this.init_status = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14: // MainPage.xaml line 34
                {
                    this.call_status = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 15: // MainPage.xaml line 35
                {
                    this.rth_status = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 16: // MainPage.xaml line 36
                {
                    this.photo_status = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 17: // MainPage.xaml line 40
                {
                    this.Client_latitude = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 18: // MainPage.xaml line 42
                {
                    this.Client_longitude = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 19: // MainPage.xaml line 44
                {
                    this.ThUp = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.ThUp).Click += this.Throttle_Up;
                }
                break;
            case 20: // MainPage.xaml line 45
                {
                    this.ThDn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.ThDn).Click += this.Throttle_Down;
                }
                break;
            case 21: // MainPage.xaml line 47
                {
                    this.SetGroundStationMode = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.SetGroundStationMode).Click += this.Set_GroundStationMode;
                }
                break;
            case 22: // MainPage.xaml line 48
                {
                    this.InitMission = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.InitMission).Click += this.Init_Mission;
                }
                break;
            case 23: // MainPage.xaml line 49
                {
                    this.LoadMission = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.LoadMission).Click += this.Load_Mission;
                }
                break;
            case 24: // MainPage.xaml line 50
                {
                    this.GetLoadMission = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.GetLoadMission).Click += this.Get_Loaded_Mission;
                }
                break;
            case 25: // MainPage.xaml line 51
                {
                    this.UploadMission = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.UploadMission).Click += this.Upload_Mission;
                }
                break;
            case 26: // MainPage.xaml line 52
                {
                    this.RetryUploadMission = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.RetryUploadMission).Click += this.Retry_Upload_Mission;
                }
                break;
            case 27: // MainPage.xaml line 53
                {
                    this.StartMission = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.StartMission).Click += this.Start_Mission;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

