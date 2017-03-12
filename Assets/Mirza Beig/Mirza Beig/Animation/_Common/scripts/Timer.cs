
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using UnityEngine.Events;

//using System.Collections;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace Animation
    {

        // =================================	
        // Classes.
        // =================================

        //[ExecuteInEditMode]
        [System.Serializable]

        public class Timer
        {
            // =================================	
            // Nested classes and structures.
            // =================================

            // ...

            public enum PlaybackState
            {
                playing,
                paused,
                stopped
            }

            // =================================	
            // Variables.
            // =================================

            // Current time and target max.

            public float time = 0.0f;
            public float duration = 1.0f;

            // Loop? Restart after every complete.

            public bool loop = true;

            // Completed a cycle?

            public bool isComplete { get; private set; }

            // Completion percentage (0.0f - 1.0f).

            //public float completion
            //{
            //    get
            //    {
            //        return time / duration;
            //    }
            //}
            public float normalizedTime
            {
                get
                {
                    return time / duration;
                }
            }

            // Internal state that decides what to do when update is called.

            public PlaybackState playbackState = PlaybackState.stopped;

            // Event delegates.

            public delegate void onTimerCompleteEventHandler();
            public event onTimerCompleteEventHandler onTimerCompleteEvent;

            // =================================	
            // Functions.
            // =================================

            // ...

            public Timer(float time = 0.0f, float duration = 1.0f)
            {
                this.time = time;
                this.duration = duration;

                isComplete = false;
                //playbackState = PlaybackState.stopped;
            }

            // ...

            public void play()
            {
                playbackState = PlaybackState.playing;
            }
            public void pause()
            {
                playbackState = PlaybackState.paused;
            }
            public void stop()
            {
                reset();
                playbackState = PlaybackState.stopped;
            }

            // ...

            public void reset()
            {
                time = 0.0f;
                //time = time - target;

                isComplete = false;
            }

            // ...

            public void setToComplete()
            {
                time = duration;
                isComplete = true;
            }

            // Returns true on every complete.

            public bool update()
            {
                return update(Time.deltaTime);
            }

            // ...

            public bool update(float t)
            {
                if (playbackState == PlaybackState.playing)
                {
                    if (!isComplete || loop)
                    {
                        isComplete = false;
                        time += t;

                        if (time >= duration)
                        {
                            if (loop)
                            {
                                reset();
                            }
                            else
                            {
                                time = duration;
                                isComplete = true;

                                playbackState = PlaybackState.stopped;
                            }

                            if (onTimerCompleteEvent != null)
                            {
                                onTimerCompleteEvent();
                            }

                            return true;
                        }
                    }
                }

                return false;
            }

            // =================================	
            // End functions.
            // =================================

        }

        // =================================	
        // End namespace.
        // =================================

    }

}

// =================================	
// --END-- //
// =================================
