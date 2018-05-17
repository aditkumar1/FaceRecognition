using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

//reference - http://ahmedopeyemi.com/main/face-detection-and-recognition-in-c-using-emgucv-3-0-opencv-wrapper-part-2/
namespace FaceDetection
{    
    class DBOperations
    {
        private MySqlConnection _sqlConnection;
        private string databaseName = "facerecognition";
        public bool IsConnect()
        {
            if (_sqlConnection == null)
            {
                if (String.IsNullOrEmpty(databaseName))
                    return false;
                string connstring = string.Format("Server=localhost; database={0}; UID=root; password=temp;SslMode=none", databaseName);
                _sqlConnection = new MySqlConnection(connstring);
            }
            return true;
        }
        public static DBOperations createInstance()
        {
            DBOperations dbOperations  = new DBOperations();
            try
            {
                if (!dbOperations.IsConnect())
                {
                    dbOperations = null;
                }
            }
            catch (Exception ex)
            {
                Debugger.writeLine(ex.Message);
            }
            return dbOperations;
        }
        public String SaveFace(string personName, Byte[] faceBlob)
        {
            try
            {
                var exisitingUserId = GetPersonID(personName);
                if (exisitingUserId == 0) exisitingUserId = CommonOperations.GenerateRandomId();
                _sqlConnection.Open();
                var insertQuery = "INSERT INTO faces(personName, faceSample, personID) VALUES(@personName,@faceSample,@personID)";
                var cmd = new MySqlCommand(insertQuery, _sqlConnection);
                cmd.Parameters.AddWithValue("personName", personName);
                cmd.Parameters.AddWithValue("personID", exisitingUserId);
                cmd.Parameters.Add("faceSample", MySqlDbType.Blob, faceBlob.Length).Value = faceBlob;
                var result = cmd.ExecuteNonQuery();
                return String.Format("{0} face(s) saved successfully", result);
            }
            catch (Exception ex)
            {
                Debugger.writeLine(ex.Message);
                return ex.Message;
            }
            finally
            {
                _sqlConnection.Close();
            }

        }

        public List<Face> CallFaces(string personName)
        {
            var faces = new List<Face>();
            try
            {
                _sqlConnection.Open();
                var query = personName.ToLower().Equals("ALL_PERSONS".ToLower()) ? "SELECT * FROM faces" : "SELECT * FROM faces WHERE personName=@personName";
                var cmd = new MySqlCommand(query, _sqlConnection);
                if (!personName.ToLower().Equals("ALL_PERSONS".ToLower())) cmd.Parameters.AddWithValue("personName", personName);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return null;

                while (result.Read())
                {
                    var face = new Face
                    {
                        Image = (byte[])result["faceSample"],
                        Id = Convert.ToInt32(result["id"]),
                        Label = (String)result["personName"],
                        PersonId = Convert.ToInt32(result["personID"])
                    };
                    faces.Add(face);
                }
                faces = faces.OrderBy(f => f.Id).ToList();
            }
            catch (Exception ex)
            {
                Debugger.writeLine(ex.Message);
                return null;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return faces;
        }


        public int GetPersonID(string personName)
        {
            var personID = 0;
            try
            {
                _sqlConnection.Open();
                var selectQuery = "SELECT personID FROM faces WHERE personName=@personName LIMIT 1";
                var cmd = new MySqlCommand(selectQuery, _sqlConnection);
                cmd.Parameters.AddWithValue("personName", personName);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return 0;
                while (result.Read())
                {
                    personID = Convert.ToInt32(result["userId"]);
                }
            }
            catch
            {
                return personID;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return personID; ;
        }

        public string GetPersonName(int personID)
        {
            var personName = "";
            try
            {
                _sqlConnection.Open();
                var selectQuery = "SELECT personName FROM faces WHERE personID=@personID LIMIT 1";
                var cmd = new MySqlCommand(selectQuery, _sqlConnection);
                cmd.Parameters.AddWithValue("personID", personID);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return personName;
                while (result.Read())
                {
                    personName = (String)result["personName"];

                }
            }
            catch
            {
                return personName;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return personName; ;
        }

        public List<string> GetAllPersonNames()
        {
            var personNames = new List<string>();
            try
            {
                _sqlConnection.Open();
                var query = "SELECT DISTINCT personName FROM faces";
                var cmd = new MySqlCommand(query, _sqlConnection);
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    personNames.Add((String)result["personName"]);
                }
                personNames.Sort();
            }
            catch (Exception ex)
            {
                Debugger.writeLine(ex.Message);
                return null;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return personNames;
        }


        public bool DeletePerson(string personName)
        {
            var toReturn = false;
            try
            {
                _sqlConnection.Open();
                var query = "DELETE FROM faces WHERE personName=@personName";
                var cmd = new MySqlCommand (query, _sqlConnection);
                cmd.Parameters.AddWithValue("personName", personName);
                var result = cmd.ExecuteNonQuery();
                if (result > 0) toReturn = true;
            }
            catch (Exception ex)
            {
                Debugger.writeLine(ex.Message);
                return toReturn;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return toReturn;
        }
    }    
}
