using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TimeClocker : MonoBehaviour
{
    // Start is called before the first frame update
    public int CTime;
    public float TTime;
    public bool Trigger=false;
    public bool ATrigger = false;
    public GameObject Main;
    public GameObject ClockPanel;
    public Sprite OneS;
    public Sprite TwoS;
    public Sprite ThreeS;
    public Sprite StartS;

    private void Update()
    {
        if (Trigger)
        {
            Debug.Log("ㄱㄱ");
            if (ATrigger == false)
            {
                TTime = TTime + (CTime * Time.deltaTime);
                if(TTime<10)
                {
                    ChangeSprite(1);
                }
                else if(TTime>=10&&TTime<20)
                {
                    ChangeSprite(2);
                }
                else if (TTime >= 20 && TTime < 30)
                {
                    ChangeSprite(3);
                }
                else
                {
                    ChangeSprite(4);
                }
                Debug.Log(TTime);
            }
            if(TTime>40)
            {
                ATrigger = true;
                Debug.Log("ATrigger True");
            }

        }
        if(Trigger&&ATrigger)
        {
            Trigger = false;
            Debug.Log("ATrigger True2");
            Main.GetComponent<Lobby>().CStart();
        }
    }
    public void Clock()
    {
        Trigger = true;
        CTime = 7;
        Debug.Log("11");

    }
    public void ChangeSprite(int i)
    {
        if (i==1)
        {
            ClockPanel.GetComponent<Image>().sprite = ThreeS;
            
        }
        else if (i == 2)
        {
            ClockPanel.GetComponent<Image>().sprite = TwoS;
        }
        else if (i == 3)
        {
            ClockPanel.GetComponent<Image>().sprite = OneS;
        }
        else if(i==4)
        {
            ClockPanel.GetComponent<Image>().sprite = StartS;
        }
    }

}
