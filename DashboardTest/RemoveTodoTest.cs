using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HazelDash;

namespace DashboardTest
{
    [TestClass]
    public class RemoveTodoTest
    {
        [TestInitialize]
        public void Initialize()
        {
            TODOs.clear();
        }

        [TestMethod, TestCategory("Unit")]
        public void TODO_RemoveValidTodos_Success()
        {
            string[] todos = { "This is a new TODO", "This is another Todo", "And another one" };

            // Add elements
            foreach (string todoElement in todos)
            {
                TODOs.add(todoElement);
            }

            int todosSize = todos.Length;

            // Remove elements
            for (uint i = 0; i < todos.Length; i++)
            {
                Assert.AreEqual(todos[i], TODOs.get(0));
                TODOs.delete(0);
            }

            Assert.AreEqual( (uint)( todosSize - todos.Length), TODOs.size());
        }

        [TestMethod, TestCategory("Unit")]
        public void TODO_RemoveTopTodos_Success()
        {
            string[] todos = { "This is a new TODO", "This is another Todo", "And another one" };

            // Add elements
            foreach (string todoElement in todos)
            {
                TODOs.add(todoElement);
            }

            int todosSize = todos.Length;

            // Remove elements
            for (uint i = 0; i < 3; i++)
            {
                uint topIndex = TODOs.size() - 1;
                Assert.AreEqual(todos[topIndex], TODOs.get(topIndex));
                TODOs.delete(topIndex);
            }

            Assert.AreEqual((uint)( todosSize - todos.Length), TODOs.size());
        }

        [TestMethod, TestCategory("Unit")]
        public void TODO_RemoveNonExistantTodo_ThrowError()
        {
            try
            {
                TODOs.delete(TODOs.size());
                Assert.Fail();
            }
            catch(ArgumentException e)
            {
                Assert.AreEqual(TODOs.TODO_INDEX_TOO_LARGE, e.Message);
            }
            
        }
    }
}
