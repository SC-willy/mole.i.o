using System;
using System.Collections;
using UnityEngine;

namespace Supercent.Util
{
    public static class CoroutineUtil
    {
        public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();


        public static IEnumerator WaitForSeconds(float sec)
        {
            var secDone = Time.time + sec;
            while (Time.time < secDone)
                yield return null;
        }
        public static IEnumerator WaitForSecondsUnscaled(float sec)
        {
            var secDone = Time.unscaledTime + sec;
            while (Time.unscaledTime < secDone)
                yield return null;
        }
        public static IEnumerator WaitForSecondsRealtime(float sec)
        {
            var secDone = Time.realtimeSinceStartup + sec;
            while (Time.realtimeSinceStartup < secDone)
                yield return null;
        }

        public static IEnumerator WaitUntil(Func<bool> predicate)
        {
            if (predicate != null)
            {
                while (!predicate())
                    yield return null;
            }
        }
        public static IEnumerator WaitForSecondsOrUntil(float sec, Func<bool> predicate)           => WaitForSeconds_PredicateJob(sec, predicate, true, true);
        public static IEnumerator WaitForSecondsAndUntil(float sec, Func<bool> predicate)          => WaitForSeconds_PredicateJob(sec, predicate, false, true);
        public static IEnumerator WaitForSecondsUnscaledOrUntil(float sec, Func<bool> predicate)   => WaitForUnscaled_PredicateJob(sec, predicate, true, true);
        public static IEnumerator WaitForSecondsUnscaledAndUntil(float sec, Func<bool> predicate)  => WaitForUnscaled_PredicateJob(sec, predicate, false, true);
        public static IEnumerator WaitForSecondsRealtimeOrUntil(float sec, Func<bool> predicate)   => WaitForRealtime_PredicateJob(sec, predicate, true, true);
        public static IEnumerator WaitForSecondsRealtimeAndUntil(float sec, Func<bool> predicate)  => WaitForRealtime_PredicateJob(sec, predicate, false, true);

        public static IEnumerator WaitWhile(Func<bool> predicate)
        {
            if (predicate != null)
            {
                while (predicate())
                    yield return null;
            }
        }
        public static IEnumerator WaitForSecondsOrWhile(float sec, Func<bool> predicate)           => WaitForSeconds_PredicateJob(sec, predicate, true, false);
        public static IEnumerator WaitForSecondsAndWhile(float sec, Func<bool> predicate)          => WaitForSeconds_PredicateJob(sec, predicate, false, false);
        public static IEnumerator WaitForSecondsUnscaledOrWhile(float sec, Func<bool> predicate)   => WaitForUnscaled_PredicateJob(sec, predicate, true, false);
        public static IEnumerator WaitForSecondsUnscaledAndWhile(float sec, Func<bool> predicate)  => WaitForUnscaled_PredicateJob(sec, predicate, false, false);
        public static IEnumerator WaitForSecondsRealtimeOrWhile(float sec, Func<bool> predicate)   => WaitForRealtime_PredicateJob(sec, predicate, true, false);
        public static IEnumerator WaitForSecondsRealtimeAndWhile(float sec, Func<bool> predicate)  => WaitForRealtime_PredicateJob(sec, predicate, false, false);


        static IEnumerator WaitForSeconds_PredicateJob(float sec, Func<bool> predicate, bool isOr, bool isUntil)
        {
            var secDone = Time.time + sec;
            if (predicate == null)
                while (Time.time < secDone)
                    yield return null;
            else
            {
                if (isOr)
                {
                    if (isUntil)
                        while (Time.time < secDone && !predicate())
                            yield return null;
                    else
                        while (Time.time < secDone && predicate())
                            yield return null;
                }
                else
                {
                    if (isUntil)
                        while (Time.time < secDone || !predicate())
                            yield return null;
                    else
                        while (Time.time < secDone || predicate())
                            yield return null;
                }
            }
        }
        static IEnumerator WaitForUnscaled_PredicateJob(float sec, Func<bool> predicate, bool isOr, bool isUntil)
        {
            var secDone = Time.unscaledTime + sec;
            if (predicate == null)
                while (Time.unscaledTime < secDone)
                    yield return null;
            else
            {
                if (isOr)
                {
                    if (isUntil)
                        while (Time.unscaledTime < secDone && !predicate())
                            yield return null;
                    else
                        while (Time.unscaledTime < secDone && predicate())
                            yield return null;
                }
                else
                {
                    if (isUntil)
                        while (Time.unscaledTime < secDone || !predicate())
                            yield return null;
                    else
                        while (Time.unscaledTime < secDone || predicate())
                            yield return null;
                }
            }
        }
        static IEnumerator WaitForRealtime_PredicateJob(float sec, Func<bool> predicate, bool isOr, bool isUntil)
        {
            var secDone = Time.realtimeSinceStartup + sec;
            if (predicate == null)
                while (Time.realtimeSinceStartup < secDone)
                    yield return null;
            else
            {
                if (isOr)
                {
                    if (isUntil)
                        while (Time.realtimeSinceStartup < secDone && !predicate())
                            yield return null;
                    else
                        while (Time.realtimeSinceStartup < secDone && predicate())
                            yield return null;
                }
                else
                {
                    if (isUntil)
                        while (Time.realtimeSinceStartup < secDone || !predicate())
                            yield return null;
                    else
                        while (Time.realtimeSinceStartup < secDone || predicate())
                            yield return null;
                }
            }
        }
    }
}