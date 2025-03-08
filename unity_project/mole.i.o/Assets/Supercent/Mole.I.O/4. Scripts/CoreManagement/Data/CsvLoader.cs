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
        private readonly string _defaultUrl = "https://script.google.com/macros/s/AKfycbwCZDqK107c4pF0jNj1JtRWI4d34k12OhU_3uiG5mg6eT7qELG6yJcpBpeEcWBimZc/exec";
        [SerializeField] DynamicGameData _gameData;

        public DynamicGameData GetDynamicGameData() => _gameData;
        public async void StartLoadText()
        {
            string csvData = await LoadDataGoogleSheet(_defaultUrl);
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
            var lines = csvData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries); // 줄단위로 분리
            List<LoadedData> LoadedDatas = new List<LoadedData>();

            if (lines.Length > 0)
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    var columns = lines[i].Split(',');
                    LoadedData entry = new LoadedData();

                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 1)
                        {
                            entry.Key = int.Parse(columns[j].Trim('"'));
                        }
                        else if (j == 2)
                        {
                            entry.Value = float.Parse(columns[j].Trim('"'));
                        }
                    }

                    LoadedDatas.Add(entry);
                }

                _gameData.LoadedDatas = LoadedDatas;
            }
        }

    }
}