using UnityEngine;

// 전역 상수 변수들, 한번의 new 연산으로 사용하기 위해 만듬 

namespace Y
{
    // Animator Hash 이 변수는 안되어서 사용 안할 예정
    //public partial class Variables
    //{
    //    public static readonly int intAnimatorIsGrab = Animator.StringToHash("Is_Grab");
    //}

    public enum enumType
    {
        Player,
        Red,
        Blue,
        RedKnight,
        BlueKnight
    }

    // WaitForSeconds
    public partial class Variables
    {
        // 1000ms == 1초
        public static readonly WaitForSeconds waitForSeconds_1000ms = new WaitForSeconds(1.0f);
        public static readonly WaitForSeconds waitForSeconds_2000ms = new WaitForSeconds(2.0f);
        public static readonly WaitForSeconds waitForSeconds_3000ms = new WaitForSeconds(3.0f);
        public static readonly WaitForSeconds waitForSeconds_4000ms = new WaitForSeconds(4.0f);
        public static readonly WaitForSeconds waitForSeconds_5000ms = new WaitForSeconds(5.0f);
        public static readonly WaitForSeconds waitForSeconds_200ms = new WaitForSeconds(0.2f);
        public static readonly WaitForSeconds waitForSeconds_100ms = new WaitForSeconds(0.1f);

        public static readonly WaitForSecondsRealtime waitForSecondsRealtime_1000ms = new WaitForSecondsRealtime(1.0f);
    }

    public partial class Variables
    {
        public static readonly string stringVertical = "Vertical";
        public static readonly string stringHorizontal = "Horizontal";
        public static readonly string stringPlayer = "Player";

        public static readonly float floatMaxHorseSpeed = 20.0f;
        public static readonly float floatHorseTurnSpeed = 33.0f;
    }

    public partial class Variables
    {       
        public static float GetDamage(string self, string damageThing)
        {
            // 졸병 > 졸병
            bool isSoldier_Hit_Sldier = (self.Equals("RED") && damageThing.Equals("BLUE")) || (self.Equals("BLUE") && damageThing.Equals("RED"));

            // 졸병 > 대장
            bool isSoldier_Hit_Leader = (self.Equals("BLUE") && damageThing.Equals("REDALPHA")) || (self.Equals("RED") && damageThing.Equals("BLUEALPHA"));
            
            // 대장 > 졸병
            bool isLeader_Hit_Soldier = (self.Equals("REDALPHA") && damageThing.Equals("BLUE")) || (self.Equals("BLUEALPHA") && damageThing.Equals("RED"));

            // 적 > 사용자
            bool isEnemy_Hit_Player = ((self.Contains("RED") || self.Contains("BLUE")) && damageThing.Equals("Player"));

            // 사용자 > 졸병
            bool isPlayer_Hit_Soldier = self.Equals("Player") && (damageThing.Equals("RED") || damageThing.Equals("BLUE"));

            // 사용자 > 대장
            bool isPlayer_Hit_Leader = self.Equals("Player") && (damageThing.Equals("REDALPHA") || damageThing.Equals("BLUEALPHA"));

            // 사용자 > 사용자
            bool isPlayer_Hit_Player = self.Equals("Player") && damageThing.Equals("Player");

            // 대장 > 대장
            bool isLeader_Hit_Leader = (self.Equals("REDALPHA") && damageThing.Equals("BLUEALPHA")) || (self.Equals("BLUEALPHA") && damageThing.Equals("REDALPHA"));

            if (isSoldier_Hit_Sldier)
            {
                //Debug.Log("졸병 > 졸병");
                return 20.0f;
            }
            else if (isSoldier_Hit_Leader)
            {
                //Debug.Log("졸병 > 대장");
                return 0.0f;
            }
            else if (isLeader_Hit_Soldier)
            {
                //Debug.Log("대장 > 졸병");
                return 50.0f;
            }
            else if (isEnemy_Hit_Player)
            {
                //Debug.Log("적 > 사용자");
                return 20.0f;
            }
            else if (isPlayer_Hit_Soldier)
            {
                //Debug.Log("사용자 > 졸병");
                return 100.0f;
            }
            else if (isPlayer_Hit_Leader)
            {
                //Debug.Log("사용자 > 대장");
                //return 50.0f;
                return 100.0f;
            }
            else if (isPlayer_Hit_Player)
            {
                //Debug.Log("사용자 > 사용자");
                return 100.0f;
            }
            else if (isLeader_Hit_Leader)
            {
                //Debug.Log("대장 > 대장");
                return 0.0f;
            }

            return 0.0f;
        }
    }
}