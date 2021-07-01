using System;
using System.Collections;
using Dapper;

namespace CribblyBackend.DataAccess.Common
{
    public static class Query
    {
        public static DynamicParameters Params(params object[] ps)
        {
            if (ps.Length % 2 != 0)
            {
                throw new ArgumentException("Must pass an even number of parameters");
            }
            var p = new DynamicParameters();
            for (int i = 0; i < ps.Length; i += 2)
            {
                if (!(ps[i] is string))
                {
                    throw new ArgumentException("Even-numbered args must be strings");
                }
                var name = (string)ps[i];
                if (ps[i + 1] is IEnumerable)
                {
                    foreach (var o in (IEnumerable)ps[i + 1])
                    {
                        p.Add(name, o);
                    }
                    continue;
                }
                p.Add(name, ps[i + 1]);
            }
            return p;
        }
    }
}
