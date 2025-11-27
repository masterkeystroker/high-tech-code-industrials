using System;
using System.IO;
using System.Xml.Serialization;

namespace CretaBase
{
    public static class CretaSerialization
    {
        //Objeto objClonado = objetoOriginal.SerializeToXml().DeserializeTo<Objecto>(); 

        /// <summary>
        /// Serializar a XML (UTF-16) un objeto cualquiera
        /// </summary>
        public static string SerializeToXml(this object obj)
        {
            try
            {
                StringWriter strWriter = new StringWriter();
                System.Type typ = obj.GetType();
                XmlSerializer serializer = new XmlSerializer(typ);

                serializer.Serialize(strWriter, obj);
                string resultXml = strWriter.ToString();
                strWriter.Close();

                return resultXml;
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaSerizlization.SerializeToXml: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserializar un XML a un objeto T
        /// </summary>
        public static T DeserializeTo<T>(this string xmlSerialized)
        {
            try
            {
                XmlSerializer xmlSerz = new XmlSerializer(typeof(T));

                using (StringReader strReader = new StringReader(xmlSerialized))
                {
                    object obj = xmlSerz.Deserialize(strReader);
                    return (T)obj;
                }
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaSerialization.DeserializeTo: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return default(T); 
            }
        } 
    }
}
