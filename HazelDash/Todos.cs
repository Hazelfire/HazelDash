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
        public static readonly string TODO_TOO_SHORT_MESSAGE = "Todo is too short, must be at least " + TODO_MIN_STRING_SIZE.ToString() + " characters";
        public static readonly string TODO_TOO_LONG_MESSAGE = "Todo is too long, cannot be longer than " + TODO_MAX_STRING_SIZE.ToString() + " characters";
        public static readonly int TODO_MIN_STRING_SIZE = 1;
        public static readonly int TODO_MAX_STRING_SIZE = 200;

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

        static public string get(int index)
        {
            todoMutex.WaitOne();
            string line = File.ReadAllLines(TODO_FILENAME)[index];
            todoMutex.ReleaseMutex();
            return line;
            
        }

        static public string last()
        {
            return get(size() - 1);
        }

        static public void delete(int index)
        {
            todoMutex.WaitOne();
            string[] todos = File.ReadAllLines(TODO_FILENAME);

            string[] newTodos = new string[todos.Length - 1];

            int todoIndex = 0;
            int newTodoIndex = 0;
            foreach (string todoElement in todos)
            {
                if (todoIndex != index)
                {
                    newTodos[newTodoIndex] = todoElement;
                    newTodoIndex++;
                }
                todoIndex++;

            }

            File.WriteAllLines(TODO_FILENAME, newTodos);

            todoMutex.ReleaseMutex();

        }

        static public int size()
        {
            todoMutex.WaitOne();
            int lineCount;
            if (File.Exists(TODO_FILENAME))
            {
                lineCount = File.ReadAllLines(TODO_FILENAME).Count();
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
