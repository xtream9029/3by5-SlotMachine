using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameControl : MonoBehaviour
{
    //row 클래스에 있는 함수를 실행시키게금 넘겨주는 부분
    public static event Action HandlePulled = delegate { };//PullHandle에서 호출됨
    public static event Action Stop = delegate { };
    public static event Action rego = delegate { };

    [SerializeField]//private 변수를 인스펙터에서 접근 가능하게 해주는 기능
    private Text prizeText;//점수표시

    public Text goldText;//점수표시

    public InputField InputMoney;

    [SerializeField]
    private Row[] rows;//슬롯 열형 배열,초기화를 유니티에서 해주고 있음 (객체를 집어 넣어서)

    [SerializeField]
    private Transform handle;//손잡이의 위치

    private int prizeValue;//실제 점수값
    public int goldValue=10000;//초기 금액
    public int bettingGold;


    private bool resultsChecked = false;

    int[,] map = new int[3, 5];

    void Update()
    {
        //슬롯의 릴이 돌고 점수가 나오는 부분을 컨트롤 하는 부분
        if (Input.GetKeyDown(KeyCode.Return))//엔터 입력
        {
            string tmp = InputMoney.text;//배팅금액 입력
            bettingGold = Convert.ToInt32(tmp);
            Debug.Log($"{bettingGold}");
        }

        //5개의 릴중에 하나라도 돌고 있으면 점수 안나옴
        if (!rows[0].rowStopped || !rows[1].rowStopped || !rows[2].rowStopped || !rows[3].rowStopped || !rows[4].rowStopped)
        {
            prizeValue = 0;
            prizeText.enabled = false;
            goldText.enabled = true;
            goldText.text = "Gold:" + goldValue;
            resultsChecked = false;
        }

        //모든슬롯의 릴이 멈춰있고 결과 체크값이 거짓일때
        else if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped && rows[3].rowStopped && rows[4].rowStopped && !resultsChecked)
        {
            CheckResults();//점수 계산하는 부분으로 빠짐
            prizeText.enabled = true;
            prizeText.text = "Prize:" + prizeValue;
            goldValue += prizeValue;
            goldText.text = "Gold:" + goldValue;
            GameObject.Find("GameControl").transform.Find("spin").gameObject.SetActive(true);
            GameObject.Find("GameControl").transform.Find("stop").gameObject.SetActive(false);

        }
    }//Update

    private void OnMouseDown()
    {
        //스핀 버튼을 눌렀을때 릴이 움직이는 부분이 실행되는 구간
        //모든 슬롯의 릴이 멈춰져있을때
        //코루틴으로 핸들 움직임 함수를 실행

        if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped && rows[3].rowStopped && rows[4].rowStopped)
        {//5개의 릴이 멈춰져있는지만 확인

            if (goldValue - bettingGold >= 0)
            {
                goldValue -= bettingGold;
                goldText.text = "Gold:" + goldValue;
                PullHandle();
                GameObject.Find("GameControl").transform.Find("spin").gameObject.SetActive(false);
                GameObject.Find("GameControl").transform.Find("stop").gameObject.SetActive(true);
            }
        }
        //5개의 릴중 하나라도 돌고 있을경우에 마우스 클릭시 실행되는 부분
        else if(!rows[0].rowStopped || !rows[1].rowStopped || !rows[2].rowStopped || !rows[3].rowStopped || !rows[4].rowStopped)
        {
            StopHandle();//여기서 stop함수가 delegate에 등록되고 stopFlag가 바뀜
        }
    }
   
    private void PullHandle()
    {
        HandlePulled();
    }

    private void StopHandle()
    {
        Stop();
    }

    private void Mapping_symbol()
    {
        //슬롯머신 돌린 결과를 2차원 배열에 맵핑하는 함수(점수 계산을 위해)
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (i == 0)
                {
                    //high
                    map[i, j] = rows[j].row_stoppedSlot3;
                }
                else if (i == 1)
                {
                    //mid
                    map[i, j] = rows[j].row_stoppedSlot2;
                }
                else if (i == 2)
                {
                    //low
                    map[i, j] = rows[j].row_stoppedSlot1;
                }
            }
        }
    }

    private void Calculate_Score(int[] A)
    {
        //맥스 심볼 맥스 카운트를 구해서 실제 점수를 계산하는 부분
        int max_symbol = -1;
        int max_cnt = -1;

        for (int j = 1; j < 8; j++)
        {
            if (max_cnt < A[j])
            {
                max_cnt = A[j];
                max_symbol = j;
            }
        }

        int LineBetting = bettingGold / 10;
        switch (max_symbol)
        {
            case 1://다이아몬드 
                if (max_cnt == 3)
                {
                    prizeValue += 3 * LineBetting;
                }
                else if (max_cnt == 4)
                {
                    prizeValue += 5 * LineBetting;
                }
                else if (max_cnt == 5)
                {
                    prizeValue += 10 * LineBetting;
                }
                break;

            case 2://수박
                if (max_cnt == 3)
                {
                    prizeValue += 3 * LineBetting;
                }
                else if (max_cnt == 4)
                {
                    prizeValue += 5 * LineBetting;
                }
                else if (max_cnt == 5)
                {
                    prizeValue += 10 * LineBetting;
                }
                break;

            case 3://스캐터 * 300
                if (max_cnt == 3)
                {
                    prizeValue += 3 * LineBetting;
                }
                else if (max_cnt == 4)
                {
                    prizeValue += 5 * LineBetting;
                }
                else if (max_cnt == 5)
                {
                    prizeValue += 10 * LineBetting;
                }
                break;

            case 4://레몬 * 400
                if (max_cnt == 3)
                {
                    prizeValue += 3 * LineBetting;
                }
                else if (max_cnt == 4)
                {
                    prizeValue += 5 * LineBetting;
                }
                else if (max_cnt == 5)
                {
                    prizeValue += 10 * LineBetting;
                }
                break;

            case 5://체리 * 500
                if (max_cnt == 3)
                {
                    prizeValue += 3 * LineBetting;
                }
                else if (max_cnt == 4)
                {
                    prizeValue += 5 * LineBetting;
                }
                else if (max_cnt == 5)
                {
                    prizeValue += 10 * LineBetting;
                }
                break;

            case 6://바 * 600
                if (max_cnt == 3)
                {
                    prizeValue += 3 * LineBetting;
                }
                else if (max_cnt == 4)
                {
                    prizeValue += 5 * LineBetting;
                }
                else if (max_cnt == 5)
                {
                    prizeValue += 10 * LineBetting;
                }
                break;

            case 7://종 * 700
                if (max_cnt == 3)
                {
                    prizeValue += 3 * LineBetting;
                }
                else if (max_cnt == 4)
                {
                    prizeValue += 5 * LineBetting;
                }
                else if (max_cnt == 5)
                {
                    prizeValue += 10 * LineBetting;
                }
                break;
        }//switch
    }

    private void CheckResults()
    {//페이라인의 결과를 계산하는 부분

        Mapping_symbol();//먼저 3X5 2d배열에다가 슬롯머신돌린결과 배열에 담음
        horizontal_ScoreDecision();//가로라인 점수를 계산
        diagonal_ScoreDecision_one();
        diagonal_ScoreDecision_two();
        payline_a();
        payline_b();
        payline_c();
        payline_d();
        payline_e();
        resultsChecked = true;//결과 체크값을 참으로 바꿔줌
    }

    private void horizontal_ScoreDecision()
    {
        //■■■■■
        //■■■■■
        //■■■■■
        for (int i = 0; i < 3; i++)
        {
            int[] A = new int[8];
            System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

            for (int j = 0; j < 5; j++)
            {
                int k = map[i, j];
                A[k]++;
            }

            Calculate_Score(A);
        }
    }//horizontal_ScoreDecision

    private void diagonal_ScoreDecision_one()
    {
        //■□□□■
        //□■□■□
        //□□■□□
        int[] D = new int[5];
        D[0] = map[0, 0];
        D[1] = map[1, 1];
        D[2] = map[2, 2];
        D[3] = map[1, 3];
        D[4] = map[0, 4];

        int[] A = new int[8];
        System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

        for (int i = 0; i < 5; i++)
        {
            A[D[i]]++;
        }

        Calculate_Score(A);
    }

    private void diagonal_ScoreDecision_two()
    {
        //□□■□□
        //□■□■□
        //■□□□■
        int[] D = new int[5];
        D[0] = map[2, 0];
        D[1] = map[1, 1];
        D[2] = map[0, 2];
        D[3] = map[1, 3];
        D[4] = map[2, 4];

        int[] A = new int[8];
        System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

        for (int i = 0; i < 5; i++)
        {
            A[D[i]]++;
        }

        Calculate_Score(A);
    }

    private void payline_a()
    {
        //■□□□■
        //□■■■□
        //□□□□□
        int[] D = new int[5];
        D[0] = map[0, 0];
        D[1] = map[1, 1];
        D[2] = map[1, 2];
        D[3] = map[1, 3];
        D[4] = map[0, 4];

        int[] A = new int[8];
        System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

        for (int i = 0; i < 5; i++)
        {
            A[D[i]]++;
        }

        Calculate_Score(A);
    }

    private void payline_b()
    {
        //□□□□□
        //□■■■□
        //■□□□■
        int[] D = new int[5];
        D[0] = map[2, 0];
        D[1] = map[1, 1];
        D[2] = map[1, 2];
        D[3] = map[1, 3];
        D[4] = map[2, 4];

        int[] A = new int[8];
        System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

        for (int i = 0; i < 5; i++)
        {
            A[D[i]]++;
        }

        Calculate_Score(A);
    }

    private void payline_c()
    {
        //■■□■■
        //□□□□□
        //□□■□□
        int[] D = new int[5];
        D[0] = map[0, 0];
        D[1] = map[0, 1];
        D[2] = map[2, 2];
        D[3] = map[0, 3];
        D[4] = map[0, 4];

        int[] A = new int[8];
        System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

        for (int i = 0; i < 5; i++)
        {
            A[D[i]]++;
        }

        Calculate_Score(A);
    }

    private void payline_d()
    {
        //□□■□□
        //□□□□□
        //■■□■■
        int[] D = new int[5];
        D[0] = map[2, 0];
        D[1] = map[2, 1];
        D[2] = map[0, 2];
        D[3] = map[2, 3];
        D[4] = map[2, 4];

        int[] A = new int[8];
        System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

        for (int i = 0; i < 5; i++)
        {
            A[D[i]]++;
        }

        Calculate_Score(A);
    }

    private void payline_e()
    {
        //■□□□■
        //□□■□□
        //□■□■□
        int[] D = new int[5];
        D[0] = map[0, 0];
        D[1] = map[2, 1];
        D[2] = map[1, 2];
        D[3] = map[2, 3];
        D[4] = map[0, 4];

        int[] A = new int[8];
        System.Array.Clear(A, 0, 8);//c++에서 memset함수와 같은 함수

        for (int i = 0; i < 5; i++)
        {
            A[D[i]]++;
        }

        Calculate_Score(A);
    }

}
