using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//referemce - http://ahmedopeyemi.com/main/face-detection-and-recognition-in-c-using-emgucv-3-0-opencv-wrapper-part-2/
namespace FaceDetection
{
    class FaceRecognizerEngine
    {
        private FaceRecognizer _faceRecognizer;
        private DBOperations _dbOperations;
        private String _recognizerFilePath;
        
        public FaceRecognizerEngine(DBOperations dBOperations)
        {
            _recognizerFilePath = CommonOperations.faceRecognizerStorageFile;
            _dbOperations =dBOperations;
            _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
        }

        public bool TrainRecognizer()
        {
            var allFaces = _dbOperations.CallFaces("ALL_PERSONS");
            if (allFaces != null)
            {
                var faceImages = new Image<Gray, byte>[allFaces.Count];
                var faceLabels = new int[allFaces.Count];
                for (int i = 0; i < allFaces.Count; i++)
                {
                    var stream = new MemoryStream(allFaces[i].Image);
                    Bitmap bitmap = new Bitmap(stream);
                    var faceImage = new Image<Gray, byte>(bitmap);
                    faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                    faceLabels[i] = allFaces[i].PersonId;
                }
                _faceRecognizer.Train(faceImages, faceLabels);
                _faceRecognizer.Save(_recognizerFilePath);
                return true;
            }
             _faceRecognizer.Save(_recognizerFilePath);
            return false;
        }
        public void LoadRecognizerData()
        {
            _faceRecognizer.Load(_recognizerFilePath);
        }

        public int RecognizePerson(Image<Gray, byte> userImage)
        {
            /*  Stream stream = new MemoryStream();
              stream.Write(userImage, 0, userImage.Length);
              var faceImage = new Image<Gray, byte>(new Bitmap(stream));*/
            _faceRecognizer.Load(_recognizerFilePath);
            try
            {
                var result = _faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
                return result.Label;
            }
            catch(Exception ex)
            {
                Debugger.writeLine(ex.Message);
                
            }
            return -1;
        }
    }
}
