using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int score = 0;

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void MinusScore(int points)
    {
        score -= points;
        if (score <= 300) score = 0;
    }    
    public void SaveGameData(float survivalTime)
    {
        // Lấy danh sách điểm số hiện tại từ PlayerPrefs
        int currentHighScoreCount = PlayerPrefs.GetInt("HighScoreCount", 0);
        string[] scores = new string[currentHighScoreCount];
        string[] times = new string[currentHighScoreCount];

        for (int i = 0; i < currentHighScoreCount; i++)
        {
            scores[i] = PlayerPrefs.GetString("Score" + i, "0");
            times[i] = PlayerPrefs.GetString("Time" + i, "00:00");
        }

        // Thêm điểm và thời gian mới
        string newTime = string.Format("{0:00}:{1:00}", (int)survivalTime / 60, (int)survivalTime % 60);
        string newEntry = score + "|" + newTime;

        // Chèn vào mảng
        System.Array.Resize(ref scores, currentHighScoreCount + 1);
        System.Array.Resize(ref times, currentHighScoreCount + 1);
        scores[currentHighScoreCount] = score.ToString();
        times[currentHighScoreCount] = newTime;

        // Sắp xếp mảng theo điểm (giảm dần)
        for (int i = 0; i < scores.Length - 1; i++)
        {
            for (int j = i + 1; j < scores.Length; j++)
            {
                if (int.Parse(scores[i]) < int.Parse(scores[j]))
                {
                    string tempScore = scores[i];
                    string tempTime = times[i];
                    scores[i] = scores[j];
                    times[i] = times[j];
                    scores[j] = tempScore;
                    times[j] = tempTime;
                }
            }
        }

        // Giữ chỉ top 5
        if (scores.Length > 5)
        {
            System.Array.Resize(ref scores, 5);
            System.Array.Resize(ref times, 5);
        }

        // Lưu lại vào PlayerPrefs
        PlayerPrefs.SetInt("HighScoreCount", scores.Length);
        for (int i = 0; i < scores.Length; i++)
        {
            PlayerPrefs.SetString("Score" + i, scores[i]);
            PlayerPrefs.SetString("Time" + i, times[i]);
        }
        PlayerPrefs.Save();
    }

    // Reset score khi bắt đầu chơi mới
    public void ResetScore()
    {
        score = 0;
    }
}