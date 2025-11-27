using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;

namespace CretaBase
{
    public static class CretaUtils
    {
        public const int INVALID_VALUE = -9999999;

        /// <summary>
        /// Return a value between min and max
        /// i.e.: CretaClamp(89,2,25) returns 25
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">Value to clamp</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>A value between min and max</returns>
        public static T CretaClamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Calcula el porcentaje de un valor dentro de un rango con un mínimo y un máximo
        /// i.e.: val=15, min=10, max=20 dará como resultado 0.5 ya que ocupa el 50% de ese segmento
        /// </summary>
        public static double CretaTransformToPercInsideRange(double val, double min, double max)
        {
            double Diff = max - min;
            return (val - min) / Diff;
        }

        /// <summary>
        /// Calcula el equivalente de un porcentaje en un rango con un mínimo y un máximo
        /// i.e.: val=80 (%), min=10, max=90 dará como resultado
        /// </summary>
        public static double CretaTransformPercToValueInsideRange(double val, double min, double max)
        {
            double Diff = max - min;
            val = val / 100;
            double res = (val * Diff) + min;
            return res;
        }

        public static long ConvertDateTimeToTicks(DateTime dtInput)
        {
            long ticks = 0;
            ticks = dtInput.Ticks;
            return ticks;
        }

        public static DateTime ConvertTicksToDateTime(long lticks)
        {
            DateTime dtresult = new DateTime(lticks);
            return dtresult;
        }

        /// <summary>
        /// Changes the cursor to loading when bStart is True and return to normal pointer when is False
        /// </summary>
        /// <param name="bStart"></param>
        public static void ShowLoadingData(bool bStart)
        {
            if (bStart)
                Mouse.OverrideCursor = Cursors.Wait;
            else
                Mouse.OverrideCursor = null;
        }

        public const double FACTORCONVERSION = 25.4;
        public static int MmToPixel(double mm, int resolution)
        {
            return (int)(MmToPixelDouble(mm,resolution));
        }
        //No redondea pixeles
        public static double MmToPixelDouble(double mm, int resolution)
        {
            return (mm * resolution) / FACTORCONVERSION;
        }
        public static double PixelToMm(int pixels, int resolution)
        {
            return PixelToMm((double)pixels,resolution);
        }
        //especial para no redondear
        public static double PixelToMm(double pixels, int resolution)
        {
            return (pixels * (FACTORCONVERSION*10.0)) / (resolution*10.0);
        }

        /// <summary>
        /// Fill the start of sOrigin string with the strChar character until the final string reaches iFinalLength
        /// i.e.: AddStartChars("95",4,"0") returns 0095
        /// </summary>
        /// <param name="sOrigin">Original string</param>
        /// <param name="iFinalLength">Total chars of final string</param>
        /// <param name="strChar">Char to fill with</param>
        /// <returns></returns>
        public static string AddStartChars(string sOrigin, int iFinalLength, string strChar)
        {
            string sRes = sOrigin;
            while (sRes.Length < iFinalLength)
            {
                sRes = sRes.Insert(0, strChar);
            }
            return sRes;
        }

        /// <summary>
        /// Converts a string to int, if the value to convert is # returns CretaTypes.INVALID_VALUE
        /// </summary>
        public static int ToInt(string str)
        {
            int i = 0;

            try
            {
                i = Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                if(str!="#")
                {
                    string sMessage = string.Format("LOG_EXCEPTION;Error Base.CretaUtils.ToInt({0}): {1}{2}", str, ex.Message, ex.StackTrace);
                    CretaUtils.WriteLogEvent(sMessage);
                }
                i = INVALID_VALUE;
            }
            return i;
        }
        /// <summary>
        /// Converts a string to double, if the value to convert is # returns CretaTypes.INVALID_VALUE
        /// </summary>
        public static double ToDouble(string str)
        {
            double d = 0;
            try
            {
                str = str.Replace(',', '.');
                d = double.Parse(str, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if(str!="#")
                {
                    string sMessage = string.Format("LOG_EXCEPTION;Error Base.CretaUtils.ToDouble({0}): {1}{2}", str, ex.Message, ex.StackTrace);
                    CretaUtils.WriteLogEvent(sMessage);
                }
                d = INVALID_VALUE;
            }
            return d;
        }

        /// <summary>
        /// Compare 2 strings and returns True if are equals, by default are case INsensitive
        /// </summary>
        /// <param name="ignoreCase">By default True, ignore case</param>
        /// <returns>True if the two strings are equals</returns>
        public static bool AreEqual(string s1, string s2, bool ignoreCase=true)
        {
            bool bEqual = (string.Compare(s1, s2, ignoreCase, System.Globalization.CultureInfo.CurrentCulture) == 0);

            return bEqual;
        }

        /// <summary>
        /// Compara dos cadenas que pueden contener wildcards (*|?). 
        /// P.e.: IsLike("TEST23","T?S*") devuelve True
        /// </summary>
        /// <param name="s1">Cadena original</param>
        /// <param name="s2">Cadena con la que se compara</param>
        /// <param name="bIgnoreCase">Indica si hay que tener en cuenta mayúsculas y minúsculas</param>
        /// <returns>True si coinciden, false si no</returns>
        public static bool IsLike(string s1, string s2, bool bIgnoreCase = true)
        {
            string sChange = "^" + Regex.Escape(s2).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";

            Regex regex;
            if (bIgnoreCase)
                regex = new Regex(sChange, RegexOptions.IgnoreCase);
            else
                regex = new Regex(sChange);

            return (regex.IsMatch(s1));
        }
        /// <summary>
        /// Indica si la cadena sLongText contiene alguna ocurrencia de la cadena sSmallWord
        /// </summary>
        /// <param name="sLongText">Texto donde buscar la ocurrencia</param>
        /// <param name="sSmallWord">Ocurrencia que buscar en el texto</param>
        /// <param name="bIgnoreCase">Indica si se quieren ignorar las mayúsculas/minúsculas</param>
        /// <returns>Devuelve True si aparece la cadena sSmallWord en el texto sLongText</returns>
        public static bool Contains(string sLongText, string sSmallWord, bool bIgnoreCase = true)
        {
            bool bContains = false;

            if (bIgnoreCase)
                bContains = System.Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(sLongText, sSmallWord, CompareOptions.IgnoreCase) >= 0;
            else
                bContains = sLongText.Contains(sSmallWord);

            return bContains;
        }

        /// <summary>
        /// Get The assembly TimeStamp to know the build's date
        /// </summary>
        /// <returns>DateTime's build</returns>
        public static DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }

