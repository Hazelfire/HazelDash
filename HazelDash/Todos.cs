using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


namespace HazelDash
{
    public class TODOs
    {
        public static readonly int TODO_MIN_STRING_SIZE = 1;
        public static readonly int TODO_MAX_STRING_SIZE = 200;
        public static readonly string TODO_TOO_SHORT_MESSAGE = "Todo is too short, must be at least " + TODO_MIN_STRING_SIZE.ToString() + " characters";
        public static readonly string TODO_TOO_LONG_MESSAGE = "Todo is too long, cannot be longer than " + TODO_MAX_STRING_SIZE.ToString() + " characters";

        public static readonly string TODO_INDEX_TOO_LARGE = "Your index is too large, and does not refer to any todo";

        private static readonly string TODO_FILENAME = "todos.txt";
        private static Mutex todoMutex = new Mutex();

        static public void add(string TODO)
        {
            assertTodoValid(TODO);
            todoMutex.WaitOne();
            if (!File.Exists(TODO_FILENAME))
            {
                File.WriteAllText(TODO_FILENAME, TODO);
            }
            else {
                File.AppendAllText(TODO_FILENAME, "\n" + TODO);
            }
            todoMutex.ReleaseMutex();
        }

        static private void assertTodoValid(string TODO)
        {
            if (TODO.Length < TODO_MIN_STRING_SIZE)
            {
                throw new ArgumentException(TODO_TOO_SHORT_MESSAGE);
            }

            if(TODO.Length > TODO_MAX_STRING_SIZE)
            {
                throw new ArgumentException(TODO_TOO_LONG_MESSAGE);
            }
        }

        static public string get(uint index)
        {
            assertTodoIndexValid(index);
            todoMutex.WaitOne();
            string[] todoElements = File.ReadAllLines(TODO_FILENAME);

            string line = File.ReadAllLines(TODO_FILENAME)[index];
            todoMutex.ReleaseMutex();
            return line;
            
        }

        private static void assertTodoIndexValid(uint index)
        {
            if(index >= TODOs.size())
            {
                throw new ArgumentException(TODO_INDEX_TOO_LARGE);
            }
        }

        static public string last()
        {
            return get(size() - 1);
        }

        static public void delete(uint index)
        {
            assertTodoIndexValid(index);
            todoMutex.WaitOne();
            string[] todos = new string[0];
            if (File.Exists(TODO_FILENAME))
            {
                todos = File.ReadAllLines(TODO_FILENAME);
            }
            string[] newTodos = new string[todos.Length - 1];
            
            int newTodoIndex = 0;
            for (int todoIndex = 0; todoIndex < todos.Length; todoIndex++)
            {
                if (todoIndex != index)
                {
                    newTodos[newTodoIndex] = todos[todoIndex];
                    newTodoIndex++;
                }

            }

            if (newTodos.Length == 0)
            {
                File.Delete(TODO_FILENAME);
            }
            else {
                File.WriteAllLines(TODO_FILENAME, newTodos);
            }
            todoMutex.ReleaseMutex();

        }

        static public uint size()
        {
            todoMutex.WaitOne();
            uint lineCount;
            if (File.Exists(TODO_FILENAME))
            {
                lineCount = (uint)File.ReadAllLines(TODO_FILENAME).Count();
            }
            else
            {
                lineCount = 0;
            }
            todoMutex.ReleaseMutex();

            return lineCount;
        }

        static public void clear()
        {
            todoMutex.WaitOne();
            if (File.Exists(TODO_FILENAME))
            {
                File.Delete(TODO_FILENAME);
            }
            todoMutex.ReleaseMutex();
        }
    }
}
