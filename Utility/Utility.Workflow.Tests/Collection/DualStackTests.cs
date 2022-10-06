using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class DualStackTests
    {
        [Fact]
        public void TestConstructors()
        {
            //queue behavior is derived from Stack and thus will not be tested
            //only constructors and Back/Front are going to be tested, here

            DualStack<string> stack = new DualStack<string>();

            Assert.Empty(stack);

            stack = new DualStack<string>(new string[] { "a", "b", "c" });

            Assert.Equal(3, stack.Count);

            stack = new DualStack<string>(3);
        }

        [Fact]
        public void TestFront()
        {
            Stack<string> stack = new Stack<string>();

            var front = new DualStackFront<string>(stack);

            stack.Push("Back");
            stack.Push("Front");

            Assert.Equal("Front", front.Peek());
            Assert.Equal("Front", front.Pop());

            stack.Clear();

            stack.Push("Back");
            stack.Push("Front");

            string[] s = front.ToArray();

            Assert.True(s.SequenceEqual(new string[] { "Front", "Back" }));

            front.CopyTo(s, 0);

            Assert.True(s.SequenceEqual(new string[] { "Front", "Back" }));

            Assert.Equal(2, front.Count);
        }

        [Fact]
        public void TestBack()
        {
            Stack<string> stack = new Stack<string>();

            var back = new DualStackBack<string>(stack);

            back.Push("Back");
            back.Push("Front");

            Assert.Equal(2, stack.Count);
            Assert.True(stack.SequenceEqual(new string[] { "Front", "Back" }));

            back.TrimExcess();
        }
    }
}
