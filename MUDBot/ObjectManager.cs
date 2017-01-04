using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUDBot
{
    public class ObjectManager
    {
        Dictionary<string, MUDObject> ITEM_DATABASE;

        public ObjectManager()
        {
            List<MUDObject> data = DataManager.ReadAllObjects();
            ITEM_DATABASE = new Dictionary<string, MUDObject>();
            foreach(var obj in data)
            {
                ITEM_DATABASE.Add(obj.Name, obj);
            }
        }

        public void AddItemToDB(MUDObject obj)
        {
            if(obj != null && !ITEM_DATABASE.ContainsKey(obj.Name))
            {
                ITEM_DATABASE.Add(obj.Name, obj);
            }
        }

        public MUDObject GetObjectBase(string name)
        {
            if (ITEM_DATABASE.ContainsKey(name))
            {
                return ITEM_DATABASE[name];
            }

            return null;
        }

        public void Shutdown()
        {
            foreach(var obj in ITEM_DATABASE)
            {
                DataManager.WriteJson(obj.Value);
            }
        }
    }
}
