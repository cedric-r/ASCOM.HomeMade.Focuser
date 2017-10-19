//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM TEMPLATEDEVICECLASS driver for TEMPLATEDEVICENAME
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM TEMPLATEDEVICECLASS interface version: <To be completed by driver developer>
// Author:		(XXX) Your N. Here <your@email.here>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code canbe deleted and this definition removed.
#define TEMPLATEDEVICECLASS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;
using System.IO;

namespace ASCOM.HomeMade
{
    //
    // Your driver's DeviceID is ASCOM.TEMPLATEDEVICENAME.TEMPLATEDEVICECLASS
    //
    // The Guid attribute sets the CLSID for ASCOM.TEMPLATEDEVICENAME.TEMPLATEDEVICECLASS
    // The ClassInterface/None addribute prevents an empty interface called
    // _TEMPLATEDEVICENAME from being created and used as the [default] interface
    //
    // TODO Replace the not implemented exceptions with code to implement the function or
    // throw the appropriate ASCOM exception.
    //

    /// <summary>
    /// ASCOM TEMPLATEDEVICECLASS Driver for TEMPLATEDEVICENAME.
    /// </summary>
    [Guid("3A02C211-FA08-4747-B0BD-4B00EB159296")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(Focuser.driverID)]
    public class Focuser : IFocuserV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal const string driverID = "ASCOM.HomeMade.Focuser";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        private static string driverDescription = "ASCOM HomeMade Driver for Focuser.";
        private static int version = 0;

        internal static string comServerName = "Server"; // Constants used for Profile persistence
        internal static string comPortDefault = "http://192.168.0.118/index.php";
        internal static string traceStateProfileName = "Trace Level";
        internal static string traceStateDefault = "false";

        internal static string comServer; // Variables to hold the currrent device configuration
        private static bool _trace = false;
        internal static bool trace { get { return _trace; } set { _trace = value; } }

        private DeviceFocuser _controller = null;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="TEMPLATEDEVICENAME"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Focuser()
        {
            _controller = new DeviceFocuser();
            _controller.Server = comServer;
            _controller.trace = trace;

            ReadProfile(); // Read device configuration from the ASCOM Profile store

            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object
            //TODO: Implement your additional construction here
        }

