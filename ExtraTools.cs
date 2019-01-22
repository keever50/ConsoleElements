using ConsoleElements;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtraTools
{
    internal static class Loader
    {

        internal static void LoadElements()
        {


            Type[] Types = Find.TypesInNamespace(Assembly.GetExecutingAssembly(), "Elements");
            foreach (Type T in Types)
            {
                Program.logger.WriteLine( "    " + T.Name );
                Simulation.Loaded.Elements.Add( T.Name, ((Simulation.Element)Activator.CreateInstance( T )) );
            }
        }

        internal static void LoadMods()
        {

            string path = "./Mods/";
            path = Path.GetFullPath( path );
            string[] ModPaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (string ModPath in ModPaths)
            {
                Program.logger.WriteLine( "     Loading " + Path.GetFileName( ModPath ) );
                Assembly DLL = Assembly.LoadFile(ModPath);
                foreach (Type type in DLL.GetTypes())
                {
                    Program.logger.WriteLine( "        " + type.Name );
                    Simulation.Element EL = (Simulation.Element)Activator.CreateInstance(type, false);
                    Simulation.Loaded.Elements.Add( type.Name, EL );

                }
            }

        }
    }

    internal class Find
    {
        public static Type[] TypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where( t => String.Equals( t.Namespace, nameSpace, StringComparison.Ordinal ) )
                      .ToArray();
        }
    }
}