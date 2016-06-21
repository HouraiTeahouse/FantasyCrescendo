using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace HouraiTeahouse {

    internal class MediatorTest {

        // Three test event classes
        class A {
            public int a;
        }

        class B : A {
            public int b;
        }

        class C : B {
            public int c;
        }

        const int aCount = 5;
        const int bCount = 1;
        const int cCount = 10;

        readonly Mediator.Event<A> eA = delegate(A a) { a.a++; };

        readonly Mediator.Event<B> eB = delegate(B b) {
            b.a++;
            b.b++;
        };

        readonly Mediator.Event<C> eC = delegate(C c) {
            c.a++;
            c.b++;
            c.c++;
        };

        Mediator CreateTestMediator() {
            var mediator = new Mediator();
            for(var i = 0; i < aCount; i ++)
                mediator.Subscribe(eA);
            for(var i = 0; i < bCount; i ++)
                mediator.Subscribe(eB);
            for(var i = 0; i < cCount; i ++)
                mediator.Subscribe(eC);
            return mediator;
        }

        void ExecuteTest(Mediator mediator, int a, int b, int c) {
            var testA = new A();
            var testB = new B();
            var testC = new C();
            mediator.Publish(testA);
            mediator.Publish(testB);
            mediator.Publish(testC);
            Assert.AreEqual(a, testA.a);
            Assert.AreEqual(a + b, testB.a);
            Assert.AreEqual(b, testB.b);
            Assert.AreEqual(a + b + c, testC.a);
            Assert.AreEqual(b + c, testC.b);
            Assert.AreEqual(c, testC.c);
        }

        [Test]
        public void GetSubscriberCountTest() {
            Mediator mediator = CreateTestMediator();
            Assert.AreEqual(aCount, mediator.GetSubscriberCount(typeof(A)));
            Assert.AreEqual(bCount, mediator.GetSubscriberCount(typeof(B)));
            Assert.AreEqual(cCount, mediator.GetSubscriberCount(typeof(C)));
            Assert.AreEqual(aCount, mediator.GetSubscriberCount<A>());
            Assert.AreEqual(bCount, mediator.GetSubscriberCount<B>());
            Assert.AreEqual(cCount, mediator.GetSubscriberCount<C>());
            Assert.Catch<ArgumentNullException>(delegate {
                mediator.GetSubscriberCount(null);
            });
        }

        [Test]
        public void ResetTest() {
            Mediator mediator = CreateTestMediator();
            Assert.AreNotEqual(0, mediator.GetSubscriberCount<A>());
            mediator.Reset<A>();
            Assert.AreEqual(0, mediator.GetSubscriberCount<A>());
            Assert.AreNotEqual(0, mediator.GetSubscriberCount<B>());
            Assert.AreNotEqual(0, mediator.GetSubscriberCount<C>());
            mediator.Reset<B>();
            Assert.AreEqual(0, mediator.GetSubscriberCount<A>());
            Assert.AreEqual(0, mediator.GetSubscriberCount<B>());
            Assert.AreNotEqual(0, mediator.GetSubscriberCount<C>());
            mediator.Reset<C>();
            Assert.AreEqual(0, mediator.GetSubscriberCount<A>());
            Assert.AreEqual(0, mediator.GetSubscriberCount<B>());
            Assert.AreEqual(0, mediator.GetSubscriberCount<C>());
            Assert.Catch<ArgumentNullException>(delegate {
                mediator.Reset(null);
            });
        }

        [Test]
        public void ResetAllTest() {
            Mediator mediator = CreateTestMediator();
            mediator.ResetAll();
            Assert.AreEqual(0, mediator.GetSubscriberCount<A>());
            Assert.AreEqual(0, mediator.GetSubscriberCount<B>());
            Assert.AreEqual(0, mediator.GetSubscriberCount<C>());
        }

        [Test]
        public void PublishTest() {
            Mediator mediator = CreateTestMediator();
            ExecuteTest(mediator, aCount, bCount, cCount);
            Assert.Catch<ArgumentNullException>(delegate {
                mediator.Publish(null);
            });
        }

        [Test]
        public void SubscribeTest() {
            Mediator mediator = CreateTestMediator();
            mediator.Subscribe(eA);
            mediator.Subscribe(eA);
            mediator.Subscribe(eB);
            mediator.Subscribe(eC);
            Assert.AreEqual(aCount + 2, mediator.GetSubscriberCount<A>());
            Assert.AreEqual(bCount + 1, mediator.GetSubscriberCount<B>());
            Assert.AreEqual(cCount + 1, mediator.GetSubscriberCount<C>());
            ExecuteTest(mediator, aCount + 2, bCount + 1, cCount + 1);
            Assert.Catch<ArgumentNullException>(delegate {
                mediator.Subscribe<A>(null);
            });
        }

        [Test]
        public void UnsubscribeTest() {
            Mediator mediator = CreateTestMediator();
            mediator.Unsubscribe(eA);
            mediator.Unsubscribe(eA);
            mediator.Unsubscribe(eB);
            mediator.Unsubscribe(eC);
            Assert.AreEqual(aCount - 2, mediator.GetSubscriberCount<A>());
            Assert.AreEqual(bCount - 1, mediator.GetSubscriberCount<B>());
            Assert.AreEqual(cCount - 1, mediator.GetSubscriberCount<C>());
            ExecuteTest(mediator, aCount - 2, bCount - 1, cCount - 1);
            Assert.Catch<ArgumentNullException>(delegate {
                mediator.Unsubscribe<A>(null);
            });
        }
    }
}

