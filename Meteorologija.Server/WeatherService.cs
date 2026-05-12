using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using Meteorologija.Common;

namespace Meteorologija.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WeatherService : IWeatherService
    {
        private FileWriterWrapper _writer;
        private FileWriterWrapper _rejectsWriter;
        private bool _sessionActive = false;
        private string _sessionFolder;

       
        private WeatherSample _previousSample = null;

        
        private double _tSum = 0;
        private int _tCount = 0;

       
        private double T_threshold;
        private double RH_threshold;
        private double DEW_threshold;

        public WeatherService()
        {
            T_threshold = double.Parse(ConfigurationManager.AppSettings["T_threshold"]);
            RH_threshold = double.Parse(ConfigurationManager.AppSettings["RH_threshold"]);
            DEW_threshold = double.Parse(ConfigurationManager.AppSettings["DEW_threshold"]);
        }

        public string StartSession(SessionMeta meta)
        {
            try
            {
                
                if (meta == null)
                    throw new FaultException<DataFormatFault>(
                        new DataFormatFault("Meta podaci su null", "meta"),
                        new FaultReason("Nevalidni meta podaci"));

                
                string basePath = ConfigurationManager.AppSettings["storagePath"];
                _sessionFolder = Path.Combine(basePath, "session_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Directory.CreateDirectory(_sessionFolder);

                
                string measurementsPath = Path.Combine(_sessionFolder, "measurements_session.csv");
                string rejectsPath = Path.Combine(_sessionFolder, "rejects.csv");

                _writer = new FileWriterWrapper(measurementsPath, append: true);
                _rejectsWriter = new FileWriterWrapper(rejectsPath, append: true);


                _writer.WriteLine("Date,T,Pressure,Tpot,Tdew,Rh,Sh");

                _sessionActive = true;
                _previousSample = null;
                _tSum = 0;
                _tCount = 0;

                Console.WriteLine("[SERVER] Sesija zapoceta: " + _sessionFolder);
                return "ACK";
            }
            catch (FaultException<DataFormatFault>)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault(ex.Message, "StartSession"),
                    new FaultReason(ex.Message));
            }
        }

        public string PushSample(WeatherSample sample)
        {
            if (!_sessionActive)
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("Nema aktivne sesije", "session"),
                    new FaultReason("Nema aktivne sesije"));

           
            try
            {
                ValidateSample(sample);
            }
            catch (FaultException<ValidationFault> ex)
            {
                
                _rejectsWriter.WriteLine(sample.Date + ",REJECTED," + ex.Detail.Message);
                _rejectsWriter.Flush();
                return "NACK";
            }

           
            _writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
                sample.Date, sample.T, sample.Pressure,
                sample.Tpot, sample.Tdew, sample.Rh, sample.Sh));
            _writer.Flush();

            Console.WriteLine("[SERVER] prenos u toku... " + sample.Date);

           
            CheckTemperatureSpike(sample);
            CheckHumiditySpike(sample);
            CheckDewSpike(sample);

            _previousSample = sample;

            return "ACK|IN_PROGRESS";
        }

        public string EndSession()
        {
            if (_writer != null) { _writer.Dispose(); _writer = null; }
            if (_rejectsWriter != null) { _rejectsWriter.Dispose(); _rejectsWriter = null; }
            _sessionActive = false;
            Console.WriteLine("[SERVER] zavrsen prenos.");
            return "ACK|COMPLETED";
        }

        private void ValidateSample(WeatherSample s)
        {
            if (s == null)
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Sample je null", "sample", 0),
                    new FaultReason("Sample je null"));

            if (s.Rh <= 0)
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Relativna vlaznost mora biti > 0", "Rh", s.Rh),
                    new FaultReason("Nevalidna Rh vrednost"));

            if (s.Rh > 100)
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Relativna vlaznost mora biti <= 100", "Rh", s.Rh),
                    new FaultReason("Nevalidna Rh vrednost"));

            if (s.Pressure <= 0)
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Pritisak mora biti > 0", "Pressure", s.Pressure),
                    new FaultReason("Nevalidni pritisak"));

            if (string.IsNullOrEmpty(s.Date))
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("Datum je obavezan", "Date"),
                    new FaultReason("Nedostaje datum"));
        }

        private void CheckTemperatureSpike(WeatherSample current)
        {
           
            _tSum += current.T;
            _tCount++;
            double tMean = _tSum / _tCount;

            if (_previousSample != null)
            {
                double deltaT = current.T - _previousSample.T;
                if (Math.Abs(deltaT) > T_threshold)
                {
                    string direction = deltaT > 0 ? "iznad ocekivanog" : "ispod ocekivanog";
                    Console.WriteLine("[ALARM] TemperatureSpike! DeltaT=" + deltaT.ToString("F2") + " - " + direction);
                }
            }

            
            if (_tCount > 1)
            {
                if (current.T < 0.75 * tMean)
                    Console.WriteLine("[UPOZORENJE] OutOfBandWarning: T=" + current.T + " ispod ocekivane vrednosti (mean=" + tMean.ToString("F2") + ")");
                else if (current.T > 1.25 * tMean)
                    Console.WriteLine("[UPOZORENJE] OutOfBandWarning: T=" + current.T + " iznad ocekivane vrednosti (mean=" + tMean.ToString("F2") + ")");
            }
        }

        private void CheckHumiditySpike(WeatherSample current)
        {
            if (_previousSample == null) return;
            double deltaRH = current.Rh - _previousSample.Rh;
            if (Math.Abs(deltaRH) > RH_threshold)
            {
                string direction = deltaRH > 0 ? "iznad ocekivanog" : "ispod ocekivanog";
                Console.WriteLine("[ALARM] RHSpike! DeltaRH=" + deltaRH.ToString("F2") + " - " + direction);
            }
        }

        private void CheckDewSpike(WeatherSample current)
        {
            if (_previousSample == null) return;
            double deltaDew = current.Tdew - _previousSample.Tdew;
            if (Math.Abs(deltaDew) > DEW_threshold)
            {
                string direction = deltaDew > 0 ? "iznad ocekivanog" : "ispod ocekivanog";
                Console.WriteLine("[ALARM] DEWSpike! DeltaDew=" + deltaDew.ToString("F2") + " - " + direction);
            }
        }
    }
}