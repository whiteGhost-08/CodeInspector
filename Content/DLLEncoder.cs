﻿/******************************************************************************
* Author      = Susan
*
* Product     = Analyzer
* 
* Project     = Content
*
* Description = Class to encode DLL files
*****************************************************************************/
using System.Xml;

namespace Content
{
    /// <summary>
    /// Class to encode and decode DLL files to send over network
    /// </summary>
    internal class DLLEncoder : IFileEncoder
    {
        private string filePath;

        /// <summary>
        /// Initializes a new instance of the DLLEncoder class with the specified file path.
        /// Files are to be loaded from this file path
        /// </summary>
        /// <param name="_filepath">The file path of the DLL to be encoded.</param>
        public DLLEncoder(string _filepath)
        {
            filePath = _filepath;
        }

        /// <summary>
        /// Encodes the DLL data into an XML format.
        /// </summary>
        /// <returns>An XML representation of the DLL data as a string.</returns>
        public string GetEncoded()
        {
            // Load the DLL data
            byte[] dllBytes = File.ReadAllBytes(filePath);

            // Create an XML document
            XmlDocument xmlDoc = new XmlDocument();

            // Create the root element
            XmlElement root = xmlDoc.CreateElement("DllData");
            xmlDoc.AppendChild(root);

            // Create an element for the file name
            XmlElement fileNameElement = xmlDoc.CreateElement("FileName");
            fileNameElement.InnerText = Path.GetFileName(filePath);
            root.AppendChild(fileNameElement);

            // Create an element for the DLL content (encoded as Base64)
            XmlElement contentElement = xmlDoc.CreateElement("Content");
            contentElement.InnerText = Convert.ToBase64String(dllBytes);
            root.AppendChild(contentElement);

            // Convert the XML document to a string
            return xmlDoc.OuterXml;
        }

        /// <summary>
        /// Decodes DLL data from an XML string and processes the extracted information.
        /// </summary>
        /// <param name="xmlData">The XML data representing the DLL content as a string.</param>
        public void DecodeFrom(string xmlData)
        {
            // Implement XML decoding logic here based on the XML string
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // Extract the file name and content
            string fileName = xmlDoc.SelectSingleNode("//FileName").InnerText;
            string base64Content = xmlDoc.SelectSingleNode("//Content").InnerText;

            // Decode the Base64 content
            byte[] dllBytes = Convert.FromBase64String(base64Content);

            // You can do further processing with 'fileName' and 'dllBytes'
        }
    }
}
