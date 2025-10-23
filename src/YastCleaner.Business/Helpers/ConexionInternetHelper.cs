using System.Net.NetworkInformation;

namespace YastCleaner.Business.Helpers
{
    public static class ConexionInternetHelper
    {
        public static bool HayConexionInternet()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch
            {
                return false;
            }
        }
    }
}
