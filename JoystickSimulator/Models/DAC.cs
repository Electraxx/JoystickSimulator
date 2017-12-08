using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JoystickSimulator.Models
{
    //Objet ?
    /// <summary>
    /// Connexion avec le simulateur
    /// </summary>
    class DAC
    {
        private enum RefID : uint
        {
            USB_AO16_16A_RevA = 0x8060,
            USB_AO16_16A = 0x8070,
            USB_AO16_16 = 0x8071,
            USB_AO16_12A = 0x8072,
            USB_AO16_12 = 0x8073,
            USB_AO16_8A = 0x8074,
            USB_AO16_8 = 0x8075,
            USB_AO16_4A = 0x8076,
            USB_AO16_4 = 0x8077,
            USB_AO12_16A = 0x8078,
            USB_AO12_16 = 0x8079,
            USB_AO12_12A = 0x807a,
            USB_AO12_12 = 0x807b,
            USB_AO12_8A = 0x807c,
            USB_AO12_8 = 0x807d,
            USB_AO12_4A = 0x807e,
            USB_AO12_4 = 0x807f
        };

        private static ushort DEFAULTOUTPUT = 0;
        private static uint productID;
        private static uint numDACs;
        public static uint DeviceIndex { get; private set; }
        public static List<double> lastOutput = new List<double>();

        //private static bool _isArmed = false;
        private static int numADCs;


        /// <summary>
        /// Outputs voltage to DAC
        /// </summary>
        /// <param name="volts"></param>
        public static void OutputVoltage(List<double> volts)
        {
            if (volts.Any(x => x >= 0 && x <= 10))
            {

                lastOutput = volts;

                ushort[] Data = new ushort[numDACs * 2];

                for (ushort count = 0; count < numDACs; count++)
                {
                    Data[count * 2] = count;
                    if (count < volts.Count)
                    {
                        int conversion = (int)(65535.0 / 10.0 * volts[count] - 328);
                        Data[count * 2 + 1] = conversion > 0 ? (ushort)conversion : (ushort)0;

                    }
                }

                AIOUSB.DACMultiDirect(DeviceIndex, Data, numDACs);
            }
        }

        public static void InitDac()
        {
            DeviceIndex = AIOUSB.diOnly;

            // Device data:
            UInt32 Status;
            UInt32 PID = 0;
            Int32 NameSize = 256;
            string strName = "name";
            UInt32 DIOBytes = 0;
            UInt32 Counters = 0;
            UInt64 SerNum = 0;

            UInt32 ERROR_SUCCESS = 0;
            //bool deviceIndexValid = false;

            // Get The Device Information test for valid device found:
            Status = AIOUSB.QueryDeviceInfo(DeviceIndex, ref PID, ref NameSize, ref strName, out DIOBytes, out Counters);

            bool deviceIndexValid = (Status == ERROR_SUCCESS && PID >= 0x8060 && PID <= 0x807F);

            //if (Status == ERROR_SUCCESS && PID >= 0x8060 && PID <= 0x807F) //All AO cards
            //{
            //    deviceIndexValid = true;
            //}

            // Check device status:
            Status = AIOUSB.ClearDevices();  // Cleans up any orphaned indexes
            Status = AIOUSB.GetDevices();
            Status = AIOUSB.GetDeviceSerialNumber(DeviceIndex, ref SerNum);


            productID = PID; // public copy	

            // Device properties depend on the product ID
            switch (productID)
            {
                // case CSample0App::USB_AO16_16A:
                default:
                    numDACs = 16;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO16_16:
                    numDACs = 16;
                    numADCs = 0;
                    break;

                case (uint)RefID.USB_AO16_12A:
                    numDACs = 12;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO16_12:
                    numDACs = 12;
                    numADCs = 0;
                    break;

                case (uint)RefID.USB_AO16_8A:
                    numDACs = 8;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO16_8:
                    numDACs = 8;
                    numADCs = 0;
                    break;

                case (uint)RefID.USB_AO16_4A:
                    numDACs = 4;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO16_4:
                    numDACs = 4;
                    numADCs = 0;
                    break;

                case (uint)RefID.USB_AO12_16A:
                    numDACs = 16;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO12_16:
                    numDACs = 16;
                    numADCs = 0;
                    break;

                case (uint)RefID.USB_AO12_12A:
                    numDACs = 12;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO12_12:
                    numDACs = 12;
                    numADCs = 0;
                    break;

                case (uint)RefID.USB_AO12_8A:
                    numDACs = 8;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO12_8:
                    numDACs = 8;
                    numADCs = 0;
                    break;

                case (uint)RefID.USB_AO12_4A:
                    numDACs = 4;
                    numADCs = 2;
                    break;

                case (uint)RefID.USB_AO12_4:
                    numDACs = 4;
                    numADCs = 0;
                    break;
            }

            // we use the same D/A count range for both the 12-bit and 16-bit D/As;
            // the reason we can do so is that the 12-bit D/As automatically truncate
            // the LS 4 bits from the count value; so the count range for the 16-bit
            // D/As is 0-0xffff, while that for the 12-bit D/As is 0-0xfff0

            bool dacError = false;

            for (int channel = 0; channel < numDACs; channel++)
            {
                //if (channel < numDACs)
                //    if (AIOUSB.DACDirect(DeviceIndex, (ushort)channel, DEFAULTOUTPUT) != ERROR_SUCCESS)
                //        dacError = true;

                dacError = (channel < numDACs && AIOUSB.DACDirect(DeviceIndex, (ushort)channel, DEFAULTOUTPUT) != ERROR_SUCCESS) || dacError == true;
            }

            if (dacError)
            {
                Console.Beep();
                Console.WriteLine("No device found");
            }

            AIOUSB.DACSetBoardRange(DeviceIndex, 3);
        }
    }

    class AIOUSB
    {
        #region Constants for simplifying use of Device Index
        public const UInt32 diNone = 0xFFFFFFFF;    // "-1"
        public const UInt32 diFirst = 0xFFFFFFFE;    // "-2"  First board (lowest DeviceIndex)
        public const UInt32 diOnly = 0xFFFFFFFD;    // "-3"  One and only board 
        #endregion
        #region Constants for Win32 Error codes
        public const UInt32 ERROR_SUCCESS = 0;
        #endregion
        #region Constants, Misc
        public const Int32 MAXNAMESIZE = 32;
        #endregion
        # region Constants related to AIOUSB_ClearFIFO() and similar
        public const UInt32 TIME_METHOD_NOW = 0;
        public const UInt32 TIME_METHOD_WAIT_INPUT_ENABLE = 86;
        public const UInt32 TIME_METHOD_NOW_AND_ABORT = 5;
        public const UInt32 TIME_METHOD_WHEN_INPUT_ENABLE = 1;
        #endregion
        #region General Board Utility Functions

        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 GetDevices();
        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 GetDeviceSerialNumber(UInt32 DeviceIndex, ref UInt64 pSerialNumber);
        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 QueryDeviceInfo(UInt32 DeviceIndex, ref UInt32 pPID, ref Int32 pNameSize, IntPtr Name, ref UInt32 DIOBytes, ref UInt32 Counters);
        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 DACSetBoardRange(UInt32 DeviceIndex, UInt32 RangeCode);

        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 QueryDeviceInfo(UInt32 DeviceIndex, ref UInt32 pPID, ref Int32 pNameSize, StringBuilder DEVName, out UInt32 DIOBytes, out UInt32 Counters);

        public static UInt32 QueryDeviceInfo(UInt32 DeviceIndex, ref UInt32 pPID, ref Int32 pNameSize, ref string Name, out UInt32 DIOBytes, out UInt32 Counters)
        {
            UInt32 Status;
            DIOBytes = 0; Counters = 0;
            StringBuilder strNameIn = new StringBuilder(AIOUSB.MAXNAMESIZE + 1);
            Int32 NameSize = AIOUSB.MAXNAMESIZE;

            Status = AIOUSB.QueryDeviceInfo(DeviceIndex, ref pPID, ref NameSize, strNameIn, out DIOBytes, out Counters);
            if (Status == AIOUSB.ERROR_SUCCESS)
            {
                Name = strNameIn.ToString(0, NameSize);
            }
            else
            {
                Name = "";
            }
            return Status;
        }

        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 ClearDevices();


        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 DACDirect(UInt32 DeviceIndex, UInt16 Channel, UInt16 Counts);
        [DllImport("AIOUSB.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 DACMultiDirect(UInt32 DeviceIndex, [In] UInt16[] pDACData, UInt32 DACDataCount);



        #endregion
    }
}