        /// <summary>
        /// Si se escribe una ruta y nombre de archivo en esta propiedad los logs se guardarán aquí con el nombre de archivo especificado
        ///     seguido de la fecha actual. p.e: LogsFilePath=C:\\TEST\\prueba hará que la función WriteLogEvent escriba en el archivo 
        ///     C:\TEST\prueba_logs_2013_1_31.txt siendo la fecha la del momento de escribir
        /// </summary>
        public static string LogsFilePath { get; set; }
        private const int MAX_CHARS = 1024;
        /// <summary>
        /// Escribe mensajes de log en el archivo indicado, si no se especifica archivo se guarda en la ruta especificada en la propiedad LogsFilePath
        /// </summary>
        /// <param name="sMessage">Mensaje a mostrar</param>
        /// <param name="sFile">Ruta del archivo de salida, si no se especifica se escribirá en un con la fecha actual en la ruta especificada en la propiedad LogsFilePath</param>
        public static void WriteLogEvent(string sMessage,string sFile="")
        {
            DateTime dt = DateTime.Now;
            if (sFile == "")
            {
                if (LogsFilePath == "") return;
                sFile = LogsFilePath + "LogGui_" + dt.Year + "_" + dt.Month + "_" + dt.Day + ".txt";
            }
            IFormatProvider culture = new System.Globalization.CultureInfo("es-ES", true);
            sMessage = dt.ToString(culture) + ";" + sMessage;
            int iMaxStrLength = sMessage.Length < MAX_CHARS ? sMessage.Length : MAX_CHARS;
            sMessage = sMessage.Substring(0,iMaxStrLength);
            sMessage = sMessage.Replace(System.Environment.NewLine, " ");
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(sFile, true))
                {
                    file.WriteLine(sMessage);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Windows.MessageBox.Show(string.Format("Error creating Log entry at file {0}",sFile));
#endif
                System.Diagnostics.Debug.WriteLine("Excepción al crear una entrada en el log: {0}", ex.Message);
            }
        }

        #region Convert from Hexadecimal string to Color
        /// <summary>
        /// Convert a hex string to a RGB Color object. (Without Alpha)
        /// </summary>
        /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
        public static Color HexStringToColor(string hexColor)
        {
            Color cEmpty = Color.FromRgb(255, 0, 255);
            string hc = ExtractHexDigits(hexColor);
            if (hc.Length != 8)
            {
                return cEmpty;
            }
            string a = hc.Substring(0, 2);
            string r = hc.Substring(2, 2);
            string g = hc.Substring(4, 2);
            string b = hc.Substring(6, 2);
            Color color = cEmpty;
            try
            {
                int ai = Int32.Parse(a, System.Globalization.NumberStyles.HexNumber);
                int ri = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                color = Color.FromArgb((byte)ai, (byte)ri, (byte)gi, (byte)bi);
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaUtils.HexStringToColor: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return cEmpty;
            }
            return color;
        }
        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        public static string ExtractHexDigits(string input)
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            string newnum = "";
            foreach (char c in input)
            {
                if (isHexDigit.IsMatch(c.ToString()))
                    newnum += c.ToString();
            }
            return newnum;
        }
        #endregion  

        /// <summary>
        /// Genera una ruta relativa a partir de una ruta absoluta dada 
        /// </summary>
        /// <param name="fromPath">Es el directorio desde el que queremos que se genere la ruta relativa</param>
        /// <param name="toPath">El directorio que queremos transformar</param>
        /// <returns>Devuelve el path relativo de toPath a partir del fromPath </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static bool ExistsFileInFolderAndSubfolders(string startDirectory, string fileName)
        {
            var Files = Directory.GetFiles(startDirectory, fileName, SearchOption.AllDirectories).FirstOrDefault();
            bool bFound = Files != null;
            return bFound;
        }

        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            BitmapSource bitmapSrc = null;
            if (source != null)
            {
                IntPtr hBitmap = IntPtr.Zero;
                try
                {
                    hBitmap = source.GetHbitmap();
                    bitmapSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    bitmapSrc = null;
                }
                finally
                {
                    if (hBitmap != IntPtr.Zero)
                    {
                        NativeMethods.DeleteObject(hBitmap);
                    }
                }
            }
            return bitmapSrc;
        }

        /// <summary>
        /// Esta función la utilizo para cargar un bitmap como un bitmapSource que puede utilizarse en un control de Imagen WPF
        /// </summary>        
        public static System.Windows.Media.Imaging.BitmapSource ConvertToBitmapSrc(System.Drawing.Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }
    }
}
