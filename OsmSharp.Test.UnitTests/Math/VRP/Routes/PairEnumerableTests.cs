﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using OsmSharp.Math.VRP.Routes;
using System.Collections.Generic;

namespace OsmSharp.Test.Unittests.Math.VRP.Routes
{
    /// <summary>
    /// Tests for the pair enumerable.
    /// </summary>
    [TestFixture]
    public class PairEnumerableTests
    {
        /// <summary>
        /// Tests the pair enumerable on a route that isn't a round.
        /// </summary>
        [Test]
        public void Test1NoRound()
        {
            var route = new RouteStub(new int[] { 0, 1, 2, 3, 4 }, false);
            var pairEnumerable = new PairEnumerable(route);
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(4, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
        }

        /// <summary>
        /// Tests the pair enumerable on a route that is a round.
        /// </summary>
        [Test]
        public void Test2Round()
        {
            var route = new RouteStub(new int[] { 0, 1, 2, 3, 4 }, true);
            var pairEnumerable = new PairEnumerable(route);
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(5, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);
        }

        /// <summary>
        /// Tests the pair enumerable on a route that is empty.
        /// </summary>
        [Test]
        public void Test3Empty()
        {
            var route = new RouteStub(new int[0], true);
            var pairEnumerable = new PairEnumerable(route);
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);

            route = new RouteStub(new int[0], false);
            pairEnumerable = new PairEnumerable(route);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);
        }

        /// <summary>
        /// Tests the pair enumerable's enumerator's reset.
        /// </summary>
        [Test]
        public void Test4Reset()
        {
            var route = new RouteStub(new int[] { 0, 1, 2, 3, 4 }, true);
            var pairEnumerable = new PairEnumerable(route);
            var pairEnumerator = pairEnumerable.GetEnumerator();

            var pairs = new List<Pair>();
            while (pairEnumerator.MoveNext())
            {
                pairs.Add(pairEnumerator.Current);
            }

            Assert.AreEqual(5, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);

            pairEnumerator.Reset();

            pairs = new List<Pair>();
            while (pairEnumerator.MoveNext())
            {
                pairs.Add(pairEnumerator.Current);
            }

            Assert.AreEqual(5, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);
        }

        /// <summary>
        /// Tests the pair enumerable on a route that contains only one customer.
        /// </summary>
        [Test]
        public void Test5OneCustomer()
        {
            var route = new RouteStub(new int[] { 0 }, true);
            var pairEnumerable = new PairEnumerable(route);
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);

            route = new RouteStub(new int[] { 0 }, false);
            pairEnumerable = new PairEnumerable(route);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);
        }
    }
}