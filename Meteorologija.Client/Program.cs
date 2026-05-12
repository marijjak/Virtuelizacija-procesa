using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Threading;
using Meteorologija.Common;

namespace Meteorologija.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = ConfigurationManager.AppSettings["csvPath"];
            string rejectLogPath = "rejects.log";

            Console.WriteLine("[CLIENT] Citam CSV: " + csvPath);

            CsvReader csvReader = new CsvReader(csvPath);
            List<WeatherSample> samples = csvReader.ReadFirst100(rejectLogPath);

            Console.WriteLine("[CLIENT] Ucitano " + samples.Count + " uzoraka.");

            ChannelFactory<IWeatherService> factory = null;
            IWeatherService proxy = null;

            try
            {
                factory = new ChannelFactory<IWeatherService>("WeatherService");
                proxy = factory.CreateChannel();

                SessionMeta meta = new SessionMeta
                {
                    StationName = "Stanica-1",
                    TotalSamples = samples.Count,
                    T = samples.Count > 0 ? samples[0].T : 0,
                    Pressure = samples.Count > 0 ? samples[0].Pressure : 0,
                    Tpot = samples.Count > 0 ? samples[0].Tpot : 0,
                    Tdew = samples.Count > 0 ? samples[0].Tdew : 0,
                    Rh = samples.Count > 0 ? samples[0].Rh : 0,
                    Sh = samples.Count > 0 ? samples[0].Sh : 0,
                    Date = samples.Count > 0 ? samples[0].Date : ""
                };

                string startResponse = proxy.StartSession(meta);
                Console.WriteLine("[CLIENT] StartSession: " + startResponse);

                for (int i = 0; i < samples.Count; i++)
                {
                    try
                    {
                        string response = proxy.PushSample(samples[i]);
                        Console.WriteLine("[CLIENT] Sample " + (i + 1) + ": " + response);

                        
                        Thread.Sleep(500);
                    }
                    catch (FaultException<ValidationFault> ex)
                    {
                        Console.WriteLine("[CLIENT] Validaciona greska na redu " + (i + 1) + ": " + ex.Detail.Message);
                    }
                    catch (FaultException<DataFormatFault> ex)
                    {
                        Console.WriteLine("[CLIENT] Format greska na redu " + (i + 1) + ": " + ex.Detail.Message);
                    }
                }

                string endResponse = proxy.EndSession();
                Console.WriteLine("[CLIENT] EndSession: " + endResponse);

                factory.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CLIENT] Greska: " + ex.Message);

                if (factory != null)
                    factory.Abort();
            }

            Console.WriteLine("[CLIENT] Gotovo. Pritisni ENTER.");
            Console.ReadLine();
        }
    }
}