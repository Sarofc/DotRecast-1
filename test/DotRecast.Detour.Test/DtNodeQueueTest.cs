using System.Collections.Generic;
using DotRecast.Core;
using DotRecast.Core.Collections;
using NUnit.Framework;

namespace DotRecast.Detour.Test;

public class DtNodeQueueTest
{
    private static List<DtNode> ShuffledNodes(int count)
    {
        var nodes = new List<DtNode>();
        for (int i = 0; i < count; ++i)
        {
            var node = new DtNode(i);
            node.total = i;
            nodes.Add(node);
        }

        nodes.Shuffle();
        return nodes;
    }

    [Test]
    public void TestPushAndPop()
    {
        // test push
        const int count = 1000;

        var queue = new DtNodeQueue(count);

        // check count
        Assert.That(queue.Count(), Is.EqualTo(0));

        // null push
        //queue.Push(null);
        Assert.That(queue.Count(), Is.EqualTo(0));

        var expectedNodes = ShuffledNodes(count);
        foreach (var node in expectedNodes)
        {
            queue.Push(node);
        }

        Assert.That(queue.Count(), Is.EqualTo(count));

        // test pop
        expectedNodes.Sort((x, y) => x.total.CompareTo(y.total));
        foreach (var node in expectedNodes)
        {
            Assert.That(queue.Top(), Is.SameAs(node));
            Assert.That(queue.Pop(), Is.SameAs(node));
        }

        Assert.That(queue.Count(), Is.EqualTo(0));
    }

    [Test]
    public void TestClear()
    {
        const int count = 555;

        var queue = new DtNodeQueue(count);

        var expectedNodes = ShuffledNodes(count);
        foreach (var node in expectedNodes)
        {
            queue.Push(node);
        }

        Assert.That(queue.Count(), Is.EqualTo(count));

        queue.Clear();
        Assert.That(queue.Count(), Is.EqualTo(0));
        Assert.That(queue.IsEmpty(), Is.True);
    }

    [Test]
    public void TestModify()
    {
        const int count = 5000;

        var queue = new DtNodeQueue(count);

        var expectedNodes = ShuffledNodes(count);

        foreach (var node in expectedNodes)
        {
            queue.Push(node);
        }

        // check modify
        //queue.Modify(null);

        // change total
        var r = System.Random.Shared;
        foreach (var node in expectedNodes)
        {
            node.total = r.Next() % (count / (count / 10)); // duplication for test
            queue.Modify(node); 
            // TODO 先改了 total，再modify，会导致优先级不太对
            // 但粗略测试起来，顶多路径不是最优的，但可以接受
        }

        Assert.That(queue.Count, Is.EqualTo(expectedNodes.Count));

        // check
        expectedNodes.Sort((x, y) => x.total.CompareTo(y.total));
        for (int i = 0; i < expectedNodes.Count; i++)
        {
            DtNode node = expectedNodes[i];
            Assert.That(queue.Pop().total, Is.EqualTo(node.total).Within(0.00001f), $"{i}/{expectedNodes.Count}");
        }
    }
}