using System;

namespace Infra.Controllers.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DebugMethodAttribute : Attribute
    {
    }
}