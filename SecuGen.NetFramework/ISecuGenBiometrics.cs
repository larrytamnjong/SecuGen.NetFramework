using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuGen.NetFramework
{
    public interface ISecuGenBiometrics
    {
        bool VerifyImage(Byte[] baseImage, Byte[] TargetImage);
        ImageResponse CaptureImage();
        Int32 ImageWidth { get; set; }
        Int32 ImageHeight { get; set; }
        List<string> EnumeratedDeviceList { get; set; }
        List<string> Enumerate();
        bool InitializeDevice(Int32? selectedDeviceIndex = null);
        void CloseDevice();
        Int32 GetMatchingScore(Byte[] baseMatchImageTemplate, Byte[] targetMatchImageTemplate);
    }
}
