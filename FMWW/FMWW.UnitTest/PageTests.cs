using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FMWW.UnitTest
{
    [TestClass]
    public class PageTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");
        private static readonly Entity.UserAccount EmptyUserAccount = new Entity.UserAccount()
        {
            UserName = String.Empty,
            Password = String.Empty,
            Person = String.Empty,
            PersonPassword = String.Empty
        };

        [TestMethod]
        [Priority(1)]
        [TestCategory("正常系")]
        public void TestSignIn()
        {
            var page = new Page()
            {
                UserAccount = UserAccount
            };
            page.SignIn();
            Assert.IsTrue(true);
        }

        [TestMethod]
        [Priority(1)]
        [TestCategory("正常系")]
        public void TestSignInAsync()
        {
            var page = new Page()
            {
                UserAccount = UserAccount
            };
            var task = page.SignInAsync();
            task.Wait();
            Assert.IsFalse(task.Result);
        }

        [TestMethod]
        [Priority(1)]
        [TestCategory("異常系")]
        [ExpectedException(typeof(Exception))]
        public void TestSignInFailure()
        {
            var page = new Page() { UserAccount = EmptyUserAccount };
            page.SignIn();
        }

        [TestMethod]
        [Priority(1)]
        [TestCategory("異常系")]
        public void TestSignInAsyncFailure()
        {
            var page = new Page()
            {
                UserAccount = EmptyUserAccount
            };
            var task = page.SignInAsync();
            task.Wait();
            Assert.IsTrue(task.Result);
        }

        private class Page : FMWW.Http.Page
        {
            public Page() : base() { }

            public new void SignIn()
            {
                base.SignIn();
            }

            public Task<bool> SignInAsync()
            {
                var task = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    var signedIn = false;
                    var hasError = false;
                    FMWW.Component.SignedInEventHandler onSignedIn = null;
                    onSignedIn = (a, b) =>
                    {
                        Auth.SignedIn -= onSignedIn;
                        if (b.Error is Exception)
                        {
                            hasError = true;
                        }
                        signedIn = true;
                    };
                    Auth.SignedIn += onSignedIn;
                    base.SignInAsync();
                    while (!signedIn)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    return hasError;
                });
                return task;
            }
        }
    }
}
