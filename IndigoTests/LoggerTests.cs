using Indigo.Core.Logging.Endpoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Core.Logging
{
    [TestClass]
    public class LoggerTests
    {
        private StringBuilder sb;
        private Logger logger;

        [TestInitialize]
        public void Initialize()
        {
            logger = new Logger();
            sb = new StringBuilder();
            logger.AddEndpoint(new StringbuilderEndpoint(sb));
        }

        [TestMethod]
        public void ContextTests()
        {
            using (logger.Context("Outer"))
            {
                using (logger.Context("Inner context 1"))
                    logger.WriteLine("Inner logline");

                logger.WriteLine("Back out");

                using (logger.Context("Inner context 2"))
                    logger.WriteLine("Inner logline");
            }

            logger.WriteLine("Done");

            var output = sb.ToString()
                .Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            Assert.AreEqual("Outer", output[0]);
            Assert.AreEqual("\tInner context 1", output[1]);
            Assert.AreEqual("\t\tInner logline", output[2]);
            Assert.AreEqual("\tBack out", output[3]);
            Assert.AreEqual("\tInner context 2", output[4]);
            Assert.AreEqual("\t\tInner logline", output[5]);
            Assert.AreEqual("Done", output[6]);
        }
    }
}
