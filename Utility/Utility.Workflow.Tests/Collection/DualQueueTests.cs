using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Utility.Collections.Objectflow;
using Xunit;

namespace Utility.Tests.Collection
{
    public class DualQueueTests
    {
        [Fact]
        public void TestConstructors()
        {
            //queue behavior is derived from Queue and thus will not be tested
            //only constructors and Back/Front are going to be tested, here

            TwoEndedQueue<string> queue = new TwoEndedQueue<string>();

            Assert.Empty(queue);

            queue = new TwoEndedQueue<string>(new string[] { "a", "b", "c" });

            Assert.Equal(3, queue.Count);

            queue = new TwoEndedQueue<string>(3);
        }

        [Fact]
        public void TestFront()
        {
            Queue<string> queue = new Queue<string>();

            var front = new QueueFront<string>(queue);

            queue.Enqueue("Front");
            queue.Enqueue("Back");

            Assert.Equal("Front", front.Peek());
            Assert.Equal("Front", front.Dequeue());

            queue.Clear();

            queue.Enqueue("Front");
            queue.Enqueue("Back");

            string[] s = front.ToArray();

            Assert.True(s.SequenceEqual(new string[] { "Front", "Back" }));

            front.CopyTo(s, 0);

            Assert.True(s.SequenceEqual(new string[] { "Front", "Back" }));

            Assert.Equal(2, front.Count);
        }

        [Fact]
        public void TestBack()
        {
            Queue<string> queue = new Queue<string>();

            var back = new QueueBack<string>(queue);

            back.Enqueue("Front");
            back.Enqueue("Back");

            Assert.Equal(2, queue.Count);
            Assert.True(queue.SequenceEqual(new string[] { "Front", "Back" }));

            back.TrimExcess();
        }
    }
}
