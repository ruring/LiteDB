﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB.Shell
{
    public class InputCommand
    {
        public Queue<string> Queue { get; set; }
        public string Last { get; set; }
        public Stopwatch Timer { get; set; }

        public Action<string> OnWrite { get; set; }

        public InputCommand()
        {
            this.Queue = new Queue<string>();
            this.Last = "";
            this.Timer = new Stopwatch();
        }

        public string ReadCommand()
        {
            if (this.Timer.IsRunning)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                this.Write(this.Timer.ElapsedMilliseconds.ToString("0000") + " ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            this.Write("> ");

            var cmd = this.ReadLine();

            // suport for multiline command
            if (cmd.StartsWith("/"))
            {
                cmd = cmd.Substring(1);

                while (!cmd.EndsWith("/"))
                {
                    if (this.Timer.IsRunning)
                    {
                        this.Write("     ");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    this.Write("| ");

                    var line = this.ReadLine();
                    cmd += line;
                }

                cmd = cmd.Substring(0, cmd.Length - 1);
            }

            cmd = cmd.Trim();

            this.Last = cmd;

            if (this.Timer.IsRunning)
            {
                this.Timer.Reset();
                this.Timer.Start();
            }

            return cmd.Trim();
        }

        /// <summary>
        /// Read a line from queue or user
        /// </summary>
        private string ReadLine()
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            if (this.Queue.Count > 0)
            {
                var cmd = this.Queue.Dequeue();
                this.Write(cmd + Environment.NewLine);
                return cmd;
            }
            else
            {
                var cmd = Console.ReadLine();

                if (this.OnWrite != null)
                {
                    this.OnWrite(cmd + Environment.NewLine);
                }

                return cmd;
            }
        }

        private void Write(string text)
        {
            Console.Write(text);

            if (this.OnWrite != null)
            {
                this.OnWrite(text);
            }
        }
    }
}
