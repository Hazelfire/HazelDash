using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HazelDash;

namespace DashboardTest
{
    [TestClass]
    public class NetwordAddTodoTest
    {
        DashboardClient mClient;
        DashboardServer mServer;


        [TestInitialize]
        public void Initialize()
        {
            mServer = new DashboardServer();
            mServer.start();
            mClient = new DashboardClient("127.0.0.1");
            
            mClient.TODOClear();
        }

        [TestMethod, TestCategory("Unit")]
        public void NETTODO_AddMultipleTodos_TodosAdded()
        {
            string[] TODOItems = { "This is a new TODO", "This is another Todo", "And another one" };
            int originalTodoSize = mClient.TODOSize();


            foreach (string todoElement in TODOItems)
            {
                mClient.TODOAdd(todoElement);
            }

            int newTodoSize = mClient.TODOSize();

            Assert.AreEqual(originalTodoSize + 3, newTodoSize);

            // Check if they have been added
            for (int i = 0; i < newTodoSize; i++)
            {
                int todoIndex = newTodoSize - TODOItems.Length + i;
                Assert.AreEqual(TODOItems[i], mClient.TODOGet(todoIndex));
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void NETTODO_AddTooShortTodo_ThrowError()
        {
            string todoElement = "";

            try
            {
                mClient.TODOAdd(todoElement);

                // If it completed successfully, test failed.
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(TODOs.TODO_TOO_SHORT_MESSAGE, e.Message);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void NETTODO_ConnectInvalidServer_ThrowError()
        {
            try {
                DashboardClient client = new DashboardClient("notARUL");
                Assert.Fail();
            }
            catch(AggregateException e)
            {
                Assert.AreEqual("Unable to connect to the remote server", e.GetBaseException().Message);
                
            }

        }

        [TestMethod, TestCategory("Unit")]
        public void NETTODO_AddMinimumSizeTodo_TodoAdded()
        {
            int originalTodoSize = mClient.TODOSize();
            string todoElement = "a";

            mClient.TODOAdd(todoElement);

            Assert.AreEqual(originalTodoSize + 1, mClient.TODOSize());
            Assert.AreEqual(todoElement, mClient.TODOLast());

        }

        [TestMethod, TestCategory("Unit")]
        public void NETTODO_AddTooLongTodo_ThrowError()
        {

            string todoElement = "";

            // Create very large todo, 1 + Max size
            for (int i = 0; i < TODOs.TODO_MAX_STRING_SIZE + 1; i++)
            {
                todoElement += "a";
            }

            try
            {
                mClient.TODOAdd(todoElement);

                // If it completed successfully, test failed.
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(TODOs.TODO_TOO_LONG_MESSAGE, e.Message);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void NETTODO_AddMaximumSizeTodo_TodoAdded()
        {
            int originalTodoSize = mClient.TODOSize();
            string todoElement = "";

            // Create maximum size Todo
            for (int i = 0; i < TODOs.TODO_MAX_STRING_SIZE; i++)
            {
                todoElement += "a";
            }

            mClient.TODOAdd(todoElement);

            Assert.AreEqual(originalTodoSize + 1, mClient.TODOSize());
            Assert.AreEqual(todoElement, mClient.TODOLast());

        }

        [TestCleanup]
        public void Cleanup()
        {
            mClient.TODOClear();
            mClient.close();
            mServer.stop();
            
        }
    }
}
