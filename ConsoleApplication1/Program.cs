using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyEditor;

namespace IcingRef
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryEditor.Init();
            Linesman linesman = new Linesman();

            while(true)
            {
                linesman.checkForIcing();
                System.Threading.Thread.Sleep(50);       // approximates ping
            }
        }
    }
}
