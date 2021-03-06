﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HazelDash;

namespace DashboardTest
{
    [TestClass]
    public class AddTodoTest
    {

        [TestInitialize]
        public void Initialize()
        {
            TODOs.clear();
        }

        [TestMethod, TestCategory("Unit")]
        public void TODO_AddMultipleTodos_TodosAdded()
        {
            string[] TODOItems = { "This is a new TODO", "This is another Todo", "And another one" };
            uint originalTodoSize = TODOs.size();


            foreach (string todoElement in TODOItems)
            {
                TODOs.add(todoElement);
            }

            Assert.AreEqual(originalTodoSize + 3, TODOs.size());

            // Check if they have been added
            for (int i = 0; i < TODOs.size(); i++)
            {
                uint todoIndex = (uint)( TODOs.size() - TODOItems.Length + i);
                Assert.AreEqual(TODOItems[i], TODOs.get(todoIndex));
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void TODO_AddTooShortTodo_ThrowError()
        {
            string todoElement = "";

            try
            {
                TODOs.add(todoElement);

                // If it completed successfully, test failed.
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(TODOs.TODO_TOO_SHORT_MESSAGE, e.Message);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void TODO_AddMinimumSizeTodo_TodoAdded()
        {
            uint originalTodoSize = TODOs.size();
            string todoElement = "a";

            TODOs.add(todoElement);

            Assert.AreEqual(originalTodoSize + 1, TODOs.size());
            Assert.AreEqual(todoElement, TODOs.last());

        }

        [TestMethod, TestCategory("Unit")]
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
                TODOs.add(todoElement);

                // If it completed successfully, test failed.
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(TODOs.TODO_TOO_LONG_MESSAGE, e.Message);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void TODO_AddMaximumSizeTodo_TodoAdded()
        {
            uint originalTodoSize = TODOs.size();
            string todoElement = "";

            // Create maximum size Todo
            for (int i = 0; i < TODOs.TODO_MAX_STRING_SIZE; i++)
            {
                todoElement += "a";
            }

            TODOs.add(todoElement);

            Assert.AreEqual(originalTodoSize + 1, TODOs.size());
            Assert.AreEqual(todoElement, TODOs.last());

        }
        

        [TestCleanup]
        public void Cleanup() {

            TODOs.clear();
        }
    }
}
