using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    //스테이트
    public enum CurrentState { idle, run, attack, victory, defeat };
    public CurrentState currentState = CurrentState.idle;

    //자신의 컴포넌트
    public HorseMoveLists horseMoveLists;
    public Transform transformParentEnemy;
    public Transform transformEnemy;
    public Collider colliderEnemyBody;


    //HP 
    public float floatHp = 100;


    //래그돌 관리
    public Rigidbody[] rigidBody;


    //오디오클립
    public AudioClip[] humanAttackClip;
    public AudioClip[] humanHittedClip;


    //오디오소스
    private AudioSource humanAudio;
    private AudioSource weaponeAudio;
    

    //애니메이션
    public Animator anim;
    private Animator horseAnim;
    private readonly int hashRun = Animator.StringToHash("IsRun");
    private readonly int hashLeft = Animator.StringToHash("LeftAtt");
    private readonly int hashRight = Animator.StringToHash("RightAtt");
    private readonly int hashHit = Animator.StringToHash("IsHit");
    private readonly int hashDead = Animator.StringToHash("Dead");
    private readonly int hashVictory = Animator.StringToHash("Victory");
    private readonly int hashDefeat = Animator.StringToHash("Defeat");


    //검색용 배열 Enemy매니저에서 참조함
    private List<GameObject> attAlpha;
    private List<GameObject> attKnight;
    private Transform attTargetTr;


    //사람 재부팅용
    public Transform saddle;
    public Transform humanPivot;

    private bool isHumanDead = false;

    void Awake()
    {
        horseMoveLists = GetComponent<HorseMoveLists>();

        humanAudio   = gameObject.AddComponent<AudioSource>();
        weaponeAudio = gameObject.AddComponent<AudioSource>();
        humanAudio.spatialBlend   = 1.0f;
        weaponeAudio.spatialBlend = 1.0f;

        rigidBody = GetComponentsInChildren<Rigidbody>();
        DoRagDoll(false);
    }

    void OnEnable()
    {
        StartCoroutine(CheckMyTag());
        StartCoroutine(CheckForAction());
        horseAnim = horseMoveLists.gameObject.GetComponent<Animator>();
        horseAnim.SetBool(hashDead, false);
        horseMoveLists.navAgent.isStopped = false;
    }
    IEnumerator CheckMyTag()
    {
        //자신의 태그 판별, 적들 태그 판별
        switch (tag)
        {
            case "REDALPHA":
                attAlpha  = EnemyManager.instance.blueAlphaList;
                attKnight = EnemyManager.instance.blueList;
                break;
            case "RED":
                attAlpha  = EnemyManager.instance.blueAlphaList;
                attKnight = EnemyManager.instance.blueList;
                break;
            case "BLUEALPHA":
                attAlpha  = EnemyManager.instance.redAlphaList;
                attKnight = EnemyManager.instance.redList;
                break;
            case "BLUE":
                attAlpha  = EnemyManager.instance.redAlphaList;
                attKnight = EnemyManager.instance.redList;
                break;
        }
        StartCoroutine(AttackEnemy());
        yield return null;
    }

    IEnumerator AttackEnemy()
    {
        while (!isHumanDead)
        {
            if (horseMoveLists.hittedTr != null)
            {
                for (int i = 0; i < attAlpha.Count; i++)
                {
                    if (horseMoveLists.hittedTr == attAlpha[i].transform)
                    {
                        attTargetTr = attAlpha[i].transform;
                    }
                }
                if (attTargetTr == null)
                {
                    for (int h = 0; h < attKnight.Count; h++)
                    {
                        if (horseMoveLists.hittedTr == attKnight[h].transform)
                        {
                            attTargetTr = attKnight[h].transform;
                        }
                    }
                }
            }

            if (attTargetTr != null )
            {
                Vector3 targetDir = transform.InverseTransformPoint(attTargetTr.position);
                if (targetDir.x <= 0.0f)
                {
                    currentState = CurrentState.attack;
                    anim.SetTrigger(hashLeft);
                    humanAudio.PlayOneShot(humanAttackClip[0]);
                    yield return new WaitForSeconds(1.0f);
                }
                else
                {
                    currentState = CurrentState.attack;
                    anim.SetTrigger(hashRight);
                    humanAudio.PlayOneShot(humanAttackClip[0]);
                    yield return new WaitForSeconds(1.0f);
                }
            }
            else if (horseMoveLists.isVictory && !horseMoveLists.isDefeat)
            {
                currentState = CurrentState.victory;
            }
            else if (!horseMoveLists.isVictory && horseMoveLists.isDefeat)
            {
                currentState = CurrentState.defeat;
            }
            else
            {
                currentState = CurrentState.run;
            }
            attTargetTr = null;
            yield return null;
        }
    }


    //스테이트별 행동
    IEnumerator CheckForAction()
    {
        while (!isHumanDead)
        {
            switch (currentState)
            {
                case CurrentState.idle:
                    anim.SetBool(hashRun, false);
                    break;
                case CurrentState.run:
                    anim.SetBool(hashRun, true);
                    break;
                case CurrentState.attack:
                    anim.SetBool(hashRun, true);
                    break;
                case CurrentState.victory:
                    anim.SetBool(hashRun, true);
                    anim.SetBool(hashVictory, true);
                    colliderEnemyBody.enabled = false;
                    break;
                case CurrentState.defeat:
                    anim.SetBool(hashRun, false);
                    anim.SetBool(hashDefeat, true);
                    colliderEnemyBody.enabled = false;
                    break;
            }
            yield return null;
        }
    }


    //데미지 받을 때
    public void HitDamage(float damage)
    {
        if (damage <= 0.0f)
        {
            return;
        }

        if (floatHp > 0)
        {
            floatHp -= damage;
            //anim.SetTrigger("IsHit");

            humanAudio.PlayOneShot(humanAttackClip[Random.Range(1, 3)]);
            humanAudio.PlayOneShot(humanHittedClip[Random.Range(0, 2)]);

            if (floatHp <= 0)
            {
                humanAudio.PlayOneShot(humanHittedClip[Random.Range(3, 4)]);

                bool isDeadHorse = (Random.Range(0, 2) == 0);
                if (isDeadHorse)
                {
                    Invoke("DeadHorseAnimation", 1.0f);
                }
                DoRagDoll(true);
                isHumanDead = true;
                StartCoroutine(ObjActiveFalse());
            }
        }
    }


    public void DoRagDoll(bool isRagDoll)
    {
        int length = rigidBody.Length - 1;
        for (int i = 0; i < length; i++)
        {
            rigidBody[i].isKinematic = !isRagDoll;
        }


        if (isRagDoll)
        {
            transformEnemy.SetParent(null);

            Vector3 pos = new Vector3(0.0f, 0.5f, -1.0f);
            rigidBody[1].AddForce(pos * 1000f, ForceMode.Impulse);
        }

        // 충돌 콜라이더
        colliderEnemyBody.enabled = !isRagDoll;

        // RagDoll
        anim.enabled = !isRagDoll;
    }


    IEnumerator ObjActiveFalse()
    {
        yield return new WaitForSeconds(5.0f);
        DoRagDoll(false);
        transformEnemy.SetParent(saddle);
        transformEnemy.transform.position = humanPivot.transform.position;
        transformEnemy.transform.rotation = humanPivot.transform.rotation;
        gameObject.SetActive(false);
    }



    void DeadHorseAnimation()
    {
        horseMoveLists.isHorseDead = true;
        horseMoveLists.navAgent.isStopped = true;
        horseAnim.SetTrigger(hashDead);
    }

    public string GetTag()
    {
        return gameObject.tag;
    }

    private void OnDisable()
    {
        isHumanDead = false;
        floatHp = 100.0f;
    }
}