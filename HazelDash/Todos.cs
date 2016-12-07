using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace HazelDash
{
    public class TODOs
    {
        public static readonly string TODO_TOO_SHORT_MESSAGE = "Todo is too short, must be at least " + TODO_MIN_STRING_SIZE.ToString() + " characters";
        public static readonly string TODO_TOO_LONG_MESSAGE = "Todo is too long, cannot be longer than " + TODO_MAX_STRING_SIZE.ToString() + " characters";
        public static readonly int TODO_MIN_STRING_SIZE = 1;
        public static readonly int TODO_MAX_STRING_SIZE = 200;

        private static readonly string TODO_FILENAME = "todos.txt";

        static public void add(string TODO)
        {
            assertTodoValid(TODO);

            if (!File.Exists(TODO_FILENAME))
            {
                File.WriteAllText(TODO_FILENAME, TODO);
            }
            else {
                File.AppendAllText(TODO_FILENAME, "\n" + TODO);
            }
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
            return File.ReadAllLines(TODO_FILENAME)[index];
        }

        static public string last()
        {
            return get(size() - 1);
        }

        static public void delete(int index)
        {
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



        }

        static public int size()
        {
            if (File.Exists(TODO_FILENAME))
            {
                return File.ReadAllLines(TODO_FILENAME).Count();
            }
            else
            {
                return 0;
            }
        }

        static public void clear()
        {
            if (File.Exists(TODO_FILENAME))
            {
                File.Delete(TODO_FILENAME);
            }
        }
    }
}
