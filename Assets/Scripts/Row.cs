using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    private int randomValue;
    private float timeInterval;

    public bool rowStopped;
    public bool stopFlag = false;

    //밑에서 부터 
    public int row_stoppedSlot1;
    public int row_stoppedSlot2;
    public int row_stoppedSlot3;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 4.5f, 0);
        rowStopped = true;

        //GameControl에서 넘겨준 함수들 from delegate
        GameControl.HandlePulled += StartRotating;
        GameControl.Stop += StopRotating;//단순 플래그처리로 릴을 멈춤
    }

    private void StopRotating()
    {
        //stop버튼을 눌렀을때 전역플래그 처리로 릴의 움직임을 강제로 정지시킴
        stopFlag = true;
    }

    private void StartRotating()
    {
        row_stoppedSlot1 = 0;//맨밑
        row_stoppedSlot2 = 0;//중간
        row_stoppedSlot3 = 0;//맨위

        StartCoroutine("Rotate");
    }

    private IEnumerator Rotate()//코루틴
    {
        rowStopped = false;//슬롯머신의 릴이 돌게 되므로
        

        randomValue = Random.Range(60, 120);

        //이 부분이 없으면 슬롯의 심볼이 각 칸에 깔끔하게 떨어지질 않음 이유는 정확히 모르겠음
        switch (randomValue % 3)
        {
            case 1:
                randomValue += 2;
                break;
            case 2:
                randomValue += 1;
                break;
        }

        //실제로 릴이 도는 것을 구현한 부분
        for (int i = 0; i < randomValue; i++)
        {
            if (stopFlag)
            {
                //stop버튼을 누를때 칸에 딱맞게 떨어지지 않을경우에 강제로 슬롯을 정해주기위해 위치를 결정지어줌(약간의 트릭임)
                if (transform.localPosition.y == 4)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, 0);
                }
                else if (transform.localPosition.y == 3.5)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.5f, 0);
                }
                else if (transform.localPosition.y == 2.5)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, 0);
                }
                else if (transform.localPosition.y == 2)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.5f, 0);
                }
                else if (transform.localPosition.y == 1)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, 0);
                }
                else if (transform.localPosition.y == 0.5)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.5f, 0);
                }
                else if (transform.localPosition.y == -0.5)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, 0);
                }
                else if (transform.localPosition.y == -1)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.5f, 0);
                }

                stopFlag = false;//stop버튼을 누른이후에 다시 spin버튼을 눌렀을때 릴이 돌수 있게
                break;//더이상 도는 부분이 실행되지 않음
            }//stop 버튼을 눌렀을 때

            if (transform.localPosition.y <= -1.5f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 4.5f, 0);
            }

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.5f, 0);

            //실제로 릴이 회전하는 속도를 결정짓는 부분,이 부분이 슬롯을 결정짓는 것과 연관이 있는지는 모르겠음
            if (i > Mathf.RoundToInt(randomValue * 0.25f))
                timeInterval = 0.05f;
            if (i > Mathf.RoundToInt(randomValue * 0.5f))
                timeInterval = 0.1f;
            if (i > Mathf.RoundToInt(randomValue * 0.75f))
                timeInterval = 0.15f;
            if (i > Mathf.RoundToInt(randomValue * 0.9f))
                timeInterval = 0.2f;

            yield return null;
        }//롤이 도는 부분까지(for)

        //===================================================================================================
        //row의 y좌표에따라 심볼들을 결정하는 부분
        if (transform.position.y == 4.5)
        {
            row_stoppedSlot1 = 1;//다이아몬드
            row_stoppedSlot2 = 2;//수박
            row_stoppedSlot3 = 3;//스캐터
        }

        else if (transform.position.y == 3)
        {
            row_stoppedSlot1 = 2;//수박
            row_stoppedSlot2 = 3;//스캐터
            row_stoppedSlot3 = 4;//레몬
        }

        else if (transform.position.y == 1.5)
        {
            row_stoppedSlot1 = 3;//스캐터
            row_stoppedSlot2 = 4;//레몬
            row_stoppedSlot3 = 5;//체리
        }

        else if (transform.position.y == 0)
        {
            row_stoppedSlot1 = 4;//레몬
            row_stoppedSlot2 = 5;//체리
            row_stoppedSlot3 = 6;//바
        }

        else if (transform.position.y == -1.5)
        {
            row_stoppedSlot1 = 5;//체리
            row_stoppedSlot2 = 6;//바
            row_stoppedSlot3 = 7;//종
        }

        rowStopped = true;

    }//Rotate

    private void OnDestroy()
    {
        GameControl.HandlePulled -= StartRotating;
        GameControl.Stop -= StopRotating;
    }//OnDestroy
}
