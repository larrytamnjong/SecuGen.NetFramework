using SecuGen.FDxSDKPro.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureGen.NetFramework
{
    public interface ISecureGenBiometrics
    {
        List<string> EnumeratedDeviceList { get; set; }
        List<string> Enumerate();
        bool InitializeDevice(Int32? selectedDeviceIndex = null);
        void CloseDevice();
        Int32 GetMatchingScore(Byte[] matchImageTemplate1, Byte[] matchImageTemplate2);
        ImageResponse CaptureImage();
        bool VerifyImage(Byte[] baseImage, Byte[] TargetImage);
    }
}
