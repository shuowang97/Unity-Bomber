using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public Text txt_HP, txt_Level, txt_Time, txt_Enemy;

    private void Awake() {
        Instance = this;
    }

    public void RefreshInfo(int HP, int level, int time, int enemy)
    {
        txt_HP.text = "HP: " + HP.ToString();
        txt_Level.text = "Level: " + level.ToString();
        txt_Time.text = "Time: " + time.ToString();
        txt_Enemy.text = "Enemy: " + enemy.ToString();
    }
}
