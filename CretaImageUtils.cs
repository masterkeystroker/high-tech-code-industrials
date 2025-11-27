using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using BitMiracle.LibTiff.Classic;

namespace CretaBase
{
    public static class CretaImageUtils
    {
        #region Save to JPG
        /// <summary>
        /// Convert a Bitmap image into a jpg
        /// </summary>
        /// <param name="path"></param>
        /// <param name="img"></param>
        /// <param name="quality"></param>
        public static bool saveJpeg(string path, Bitmap img, long quality)
        {
            bool bOk = true;

            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            if (jpegCodec != null)
            {
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;

                try
                {
                    img.Save(path, jpegCodec, encoderParams);
                    img.Dispose();
                }
                catch (Exception ex)
                {
                    string sMessage = "LOG_EXCEPTION;Error Base.CretaImageUtils.saveJpeg: " + ex.Message + ex.StackTrace;
                    CretaUtils.WriteLogEvent(sMessage);
                    bOk = false;
                }
            }
            else
                bOk = false;

            return bOk;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
        #endregion

        #region Crop
        /// <summary>
        /// Creates an jpg image like a thumbnail from other image file (of any type)
        /// </summary>
        /// <param name="sInputFile">The path of the origin image file</param>
        /// <param name="iWidth">The width of result image</param>
        /// <param name="iHeight">The height of result image</param>
        /// <param name="sOutput">The result file path, if nothing is specified the result file will be the same filename plus "sm.jpg"</param>
        /// <param name="Quality">The jpg quality, 75% by default</param>
        /// <param name="x">The X start position from original image</param>
        /// <param name="y">The Y start position from original image</param>
        /// <param name="sizeFactor">The factor of zoom aplied to the original image (minor than 1 -> Zoom In) (more than 1 -> Zoom Out)</param>
        /// <returns></returns>
        public static bool SaveCropImage(string sInputFile, int iWidth, int iHeight, string sOutput = "", long Quality = 75, int x=0, int y=0, double sizeFactor=1)
        {
            bool bOk = true;
            try
            {
                Bitmap bmpOrigin = new Bitmap(sInputFile);
                //Ajusto el factor para que el trozo elegido quepa en la imagen
                if ((iWidth * sizeFactor) > bmpOrigin.Width)
                {
                    sizeFactor = (double)bmpOrigin.Width / (double)iWidth;
                }
                if ((iHeight * sizeFactor) > bmpOrigin.Height)
                {
                    double sizeFactorH = (double)bmpOrigin.Height / (double)iHeight;
                    sizeFactor = sizeFactorH > sizeFactor ? sizeFactor : sizeFactorH;
                }
                int FinalWidth = (int)(iWidth*sizeFactor);
                int FinalHeight = (int)(iHeight*sizeFactor);
                
                //Limits of the final rectangle size 
                if (FinalWidth + x > bmpOrigin.Width) FinalWidth = bmpOrigin.Width;
                    //if (FinalWidth + x < iWidth) FinalWidth = iWidth;
                if (FinalHeight + y > bmpOrigin.Height) FinalHeight = bmpOrigin.Height;
                    //if (FinalHeight + y < iHeight) FinalHeight = iHeight;

                //Get a rectangle from original image, this rectangle size determines the zoom, smaller rectangle more zoom in, bigger rectangle more zoom out.
                Rectangle rect = new Rectangle(x, y, FinalWidth, FinalHeight);
                Bitmap ResizedBmp = cropImage(bmpOrigin, rect);
                //Finally resize the previous resized bmp to the original required size
                Bitmap newBitmap = new System.Drawing.Bitmap(ResizedBmp, iWidth, iHeight);

                if (sOutput == "")
                {
                    int iIdxExt = sInputFile.LastIndexOf('.');
                    sOutput = sInputFile.Substring(0, iIdxExt) + "sm.jpg";
                }
                bOk = saveJpeg(sOutput, newBitmap, Quality);
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaImageUtils.SaveCropImage: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                bOk = false;
            }
            return bOk;
        }
        public static Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea,
            bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }
        public static Bitmap cropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpCrop = img.Clone(cropArea, img.PixelFormat);
            return bmpCrop;
        }
        #endregion

