using System.Collections.Generic;

namespace Daily.Build
{
    interface IBuildRetriver
    {
        List<TcBuild> Get();
    }
}
