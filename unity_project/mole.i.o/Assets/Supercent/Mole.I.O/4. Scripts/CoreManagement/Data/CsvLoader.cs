using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
namespace Supercent.MoleIO.Management
{
    [Serializable]
    public class CSVLoader
    {
        [SerializeField] LocalizationData localizationData;
        // [SerializeField] bool _forceUpdate;
        public void StartLoadText(MonoBehaviour mono)
        {
            // mono.StartCoroutine(LoadCSV());
            LoadCSV();
        }

        // private IEnumerator LoadCSV()
        // {
        //     string filePath = Path.Combine(Application.streamingAssetsPath, "Localization.csv");
        //     string csvData = "";

        //     if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //     {
        //         UnityWebRequest request = UnityWebRequest.Get(filePath);
        //         yield return request.SendWebRequest();
        //         if (request.result == UnityWebRequest.Result.Success)
        //             csvData = request.downloadHandler.text;
        //         else
        //             Debug.LogError("CSV 파일을 찾을 수 없음: " + request.error);
        //     }
        //     else
        //     {
        //         if (File.Exists(filePath))
        //             csvData = File.ReadAllText(filePath);
        //         else
        //             Debug.LogError("CSV 파일을 찾을 수 없음! 경로: " + filePath);
        //     }

        //     if (!string.IsNullOrEmpty(csvData))
        //         ParseCSV(csvData);
        // }

        private void LoadCSV()
        {
            TextAsset csvFile = Resources.Load<TextAsset>("Localization");

            if (csvFile != null)
                ParseCSV(csvFile.text);
        }


        private void ParseCSV(string csvData)
        {
            string[] lines = csvData.Split('\n');
            localizationData.Texts = new List<LocalizedText>();

            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(',');
                if (columns.Length < 3) continue;

                LocalizedText entry = new LocalizedText
                {
                    key = columns[1].Trim(),
                    value = columns[2].Trim()
                };

                localizationData.Texts.Add(entry);
            }

            Debug.Log("CSV 파싱 완료! 총 " + localizationData.Texts.Count + "개의 번역 데이터 로드됨.");
        }
    }
}