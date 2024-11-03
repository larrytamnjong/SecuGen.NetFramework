using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureGen.NetFramework
{
    public class ImageResponse
    {
        /// <summary>
        /// Raw Image Captured
        /// </summary>
        public Byte[] RawImage { get; set; }
        /// <summary>
        /// Image converted into a Match Template that is to be used for verification
        /// </summary>
        public Byte[] MatchImageTemplate { get; set; } = new Byte[400];

        /// <summary>
        /// The quality of the image
        /// </summary>
        public int ImageQuality;
    }
}
