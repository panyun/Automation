using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL
{
    public class ProtoObject : ELObject
    {
        public object Clone()
        {
            //byte[] bytes = ProtobufHelper.ToBytes(this);
            //return ProtobufHelper.FromBytes(this.GetType(), bytes, 0, bytes.Length);
            return default;
        }
    }
}
