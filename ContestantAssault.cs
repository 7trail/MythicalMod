using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rewired;
using UnityEngine;
namespace Mythical
{
    public class ContestantAssault : MonoBehaviour
    {
        public static List<Enemy> spawnedBosses = new List<Enemy>();

        public static ContestantAssault instance;
        public PvpController self;
        public static void Init()
        {
            /*On.EnemyHealthBar.Claim += (On.EnemyHealthBar.orig_Claim orig, EnemyHealthBar self, Enemy e) =>
            {
                orig(self, e);
                if (e is Boss && ContentLoader.inAPVPScene)
                {
                    foreach (GameObject tr in self.GetComponentsInChildren<GameObject>())
                    {

                        tr.transform.position += Vector3.up * (dist % 100);
                        if (dist <= -100)
                        {
                            tr.transform.position += Vector3.left * 150;
                        }
                        else
                        {
                            tr.transform.position -= Vector3.left * 150;
                        }
                    }
                    //dist += 50;
                }
            };*/
        }
        ChaosInputDevice.InputScheme scheme;
        public void Start()
        {
            instance = this;
            scheme = GameController.players[0].GetComponent<Player>().inputDevice.inputScheme;
            SpawnBosses();
            GetComponent<PvpController>().enabled = false;
            self.buttonPrompts.gameObject.SetActive(false);
        }
        public bool failure = false;
        public bool success = false;
        public void Update()
        {

            if (!success && spawnedBosses.Count>0)
            {
                bool allDead = true;
                int count = 0;
                foreach (Enemy mb in spawnedBosses)
                {
                    if (mb != null && mb.fsm.currentStateName != "Dead")
                    {
                        allDead = false;
                        count++;
                    }
                }

                GameUI.BroadcastNoticeMessage("Contestants Left: " + count, 0.1f);

                if (allDead)
                {
                    success = true;
                    GameController.stageTitleUI.AnnounceVictory("YOU WIN");
                    PlayerPrefs.SetInt("mythical::CA", 1);
                    PlayerPrefs.Save();
                    StartCoroutine("ResetMatch");
                }

                
            }
            if (GameController.AllPlayersDead() && !failure)
            {
                failure = true;
                GameController.stageTitleUI.AnnounceFight("LOSS");
                StartCoroutine("ResetMatch");
            }
        }
        
        public IEnumerator ResetMatch()
        {
            TimeScaleController.StandardFreezeIntoEaseOutTimeScale();
            SoundManager.PlayAudio("Explosion2", 1f, false, -1f, -1f);
            SoundManager.PlayAudio("FloorVictory", 1f, false, -1f, -1f);
            bool inputDetected = false;
            int selection = 0;
            self.confirmImage.sprite = GameUI.GetInputSprite(scheme, "Confirm");
            self.cancelImage.sprite = GameUI.GetInputSprite(scheme, "Cancel");
            self.buttonPrompts.gameObject.SetActive(true);
            //GameController.PauseAllPlayers(true);
            self.TogglePlayerInvulnerable(true);

            foreach (Enemy mb in FindObjectsOfType<Enemy>())
            {
                mb.health.CurrentHealthValue = -1;
                if (spawnedBosses.Contains(mb))
                {
                    spawnedBosses.Remove(mb);
                }
                mb.fsm.QueueChangeState("Dead", false);
                mb.health.AnnounceDeathEvent(mb);
                Destroy(mb.gameObject,1.5f);
                
            }
            spawnedBosses.Clear();
            GameController.bosses.Clear();
            while (!inputDetected)
            {
                
                if (InputController.GetAnyDeviceButtonDown("Confirm") != null)
                {
                    selection = 0;
                    inputDetected = true;
                }
                else if (InputController.GetAnyDeviceButtonUp("Cancel") != null)
                {
                    selection = 1;
                    inputDetected = true;
                }
                yield return null;
            }
            failure = false;
            SoundManager.PlayBGM(string.Empty);
            self.matchInProgress = false;
            

            if (selection != 0)
            {
                if (selection == 1)
                {
                    self.ResetStage(true);
                    GameController.LoadLevel("PvP");
                }
            }
            else
            {
                
                FakeResetStage(true);
                SpawnBosses();
                //GameController.p1Joined = true;
                //GameController.p2Joined = true;
            }
        }

        public void FakeResetStage(bool b)
        {
            GameUI.BroadcastNoticeMessage(" ", 3f);
            GameController.itemSpawner.Reset();
            self.buttonPrompts.gameObject.SetActive(false);
            ChaosStopwatch.Stop(self.skillSpawnStopwatchID);
            LootManager.Reset();
            //foreach (Player player in GameController.activePlayers)
            //{
            //   UnityEngine.Object.DestroyImmediate(player.gameObject);
            //}
            //GameController.p1Joined = false;
            //GameController.p2Joined = false;
            self.ResetPlayers(true);
            self.playersReady = false;
            self.matchOver = false;
        }

        public static void SpawnBosses()
        {
            dist = 0;
            instance.StartCoroutine("BossSpawnRoutine");
        }
        public static int dist = 0;
        IEnumerator BossSpawnRoutine()
        {
            failure = false;
            success = false;
            yield return new WaitForSeconds(1.5f);

            SoundManager.PlayAudio("Woosh", 1f, false, -1f, -1f);
            GameController.stageTitleUI.AnnounceFight("Contestant");
            yield return new WaitForSeconds(1f);
            SoundManager.PlayAudio("Woosh", 1f, false, -1f, -1f);
            GameController.stageTitleUI.AnnounceFight("Assault");
            yield return new WaitForSeconds(2f);
            SoundManager.PlayAudio("MetalBell", 1f, false, -1f, -1f);
            GameController.stageTitleUI.AnnounceFight("GO!");
            self.TogglePlayerInvulnerable(false);
            int ct = ContentLoader.configContestantCount.Value;
            for (int i = 0; i < ct; i++)
            {
                Enemy boss = null;
                try
                {
                    float a = i;
                    Vector3 position = ChaosArenaChanges.offset+ new Vector3(Mathf.Cos(a),Mathf.Sin(a),0)* 6;
                    boss = Enemy.Spawn(Enemy.EName.Contestant,position);

                    //boss.fsm.ChangeState(boss.bossReadyState.name, false);
                    boss.chestLootTableID = String.Empty;
                } catch { }

                if (boss != null)
                {
                    spawnedBosses.Add(boss);
                    //Debug.Log("Adjusting Position");
                }
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
