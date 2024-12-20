using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace TeamB.Develop
{
    public class Test : MonoBehaviour
    {
        void Start()
        {
            WebRequest request = WebRequest.Create("http://localhost/hello.php?table=parameter");
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream, Encoding.UTF8);
            string responseFromServer = reader.ReadToEnd();
            Debug.Log(responseFromServer);
            Hoge line = JsonUtility.FromJson<Hoge>(responseFromServer);
            Debug.Log(line.ID);
        }

    }

    public class Hoge
    {
        public int ID;
        public string Name;
        public int Hp;
        public int ATK;
    }
}
