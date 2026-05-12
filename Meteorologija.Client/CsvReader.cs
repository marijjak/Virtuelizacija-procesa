using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Meteorologija.Common;

namespace Meteorologija.Client
{
    public class CsvReader
    {
        private string _path;

        public CsvReader(string path)
        {
            _path = path;
        }

        public List<WeatherSample> ReadFirst100(string rejectLogPath)
        {
            List<WeatherSample> samples = new List<WeatherSample>();

            using (StreamReader reader = new StreamReader(_path))
            {
               
                string header = reader.ReadLine();
                int rowNumber = 0;

                while (!reader.EndOfStream && rowNumber < 100)
                {
                    string line = reader.ReadLine();
                    rowNumber++;

                    try
                    {
                        WeatherSample sample = ParseLine(line, rowNumber);
                        samples.Add(sample);
                    }
                    catch (Exception ex)
                    {
                        
                        LogReject(rejectLogPath, rowNumber, line, ex.Message);
                    }
                }
            }

            return samples;
        }

        private WeatherSample ParseLine(string line, int rowNumber)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new Exception("Prazan red");

            string[] parts = line.Split(',');

            if (parts.Length < 7)
                throw new Exception("Nedovoljno kolona: " + parts.Length);

            WeatherSample sample = new WeatherSample();

            sample.Date = parts[0].Trim();
            sample.Pressure = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
            sample.T = double.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
            sample.Tpot = double.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
            sample.Tdew = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
            sample.Rh = double.Parse(parts[5].Trim(), CultureInfo.InvariantCulture);
            sample.Sh = double.Parse(parts[6].Trim(), CultureInfo.InvariantCulture);

            return sample;
        }

        private void LogReject(string logPath, int rowNumber, string line, string reason)
        {
            using (StreamWriter writer = new StreamWriter(logPath, append: true))
            {
                writer.WriteLine(string.Format("[RED {0}] Razlog: {1} | Sadrzaj: {2}",
                    rowNumber, reason, line));
            }
            Console.WriteLine("[CLIENT] Nevalidan red " + rowNumber + ": " + reason);
        }
    }
}