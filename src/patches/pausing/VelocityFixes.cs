using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;
using UnityEngine;

namespace UILib.Patches {
    /**
     * <summary>
     * Patches falling rocks to make
     * sure they pause and restore correctly.
     * </summary>
     */
    internal static class VelocityFixes {
        private static Logger logger = new Logger(typeof(VelocityFixes));

        private class RockState {
            internal FallingRock rock;
            internal bool kinematic;
            internal Vector3 velocity;
            internal float volume;
        }

        private class IceSpawnerState {
            internal IceFallRandomiser spawner;
            internal bool enabled;
        }

        private static List<RockFallSpawner> rockSpawners;
        private static List<IceSpawnerState> iceSpawners;
        private static List<RockState> states;

        /**
         * <summary>
         * Initializes the fixes.
         * </summary>
         */
        internal static void Init() {
            rockSpawners = new List<RockFallSpawner>();
            iceSpawners = new List<IceSpawnerState>();
            states = new List<RockState>();

            PauseHandler.onPause.AddListener(() => {
                PauseRocks();
            });
            PauseHandler.onUnpause.AddListener(() => {
                UnpauseRocks();
            });

            SceneLoads.onAnyUnload.AddListener(delegate {
                rockSpawners.Clear();
                iceSpawners.Clear();
                states.Clear();
            });
        }

        /**
         * <summary>
         * Pauses falling rocks.
         * </summary>
         */
        private static void PauseRocks() {
            logger.LogDebug("Pausing rock fall spawners");
            foreach (RockFallSpawner spawner in GameObject.FindObjectsOfType<RockFallSpawner>()) {
                spawner.StopAllCoroutines();
                rockSpawners.Add(spawner);
            }

            logger.LogDebug("Pausing ice fall spawners");
            foreach (IceFallRandomiser spawner in GameObject.FindObjectsOfType<IceFallRandomiser>()) {
                spawner.StopAllCoroutines();
                iceSpawners.Add(new IceSpawnerState() {
                    spawner = spawner,
                    enabled = spawner.gameObject.activeSelf,
                });

                spawner.gameObject.SetActive(false);
            }

            logger.LogDebug("Pausing rocks");
            foreach (FallingRock rock in GameObject.FindObjectsOfType<FallingRock>()) {
                states.Add(new RockState() {
                    rock = rock,
                    kinematic = rock.rb.isKinematic,
                    velocity = rock.rb.velocity,
                    volume = rock.flyingSound.volume,
                });

                rock.rb.isKinematic = true;
                rock.rb.velocity = Vector3.zero;
                rock.flyingSound.volume = 0f;
            }
        }

        /**
         * <summary>
         * Unpauses falling rocks.
         * </summary>
         */
        private static void UnpauseRocks() {
            MethodInfo rockFallStart = AccessTools.Method(
                typeof(RockFallSpawner), "Start"
            );
            FieldInfo startedIceFall = AccessTools.Field(
                typeof(IceFallRandomiser), "startedIceFall"
            );

            logger.LogDebug("Unpausing rocks");
            foreach (RockState state in states) {
                state.rock.rb.isKinematic = state.kinematic;
                state.rock.rb.velocity = state.velocity;
                state.rock.flyingSound.volume = state.volume;
            }

            logger.LogDebug("Unpausing rock fall spawners");
            foreach (RockFallSpawner spawner in rockSpawners) {
                rockFallStart.Invoke(spawner, new object[] {});
            }

            logger.LogDebug("Unpausing ice fall spawners");
            foreach (IceSpawnerState state in iceSpawners) {
                state.spawner.gameObject.SetActive(state.enabled);
                startedIceFall.SetValue(state.spawner, false);
            }

            logger.LogDebug("Clearing internal cache");
            rockSpawners.Clear();
            iceSpawners.Clear();
            states.Clear();
        }
    }
}
