
using System;
using System.Linq;
using System.Reflection;
using Glide;

namespace Indigo.Utility
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterLerper : Attribute
    {
        public Type TypeToLerp { get; private set; }

        public RegisterLerper(Type type)
        {
            TypeToLerp = type;
        }

        public static void AutoLoadFromAssembly(Assembly asm)
        {
            foreach (var lerper in asm.GetTypes().Where(t => typeof(MemberLerper).IsAssignableFrom(t)))
            {
                var attrs = lerper.GetCustomAttributes(typeof(RegisterLerper), false);
                if (attrs.Length == 0)
                    continue;

                foreach (RegisterLerper attr in attrs)
                    Tweener.SetLerper(lerper, attr.TypeToLerp);
            }
        }
    }
}
