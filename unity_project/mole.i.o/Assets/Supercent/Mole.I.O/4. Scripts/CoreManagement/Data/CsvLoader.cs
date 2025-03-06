using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Supercent.MoleIO.Management
{
    [Serializable]
    public class CSVLoader
    {
        private readonly string _url = "https://script.google.com/macros/s/AKfycbwCZDqK107c4pF0jNj1JtRWI4d34k12OhU_3uiG5mg6eT7qELG6yJcpBpeEcWBimZc/exec";
        [SerializeField] DynamicGameData localizationData;
        public async void StartLoadText()
        {
            string csvData = await LoadDataGoogleSheet(_url);
            if (csvData != null)
            {
                ParseCSV(csvData);
            }
        }

        async Task<string> LoadDataGoogleSheet(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] dataBytes = await client.GetByteArrayAsync(url);
                    return Encoding.UTF8.GetString(dataBytes);
                }
                catch (HttpRequestException e)
                {
                    Debug.LogError($"Request error: {e.Message}");
                    return null;
                }
            }
        }

        private void ParseCSV(string csvData)
        {
            localizationData.Texts = new List<LocalizedText>();
            csvData = csvData.Trim('[', ']');
            var items = csvData.Split(new[] { "},{" }, StringSplitOptions.None);

            for (int i = 0; i < items.Length; i++)
            {
                var cleanItem = items[i].Trim('{', '}');
                var keyValuePairs = cleanItem.Split(',');

                LocalizedText entry = new LocalizedText();
                foreach (var keyValue in keyValuePairs)
                {
                    var keyValuePair = keyValue.Split(':');
                    var key = keyValuePair[0].Trim('"');
                    var value = keyValuePair[1].Trim('"');
                    if (key == "Key")
                    {
                        entry.Key = int.Parse(value);
                    }
                    else if (key == "Value")
                    {
                        entry.Value = float.Parse(value);
                    }
                }

                localizationData.Texts.Add(entry);
            }
        }
    }
}