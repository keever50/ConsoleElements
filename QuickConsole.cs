using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Numerics;
using System.Threading.Tasks;

namespace QuickConsole
{

    //Slow but usefull tools
    #region SlowConsoleStuff
    internal static class Drawing
    {
        public static void Put(Vector2 pos, string Str)
        {
            Console.SetCursorPosition( (int)pos.X, (int)pos.Y );
            Console.Write( Str );
        }
    }
    #endregion

    //Seperated console for improved peformance
    /// <summary>
    /// Automatically creates a seperated console application. 
    /// </summary>
    internal class Log 
    {
        public NamedPipeServerStream pipe;
        public Log()
        {
            try
            {
                Process p = Process.Start(@".\ExtLogger.exe");
                pipe = new NamedPipeServerStream( "ConsoleElements", PipeDirection.InOut );
                pipe.WaitForConnection();
            }
            catch (Exception E)
            {
                Console.WriteLine( "Couln't start the console." );
                Console.WriteLine( E.Message );
                Console.WriteLine( "Press any key to continue" );
                Console.ReadKey();
            }
        }

        ///<summary>
        ///Sends a line of text to the connected console. 
        ///Fast mode is threaded and might send the messages in the wrong order and use more memory.
        ///</summary>
        public void WriteLine(string str, bool Fast = false)
        {
            try
            {
                void act()
                {
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str + "\n");
                    pipe.Write( bytes, 0, bytes.Length );

                }
                Task t = new Task(act);
                if (Fast)
                {
                    t.Start();
                }
                else
                {
                    act();
                }
            }
            catch (Exception)
            {

            }
        }

    }

    //Quicker method for drawing on console screens.
    /// <summary>
    /// Quicker method for console drawing. 
    /// First use <see cref="Put(Vector2, char)"></see> to draw pixels onto the buffer.
    /// Then use <see cref="Flush()"/> to draw it into the screen.
    /// </summary>
    internal static class Buffering
    {

        private static readonly char[] Screen = new char[100 * 100];


        public static List<Layer> Layers = new List<Layer>();
        /// <summary>
        /// Selects the layer to draw on.
        /// </summary>
        public static int TargetLayer = 0;


        /// <summary>
        /// Actually just a preset 100x100 buffer with a name. 
        /// </summary>
        public class Layer
        {
            public char[,] Buffer = new char[100,100];
        }


        /// <summary>
        /// A group of <see cref="BoxCharSheet"/> presets.
        /// </summary>
        public static class Boxes
        {
            /// <summary>
            /// Simple line <see cref="BoxCharSheet"/>
            /// </summary>
            /// <returns>BoxCharSheet</returns>
            public static BoxCharSheet Line()
            {
                BoxCharSheet Sheet;
                Sheet.horizontal = '─';
                Sheet.uppercornerleft = '┌';
                Sheet.uppercornerright = '┐';
                Sheet.bottomcornerleft = '└';
                Sheet.bottomcornerright = '┘';
                Sheet.vertical = '│';
                return Sheet;
            }
            /// <summary>
            /// Simple dubble line <see cref="BoxCharSheet"/>
            /// </summary>
            /// <returns>BoxCharSheet</returns>
            public static BoxCharSheet DubbleLine()
            {
                BoxCharSheet Sheet;
                Sheet.horizontal = '═';
                Sheet.uppercornerleft = '╔';
                Sheet.uppercornerright = '╗';
                Sheet.bottomcornerleft = '╚';
                Sheet.bottomcornerright = '╝';
                Sheet.vertical = '║';
                return Sheet;
            }
        }

        /// <summary>
        /// Used in <see cref="DrawBox"/>
        /// <para>Use <see cref="Boxes"/> and its methods to create a preset. </para>
        /// </summary>
        public struct BoxCharSheet
        {
            public char horizontal;
            public char uppercornerleft;
            public char uppercornerright;
            public char bottomcornerleft;
            public char bottomcornerright;
            public char vertical;
        }

        /// <summary>
        /// Puts a character in the buffer. 
        /// <para><see cref="Flush()"/> after you used it to show on screen. 🚽</para>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="c"></param>
        public static void Put(Vector2 pos, char c)
        {
            if (pos.Y <= 99 & pos.X <= 99 & pos.Y >= 0 & pos.X >= 0)
            {
                try
                {
                    Layers[TargetLayer].Buffer[(int)pos.Y, (int)pos.X] = c;
                }
                catch (Exception)
                {

                }
            }

        }

        /// <summary>
        /// Moves the <see cref="CBuffer"/> to <see cref="Screen"/>
        /// <para>This means it will show what you draw. Though this overwrites the whole screen.</para>
        /// </summary>
        public static void Flush( int targetlayer )
        {
            Buffer.BlockCopy( Layers[targetlayer].Buffer, 0, Screen, 0, Screen.Length );
            Console.SetCursorPosition( 0, 0 );
            Console.Write( Screen, 0, Screen.Length );
            Console.SetCursorPosition( 0, 0 );
        }

        /// <summary>
        /// This is the same as normal Flush, but is a little slower.
        /// This is because it makes the selected layer drawn transparant. 
        /// <para>You can see whats underneath it now ^^</para>
        /// <para1>Note: Flushing the layer as transparant will cause the layer to persist after clearing the buffer.</para1>
        /// </summary>
        /// <param name="targetlayer"></param>
        public static void FlushTransparant( int targetlayer )
        {
            int index=0;
            foreach(char c in Layers[targetlayer].Buffer)
            {
                if (c != 0)
                {
                    Screen[index] = c;
                }
                else
                {
   
                }
                index++;
            }
            Console.SetCursorPosition( 0, 0 );
            Console.Write( Screen, 0, Screen.Length );
            Console.SetCursorPosition( 0, 0 );
        }

        /// <summary>
        /// Same as <see cref="Flush"/> but flushes it all.
        /// </summary>
        public static void FlushAll()
        {
            for (int I = 0; I <= Layers.Count-1; I++)
            {
                Buffer.BlockCopy( Layers[I].Buffer, 0, Screen, 0, Screen.Length );
            }
            Console.SetCursorPosition( 0, 0 );
            Console.Write( Screen, 0, Screen.Length );
            Console.SetCursorPosition( 0, 0 );
        }

        /// <summary>
        /// Same as <see cref="FlushTransparant"/> but flushes it all.
        /// </summary>
        public static void FlushTransparantAll()
        {
            for(int I = 0; I<=Layers.Count-1; I++)
            {
                int index=0;
                foreach (char c in Layers[I].Buffer)
                {
                    if (c != 0)
                    {
                        Screen[index] = c;
                    }
                    else
                    {

                    }
                    index++;
                }
            }
            Console.SetCursorPosition( 0, 0 );
            Console.Write( Screen, 0, Screen.Length );
            Console.SetCursorPosition( 0, 0 );
        }

        /// <summary>
        /// Clears the buffer, not the screeen.
        /// <para><see cref="Flush()"/> after you used it. 🚽</para>
        /// </summary>
        public static void Clear( int targetlayer )
        {
            Array.Clear( Layers[targetlayer].Buffer, 0, Layers[targetlayer].Buffer.Length );
        }

        public static void ClearScreen()
        {
            Array.Clear( Screen, 0, Screen.Length );
            Console.SetCursorPosition( 0, 0 );
            Console.Write( Screen, 0, Screen.Length );
            Console.SetCursorPosition( 0, 0 );
        }

        /// <summary>
        /// Writes a little message at <paramref name="pos"/>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="str"></param>
        public static void Message(Vector2 pos, string str)
        {
            char[] chars = str.ToCharArray();
            int index = 0;
            foreach (char c in chars)
            {
                Vector2 newpos = new Vector2(pos.X + index, pos.Y);
                Put( newpos, c );
                index++;
            }

        }

        /// <summary>
        /// Draws a box from A to B using <see cref="Buffering"/>.
        /// <para>So, <see cref="Flush()"/> after you used it. 🚽</para>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        public static void DrawBox(BoxCharSheet boxtype, Vector2 A, Vector2 B)
        {
            //Upside//
            for (int x = (int)A.X; x <= B.X; x++) //Upper
            {
                Put( new Vector2( x, A.Y ), boxtype.horizontal );
            }

            //Middle//
            for (int y = (int)A.Y; y <= B.Y; y++)
            {
                Put( new Vector2( A.X, y ), boxtype.vertical );
                Put( new Vector2( B.X, y ), boxtype.vertical );
            }

            //Downside//
            for (int x = (int)A.X; x <= B.X; x++) //Upper
            {
                Put( new Vector2( x, B.Y ), boxtype.horizontal );
            }

            //Corners
            Put( A, boxtype.uppercornerleft ); //Upper corner left
            Put( new Vector2( B.X, A.Y ), boxtype.uppercornerright ); //Upper corner left    
            Put( new Vector2( A.X, B.Y ), boxtype.bottomcornerleft ); //Upper corner left
            Put( B, boxtype.bottomcornerright ); //Upper corner left

        }
    }
}