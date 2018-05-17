using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//http://www.emgu.com/forum/viewtopic.php?t=4175
namespace FaceDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            var capture = new Capture();
            using (Mat nextFrame = capture.QueryFrame())
            {
                if (nextFrame != null)
                {
                    FaceDetectionEngine faceDetectionEngine = CommonOperations.faceDetectionEngine;
                    FaceRecognizerEngine faceRecognizerEngine = CommonOperations.faceRecognizerEngine;
                    var faceAreas = faceDetectionEngine.extractFaceAreas(nextFrame);
                    if (faceAreas != null && faceAreas.Length > 0)
                    {
                        foreach (var face in faceAreas)
                        {
                            var result = faceRecognizerEngine.RecognizePerson(face);
                            if (result < 1)
                            {
                                faceDetectionEngine.saveFace(face);
                            }
                            else
                            {
                                Debugger.writeLine("ID of person recognized is "+result);
                                string personName = CommonOperations.dBOperations.GetPersonName(result);
                                MessageBox.Show("This face belongs to " + personName,"Face Recognizition");
                            }
                        }
                    }
                }
            }
        }
    }
}
