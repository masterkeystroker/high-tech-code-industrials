using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CretaBase
{
    public static class CretaEncrypt
    {
        public static byte[] Encrypt_AES(byte[] Matriz, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (Matriz == null || Matriz.Length <= 0) throw new ArgumentNullException("Cadena");
            if (Key == null || Key.Length != 32) throw new ArgumentNullException("Key");
            if (IV == null || IV.Length != 16) throw new ArgumentNullException("VI");

            // Declare the streams used to encrypt to an in memory array of bytes.
            System.IO.MemoryStream msEncrypt = null;
            System.Security.Cryptography.CryptoStream csEncrypt = null;
            System.IO.BinaryWriter swEncrypt = null;

            // Declare the RijndaelManaged object used to encrypt the data.
            System.Security.Cryptography.RijndaelManaged aesAlg = null;

            // Declare the bytes used to hold the encrypted data.
            //byte[] encrypted = null;

            try
            {
                // Create a RijndaelManaged object with the specified key and IV.
                aesAlg = new System.Security.Cryptography.RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                System.Security.Cryptography.ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new System.IO.MemoryStream();
                csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write);
                swEncrypt = new System.IO.BinaryWriter(csEncrypt);

                //Write all data to the stream.
                swEncrypt.Write(Matriz);
            }
            finally
            {
                // Close the streams.
                if (swEncrypt != null) swEncrypt.Close();
                if (csEncrypt != null) csEncrypt.Close();
                if (msEncrypt != null) msEncrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null) aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();
        }
        public static byte[] Decrypt_AES(byte[] Encripted, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (Encripted == null || Encripted.Length <= 0) throw new ArgumentNullException("Encripted");
            if (Key == null || Key.Length != 32) throw new ArgumentNullException("Key");
            if (IV == null || IV.Length != 16) throw new ArgumentNullException("VI");

            // TDeclare the streams used to decrypt to an in memory array of bytes.
            System.IO.MemoryStream msDecrypt = null;
            System.Security.Cryptography.CryptoStream csDecrypt = null;
            System.IO.BinaryReader srDecrypt = null;

            // Declare the RijndaelManaged object used to decrypt the data.
            System.Security.Cryptography.RijndaelManaged aesAlg = null;

            List<byte> Lista = new List<byte>();
            byte[] Buf = new byte[5000];

            try
            {
                // Create a RijndaelManaged object with the specified key and IV.
                aesAlg = new System.Security.Cryptography.RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                System.Security.Cryptography.ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                msDecrypt = new System.IO.MemoryStream(Encripted);
                csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
                srDecrypt = new System.IO.BinaryReader(csDecrypt);

                while (true)
                {
                    int i = srDecrypt.Read(Buf, 0, Buf.Length);
                    if (i == 0)
                    {
                        break;
                    }
                    else
                    {
                        for (int x = 0; x < i; x++) Lista.Add(Buf[x]);
                    }
                }
                return Lista.ToArray();
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaEncrypt.Decrypt_AES: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return null;
            }
            finally
            {
                // Clean things up.
                // Close the streams.
                if (srDecrypt != null) srDecrypt.Close();
                if (csDecrypt != null) csDecrypt.Close();
                if (msDecrypt != null) msDecrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null) aesAlg.Clear();
            }
        }
        public static byte[] Compress(byte[] Datos)
        {
            try
            {
                //Comprime
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
                compressedzipStream.Write(Datos, 0, Datos.Length);
                compressedzipStream.Close();
                byte[] X = ms.ToArray();
                ms.Close();
                return X;
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaEncrypt.Compress: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return null;
            }
        }
        public static byte[] Decompress(byte[] Datos)
        {
            try
            {
                // Reset the memory stream position to begin decompression.
                System.IO.MemoryStream ms2 = new System.IO.MemoryStream(Datos);
                System.IO.Compression.GZipStream zipStream = new System.IO.Compression.GZipStream(ms2, System.IO.Compression.CompressionMode.Decompress);
                byte[] decompressedBuffer = new byte[1023];
                List<byte> Lista = new List<byte>();

                while (true)
                {
                    int bytesRead = zipStream.Read(decompressedBuffer, 0, decompressedBuffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        for (int i = 0; i < bytesRead; i++) Lista.Add(decompressedBuffer[i]);
                    }
                }

                zipStream.Close();
                return Lista.ToArray();
            }
            catch (Exception ex)
            {
                string sMessage = "LOG_EXCEPTION;Error Base.CretaEncrypt.Decompress: " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
                return null;
            }
        }
    }
}
