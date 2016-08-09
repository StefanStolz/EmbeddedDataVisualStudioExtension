using SomeClassLibrary.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeClassLibrary
{
    public class SomeCode
    {
        public void UseFileHandle()
        {
            using (var file = Files.avatar_med.OpenFile())
            {
                string path = file.Path;
            }
        }
    }
}