        //
        // PUBLIC COM INTERFACE ITEMPLATEDEVICEINTERFACE IMPLEMENTATION
        //

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public void SetupDialog()
        {
            // consider only showing the setup dialog if not connected
            // or call a different dialog if connected
            if (_controller.Connected)
                System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

            using (SetupDialogForm F = new SetupDialogForm())
            {
                var result = F.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    WriteProfile(); // Persist device configuration values to the ASCOM Profile store
                }
            }
        }

        public ArrayList SupportedActions
        {
            get
            {
                LogMessage("SupportedActions Get", "Returning empty arraylist");
                return new ArrayList();
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            LogMessage("", "Action {0}, parameters {1} not implemented", actionName, actionParameters);
            throw new ASCOM.ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
        }

        public void CommandBlind(string command, bool raw)
        {
            // Call CommandString and return as soon as it finishes
            this.CommandString(command, raw);
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBlind");
            // DO NOT have both these sections!  One or the other
        }

        public bool CommandBool(string command, bool raw)
        {
            string ret = CommandString(command, raw);
            // TODO decode the return string and return true or false
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBool");
            // DO NOT have both these sections!  One or the other
        }

        public string CommandString(string command, bool raw)
        {
            // it's a good idea to put all the low level communication with the device here,
            // then all communication calls this function
            // you need something to ensure that only one command is in progress at a time

            throw new ASCOM.MethodNotImplementedException("CommandString");
        }

        public void Dispose()
        {
            // Clean up the tracelogger and util objects
            trace = false;
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }

        public bool Connected
        {
            get
            {
                bool IsConnected = _controller.Connected;
                LogMessage("Connected", "Get {0}", IsConnected);
                return IsConnected;
            }
            set
            {
                LogMessage("Connected", "Set {0}", value);

                return;
            }
        }

        public string Description
        {
            // TODO customise this device description
            get
            {
                LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // TODO customise this driver des[cription
                string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                LogMessage("InterfaceVersion Get", version.ToString());
                return Convert.ToInt16(version);
            }
        }

        public string Name
        {
            get
            {
                string name = "HomeMade.Focuser";
                LogMessage("Focuser", "Name Get");
                return name;
            }
        }

        #endregion

        //INTERFACECODEINSERTIONPOINT
        #region Private properties and methods
        // here are some useful properties and methods that can be used as required
        // to help with driver development

        #region ASCOM Registration

        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        /// <summary>
        /// Register or unregister the driver with the ASCOM Platform.
        /// This is harmless if the driver is already registered/unregistered.
        /// </summary>
        /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
        private static void RegUnregASCOM(bool bRegister)
        {
            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "Focuser";
                if (bRegister)
                {
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
                }
            }
        }

        /// <summary>
        /// This function registers the driver with the ASCOM Chooser and
        /// is called automatically whenever this class is registered for COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is successfully built.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
        /// </remarks>
        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        /// <summary>
        /// This function unregisters the driver from the ASCOM Chooser and
        /// is called automatically whenever this class is unregistered from COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is cleaned or prior to rebuilding.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
        /// </remarks>
        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }

        #endregion

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Focuser";
                trace = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
                comServer = driverProfile.GetValue(driverID, comServerName, string.Empty, comPortDefault);
                _controller.Server = comServer;
                _controller.trace = trace;
            }
        }

        /// <summary>
        /// Write the device configuration to the  ASCOM  Profile store
        /// </summary>
        internal void WriteProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Focuser";
                driverProfile.WriteValue(driverID, traceStateProfileName, trace.ToString());
                driverProfile.WriteValue(driverID, comServerName, comServer.ToString());
            }
        }

        /// <summary>
        /// Log helper function that takes formatted strings and arguments
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void LogMessage(string identifier, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            LogMessage(identifier+": "+msg);
        }

        internal static void LogMessage(string message)
        {
            File.AppendAllText(@"c:\temp\driver.log", message + "\n");
        }

        #endregion

        #region IFocuser Members

        public bool Absolute
        {
            get
            {
                LogMessage("Focuser", "Absolute Get");
                if (!Link)
                    LogMessage("Focuser", "Link not active");
                return _controller.Absolute;
            }
        }

        public void Halt()
        {
            LogMessage("Focuser", "Halt");
            if (!Link)
                LogMessage("Focuser", "Link not active");
            if (!Link)
                throw new InvalidOperationException("Focuser link not activated");
            _controller.Halt();
        }

        public bool IsMoving
        {
            get
            {
                LogMessage("Focuser", "IsMoving Get");
                if (!Link)
                    LogMessage("Focuser", "Link not active");
                return _controller.IsMoving;
            }
        }

        public bool Link
        {
            get
            {
                LogMessage("Focuser", "Link Get "+ (_controller != null));
                return (_controller != null);
            }
            set
            {
                LogMessage("Focuser", "Link Set");
                if (_controller != null)
                    _controller.Dispose();
                if (value)
                {
                    BuildController();
                }
                else
                {
                    _controller = null;
                }
            }
        }

        private void BuildController()
        {
            LogMessage("Focuser", "BuildController");
            if (string.IsNullOrEmpty(_controller.Server))
            {
                SetupDialog();
            }
            try
            {
                _controller = new DeviceFocuser();
                _controller.Server = comServer;
                _controller.trace = trace;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                LogMessage("Focuser", "BuildController "+e.ToString());
                SetupDialog();
                _controller = new DeviceFocuser();
                _controller.Server = comServer;
                _controller.trace = trace;
            }
        }
        /*
        private string GetPort()
        {
            return GetValue("port");
        }

        private void SetPort(string portName)
        {
            SetValue("port", portName);
        }
        */
        public int MaxIncrement
        {
            get
            {
                LogMessage("Focuser", "MaxIncremment Get");
                if (!Link)
                    LogMessage("Focuser", "Link not active");
                return _controller.MaxIncrement;
            }
        }

        public int MaxStep
        {
            get
            {
                LogMessage("Focuser", "MaxStep Get");
                if (!Link)
                    LogMessage("Focuser", "Link not active");
                return _controller.MaxStep;
            }
        }

        public void Move(int val)
        {
            LogMessage("Focuser", "Move "+val);
            if (!Link)
                LogMessage("Focuser", "Link not active");
            if (!Link)
                throw new InvalidOperationException("Focuser link not activated");
            LogMessage("Focuser", "Calling move");
            try
            {
                _controller.Move(val);
            }
            catch(Exception e)
            {
                LogMessage("Focuser", e.ToString());
            }
            LogMessage("Focuser", "Calling move done");
        }


        public int Position
        {
            get
            {
                LogMessage("Focuser", "Position Get");
                if (!Link)
                    LogMessage("Focuser", "Link not active");
                if (!Link)
                    throw new InvalidOperationException("Focuser link not activated");
                return _controller.Position;
            }
        }
        /*
        private void SetValue(string key, string value)
        {
            _profile.WriteValue(s_csDriverID, key, value ?? "", "");
        }

        private string GetValue(string key)
        {
            return _profile.GetValue(s_csDriverID, key, "");
        }


        private void SetFlags()
        {
            _reversed = GetValue("reverse") == "True";
            _absolute = GetValue("absolute") == "True";

        }
        */


        public double StepSize
        {
            get
            {
                if (!Link)
                    LogMessage("Focuser", "Link not active");
                LogMessage("Focuser", "StepSize Get");
                return 1;
            }
        }

        public bool TempCompAvailable
        {
            get
            {
                if (!Link)
                    LogMessage("Focuser", "Link not active");
                LogMessage("Focuser", "TempAvailable Get");
                return false;
            }
        }


        #endregion


        //
        // PUBLIC COM INTERFACE IFocuser IMPLEMENTATION
        //

        #region IFocuser Members


        public bool TempComp
        {
            get { return false; }
            set { throw new PropertyNotImplementedException("TempComp", true); }
        }

        public double Temperature
        {
            // TODO Replace this with your implementation
            get { throw new PropertyNotImplementedException("Temperature", false); }
        }

        #endregion
    }
}
