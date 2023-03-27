using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Messaging;
using Ubiq.Rooms;


public class RestartMenu : MonoBehaviour
{
    private NetworkContext context;
    public Button button;
    public GameObject left;
    public GameObject right;
    public Slider time;
    private bool updating;
    public GameObject PS;

    private struct Message
    {
        public bool flag;
        public float time;

        public Message(bool flag, float time)
        {
            this.flag = flag;
            this.time = time;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
    {
        var data = msg.FromJson<Message>();

        if (data.flag)
        {
            Reset();
        } 
        else
        {
            updating = true;
            UpdateTime(data.time);
            UpdateSlider(data.time);
            updating = false;
        }
    }

    void Update()
    {
        if (PS.activeSelf)
        {
            time.gameObject.SetActive(true);
        } 
        else
        {
            time.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        button.onClick.AddListener(ResetListener);

        time.onValueChanged.AddListener(delegate { TimerListener(); });
        time.minValue = 10;
        time.maxValue = 300;
        time.value = 10;

        updating = false;
    }

    void TimerListener() 
    {
        if (!updating)
        {
            UpdateTime(time.value);
            context.SendJson(new Message(false, time.value));
        }
    }

    void UpdateTime(float value)
    {
        GameObject GO = GameObject.Find("Board");
        DrawableSurface canvas = GO.GetComponent<DrawableSurface>();
        canvas.timer = Mathf.Round(value);
    }

    void UpdateSlider(float value)
    {
        time.value = value;
    }

    private void Reset()
    {
        GameObject GO = GameObject.Find("Board");
        DrawableSurface canvas = GO.GetComponent<DrawableSurface>();
        canvas.Start();

        GameObject TimerGO = GameObject.Find("Timer");
        Timer timer = TimerGO.GetComponent<Timer>();
        timer.Start();

        if (left != null)
        {
            PSButton btn_left = left.GetComponent<PSButton>();
            btn_left.Start();
        }

        if (right != null)
        {
            PSButton btn_right = right.GetComponent<PSButton>();
            btn_right.Start();
        }
    }


    public void ResetListener()
    {
        Reset();
        context.SendJson(new Message(true, 0));
    }
}