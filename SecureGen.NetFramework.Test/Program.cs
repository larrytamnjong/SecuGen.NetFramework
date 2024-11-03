using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureGen.NetFramework.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {

            ImageResponse finger1 = null;
            ImageResponse finger2 = null;


            var secureGenBiometrics = new SecureGenBiometrics();
            secureGenBiometrics.Enumerate();
            secureGenBiometrics.InitializeDevice();

            int option;

            Console.WriteLine(" 1. Read finger 1\n 2. Read finger 2\n 3. Verify\n 4. Exit");

            while (true)
            {
                Console.WriteLine(" 1. Read finger 1\n 2. Read finger 2\n 3. Verify\n 4. Exit");
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option)
                    {
                        case 1:

                            finger1 = secureGenBiometrics.CaptureImage();
                            break;
                        case 2:
                            finger2 = secureGenBiometrics.CaptureImage();
                            break;
                        case 3:
                            if (finger1 != null && finger2 != null)
                            {
                                bool match = secureGenBiometrics.VerifyImage(finger1.MatchImageTemplate, finger2.MatchImageTemplate);
                                if (match)
                                {
                                    Console.WriteLine("Finger prints Match!");
                                }
                                else
                                {
                                    Console.WriteLine("Finger prints do not match");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Please capture both fingerprints first.");
                            }
                            break;
                        case 4:
                            return;
                        default:
                            Console.WriteLine("Invalid Option");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input. Please enter a number.");
                }
            }
        }
    }
}