        #region Resize
        public static Image resizeImage(Image imgToResize, int iWidth, int iHeight, bool bLockSize = true)
        {
            return (Image)resizeImage((Bitmap)imgToResize,iWidth,iHeight,bLockSize);
        }
        public static Bitmap resizeImage(Bitmap imgToResize, int iWidth, int iHeight, bool bLockSize = true)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)iWidth / (float)sourceWidth);
            nPercentH = ((float)iHeight / (float)sourceHeight);

            if (bLockSize)
            {
                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;

                nPercentW = nPercentH = nPercent;
            }

            int destWidth = (int)(sourceWidth * nPercentW);
            int destHeight = (int)(sourceHeight * nPercentH);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }
        #endregion

        #region SavePreviewFile (Save JPG with resize and qualitylevel)
        public static bool SavePreview(string sInputFile, int iWidth=0, int iHeight=0, long Quality=75,string sOutput="", bool bLockAspectRatio=true)
        {
            bool bOk = true;

            //Load the tiff file
            try
            {
                Bitmap newBitmap = CrearBitmap(sInputFile, iWidth, iHeight);
                //Bitmap newBitmap = GenerarBitmapTest(sInputFile);
                /*if (iWidth != 0 && iHeight != 0)
                {
                    newBitmap = resizeImage(newBitmap, iWidth, iHeight, bLockAspectRatio);
                }*/

                //save the file into JPG file
                //if (imgFormat == )
                {
                    if (sOutput == "")
                    {
                        int iIdxExt = sInputFile.LastIndexOf('.');
                        sOutput = sInputFile.Substring(0, iIdxExt) + ".jpg";
                    }
                    bOk = saveJpeg(sOutput, newBitmap, Quality);
                }
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaImageUtils.SavePreview: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                bOk = false;
            }

            return bOk;
        }
        #endregion
        
        #region Generate bitmap
        public static bool CrearMiniatura(string Origen, string Destino, int Width, int Height, System.Drawing.Imaging.ImageFormat Formato)
        {
            bool bOk = true;
            Bitmap bmp = CrearBitmap(Origen, Width, Height);

            if (bmp == null) bOk = false;

            try
            {
                bmp.Save(Destino, Formato);
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaImageUtils.CrearMiniatura: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                bOk = false;
            }
            return bOk;
        }
        public static Bitmap CrearBitmap(string Origen, int Width, int Height, bool bLockAspectRatio=true)
        {
            try
            {
                double C, M, Y, K, xC, xM, xY, xK;
                double C1, M1, Y1, K1, xC1, xM1, xY1, xK1;
                double C2, M2, Y2;
                double R, G, B;

                // Open the TIFF image
                using (BitMiracle.LibTiff.Classic.Tiff image = BitMiracle.LibTiff.Classic.Tiff.Open(Origen, "r"))
                {
                    if (image == null) return null;

                    //Lee el ancho original
                    BitMiracle.LibTiff.Classic.FieldValue[] value = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGEWIDTH);
                    int imageWidth = value[0].ToInt();

                    //Lee el alto original
                    value = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGELENGTH);
                    int imageHeight = value[0].ToInt();

                    //Miro si tengo que guardar relación de aspecto
                    if (bLockAspectRatio)
                    {
                        float nPercent = 0;
                        float nPercentW = 0;
                        float nPercentH = 0;

                        nPercentW = ((float)Width / (float)imageWidth);
                        nPercentH = ((float)Height / (float)imageHeight);
                        if (nPercentH < nPercentW)
                            nPercent = nPercentH;
                        else
                            nPercent = nPercentW;

                        Width = (int)(imageWidth * nPercent);
                        Height = (int)(imageHeight * nPercent);
                    }

                    //Lee los bits por color
                    value = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.BITSPERSAMPLE);
                    int BitsColor = value[0].ToInt();

                    //Lee los canales por pixel
                    value = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.SAMPLESPERPIXEL);
                    int Canales = value[0].ToInt();

                    //Test
                    value = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.ICCPROFILE);
                    object iccProfile = null;
                    if(value != null) iccProfile = value[0].ToInt();

                    value = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.CURRENTICCPROFILE);
                    iccProfile = null;
                    if (value != null) iccProfile = value[0].ToInt();

                    //=====================================================================================================

                    // Reserva memoria para el Bitmap
                    byte[] bmpData2 = new byte[(int)(Width * Height * 3)];

                    // Reserva memoria para una linea de la imagen original
                    int scanlineSize = imageWidth * Canales * (BitsColor / 8);
                    byte[] Row = new byte[scanlineSize];

                    //Contadores para las posiciones de bytes de la imagen de origen y de destino
                    int ContDest = 0;
                    int ContOrig = 0;

                    //Representa una linea de destino de la imagen de destino para hacer la media
                    double[,] Linea = new Double[Width, 4];

                    //Pone la linea a 0
                    for (int i = 0; i < Width; i++)
                    {
                        Linea[i, 0] = 0.0;
                        Linea[i, 1] = 0.0;
                        Linea[i, 2] = 0.0;
                        Linea[i, 3] = 0.0;
                    }

                    //Repasa las lineas de la imagen original
                    for (int row = 0; row < imageHeight; row++)
                    {
                        //Escribe el progreso de la miniatura
                        //_ProgresoMiniatura = (int)(100 * row / imageLength);

                        //Lee una linea de la imagen original
                        if (!image.ReadScanline(Row, row)) return null;

                        //Pone el contador de la imagen 0
                        ContOrig = 0;

                        //Repasa los pixeles de esta linea
                        for (int i = 0; i < imageWidth; i++)
                        {
                            //http://web.forret.com/tools/color.asp?C=0.709&M=0.368&Y=0.227&K=0.031
                            //http://www.usq.edu.au/users/grantd/WORK/216color/ConvertRGB-CMYK-Grey.htm

                            //Extrae los bytes de un pixel
                            if (Canales >= 1) C = Row[(ContOrig * Canales) + 0]; else C = 0.0;
                            if (Canales >= 2) M = Row[(ContOrig * Canales) + 1]; else M = 0.0;
                            if (Canales >= 3) Y = Row[(ContOrig * Canales) + 2]; else Y = 0.0;
                            if (Canales >= 4) K = Row[(ContOrig * Canales) + 3]; else K = 0.0;
                            if (Canales >= 5) xC = Row[(ContOrig * Canales) + 4]; else xC = 0.0;
                            if (Canales >= 6) xM = Row[(ContOrig * Canales) + 5]; else xM = 0.0;
                            if (Canales >= 7) xY = Row[(ContOrig * Canales) + 6]; else xY = 0.0;
                            if (Canales >= 8) xK = Row[(ContOrig * Canales) + 7]; else xK = 0.0;

                            //Conversion de CMYK 0-255 0-1
                            C1 = C / 255.0;
                            M1 = M / 255.0;
                            Y1 = Y / 255.0;
                            K1 = K / 255.0;
                            xC1 = xC / 255.0;
                            xM1 = xM / 255.0;
                            xY1 = xY / 255.0;
                            xK1 = xK / 255.0;

                            //Convierte de CMYK a CMY
                            C2 = C1 * (1.0 - K1) + 1.0 * K1;
                            if (C2 > 1.0) C2 = 1.0;

                            M2 = M1 * (1.0 - K1) + 1.0 * K1;
                            if (M2 > 1.0) M2 = 1.0;

                            Y2 = Y1 * (1.0 - K1) + 1.0 * K1;
                            if (Y2 > 1.0) Y2 = 1.0;

                            //Pasa de CMY a RGB
                            R = (1.0 - C2) * 255.0;
                            G = (1.0 - M2) * 255.0;
                            B = (1.0 - Y2) * 255.0;

                            int x = (int)(ContOrig * Width / imageWidth);
                            if (x >= Width) x = Width;
                            Linea[x, 0] += R;
                            Linea[x, 1] += G;
                            Linea[x, 2] += B;
                            Linea[x, 3] += 1.0;

                            //Incrementa el puntero
                            ContOrig++;
                        }

                        //Aqui detecta el cambio de linea del destino
                        //ContDest indica el numero de linea donde apunta el destino

                        int y = (int)(row * Height / imageHeight);
                        if (y >= Height) y = Height;
                        if (y > ContDest)
                        {
                            //Pasa la Linea al destino
                            for (int i = 0; i < Width; i++)
                            {
                                //Escribe el pixel en el area de memoira
                                int p = (ContDest * Width * 3) + (i * 3);
                                bmpData2[p + 0] = (byte)(Linea[i, 0] / Linea[i, 3]);
                                bmpData2[p + 1] = (byte)(Linea[i, 1] / Linea[i, 3]);
                                bmpData2[p + 2] = (byte)(Linea[i, 2] / Linea[i, 3]);
                            }

                            ContDest = y;

                            //Pone la linea a 0
                            for (int i = 0; i < Width; i++)
                            {
                                Linea[i, 0] = 0.0;
                                Linea[i, 1] = 0.0;
                                Linea[i, 2] = 0.0;
                                Linea[i, 3] = 0.0;
                            }
                        }
                    }

                    //Pasa la ultima Linea al destino
                    ContDest = Height - 1;
                    for (int i = 0; i < Width; i++)
                    {
                        //Escribe el pixel en el area de memoria
                        int p = (ContDest * Width * 3) + (i * 3);
                        bmpData2[p + 0] = (byte)(Linea[i, 0] / Linea[i, 3]);
                        bmpData2[p + 1] = (byte)(Linea[i, 1] / Linea[i, 3]);
                        bmpData2[p + 2] = (byte)(Linea[i, 2] / Linea[i, 3]);
                    }

                    //Crea un bmp 
                    Bitmap bmpx = new Bitmap(Width, Height);
                    
                    for (int i = 0; i < Width; ++i)
                        for (int j = 0; j < Height; ++j)
                            bmpx.SetPixel(i, j, Color.FromArgb(bmpData2[(j * Width * 3) + (i * 3) + 0], bmpData2[(j * Width * 3) + (i * 3) + 1], bmpData2[(j * Width * 3) + (i * 3) + 2]));

                    //pasar el bmp por referencia y hacer el save en otra función    
                    return bmpx;
                }
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaImageUtils.CrearBitmap: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return null;
            }
        }
        #endregion

        public static bool GetTiffInfo(string sTiffFile, ref int width, ref int heigh, ref int channels, ref int bitsPerChannel, ref int resX, ref int resY)
        { 
            bool bOk = true;

            if (System.IO.File.Exists(sTiffFile))
            {
                Creta.Tiff.TiffInfo TifInfo = Creta.Tiff.cTiff.Info(sTiffFile);

                bitsPerChannel = TifInfo.BitsCanal;
                channels = TifInfo.CanalesPixel;
                width = TifInfo.Width;
                heigh = TifInfo.Heigh;
                resX = TifInfo.ResX;
                resY = TifInfo.ResY;
            }
            else
                bOk = false;

            return bOk;
        }

        public static bool IsCMYK(string sTiffFile)
        {
            bool IsCMYK = false;

            if (System.IO.File.Exists(sTiffFile))
            {
                Creta.Tiff.TiffInfo TifInfo = Creta.Tiff.cTiff.Info(sTiffFile);
                IsCMYK = TifInfo.PhotoMetric == "SEPARATED";
            }

            return IsCMYK;
        }

        #region Provisional No funciona correctamente
        public static void Rotate(string fileName, eRotation eRot)
        {
            try
            {
                using (Tiff input = Tiff.Open(fileName, "r"))
                {
                    int width = input.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                    int height = input.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                    int samplesPerPixel = input.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                    int bitsPerSample = input.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                    int photo = input.GetField(TiffTag.PHOTOMETRIC)[0].ToInt();
                    FieldValue[] test = input.GetField(TiffTag.ORIENTATION);

                    int scanlineSize = input.ScanlineSize();
                    byte[][] buffer = new byte[height][];
                    for (int i = 0; i < height; ++i)
                    {
                        buffer[i] = new byte[scanlineSize];
                        input.ReadScanline(buffer[i], i);
                    }
                    string sFolder = System.IO.Path.GetDirectoryName(fileName);
                    using (Tiff output = Tiff.Open(sFolder + "\\SimpleTiffCopy.tif", "w"))
                    {
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, samplesPerPixel);
                        output.SetField(TiffTag.BITSPERSAMPLE, bitsPerSample);
                        output.SetField(TiffTag.ROWSPERSTRIP, output.DefaultStripSize(0));
                        output.SetField(TiffTag.PHOTOMETRIC, photo);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                        // change orientation of the image
                        output.SetField(TiffTag.ORIENTATION, Orientation.LEFTBOT);

                        for (int i = 0; i < height; ++i)
                            output.WriteScanline(buffer[i], i);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public enum eRotation
        {
            ROT_90 = 90,
            ROT_180 = 180,
            ROT_270 = 270,
        }
        public static void RotateTiff(string sFileInput, string sFileOutput,eRotation eRot)
        {

            string outputFileName = sFileOutput;

            using (Tiff input = Tiff.Open(sFileInput, "r"))
            {
                using (Tiff output = Tiff.Open(outputFileName, "w"))
                {
                    for (short page = 0; page < input.NumberOfDirectories(); page++)
                    {
                        input.SetDirectory(page);
                        output.SetDirectory(page);

                        int width = input.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                        int height = input.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                        int samplesPerPixel = input.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                        int bitsPerSample = input.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                        int photo = input.GetField(TiffTag.PHOTOMETRIC)[0].ToInt();

                        int[] raster = new int[width * height];
                        input.ReadRGBAImageOriented(width, height, raster, Orientation.TOPLEFT);

                        raster = rotate(raster, (int)eRot, ref width, ref height);

                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.COMPRESSION, Compression.DEFLATE);

                        byte[] strip = rasterToRgbBuffer(raster);
                        output.WriteEncodedStrip(0, strip, strip.Length);

                        output.WriteDirectory();
                    }
                }
            }
        }

        private static byte[] rasterToRgbBuffer(int[] raster)
        {
            byte[] buffer = new byte[raster.Length * 3];
            for (int i = 0; i < raster.Length; i++)
                Buffer.BlockCopy(raster, i * 4, buffer, i * 3, 3);

            return buffer;
        }

        private static int[] rotate(int[] buffer, int angle, ref int width, ref int height)
        {
            int rotatedWidth = width;
            int rotatedHeight = height;
            int numberOf90s = angle / 90;
            if (numberOf90s % 2 != 0)
            {
                int tmp = rotatedWidth;
                rotatedWidth = rotatedHeight;
                rotatedHeight = tmp;
            }

            int[] rotated = new int[rotatedWidth * rotatedHeight];

            for (int h = 0; h < height; ++h)
            {
                for (int w = 0; w < width; ++w)
                {
                    int item = buffer[h * width + w];
                    int x = 0;
                    int y = 0;
                    switch (numberOf90s % 4)
                    {
                        case 0:
                            x = w;
                            y = h;
                            break;

                        case 1:
                            x = (height - h - 1);
                            y = (rotatedHeight - 1) - (width - w - 1);
                            break;

                        case 2:
                            x = (width - w - 1);
                            y = (height - h - 1);

                            break;

                        case 3:
                            x = (rotatedWidth - 1) - (height - h - 1);
                            y = (width - w - 1);
                            break;
                    }

                    rotated[y * rotatedWidth + x] = item;
                }
            }

            width = rotatedWidth;
            height = rotatedHeight;
            return rotated;
        }
        #endregion

    }
}

