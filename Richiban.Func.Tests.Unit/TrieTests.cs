using System;
using System.Collections.Generic;
using NUnit.Framework;
using static PowerAssert.PAssert;

namespace Richiban.Func.Tests.Unit
{
    [TestFixture]
    public sealed class TrieTests
    {
        [Test]
        public void GivenATrieThatContainsAValue_WhenIRetrieveThatKey_ThenTheValueIsReturned()
        {
            var key = Guid.NewGuid().ToString();

            var trie = new Trie<int>();
            
            trie.Set(key, 1);

            IsTrue(() => trie.Retrieve(key).Value == 1);
        }

        [Test]
        public void GivenAnEmptyTrie_WhenIRetrieveAKey_ThenAnExceptionIsThrown()
        {
            var trie = new Trie<int>();
            var key = "hello";

            IsTrue(() => trie.Retrieve(key).HasValue == false);
        }
    }
}
