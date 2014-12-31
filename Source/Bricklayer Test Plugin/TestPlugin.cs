using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bricklayer.API;

namespace TestPlugin
{
    public class TestPlugin : BasePlugin
    {
        public override string Name { get { return "Test Plugin"; } }

        public override string Description { get { return "A test Bricklayer server plugin, built with the Bricklayer Plugin API."; } }

        public override string Author { get { return "Cyral"; } }

        public override string Version { get { return "1.0.0"; } }

        public TestPlugin()
        {

        }

        public override void Load()
        {
            Console.WriteLine(string.Format("{0} by {2} - {1} (Version {3}) loaded.", Name, Description, Author, Version));
            Console.WriteLine("It works!");
        }

        public override void Unload()
        {

        }
    }
}