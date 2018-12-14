using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin
{
    class UUIDConverter
    {
        public UUIDConverter()
        {
            float lon = 25.037724F;

            float lat = 121.564063F;


            byte[] aBytes = BitConverter.GetBytes(lon);
            byte[] bBytes = BitConverter.GetBytes(lat);
            string uuid = "40200000-0000-";
            int _tmpCount = 0;
            foreach (var q in aBytes)
            {
                uuid += (Convert.ToString(q, 16).PadLeft(2, '0'));
                if (_tmpCount++ == 1) uuid += "-";
            }
            uuid += "-0000";
            foreach (var q in bBytes)
                uuid += (Convert.ToString(q, 16).PadLeft(2, '0'));
            Console.Write(uuid.ToUpper());
        }
    }
}
