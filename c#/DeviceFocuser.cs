// All lines from line 1 to the device interface implementation region will be discarded by the project wizard when the template is used
// Required code must lie within the device implementation region
// The //ENDOFINSERTEDFILE tag must be the last but one line in this file

using ASCOM.DeviceInterface;
using System;
using ASCOM;
using ASCOM.Utilities;
using System.Net.Http;
using System.IO;

class DeviceFocuser
{
    private Util util = new Util();
    private const int _StepSize = 1;
    private int focuserPosition = 50000; // Class level variable to hold the current focuser position
    private const int focuserSteps = 100000;
    private const int increment = 1000;
    private bool connected = false;
    private bool absolute = true;
    private string server;
    public bool trace = true;

    public DeviceFocuser()
    {
        LogMessage("Constructor");
    }

    private void LogMessage(string message)
    {
        if (trace)
            File.AppendAllText(@"c:\temp\focuser.log", message+"\n");
    }

    public bool Connected // Dummy Connected method because this is referenced in the Link method
    {
        get { return connected; }
        set {
            if (value)
            {
                Connect();
            }
            connected = value;
        }
    }

    private void Connect()
    {
        try
        {
            HttpClient _Client = new HttpClient();
            String request = server + "?op=position";
            LogMessage("URL is " + request);
            using (HttpResponseMessage response = _Client.GetAsync(request).Result)
            {
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    string result = content.ReadAsStringAsync().Result;
                    LogMessage("Set Server result " + result);
                    int.TryParse(result, out this.focuserPosition);
                }
            }
            connected = true;
        }
        catch (Exception e)
        {
            LogMessage("Set Server error " + e.ToString());
            //throw;
        }

    }

    public string Server
    {
        get
        {
            LogMessage("Get Server ");
            return server; }
        set
        {
            if (value != null)
            {
                LogMessage("Set Server " + value.ToString());
                server = value;
                Connect();
            }
        }
    }

    #region IFocuser Implementation

    public bool Absolute
    {
        get
        {
            LogMessage("Absolute Get "+ absolute.ToString());
            return absolute; // Is this is an absolute focuser?
        }
    }

    public void Halt()
    {
        LogMessage("Halt Not implemented");
        throw new ASCOM.MethodNotImplementedException("Halt");
    }

    public bool IsMoving
    {
        get
        {
            LogMessage("IsMoving Get "+ false.ToString());
            return false; // This focuser always moves instantaneously so no need for IsMoving ever to be True
        }
    }

    public bool Link
    {
        get
        {
            LogMessage("Link Get "+ this.Connected.ToString());
            return this.Connected; // Direct function to the connected method, the Link method is just here for backwards compatibility
        }
        set
        {
            LogMessage("Link Set "+ value.ToString());
            if (value)
            {
                Connect();
            }
        }
    }

    public int MaxIncrement
    {
        get
        {
            LogMessage("MaxIncrement Get "+ increment.ToString());
            return increment; // Maximum change in one move
        }
    }

    public int MaxStep
    {
        get
        {
            LogMessage("MaxStep Get "+ focuserSteps.ToString());
            return focuserSteps; // Maximum extent of the focuser, so position range is 0 to 100,000
        }
    }

    private void MoveAbsolute(int position)
    {
        LogMessage("Move absolute " + position.ToString());

        try
        {
            HttpClient _Client = new HttpClient();
            String request = server + "?op=move";
            request += "&position=" + Math.Abs(position);
            LogMessage("URL is " + request);
            using (HttpResponseMessage response = _Client.GetAsync(request).Result)
            {
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    string result = content.ReadAsStringAsync().Result;
                    LogMessage("Move abslute result " + result);
                }
            }
            focuserPosition = position; // Set the focuser position
        }
        catch (Exception e)
        {
            LogMessage("Move abslute error " + e.ToString());
            //throw;
        }
    }

    public void MoveRelative(int steps)
    {
        LogMessage("Move relative "+ steps.ToString());

        try
        {
            HttpClient _Client = new HttpClient();
            String request = server;
            if (steps > 0)
            {
                request += "?op=forwardrelative";
            }
            else
            {
                request += "?op=backwardrelative";
            }
            request += "&steps=" + Math.Abs(steps);
            LogMessage("URL is "+request);
            using (HttpResponseMessage response = _Client.GetAsync(request).Result)
            {
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    string result = content.ReadAsStringAsync().Result;
                    LogMessage("Move relative result " + result);
                }
            }
            focuserPosition = focuserPosition + steps; // Set the focuser position
        }
        catch(Exception e)
        {
            LogMessage("Move relative error " + e.ToString());
            //throw;
        }
    }

    public void Move(int position)
    {
        if (absolute)
            MoveAbsolute((position));
        else
        {
            MoveRelative(position);
        }
    }

    public int Position
    {
        get
        {
            return focuserPosition; // Return the focuser position
        }
    }

    public double StepSize
    {
        get
        {
            LogMessage("StepSize Get "+_StepSize);
            return _StepSize;
        }
    }

    public bool TempComp
    {
        get
        {
            LogMessage("TempComp Get "+ false.ToString());
            return false;
        }
        set
        {
            LogMessage("TempComp Set Not implemented");
            throw new ASCOM.PropertyNotImplementedException("TempComp", false);
        }
    }

    public bool TempCompAvailable
    {
        get
        {
            LogMessage("TempCompAvailable Get "+ false.ToString());
            return false; // Temperature compensation is not available in this driver
        }
    }

    public double Temperature
    {
        get
        {
            LogMessage("Temperature Get Not implemented");
            throw new ASCOM.PropertyNotImplementedException("Temperature", false);
        }
    }

    #endregion

    public void Dispose()
    {

    }

    //ENDOFINSERTEDFILE
}