using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    private readonly static string SAVELOAD_PATH = Application.dataPath + "/highscores.hs";

    private static List<Highscore> _highscoreList = new List<Highscore>();

    public static void Save(Highscore highscore)
    {
        int maxHighscores = 10;

        List<Highscore> tempList = new List<Highscore>();

        if (File.Exists(SAVELOAD_PATH))
        {
            tempList = Load();

            tempList.Add(highscore);

            _highscoreList = OrderHighscoreList(tempList);

            if (_highscoreList.Count > maxHighscores)
            {
                _highscoreList.RemoveRange(maxHighscores, 1);
            }

            File.Delete(SAVELOAD_PATH);
            for (int i = 0; i < _highscoreList.Count; i++)
            {
                if (_highscoreList[i].score == 0)
                    continue;

                string saveStringLine = JsonUtility.ToJson(_highscoreList[i]);
                File.AppendAllText(SAVELOAD_PATH, saveStringLine);
                File.AppendAllText(SAVELOAD_PATH, "\r\n");
            }
        }
        else
        {
            string saveString = JsonUtility.ToJson(highscore);
            File.WriteAllText(SAVELOAD_PATH, saveString);
        }
    }

    public static List<Highscore> Load()
    {
        List<Highscore> highscoreList = new List<Highscore>();

        if (File.Exists(SAVELOAD_PATH))
        {
            string[] highscores = File.ReadAllLines(SAVELOAD_PATH);
            foreach (string highscoreString in highscores)
            {
                Highscore highscore = JsonUtility.FromJson<Highscore>(highscoreString);
                highscoreList.Add(highscore);
            }

            return highscoreList;
        }
        else
        {
            return null;
        }
    }

    public static List<Highscore> OrderHighscoreList(List<Highscore> list)
    {
        List<Highscore> orderedList = new List<Highscore>();

        while (list.Count > 0)
        {
            int maxIndex = FindMaxHighscoreIndex(list);
            orderedList.Add(list[maxIndex]);

            list.RemoveAt(maxIndex);
        }

        return orderedList;
    }

    public static int FindMaxHighscoreIndex(List<Highscore> list)
    {
        int tempMaxIndex = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[tempMaxIndex].score < list[i].score)
                tempMaxIndex = i;
        }

        return tempMaxIndex;
    }

    public static Highscore FindMaxHighscore(List<Highscore> list)
    {
        return list[FindMaxHighscoreIndex(list)];
    }
}
