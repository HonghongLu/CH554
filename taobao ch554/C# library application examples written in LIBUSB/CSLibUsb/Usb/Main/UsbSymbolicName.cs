using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LibUsbDotNet.Usb.Main
{
    /// <summary>
    /// USB devices contain a symbolic name.  The symbolic name is persistent accrossed boots and uniquely identifies each device.
    /// </summary>
    public class UsbSymbolicName
    {
        //\\?\USB#Vid_04d8&Pid_0f01#00000001#{a5dcbf10-6530-11d2-901f-00c04fb951ed}
        private static string REGEX_FMT_TYPE = "[" + Regex.Escape(@"\?") + "]+?(?<" + REGEX_FMT.TYPE + ">.+?)#";
        private static string REGEX_FMT_VID = ".*?Vid_(?<" + REGEX_FMT.VID + ">[0-9a-fA-F]+)";
        private static string REGEX_FMT_PID = ".*?Pid_(?<" + REGEX_FMT.PID + ">[0-9a-fA-F]+)";
        private static string REGEX_FMT_SERIAL = "#(?<" + REGEX_FMT.SERIAL + ">.+)#";
        private static string REGEX_FMT_CGUID = @"\{(?<" + REGEX_FMT.CGUID + ">.+)}";
        private static string OR = "|";
        private static string REGEX_FMT_SYMBOLICNAME = REGEX_FMT_TYPE + OR + REGEX_FMT_VID + OR + REGEX_FMT_PID + OR + REGEX_FMT_SERIAL + OR + REGEX_FMT_CGUID;
        private static Regex mRegEx = new Regex(REGEX_FMT_SYMBOLICNAME, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private readonly string mSymbolicName;
        private short mVid = 0;
        private short mPid = 0;
        private string mSerial = "";

        private bool mbIsParsed = false;
        private Guid mClassGuid;

        internal UsbSymbolicName(string symbolicName)
        {
            mSymbolicName = symbolicName;
        }

        /// <summary>
        /// The full symbolic name of the device.
        /// </summary>
        public string FullName
        {
            get { return mSymbolicName.TrimStart(new char[] {'\\', '?'}); }
        }

        /// <summary>
        /// VendorId parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public short Vid
        {
            get
            {
                parse();
                return mVid;
            }
        }

        /// <summary>
        /// ProductId parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public short Pid
        {
            get
            {
                parse();
                return mPid;
            }
        }

        /// <summary>
        /// SerialNumber parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public string SerialNumber
        {
            get
            {
                parse();
                return mSerial;
            }
        }

        /// <summary>
        /// Device class parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public Guid ClassGuid
        {
            get
            {
                parse();
                return mClassGuid;
            }
        }

        #region PUBLIC METHODS

        /// <summary>
        /// Parses registry strings containing USB information.  This function can parse symbolic names as well as hardware ids, compatible ids, etc.
        /// </summary>
        /// <param name="sIdentifiers"></param>
        /// <returns>A <see cref="UsbSymbolicName"/> class with all the available information from the <paramref name="sIdentifiers"/> string.</returns>
        public static UsbSymbolicName Parse(string sIdentifiers)
        {
            return new UsbSymbolicName(sIdentifiers);
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="UsbSymbolicName"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="UsbSymbolicName"></see>.
        ///</returns>
        public override string ToString()
        {
            object[] o = new object[] {FullName, Vid.ToString("X4"), Pid.ToString("X4"), SerialNumber, ClassGuid};
            return string.Format("FullName:{0}\r\nVid:0x{1}\r\nPid:0x{2}\r\nSerialNumber:{3}\r\nClassGuid:{4}\r\n", o);
        }

        #endregion

        #region PRIVATE METHODS

        private void parse()
        {
            if (!mbIsParsed)
            {
                mbIsParsed = true;
                MatchCollection matches = mRegEx.Matches(mSymbolicName);
                foreach (Match match in matches)
                {
                    if (match.Groups[REGEX_FMT.VID.ToString()].Success)
                    {
                        short.TryParse(match.Groups[REGEX_FMT.VID.ToString()].Value, NumberStyles.HexNumber, null, out mVid);
                    }
                    else if (match.Groups[REGEX_FMT.PID.ToString()].Success)
                    {
                        short.TryParse(match.Groups[REGEX_FMT.PID.ToString()].Value, NumberStyles.HexNumber, null, out mPid);
                    }
                    else if (match.Groups[REGEX_FMT.SERIAL.ToString()].Success)
                    {
                        mSerial = match.Groups[REGEX_FMT.SERIAL.ToString()].Value;
                    }
                    else if (match.Groups[REGEX_FMT.CGUID.ToString()].Success)
                    {
                        try
                        {
                            mClassGuid = new Guid(match.Groups[REGEX_FMT.CGUID.ToString()].Value);
                        }
                        catch (Exception)
                        {
                            mClassGuid = Guid.Empty;
                        }
                    }
                }
            }
        }

        #endregion

        #region Nested type: REGEX_FMT

        private enum REGEX_FMT
        {
            TYPE,
            VID,
            PID,
            SERIAL,
            CGUID
        }

        #endregion
    }
}