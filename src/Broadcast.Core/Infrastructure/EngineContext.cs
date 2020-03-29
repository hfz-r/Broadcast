using System.Runtime.CompilerServices;

namespace Broadcast.Core.Infrastructure
{
    public class EngineContext
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new Engine());
        }

        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #region Properties

        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null) Create();
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}