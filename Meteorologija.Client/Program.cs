using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
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

            // citanje CSV-a
            CsvReader csvReader = new CsvReader(csvPath);
            List<WeatherSample> samples = csvReader.ReadFirst100(rejectLogPath);

            Console.WriteLine("[CLIENT] Ucitano " + samples.Count + " uzoraka.");

            // povezivanje na server
            ChannelFactory<IWeatherService> factory = null;
            IWeatherService proxy = null;

            try
            {
                factory = new ChannelFactory<IWeatherService>("WeatherService");
                proxy = factory.CreateChannel();

                // StartSession
                SessionMeta meta = new SessionMeta
                {
                    StationName = "Stanica-1",
                    DatasetPath = csvPath,
                    TotalSamples = samples.Count
                };

                string startResponse = proxy.StartSession(meta);
                Console.WriteLine("[CLIENT] StartSession: " + startResponse);

                // PushSample - saljemo red po red
                for (int i = 0; i < samples.Count; i++)
                {
                    try
                    {
                        string response = proxy.PushSample(samples[i]);
                        Console.WriteLine("[CLIENT] Sample " + (i + 1) + ": " + response);
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

                // EndSession
                string endResponse = proxy.EndSession();
                Console.WriteLine("[CLIENT] EndSession: " + endResponse);

                factory.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CLIENT] Greska: " + ex.Message);
                if (factory != null) factory.Abort();
            }

            Console.WriteLine("[CLIENT] Gotovo. Pritisni ENTER.");
            Console.ReadLine();
        }
    }
}