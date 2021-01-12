using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{
    public static class MapperExtensions
    {
        public static T MapTo<T>(this object obj, IScope scope)
            where T : class
        {
            var mapper = scope.Resolve<IMapper>();
            return MapTo<T>(obj, mapper);
        }


        public static T MapTo<T>(this object obj, IMapper mapper)
            where T : class
        {
            if (obj == null) return default(T);
            return mapper.MapTo<T>(obj);
        }


        public static T SetProperty<T>(this T source, Action<T> act)
            where T : class
        {
            if (source != null)
            {
                act(source);
            }
            return source;
        }
    }
}
