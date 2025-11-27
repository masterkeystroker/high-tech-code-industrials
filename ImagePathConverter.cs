using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace CretaBase
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                //Si el valor de imagen es null no seguir
                if (value == null || value.ToString() == "")
                {
                    return null;
                    //value = "/CretaBase;component/Images/Void.png";
                    //value = "pack://application:,,,/CretaBase;component/Images/Transparent.png";
                    //value = "pack://application:,,,/Images/Transparent.png";
                }
                // value contains the full path to the image
                string path = (string)value;

                // load the image, specify CacheOption so the file is not locked
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;//Lo he añadido para evitar que se quede en memoria
                image.UriSource = new Uri(path);
                image.EndInit();

                return image;
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.ImagePathConverter.Convert: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }

    public sealed class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    Bitmap bitmap = (Bitmap)value;
                    using (MemoryStream memory = new MemoryStream())
                    {
                        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                        memory.Position = 0;
                        BitmapImage bitmapimage = new BitmapImage();
                        bitmapimage.BeginInit();
                        bitmapimage.StreamSource = memory;
                        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapimage.EndInit();

                        return bitmapimage;
                    }
                }
                else { return new BitmapImage(); }
            }
            catch { return new BitmapImage(); }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
