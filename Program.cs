using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace ConsoleElements
{
    using QuickConsole;
    using Simulation;
    using static QuickConsole.Buffering;
    internal class Program
    {
        public static Log logger = new Log(); //Automatically starts the logger application.

        private static void Exit()
        {
            logger.WriteLine( "Game stopped" );
            logger.pipe.Disconnect();
            logger.pipe.Dispose();
            Environment.Exit( Environment.ExitCode );
            Console.Read();
        }

        private static void Main(string[] args)
        {
            Console.BufferHeight = 100;
            Console.BufferWidth = 100;
            Console.SetWindowSize(100,100); 

            logger.WriteLine( "Game started" );
            #region INIT
            logger.WriteLine( "Loading Elements: " );
            ExtraTools.Loader.LoadElements();

            logger.WriteLine( "Loading Mods: " );
            ExtraTools.Loader.LoadMods();

            logger.WriteLine( "Elements loaded: " );
            foreach (KeyValuePair<string, Element> Entry in Simulation.Loaded.Elements)
            {
                logger.WriteLine( "    " + Entry.Key );
                Entry.Value.Spawn();
                logger.WriteLine( "        " + Entry.Value.Description.Category + " - " + Entry.Value.Description.Name4 );
            }
            GC.Collect();
            #endregion

            Buffering.Layers.Add( new Layer() );
            Buffering.Layers.Add( new Layer() );

            Buffering.TargetLayer = 0;
            Buffering.Put( new Vector2(10,10), 'A' );
            Buffering.Flush( 0 );

            Buffering.TargetLayer = 1;
            Buffering.DrawBox( Boxes.DubbleLine(), new Vector2( 0,45 ), new Vector2( 99,49 ) );
            Buffering.FlushTransparant( 1 );
            //Buffering.Clear( 1 );

            Buffering.TargetLayer = 0;
            while (true)
            {
                logger.WriteLine("dsgdsgdsg");
                Thread.Sleep( 100 );
                Buffering.Put( new Vector2( 10, 10 ), 'A' );
                Buffering.FlushTransparant( 0 );
            }

            Console.Read();
            Exit();
        }


    }
}
