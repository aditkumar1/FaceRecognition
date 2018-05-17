using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace FaceDetection
{
    class FaceDetectionEngine
    {
        public Image<Gray, Byte>[] extractFaceAreas(Mat originalMat)
        {
            Rectangle[] faceRectangles = detectFaceRectangle(originalMat);
            Image<Gray, Byte>[] faceImages = new Image<Gray, byte>[faceRectangles.Length];
            Image<Gray, Byte> originalImage = originalMat.ToImage<Gray, Byte>();
            for (int i = 0; i < faceImages.Length; i++)
            {
                originalImage.ROI = faceRectangles[i];
                faceImages[i] = originalImage.Copy();
                originalImage.ROI = System.Drawing.Rectangle.Empty;
            }
            return faceImages;
        }
        public Rectangle[] detectFaceRectangle(Mat originalMat)
        {
            String path = CommonOperations.haarClassifierFilePath;
            //Console.WriteLine(Path.GetFullPath(path));
            Mat clonedFrame = originalMat.Clone();
            CascadeClassifier harcCascadeClassifier = new CascadeClassifier(path);
            CvInvoke.CvtColor(clonedFrame, clonedFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            Rectangle[] faceRectangle = harcCascadeClassifier.DetectMultiScale(clonedFrame);
            Debugger.writeLine("Faces found- " + faceRectangle.Length);
            return faceRectangle;
        }
        
        public void saveFace(Image<Gray,Byte> face)
        {
            DBOperations dBOperations = DBOperations.createInstance();
            var personName = CommonOperations.getuserInputFromGUI("New Face Detected||Owner Recognition Service","Enter the Person Name:");
            Bitmap faceBitmap = face.ToBitmap();
            string result=dBOperations.SaveFace(personName, CommonOperations.ImageToByte2(faceBitmap));
            Debugger.writeLine(result);
            CommonOperations.faceRecognizerEngine.TrainRecognizer();
        }
    }
}
