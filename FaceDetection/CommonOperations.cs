using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDetection
{
    class CommonOperations
    {
        public static string faceRecognizerStorageFile= "EigenRecognizerTrainedFile";
        public static string ownerName = "adit";
        public static string haarClassifierFilePath = "haarcascade_frontalface_alt.xml";
        public static FaceDetectionEngine faceDetectionEngine;
        public static FaceRecognizerEngine faceRecognizerEngine;
        public static DBOperations dBOperations;
        static CommonOperations()
        {
            initializeApplication();
        }
        public static void initializeApplication()
        {
            dBOperations = DBOperations.createInstance();
            faceDetectionEngine = new FaceDetectionEngine();
            faceRecognizerEngine = new FaceRecognizerEngine(dBOperations);
            checkHaarClassifierFile();
            checkEigenFaceRecognizerTrainedFile();
        }
        public static int GenerateRandomId()
        {
            var date = DateTime.Now.ToString("MMddHHmmss");
            return Convert.ToInt32(date);
        }
        public static string getuserInputFromGUI(string title,string message)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(message, title, "Default", -1, -1);
            return input;
        }
        public static void GrantAccess()
        {
            string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }
        public static void checkHaarClassifierFile()
        {
            while(!File.Exists(haarClassifierFilePath))
            {
                string results = "N/A";

                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_frontalface_alt.xml");
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    StreamReader sr = new StreamReader(resp.GetResponseStream());
                    results = sr.ReadToEnd();
                    sr.Close();
                    File.WriteAllText(haarClassifierFilePath,results);
                }
                catch (Exception ex)
                {
                    Debugger.writeLine(ex.Message);
                }
                Thread.Sleep(10000);
            }
        }
        public static void checkEigenFaceRecognizerTrainedFile()
        {
            while (!File.Exists(faceRecognizerStorageFile))
            {
                if (faceRecognizerEngine.TrainRecognizer())
                {
                    Debugger.writeLine("Recognizer trained successfully");
                }
                else
                {
                    Debugger.writeLine("Recognizer training Failed");
                }
                Thread.Sleep(10000);
            }
        }
    }
}
