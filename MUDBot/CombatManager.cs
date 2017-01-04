using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUDBot
{
    public class CombatManager
    {
        private MUDWorld world;
        private List<CombatInstance> instances;

        public CombatManager(MUDWorld _world)
        {
            instances = new List<CombatInstance>();
            world = _world;
        }

        public int CreateCombatInstance()
        {
            instances.Add(new CombatInstance(world));
            return instances.Count - 1;
        }

        public CombatInstance GetInstance(int id)
        {
            if (id >= instances.Count || id <= 0)
                return null;

            return instances[id];
        }

    }
}
