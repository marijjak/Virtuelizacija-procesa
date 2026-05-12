using System;
using System.ServiceModel;

namespace Meteorologija.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(WeatherService));

            try
            {
                host.Open();
                Console.WriteLine("[SERVER] Servis pokrenut. Cekam klijenta...");
                Console.WriteLine("[SERVER] Pritisni ENTER za gasenje.");
                Console.ReadLine();
                host.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SERVER] Greska: " + ex.Message);
                host.Abort();
            }
        }
    }
}