using System.Collections;
using System.Collections.Generic;

namespace Supercent.Util
{
    public static class EnumeratorUtil
    {
        public static IEnumerator Start(IEnumerator enumerator)
        {
            if (enumerator != null)
                enumerator.MoveNext();
            return enumerator;
        }

        public static bool Update(IEnumerator enumerator)
        {
            return enumerator == null ? false
                 : enumerator.Current is IEnumerator sub && Update(sub) ? true
                 : enumerator.MoveNext();
        }
        public static bool Update(IEnumerator<IEnumerator> enumerator)
        {
            return enumerator == null ? false
                 : Update(enumerator.Current) ? true
                 : enumerator.MoveNext();
        }
    }
}