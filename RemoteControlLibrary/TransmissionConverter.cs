using System.Text;

namespace RemoteShutdownLibrary
{
    public class TransmissionConverter
    {
        public static string ConvertByteToString(byte[] data)
        {
            return Encoding.ASCII.GetString(data, 0, data.Length);
        }

        public static byte[] ConvertStringToByte(string cmd)
        {
            return Encoding.ASCII.GetBytes(cmd);
        }
    }
}