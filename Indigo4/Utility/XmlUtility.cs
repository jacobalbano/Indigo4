using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Indigo.Utility
{
    public static class XmlUtility
    {
        public static T Clone<T>(T obj)
        {
            return DeserializeFromString<T>(SerializeToString(obj));
        }

        public static XmlDocument ReadDocumentFromString(string xmlString)
        {
            var result = new XmlDocument();
            result.LoadXml(xmlString);
            return result;
        }

        public static XmlDocument ReadDocumentFromPath(string fullPath)
        {
            var result = new XmlDocument();
            result.Load(fullPath);
            return result;
        }

        public static XmlDocument ReadDocumentFromStream(Stream stream)
        {
            var result = new XmlDocument();
            result.Load(stream);
            return result;
        }

        public static XmlDocument ReadDocumentFromXmlReader(XmlReader reader)
        {
            var result = new XmlDocument();
            result.Load(reader);
            return result;
        }

        /// <summary>
        /// Deserialize an object from a string and return it strongly-typed
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="xmlString">A string containing the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromString<T>(string xmlString)
        {
            using (var sr = new StringReader(xmlString))
            using (var reader = XmlReader.Create(sr))
                return DeserializeFromXmlReader<T>(reader);
        }

        /// <summary>
        /// Deserialize an object from a file path and return it strongly-typed
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="fullPath">A path to a file containing the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromPath<T>(string fullPath)
        {
            using (var stream = File.OpenRead(fullPath))
                return DeserializeFromStream<T>(stream);
        }

        /// <summary>
        /// Deserialize an object from a stream and return it strongly-typed
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="stream">A stream containing the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromStream<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return DeserializeFromString<T>(reader.ReadToEnd());
        }

        /// <summary>
        /// Deserialize an object from an XmlReader and return it strongly-typed
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="reader">An XmlReader that will read the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromXmlReader<T>(XmlReader reader)
        {
            return Implementor<T>.Deserialize(reader);
        }

        /// <summary>
        /// Deserialize an object from a string and return it
        /// </summary>
        /// <param name="resultType">The type to deserialize into</typeparam>
        /// <param name="xmlString">A string containing the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static object DeserializeFromString(string xmlString, Type resultType)
        {
            using (var sr = new StringReader(xmlString))
            using (var reader = XmlReader.Create(sr))
                return Implementor.Deserialize(reader, resultType);
        }

        /// <summary>
        /// Deserialize an object from a file path and return it
        /// </summary>
        /// <param name="resultType">The type to deserialize into</typeparam>
        /// <param name="fullPath">A path to a file containing the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static object DeserializeFromPath(string fullPath, Type resultType)
        {
            using (var stream = File.OpenRead(fullPath))
                return DeserializeFromStream(stream, resultType);
        }

        /// <summary>
        /// Deserialize an object from a stream and return it
        /// </summary>
        /// <param name="resultType">The type to deserialize into</typeparam>
        /// <param name="stream">A stream containing the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static object DeserializeFromStream(FileStream stream, Type resultType)
        {
            using (var reader = new StreamReader(stream))
                return DeserializeFromString(reader.ReadToEnd(), resultType);
        }

        /// <summary>
        /// Deserialize an object from an XmlReader and return it
        /// </summary>
        /// <typeparam name="resultType">The type to deserialize into</typeparam>
        /// <param name="reader">An XmlReader that will read the XML from which to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static object DeserializeFromXmlReader(XmlReader reader, Type resultType)
        {
            return Implementor.Deserialize(reader, resultType);
        }

        /// <summary>
        /// Serialize an object to an XML string
        /// </summary>
        /// <typeparam name="T">Object type (usually inferred)</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The serialized XML string</returns>
        public static string SerializeToString<T>(T obj)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw, settings))
            {
                Implementor<T>.Serialize(obj, writer);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Serialize an object to an XML string
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The serialized XML string</returns>
        public static string SerializeToString(object obj)
        {
            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw))
            {
                Implementor.Serialize(obj, writer);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Serialize an object to an XmlWriter
        /// </summary>
        /// <typeparam name="T">Object type (usually inferred)</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <param name="writer">The XmlWriter to serialize to</param>
        public static void SerializeToXmlWriter<T>(T obj, XmlWriter writer)
        {
            Implementor<T>.Serialize(obj, writer);
        }

        /// <summary>
        /// Serialize an object to an XmlWriter
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="writer">The XmlWriter to serialize to</param>
        public static void SerializeToXmlWriter(object obj, XmlWriter writer)
        {
            Implementor.Serialize(obj, writer);
        }

        /// <summary>
        /// Serialize an object to a stream
        /// </summary>
        /// <typeparam name="T">The type of the object (usually inferred)</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <param name="stream">The stream to write to</param>
        public static void SerializeToStream<T>(T obj, FileStream stream)
        {
            using (var sw = new StreamWriter(stream))
                sw.Write(SerializeToString(obj));
        }

        /// <summary>
        /// Serialize an object to a stream
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="stream">The stream to write to</param>
        public static void SerializeToStream(object obj, FileStream stream)
        {
            using (var sw = new StreamWriter(stream))
                sw.Write(SerializeToString(obj));
        }

        /// <summary>
        /// Serialize an object to an XML file at a given path
        /// </summary>
        /// <typeparam name="T">The type of the object (usually inferred)</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <param name="fullPath">The path to write the XML file</param>
        public static void SerializeToPath<T>(T obj, string fullPath)
        {
            using (var fs = File.Create(fullPath))
                SerializeToStream(obj, fs);
        }

        /// <summary>
        /// Serialize an object to an XML file at a given path
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="fullPath">The path to write the XML file</param>
        public static void SerializeToPath(object obj, string fullPath)
        {
            using (var fs = File.OpenWrite(fullPath))
                SerializeToStream(obj, fs);
        }

        private static class Implementor
        {
            public static void Serialize(object obj, XmlWriter writer)
            {
                var serializer = CreateSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
            }

            public static object Deserialize(XmlReader reader, Type t)
            {
                return CreateSerializer(t).Deserialize(reader);
            }

            public static XmlSerializer CreateSerializer(Type t)
            {
                if (!lookup.TryGetValue(t, out var serializer))
                {
                    //TODO: in the future add support for including implementations of abstract types etc
                    serializer = new XmlSerializer(t);
                    lookup[t] = serializer;
                }

                return serializer;
            }

            private static Dictionary<Type, XmlSerializer> lookup = new Dictionary<Type, XmlSerializer>();
        }

        private static class Implementor<T>
        {
            private static readonly XmlSerializer serializer = Implementor.CreateSerializer(typeof(T));

            public static void Serialize(T obj, XmlWriter writer)
            {
                serializer.Serialize(writer, obj);
            }

            public static T Deserialize(XmlReader reader)
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
