﻿#region
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetMQ.Sockets;
using NUnit.Framework;
using ProtoBuf;
// ReSharper disable InconsistentNaming
#endregion

// ReSharper disable ConvertClosureToMethodGroup

namespace NetMQ.ReactiveExtensions.Tests
{
    [TestFixture]
    public class NetMQ_ReactiveExtensions_Tests
    {
        public static TimeSpan Timeout = TimeSpan.FromSeconds(90);

        [Test]
        public void Simplest_Test_Subject()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent cd = new CountdownEvent(5);
            {
                int freePort = TcpPortFree();

                var pubSub = new SubjectNetMQ<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                pubSub.Subscribe(o =>
                {
                    Console.Write("Test 1: {0}\n", o);
                    cd.Signal();
                },
                    ex => { Console.WriteLine("Exception! {0}", ex.Message); });

                pubSub.OnNext(38);
                pubSub.OnNext(39);
                pubSub.OnNext(40);
                pubSub.OnNext(41);
                pubSub.OnNext(42);
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public void Simplest_Test_Publisher_To_Subscriber()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent cd = new CountdownEvent(5);
            {
                int freePort = TcpPortFree();

                var publisher = new PublisherNetMq<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                var subscriber = new SubscriberNetMq<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);

                subscriber.Subscribe(o =>
                {
                    Console.Write("Test 1: {0}\n", o);
                    cd.Signal();
                },
                    ex => { Console.WriteLine("Exception! {0}", ex.Message); });

                publisher.OnNext(38);
                publisher.OnNext(39);
                publisher.OnNext(40);
                publisher.OnNext(41);
                publisher.OnNext(42);
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }


