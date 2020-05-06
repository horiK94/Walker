using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text floorText = null;

    [SerializeField]
    private Text retryText = null;

    [SerializeField]
    private GameObject postTwitterParent = null;

    [SerializeField]
    private Button postTwitterButton = null;

    public void SetFloor(int _floor)
    {
        floorText.text = _floor + "F";

        postTwitterButton.onClick.RemoveAllListeners();
        postTwitterButton.onClick.AddListener(() =>
        {
            if (_floor == 1)
            {
                naichilab.UnityRoomTweet.Tweet(GameSetting.GAME_ID, "1Fも登頂できませんでした....。", new string[] { "NOT密着ダンジョン", "unityroom", "unity1week" });

            }
            else
            {
                naichilab.UnityRoomTweet.Tweet(GameSetting.GAME_ID, (_floor - 1) + "Fまで登頂できました!", new string[] { "NOT密着ダンジョン", "unityroom", "unity1week" });
            }
        });
    }

    public void ApplyGameOverUI()
    {
        retryText.enabled = true;
        postTwitterParent.SetActive(true);
    }
}
