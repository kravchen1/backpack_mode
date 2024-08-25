using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PotionHPCommon : Flask
{
    public float howActivation = 30; //��� 30%(����� ������) ��� ���� ��������� ���������
    public float percentHP = 20; //���� ��������� ����� �����������
    public float percentRegenerate = 5; //���� ��������� ����� ��������������
    public float timerRegenerate = 1; //��� ����� � �������� ����� ����������� �����������
    public float maxTimeRegenerate = 4; //������ ��� ����� ����������� �����������
    private bool isUse = false;
    private bool usable = false;
    private int currentTick = 0;
    private void Start()
    {
        timer = timerRegenerate;
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //animator.speed = 1f / timer_cooldown;
            //animator.enabled = true;
        }
    }
    float f_ToPercent(float a, float p)
    {
        return (a / 100 * p); //���������� �������� �� ���������, �������� ���� �������� 200 � 30, �� ����� 60
    }
    public override void Activation()
    {
        if (!isUse)
        {
            if (Player.hp <= f_ToPercent(Player.maxHP, howActivation) )
            {
                isUse = true;
                usable = true;
                Player.hp += f_ToPercent(Player.maxHP, percentHP);
                Debug.Log("������ ��������");
                image.color = Color.gray;
            }
        }
    }

    public void TickHeal()
    {
        if (usable && currentTick < maxTimeRegenerate)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timerRegenerate;
                Player.hp += f_ToPercent(Player.maxHP, percentHP);
                currentTick += 1;
                Debug.Log("� �������� �� ��������� " + currentTick);
            // a delayed action could be called from here
            // once the lock-out period expires
            }
        }
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            TickHeal();
            Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            Rotate();
            SwitchDynamicStatic();
            OnImpulse();
            RotationToStartRotation();

        }
    }
    
}