        [Test]
        public void Can_Serialize_Using_Protobuf_With_Class()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent cd = new CountdownEvent(5);
            {
                int freePort = TcpPortFree();

                var pubSub = new SubjectNetMQ<MyMessageClassType1>("tcp://127.0.0.1:" + freePort,
                    loggerDelegate: Console.Write);
                pubSub.Subscribe(o =>
                {
                    Assert.IsTrue(o.Name == "Bob");
                    Console.Write("Test: Num={0}, Name={1}\n", o.Num, o.Name);
                    cd.Signal();
                },
                    ex => { Console.WriteLine("Exception! {0}", ex.Message); });

                pubSub.OnNext(new MyMessageClassType1(38, "Bob"));
                pubSub.OnNext(new MyMessageClassType1(39, "Bob"));
                pubSub.OnNext(new MyMessageClassType1(40, "Bob"));
                pubSub.OnNext(new MyMessageClassType1(41, "Bob"));
                pubSub.OnNext(new MyMessageClassType1(42, "Bob"));
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public void Can_Serialize_Using_Protobuf_With_Struct()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent cd = new CountdownEvent(5);
            {
                int freePort = TcpPortFree();

                var pubSub = new SubjectNetMQ<MyMessageStructType1>("tcp://127.0.0.1:" + freePort,
                    loggerDelegate: Console.Write);
                pubSub.Subscribe(o =>
                {
                    Assert.IsTrue(o.Name == "Bob");
                    Console.Write("Test: Num={0}, Name={1}\n", o.Num, o.Name);
                    cd.Signal();
                },
                    ex => { Console.WriteLine("Exception! {0}", ex.Message); });

                pubSub.OnNext(new MyMessageStructType1(38, "Bob"));
                pubSub.OnNext(new MyMessageStructType1(39, "Bob"));
                pubSub.OnNext(new MyMessageStructType1(40, "Bob"));
                pubSub.OnNext(new MyMessageStructType1(41, "Bob"));
                pubSub.OnNext(new MyMessageStructType1(42, "Bob"));
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public void Can_Serialize_Class_Name_Longer_Then_Thirty_Two_Characters()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent cd = new CountdownEvent(5);
            {
                int freePort = TcpPortFree();

                var pubSub =
                    new SubjectNetMQ<ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure>(
                        "tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                pubSub.Subscribe(o =>
                {
                    Assert.IsTrue(o.Name == "Bob");
                    Console.Write("Test: Num={0}, Name={1}\n", o.Num, o.Name);
                    cd.Signal();
                },
                    ex => { Console.WriteLine("Exception! {0}", ex.Message); });

                pubSub.OnNext(new ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure(38, "Bob"));
                pubSub.OnNext(new ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure(39, "Bob"));
                pubSub.OnNext(new ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure(40, "Bob"));
                pubSub.OnNext(new ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure(41, "Bob"));
                pubSub.OnNext(new ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure(42, "Bob"));
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }


        [Test]
        public void Initialize_Publisher_Then_Subscriber()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent cd = new CountdownEvent(5);
            {
                int freePort = TcpPortFree();

                var pubSub = new SubjectNetMQ<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);

                // Forces the publisher to be initialized. Subscriber not set up yet, so this message will never get
                // delivered to the subscriber, which is what is should do.
                pubSub.OnNext(1);

                pubSub.Subscribe(o =>
                {
                    Assert.IsTrue(o != 1);
                    Console.Write("Test 1: {0}\n", o);
                    cd.Signal();
                },
                    ex => { Console.WriteLine("Exception! {0}", ex.Message); });

                pubSub.OnNext(38);
                pubSub.OnNext(39);
                pubSub.OnNext(40);
                pubSub.OnNext(41);
                pubSub.OnNext(42);
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public void Simplest_Fanout_Sub()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent cd = new CountdownEvent(3);
            {
                int freePort = TcpPortFree();
                var pubSub = new SubjectNetMQ<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                pubSub.Subscribe(o =>
                {
                    Assert.AreEqual(o, 42);
                    Console.Write("PubTwoThreadFanoutSub1: {0}\n", o);
                    cd.Signal();
                });
                pubSub.Subscribe(o =>
                {
                    Assert.AreEqual(o, 42);
                    Console.Write("PubTwoThreadFanoutSub2: {0}\n", o);
                    cd.Signal();
                });
                pubSub.Subscribe(o =>
                {
                    Assert.AreEqual(o, 42);
                    Console.Write("PubTwoThreadFanoutSub3: {0}\n", o);
                    cd.Signal();
                });

                pubSub.OnNext(42);
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public void OnException_Should_Get_Passed_To_Subscribers()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent weAreDone = new CountdownEvent(1);
            {
                int freePort = TcpPortFree();
                var pubSub = new SubjectNetMQ<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                pubSub.Subscribe(
                    o =>
                    {
                        // If this gets called more than max times, it will throw an exception as it is going through 0.
                        Assert.Fail();
                    },
                    ex =>
                    {
                        Console.Write("Exception: {0}", ex.Message);
                        Assert.True(ex.Message.Contains("passed"));
                        weAreDone.Signal();
                    },
                    () => { Assert.Fail(); });

                pubSub.OnError(new Exception("passed"));
            }

            if (weAreDone.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public void OnCompleted_Should_Get_Passed_To_Subscribers()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();

            CountdownEvent weAreDone = new CountdownEvent(1);
            {
                int freePort = TcpPortFree();
                var pubSub = new SubjectNetMQ<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                pubSub.Subscribe(
                    o =>
                    {
                        // If this gets called more than max times, it will throw an exception as it is going through 0.
                        Console.Write("FAIL!");
                        Assert.Fail();
                    },
                    ex =>
                    {
                        Console.Write("FAIL!");
                        Assert.Fail();
                    },
                    () =>
                    {
                        Console.Write("Pass!");
                        weAreDone.Signal();
                    });

                pubSub.OnCompleted();
            }

            if (weAreDone.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public static void Disposing_Of_One_Does_Not_Dispose_Of_The_Other()
        {
            PrintTestName();
            Stopwatch sw = new Stopwatch();

            int max = 1000;
            CountdownEvent cd = new CountdownEvent(max);
            {
                int freePort = TcpPortFree();
                var pubSub = new SubjectNetMQ<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                var d1 = pubSub.Subscribe(o => { cd.Signal(); });

                var d2 = pubSub.Subscribe(o => { Assert.Fail(); },
                    ex => { Console.WriteLine("Exception in subscriber thread."); });
                d2.Dispose();

                for (int i = 0; i < max; i++)
                {
                    pubSub.OnNext(i);
                }
            }

            if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public static void Speed_Test_Subject()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();
            {
                var max = 100*1000;

                CountdownEvent cd = new CountdownEvent(max);
                var receivedNum = 0;
                {
                    Console.Write("Speed test with {0} messages:\n", max);

                    int freePort = TcpPortFree();
                    var pubSub = new SubjectNetMQ<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);

                    pubSub.Subscribe(i =>
                    {
                        receivedNum++;
                        cd.Signal();
                        if (i%10000 == 0)
                        {
                            //Console.Write("*");
                        }
                    });

                    sw.Start();
                    for (int i = 0; i < max; i++)
                    {
                        pubSub.OnNext(i);
                    }
                }

                if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
                {
                    Assert.Fail("\nTimed out, this test should complete in {0} seconds. receivedNum={1}",
                        Timeout.TotalSeconds, receivedNum);
                }

                sw.Stop();

                // On my machine, achieved >120,000 messages per second.
                PrintElapsedTime(sw.Elapsed, max);                
            }
        }

        [Test]
        public static void Speed_Test_Publisher_Subscriber()
        {
            PrintTestName();

            Stopwatch sw = new Stopwatch();
            {
                var max = 200*1000;

                CountdownEvent cd = new CountdownEvent(max);
                var receivedNum = 0;
                {
                    Console.Write("Speed test with {0} messages:\n", max);

                    int freePort = TcpPortFree();
                    var publisher = new PublisherNetMq<int>("tcp://127.0.0.1:" + freePort, loggerDelegate: Console.Write);
                    var subscriber = new SubscriberNetMq<int>("tcp://127.0.0.1:" + freePort,
                        loggerDelegate: Console.Write);

                    subscriber.Subscribe(i =>
                    {
                        receivedNum++;
                        cd.Signal();
                        if (i%10000 == 0)
                        {
                            //Console.Write("*");
                        }
                    });

                    sw.Start();
                    for (int i = 0; i < max; i++)
                    {
                        publisher.OnNext(i);
                    }
                }

                if (cd.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
                {
                    Assert.Fail("\nTimed out, this test should complete in {0} seconds. receivedNum={1}", Timeout.TotalSeconds,
                        receivedNum);
                }

                sw.Stop();
                // On my machine, achieved >120,000 messages per second.
                PrintElapsedTime(sw.Elapsed, max);
            }            
        }

        [Test]
        public void PubSub_Should_Not_Crash_If_No_Thread_Sleep()
        {
            PrintTestName();
            Stopwatch swAll = Stopwatch.StartNew();

            using (var pub = new PublisherSocket())
            {
                using (var sub = new SubscriberSocket())
                {
                    int freePort = TcpPortFree();
                    pub.Bind("tcp://127.0.0.1:" + freePort);
                    sub.Connect("tcp://127.0.0.1:" + freePort);

                    sub.Subscribe("*");

                    Stopwatch sw = Stopwatch.StartNew();
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            pub.SendFrame("*"); // Ping.

                            Console.Write("*");
                            string topic;
                            var gotTopic = sub.TryReceiveFrameString(TimeSpan.FromMilliseconds(100), out topic);
                            string ping;
                            var gotPing = sub.TryReceiveFrameString(TimeSpan.FromMilliseconds(100), out ping);
                            if (gotTopic)
                            {
                                Console.Write("\n");
                                break;
                            }
                        }
                    }
                    Console.WriteLine("Connected in {0} ms.", sw.ElapsedMilliseconds);
                }
            }
            PrintElapsedTime(swAll.Elapsed);
        }

        [Test]
        public void Test_Two_Subscribers()
        {
            PrintTestName();
            Stopwatch swAll = Stopwatch.StartNew();            

            using (var pub = new PublisherSocket())
            {
                using (var sub1 = new SubscriberSocket())
                {
                    using (var sub2 = new SubscriberSocket())
                    {
                        int freePort = TcpPortFree();
                        pub.Bind("tcp://127.0.0.1:" + freePort);
                        sub1.Connect("tcp://127.0.0.1:" + freePort);
                        sub1.Subscribe("A");
                        sub2.Connect("tcp://127.0.0.1:" + freePort);
                        sub2.Subscribe("B");

                        Thread.Sleep(500);

                        Stopwatch sw = Stopwatch.StartNew();
                        {
                            pub.SendFrame("A\n"); // Ping.
                            {
                                string topic;
                                bool pass1 = sub1.TryReceiveFrameString(TimeSpan.FromMilliseconds(250), out topic);
                                if (pass1)
                                {
                                    Console.Write(topic);
                                }
                                else
                                {
                                    Assert.Fail();
                                }
                            }
                            pub.SendFrame("B\n"); // Ping.
                            {
                                string topic;
                                bool pass2 = sub2.TryReceiveFrameString(TimeSpan.FromMilliseconds(250), out topic);
                                if (pass2)
                                {
                                    Console.Write(topic);
                                }
                                else
                                {
                                    Assert.Fail();
                                }
                            }
                        }
                        Console.WriteLine("Connected in {0} ms.", sw.ElapsedMilliseconds);
                    }
                }
            }
            PrintElapsedTime(swAll.Elapsed);
        }

        [Test]
        public void If_Message_Not_Serializable_By_Protobuf_Throw_A_Meaningful_Error()
        {
            PrintTestName();
            Stopwatch sw = Stopwatch.StartNew();

            int freePort = TcpPortFree();
            var pubSub = new SubjectNetMQ<MessageNotSerializableByProtobuf>("tcp://127.0.0.1:" + freePort,
                loggerDelegate: Console.Write);
            try
            {
                pubSub.OnNext(new MessageNotSerializableByProtobuf());

                // We should have thrown an exception if the class was not serializable by ProtoBuf-Net.
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.True(ex.Message.ToLower().Contains("protobuf"));
                Console.Write("Pass - meaningful message thrown if class was not serializable by ProtoBuf-Net.");
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            PrintElapsedTime(sw.Elapsed);
        }

        [Test]
        public void Send_Two_Types_Simultaneously_Over_Same_Transport()
        {
            PrintTestName();
            Stopwatch sw = Stopwatch.StartNew();

            CountdownEvent cd1 = new CountdownEvent(5);
            CountdownEvent cd2 = new CountdownEvent(5);
            {
                int freePort = TcpPortFree();

                var pubSub1 = new SubjectNetMQ<MyMessageStructType1>("tcp://127.0.0.1:" + freePort,
                    loggerDelegate: Console.Write);
                var pubSub2 = new SubjectNetMQ<MyMessageStructType2>("tcp://127.0.0.1:" + freePort,
                    loggerDelegate: Console.Write);
                pubSub1.Subscribe(o =>
                {
                    Assert.IsTrue(o.Name == "Bob");
                    Console.Write("Test 1: Num={0}, Name={1}\n", o.Num, o.Name);
                    cd1.Signal();
                },
                    ex =>
                    {
                        Console.WriteLine("Exception! {0}", ex.Message);
                    });

                pubSub2.Subscribe(o =>
                {
                    Assert.IsTrue(o.Name == "Bob");
                    Console.Write("Test 2: Num={0}, Name={1}\n", o.Num, o.Name);
                    cd2.Signal();
                },
                    ex =>
                    {
                        Console.WriteLine("Exception! {0}", ex.Message);
                    });

                pubSub1.OnNext(new MyMessageStructType1(38, "Bob"));
                pubSub1.OnNext(new MyMessageStructType1(39, "Bob"));
                pubSub1.OnNext(new MyMessageStructType1(40, "Bob"));
                pubSub1.OnNext(new MyMessageStructType1(41, "Bob"));
                pubSub1.OnNext(new MyMessageStructType1(42, "Bob"));

                pubSub2.OnNext(new MyMessageStructType2(38, "Bob"));
                pubSub2.OnNext(new MyMessageStructType2(39, "Bob"));
                pubSub2.OnNext(new MyMessageStructType2(40, "Bob"));
                pubSub2.OnNext(new MyMessageStructType2(41, "Bob"));
                pubSub2.OnNext(new MyMessageStructType2(42, "Bob"));
            }

            if (cd1.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            if (cd2.Wait(Timeout) == false) // Blocks until _countdown.Signal has been called.
            {
                Assert.Fail("Timed out, this test should complete in {0} seconds.", Timeout.TotalSeconds);
            }

            PrintElapsedTime(sw.Elapsed);
        }

        #region Message types.

        [ProtoContract]
        public class MyMessageClassType1
        {
            public MyMessageClassType1()
            {
            }

            public MyMessageClassType1(int num, string name)
            {
                Num = num;
                Name = name;
            }

            [ProtoMember(1)]
            public int Num { get; set; }

            [ProtoMember(2)]
            public string Name { get; set; }
        }

        [ProtoContract]
        public class MyMessageClassType2
        {
            public MyMessageClassType2()
            {
            }

            public MyMessageClassType2(int num, string name)
            {
                Num = num;
                Name = name;
            }

            [ProtoMember(1)]
            public int Num { get; set; }

            [ProtoMember(2)]
            public string Name { get; set; }
        }

        [ProtoContract]
        public struct MyMessageStructType1
        {
            public MyMessageStructType1(int num, string name)
            {
                Num = num;
                Name = name;
            }

            [ProtoMember(1)]
            public int Num { get; set; }

            [ProtoMember(2)]
            public string Name { get; set; }
        }

        [ProtoContract]
        public struct MyMessageStructType2
        {
            public MyMessageStructType2(int num, string name)
            {
                Num = num;
                Name = name;
            }

            [ProtoMember(1)]
            public int Num { get; set; }

            [ProtoMember(2)]
            public string Name { get; set; }
        }

        public class MessageNotSerializableByProtobuf
        {
            public int NotSerializable { get; set; }
        }

        [ProtoContract]
        public class ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure
        {
            public ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure()
            {

            }

            public ClassNameIsLongerThenThirtyTwoCharactersForAbsolutelySure(int num, string name)
            {
                Num = num;
                Name = name;
            }

            [ProtoMember(1)]
            public int Num { get; set; }

            [ProtoMember(2)]
            public string Name { get; set; }
        }

        #endregion

        /// <summary>
        /// Intent: Returns next free TCP/IP port. See
        /// http://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net
        /// </summary>
        /// <threadSafe>Yes. Quote: "I successfully used this technique to get a free port. I too was concerned about
        /// race-conditions, with some other process sneaking in and grabbing the recently-detected-as-free port. So I
        /// wrote a test with a forced Sleep(100) between var port = FreeTcpPort() and starting an HttpListener on the
        /// free port. I then ran 8 identical processes hammering on this in a loop. I could never hit the race
        /// condition. My anecdotal evidence (Win 7) is that the OS apparently cycles through the range of ephemeral
        /// ports (a few thousand) before coming around again. So the above snippet should be just fine." </threadSafe>
        /// <returns>A free TCP/IP port.</returns>
        public static int TcpPortFree()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public static void PrintTestName()
        {
            Console.Write("\n\n*****\nTest: {0}\n*****\n", TestContext.CurrentContext.Test.Name);
        }

        public static void PrintElapsedTime(TimeSpan sw, int? max = null)
        {

            Console.Write("\nElapsed time: {0} milliseconds", sw.TotalMilliseconds);
            if (max != null)
            {
                Console.Write(" ({0:0,000}/sec)", (double)max / (double)sw.TotalSeconds);
            }
            Console.Write("\n");
        }
    }
}

