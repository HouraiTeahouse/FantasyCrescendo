using System;
using NUnit.Framework;

namespace HouraiTeahouse {

    //TODO: More thoroughly test this class
    public class MediatorTest {

        // Three test event classes
        class A : I {

            public int Ia;

            public int IA {
                get { return Ia; }
                set { Ia = value; }
            }

        }

        class B : A {

            public int b;

        }

        class C : B {

            public int c;

        }

        interface I {

            int IA { get; set; }

        }

        const int aCount = 5;
        const int bCount = 1;
        const int cCount = 10;
        const int iCount = 5;

        readonly Mediator.Event<A> eA = delegate(A a) { a.Ia++; };

        readonly Mediator.Event<B> eB = delegate(B b) {
            b.Ia++;
            b.b++;
        };

        readonly Mediator.Event<C> eC = delegate(C c) {
            c.Ia++;
            c.b++;
            c.c++;
        };

        readonly Mediator.Event<I> eI = delegate(I i) { i.IA++; };

        Mediator CreateTestMediator() {
            var mediator = new Mediator();
            for (var i = 0; i < aCount; i ++)
                mediator.Subscribe(eA);
            for (var i = 0; i < bCount; i ++)
                mediator.Subscribe(eB);
            for (var i = 0; i < cCount; i ++)
                mediator.Subscribe(eC);
            for (var i = 0; i < iCount; i ++)
                mediator.Subscribe(eI);
            return mediator;
        }

        void ExecuteTest(Mediator mediator, int a, int b, int c, int i) {
            a += i;
            var testA = new A();
            var testB = new B();
            var testC = new C();
            mediator.Publish(testA);
            mediator.Publish(testB);
            mediator.Publish(testC);
            Assert.AreEqual(a, testA.Ia);
            Assert.AreEqual(a + b, testB.Ia);
            Assert.AreEqual(b, testB.b);
            Assert.AreEqual(a + b + c, testC.Ia);
            Assert.AreEqual(b + c, testC.b);
            Assert.AreEqual(c, testC.c);
        }

        [Test]
        public void get_count() {
            Mediator mediator = CreateTestMediator();
            Assert.AreEqual(aCount, mediator.GetCount(typeof(A)));
            Assert.AreEqual(bCount, mediator.GetCount(typeof(B)));
            Assert.AreEqual(cCount, mediator.GetCount(typeof(C)));
            Assert.AreEqual(aCount, mediator.GetCount<A>());
            Assert.AreEqual(bCount, mediator.GetCount<B>());
            Assert.AreEqual(cCount, mediator.GetCount<C>());
            Assert.Catch<ArgumentNullException>(delegate { mediator.GetCount(null); });
        }

        [Test]
        public void reset() {
            Mediator mediator = CreateTestMediator();
            Assert.AreNotEqual(0, mediator.GetCount<A>());
            mediator.Reset<A>();
            Assert.AreEqual(0, mediator.GetCount<A>());
            Assert.AreNotEqual(0, mediator.GetCount<B>());
            Assert.AreNotEqual(0, mediator.GetCount<C>());
            mediator.Reset<B>();
            Assert.AreEqual(0, mediator.GetCount<A>());
            Assert.AreEqual(0, mediator.GetCount<B>());
            Assert.AreNotEqual(0, mediator.GetCount<C>());
            mediator.Reset<C>();
            Assert.AreEqual(0, mediator.GetCount<A>());
            Assert.AreEqual(0, mediator.GetCount<B>());
            Assert.AreEqual(0, mediator.GetCount<C>());
            Assert.Catch<ArgumentNullException>(delegate { mediator.Reset(null); });
        }

        [Test]
        public void reset_all() {
            Mediator mediator = CreateTestMediator();
            mediator.ResetAll();
            Assert.AreEqual(0, mediator.GetCount<A>());
            Assert.AreEqual(0, mediator.GetCount<B>());
            Assert.AreEqual(0, mediator.GetCount<C>());
        }

        [Test]
        public void publish() {
            Mediator mediator = CreateTestMediator();
            ExecuteTest(mediator, aCount, bCount, cCount, iCount);
            Assert.Catch<ArgumentNullException>(delegate { mediator.Publish(null); });
        }

        [Test]
        public void subscribe() {
            Mediator mediator = CreateTestMediator();
            mediator.Subscribe(eA);
            mediator.Subscribe(eA);
            mediator.Subscribe(eB);
            mediator.Subscribe(eC);
            Assert.AreEqual(aCount + 2, mediator.GetCount<A>());
            Assert.AreEqual(bCount + 1, mediator.GetCount<B>());
            Assert.AreEqual(cCount + 1, mediator.GetCount<C>());
            ExecuteTest(mediator, aCount + 2, bCount + 1, cCount + 1, iCount);
            Assert.Catch<ArgumentNullException>(delegate { mediator.Subscribe<A>(null); });
        }

        [Test]
        public void unsubscribe() {
            Mediator mediator = CreateTestMediator();
            mediator.Unsubscribe(eA);
            mediator.Unsubscribe(eA);
            mediator.Unsubscribe(eB);
            mediator.Unsubscribe(eC);
            Assert.AreEqual(aCount - 2, mediator.GetCount<A>());
            Assert.AreEqual(bCount - 1, mediator.GetCount<B>());
            Assert.AreEqual(cCount - 1, mediator.GetCount<C>());
            ExecuteTest(mediator, aCount - 2, bCount - 1, cCount - 1, iCount);
            Assert.Catch<ArgumentNullException>(delegate { mediator.Unsubscribe<A>(null); });
        }

    }

}
