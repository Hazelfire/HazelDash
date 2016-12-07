using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HazelDash;

namespace DashboardTest
{
    [TestClass]
    public class AddTodoTest
    {

        private int mTodosAdded = 0;

        [TestInitialize]
        public void Initialize()
        {
            TODOs.clear();
        }

        private void addTodo(string todo)
        {
            TODOs.add(todo);
            mTodosAdded++;
        }

        [TestMethod]
        public void TODO_AddMultipleTodos_TodosAdded()
        {
            string[] TODOItems = { "This is a new TODO", "This is another Todo", "And another one" };
            int originalTodoSize = TODOs.size();


            foreach (string todoElement in TODOItems)
            {
                addTodo(todoElement);
            }

            Assert.AreEqual(originalTodoSize + 3, TODOs.size());

            // Check if they have been added
            for (int i = 0; i < TODOs.size(); i++)
            {
                int todoIndex = TODOs.size() - TODOItems.Length + i;
                Assert.AreEqual(TODOItems[i], TODOs.get(todoIndex));
            }
        }

        [TestMethod]
        public void TODO_AddTooShortTodo_ThrowError()
        {
            string todoElement = "";

            try
            {
                addTodo(todoElement);

                // If it completed successfully, test failed.
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(TODOs.TODO_TOO_SHORT_MESSAGE, e.Message);
            }
        }

        [TestMethod]
        public void TODO_AddMinimumSizeTodo_TodoAdded()
        {
            int originalTodoSize = TODOs.size();
            string todoElement = "a";

            addTodo(todoElement);

            Assert.AreEqual(originalTodoSize + 1, TODOs.size());
            Assert.AreEqual(todoElement, TODOs.last());

        }

        [TestMethod]
        public void TODO_AddTooLongTodo_ThrowError()
        {

            string todoElement = "";

            // Create very large todo, 1 + Max size
            for (int i = 0; i < TODOs.TODO_MAX_STRING_SIZE + 1; i++)
            {
                todoElement += "a";
            }

            try
            {
                addTodo(todoElement);

                // If it completed successfully, test failed.
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(TODOs.TODO_TOO_LONG_MESSAGE, e.Message);
            }
        }

        [TestMethod]
        public void TODO_AddMaximumSizeTodo_TodoAdded()
        {
            int originalTodoSize = TODOs.size();
            string todoElement = "";

            // Create maximum size Todo
            for (int i = 0; i < TODOs.TODO_MAX_STRING_SIZE; i++)
            {
                todoElement += "a";
            }

            addTodo(todoElement);

            Assert.AreEqual(originalTodoSize + 1, TODOs.size());
            Assert.AreEqual(todoElement, TODOs.last());

        }

        [TestMethod]
        public void TODO_NetworkAddTodo_TodoAdded()
        {
            DashboardServer server = new DashboardServer();
            server.start();
            
            int originalTodosSize = TODOs.size();
            string todoElement = "This is a todo";

            TestClient client = new TestClient();
            client.connect("127.0.0.1");
            client.send("addtodo " + todoElement);

            Assert.AreEqual(originalTodosSize + 1, TODOs.size());
            Assert.AreEqual(todoElement, TODOs.last());

        }

        [TestCleanup]
        public void Cleanup() {

            int deleteFrom = TODOs.size() - mTodosAdded;
            for(int i = 0; i < mTodosAdded; i++)
            {
                TODOs.delete(deleteFrom);
            }

            mTodosAdded = 0;
        }
    }
}
