using SecuGen.FDxSDKPro.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuGen.NetFramework
{
    public class SecuGenBiometrics : ISecuGenBiometrics
    {
        private SGFingerPrintManager FingerPrintManager;

        public static int ImageWidth = 200;
        public static int ImageHeight = 300;
        private SGFPMDeviceList[] DeviceList;

        /// <summary>
        /// You can display this list after calling Emumerate for the user to choose a device
        /// </summary>
        public List<string> EnumeratedDeviceList { get; set; } = new List<string>();

        public SecuGenBiometrics()
        {
            FingerPrintManager = new SGFingerPrintManager();
            Enumerate();
        }

        /// <summary>
        /// Call this function first to get connected devices this method returns a SGFPMDeviceList[].
        /// After calling this function you can access a list of device names from EnumeratedDeviceList
        /// </summary>
        public List<string> Enumerate()
        {
            EnumeratedDeviceList.Clear();

            Int32 iError = FingerPrintManager.EnumerateDevice();
            string enumDevice;

            DeviceList = new SGFPMDeviceList[FingerPrintManager.NumberOfDevice];

            for (int i = 0; i < FingerPrintManager.NumberOfDevice; i++)
            {
                DeviceList[i] = new SGFPMDeviceList();
                FingerPrintManager.GetEnumDeviceInfo(i, DeviceList[i]);
                enumDevice = DeviceList[i].DevName.ToString() + " : " + DeviceList[i].DevID;
                this.EnumeratedDeviceList.Add(enumDevice);
            }

            return EnumeratedDeviceList;
        }

        /// <summary>
        /// Initialize SGFingerPrint with the selected device
        /// </summary>
        /// <param name="selectedDeviceIndex">Index of the selected device from EnumeratedDeviceList if you leave this parameter null a device will be auto selected.
        /// Order of auto select Hamster IV(HFDU04) -> Plus(HFDU03) -> III (HFDU02)</param>
        public bool InitializeDevice(Int32? selectedDeviceIndex = null)
        {
            if (FingerPrintManager.NumberOfDevice == 0)
                throw new Exception(GetErrorMessage(55));


            SGFPMDeviceName deviceName;
            Int32 DeviceId;
            Int32 numberOfDevices = EnumeratedDeviceList.Count();


            if (selectedDeviceIndex == null)
            {
                deviceName = SGFPMDeviceName.DEV_AUTO;
                DeviceId = (Int32)(SGFPMPortAddr.USB_AUTO_DETECT);
            }
            else
            {
                deviceName = DeviceList[(int)selectedDeviceIndex].DevName;
                DeviceId = DeviceList[(int)selectedDeviceIndex].DevID;
            }

            Int32 iError = OpenDevice(deviceName, DeviceId);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Int32 OpenDevice(SGFPMDeviceName device_name, Int32 device_id)
        {
            Int32 iError = FingerPrintManager.Init(device_name);
            iError = FingerPrintManager.OpenDevice(device_id);
            return iError;
        }

        public void CloseDevice()
        {
            FingerPrintManager.CloseDevice();
        }

        /// <summary>
        /// Caputures image and returns a byte data which you can draw on the screen as an image
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Throws Exception if Capture Image fails</exception>
        public ImageResponse CaptureImage()
        {
            ImageResponse image = new ImageResponse();

            Int32 iError;

            image.RawImage = new Byte[ImageWidth * ImageHeight];

            iError = FingerPrintManager.GetImage(image.RawImage);
            FingerPrintManager.GetImageQuality(ImageWidth, ImageHeight, image.RawImage, ref image.ImageQuality);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                iError = FingerPrintManager.CreateTemplate(image.RawImage, image.MatchImageTemplate);

                if (iError == (Int32)SGFPMError.ERROR_NONE)
                    return image;
                else
                    throw new Exception(GetErrorMessage(iError));
            }
            else
            {
                throw new Exception(GetErrorMessage(iError));
            }

        }

        /// <summary>
        /// This function is useful during registration you can collect 2 finger prints template and verify the matching score before saving to the database
        /// </summary>
        /// <returns></returns>
        public Int32 GetMatchingScore(Byte[] matchImageTemplate1, Byte[] matchImageTemplate2)
        {
            Int32 iError;
            bool matched = false;
            Int32 matchScore = 0;

            iError = FingerPrintManager.MatchTemplate(matchImageTemplate1, matchImageTemplate2, SGFPMSecurityLevel.NORMAL, ref matched);
            iError = FingerPrintManager.GetMatchingScore(matchImageTemplate1, matchImageTemplate2, ref matchScore);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                return matchScore;
            }
            else
            {
                throw new Exception(GetErrorMessage(iError));

            }
        }


        /// <summary>
        /// </summary>
        /// <param name="baseMatchImageTemplate">Match template of image to verify</param>
        /// <param name="targetMatchImageTemplate">Match template image to verify</param>
        /// <returns>True if successful else it return false</returns>
        public bool VerifyImage(Byte[] baseMatchImageTemplate, Byte[] targetMatchImageTemplate)
        {
            Int32 iError;
            bool matched = false;


            iError = FingerPrintManager.MatchTemplate(baseMatchImageTemplate, targetMatchImageTemplate, SGFPMSecurityLevel.NORMAL, ref matched);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                if (matched)
                    return true;
                else
                    return false;
            }
            else
            {
                throw new Exception(GetErrorMessage(iError));
            }
        }

        string GetErrorMessage(int iError)
        {
            string text = "";

            switch (iError)
            {
                case 0:  // No error occurred.
                    text = "Error none";
                    break;

                case 1:  // Failed to create an object.
                    text = "Cannot create object";
                    break;

                case 2:  // Function execution failed.
                    text = "Function failed";
                    break;

                case 3:  // Invalid parameter provided.
                    text = "Invalid parameter";
                    break;

                case 4:  // Function is not used.
                    text = "Not used function";
                    break;

                case 5:  // Failed to load required DLL.
                    text = "Cannot load required DLL";
                    break;

                case 6:  // Failed to load device driver.
                    text = "Cannot load device driver";
                    break;

                case 7:  // Failed to load 'sgfpamx.dll' file.
                    text = "Cannot load sgfpamx.dll";
                    break;

                case 51:  // System file load failure.
                    text = "Cannot load driver kernel file";
                    break;

                case 52:  // Device initialization failure.
                    text = "Failed to initialize the device";
                    break;

                case 53:  // Data transmission error.
                    text = "Data transmission is not good";
                    break;

                case 54:  // Timeout error during image capture.
                    text = "Timeout";
                    break;

                case 55:  // Device not detected.
                    text = "Device not found";
                    break;

                case 56:  // Failed to load driver file.
                    text = "Cannot load driver file";
                    break;

                case 57:  // Captured image is incorrect.
                    text = "Wrong image";
                    break;

                case 58:  // Insufficient USB bandwidth.
                    text = "Lack of USB bandwidth";
                    break;

                case 59:  // Device is already open and in use.
                    text = "Device is already opened";
                    break;

                case 60:  // Device serial number retrieval failure.
                    text = "Device serial number error";
                    break;

                case 61:  // Unsupported device detected.
                    text = "Unsupported device";
                    break;

                // Errors related to feature extraction and verification
                case 101:  // Insufficient number of minutiae.
                    text = "The number of minutiae is too small";
                    break;

                case 102:  // Invalid template type provided.
                    text = "Template is invalid";
                    break;

                case 103:  // First template is invalid.
                    text = "1st template is invalid";
                    break;

                case 104:  // Second template is invalid.
                    text = "2nd template is invalid";
                    break;

                case 105:  // Minutiae extraction failed.
                    text = "Minutiae extraction failed";
                    break;

                case 106:  // Template matching failed.
                    text = "Matching failed";
                    break;
            }

            return "Error #" + iError + ": " + text;
        }
    }
}
