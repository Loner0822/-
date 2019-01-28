using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EnvirInfoSys
{
    public class PictureHelper
    {
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;

        public DataTable QueryData(string url, string[] parameter)
        {
            string Xml = Webservice.InvokeWebservice(url, "QueryData", parameter);
            DataTable res = XmlToDataTable(Xml);
            return res;
        }

        public void DownloadPic(string url, string[] parameter, string FormName)
        {
            Webservice.Download(url, parameter[0] + parameter[1], FormName);
        }

        public void ClearDir(string url, string[] parameter)
        {
            string Path = parameter[0];
            if (Directory.Exists(Path))
            {
                foreach (string f in Directory.GetFileSystemEntries(Path))
                    if (File.Exists(f))
                        File.Delete(f);
            }
            else
                Directory.CreateDirectory(Path);
        }

        public static DataTable XmlToDataTable(string xmlString)
        {
            XmlDocument xmldoc = new XmlDocument();
            if (xmlString == "0")
                return new DataTable();
            xmldoc.LoadXml(xmlString);
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmldoc.InnerXml);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                reader.Close();
                foreach (DataTable xmlDT in xmlDS.Tables)
                    if (xmlDT != null)
                        return xmlDT;
            }
            catch (System.Exception ex)
            {
                reader.Close();
                throw ex;
            }
            return null;
        }
    }     
}
