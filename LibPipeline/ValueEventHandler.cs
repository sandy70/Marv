using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    public delegate void ValueEventHandler<T>(object sender, ValueEventArgs<T> e);
}
