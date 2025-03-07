using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class LeaderBoard : MonoBehaviour
    {
        const int USER_CODE = -1;
        const int RAND_MAX_COUNT = 20;
        public TMP_Text[] _names;
        public TMP_Text[] _scores;

        private List<UnitBattleController> players = new List<UnitBattleController>();
        [SerializeField] string[] _botNames = { "Muncher", "Digby", "Snuffles", "Pebble", "Nibbles", "Foxtrot", "Golfer", "HotelMaster", "Clumpy", "Sooty", "Vev", "Paper", "Wully", "Melmer", "WaterLemon", "Lemini", "Bike", "Dot", "Rong", "MoonWalker", "Glue", "Flour", "Malson" };
        public int NameCount => _botNames.Length;
        StringBuilder _stringBuilder = new StringBuilder();

        public void RegistUnit(UnitBattleController unit)
        {
            players.Add(unit);
            unit.OnChangeXp += UpdateScores;
        }

        public void UpdateScores()
        {
            for (int i = 1; i < players.Count; i++)
            {
                int j = i;
                while (j > 0 && players[j - 1].Xp < players[j].Xp)
                {
                    (players[j - 1], players[j]) = (players[j], players[j - 1]);
                    j--;
                }
            }

            UpdateLeaderboard();
        }

        private void UpdateLeaderboard()
        {
            for (int i = 0; i < _names.Length; i++)
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(players[i].Xp);
                _names[i].text = players[i].PlayerCode == USER_CODE ? GameManager.PlayerName : _botNames[players[i].PlayerCode];
                _scores[i].text = _stringBuilder.ToString();
            }
        }

        public int GetRandomNameCode()
        {
            bool _isReady = false;
            int code = 1;
            int tryCount = 0;
            while (!_isReady)
            {
                tryCount++;
                code = Random.Range(1, NameCount);

                for (int i = 0; i < players.Count; i++)
                {
                    if (tryCount > RAND_MAX_COUNT)
                        break;
                    if (players[i].PlayerCode == code)
                        continue;
                }
                _isReady = true;
            }
            return code;
        }

        public string GetName(int index)
        {
            if (index >= NameCount)
                index = NameCount - 1;
            return _botNames[index];
        }
    }
}