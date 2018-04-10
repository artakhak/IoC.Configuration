using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests
{
    public static class XmlDocumentExtensions
    {
        #region Member Functions

        public static XmlElement AddSiblingClone(this XmlDocument xmlDocument, string xmlElementPath, Predicate<XmlElement> predicate = null)
        {
            var xmlElement = xmlDocument.SelectElement(xmlElementPath, predicate);
            var clonedXmlElement = (XmlElement) xmlElement.Clone();
            xmlElement.ParentNode.InsertAfter(clonedXmlElement, xmlElement);
            return clonedXmlElement;
        }

        public static XmlElement InsertChildElement(this XmlElement xmlElement, string elementName, int? index = null)
        {
            var childElement = xmlElement.OwnerDocument.CreateElement(elementName);

            XmlElement siblingElementBeforeWhichToInsert = null;

            if (index != null)
            {
                var currentSiblingIndex = 0;

                foreach (var siblingNode in xmlElement.ChildNodes)
                {
                    var siblingElement = siblingNode as XmlElement;

                    if (siblingElement == null)
                        continue;

                    if (currentSiblingIndex == index)
                    {
                        siblingElementBeforeWhichToInsert = siblingElement;
                        break;
                    }

                    ++currentSiblingIndex;
                }
            }

            if (siblingElementBeforeWhichToInsert != null)
                xmlElement.InsertBefore(childElement, siblingElementBeforeWhichToInsert);
            else
                xmlElement.AppendChild(childElement);

            return childElement;
        }

        public static XmlElement Remove(this XmlElement xmlElement, string attributeName)
        {
            xmlElement.RemoveAttribute(attributeName);
            return xmlElement;
        }

        public static void Remove(this XmlElement xmlElement)
        {
            xmlElement.ParentNode.RemoveChild(xmlElement);
        }

        public static XmlElement RemoveChildElement(this XmlElement xmlElement, [NotNull] string childElementPath,
                                                    [CanBeNull] Predicate<XmlElement> predicate = null)
        {
            var childElement = xmlElement.SelectChildElement(childElementPath, predicate);

            if (childElement != null)
                childElement.ParentNode.RemoveChild(childElement);

            return xmlElement;
        }

        [NotNull]
        public static XmlElement SelectChildElement([NotNull] this XmlElement xmlElement, [NotNull] string childElementPath,
                                                    [CanBeNull] Predicate<XmlElement> predicate = null)
        {
            var allChildNodes = xmlElement.SelectNodes(childElementPath);

            foreach (var xmlNode in allChildNodes)
            {
                var childElement = xmlNode as XmlElement;

                if (childElement == null)
                    continue;

                if (predicate == null || predicate(childElement))
                    return childElement;
            }

            throw new Exception("Child element not found.");
        }

        [NotNull]
        public static XmlElement SelectElement(this XmlDocument xmlDocument, string xmlElementPath, Predicate<XmlElement> predicate = null)
        {
            var firstElement = xmlDocument.SelectElements(xmlElementPath).FirstOrDefault(x => predicate == null ? true : predicate(x));
            Assert.IsNotNull(firstElement, $"Element '{xmlElementPath}' was not found.");
            return firstElement;
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XmlElement> SelectElements(this XmlDocument xmlDocument, string xmlElementPath)
        {
            var selectedNodes = xmlDocument.SelectNodes(xmlElementPath);
            foreach (var node in selectedNodes)
                if (node is XmlElement)
                    yield return (XmlElement) node;
        }

        public static XmlElement SetAttributeValue(this XmlElement xmlElement, string attributeName, string newValue)
        {
            xmlElement.SetAttribute(attributeName, newValue);
            return xmlElement;
        }

        public static void ValidateElementExists(this XmlDocument xmlDocument, string xmlElementPath, Predicate<XmlElement> predicate = null)
        {
            xmlDocument.SelectElement(xmlElementPath, predicate);
        }

        #endregion
    }
}