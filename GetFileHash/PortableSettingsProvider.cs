using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace GetFileHash
{
    /// <summary>
    /// Inherits the <c>SettingsProvider</c> class to provide a portable method of saving application settings.
    /// </summary>
    public class PortableSettingsProvider : SettingsProvider
    {
        // Define some static strings later used in our XML creation...
        /// <summary>
        /// XML Root node.
        /// </summary>
        const string XMLROOT = "configuration";

        /// <summary>
        /// Configuration declaration node.
        /// </summary>
        const string CONFIGNODE = "configSections";

        /// <summary>
        /// Configuration section group declaration node.
        /// </summary>
        const string GROUPNODE = "sectionGroup";

        /// <summary>
        /// User section node.
        /// </summary>
        const string USERNODE = "userSettings";

        /// <summary>
        /// Application Specific Node.
        /// </summary>
        private string APPNODE = Assembly.GetExecutingAssembly().GetName().Name + ".Properties.Settings";

        private XmlDocument xmlDoc = null;

        /// <summary>
        /// Initialize the Portable Settings Provider.
        /// </summary>
        /// <param name="Name">The Friendly Name of the Provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string Name, NameValueCollection config)
        {
            base.Initialize(ApplicationName, config);
        }

        /// <summary>
        /// The name of the Executing Assembly.
        /// </summary>
        private string _Name = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// Obtains the <c>AssemblyCompanyAttribute</c> from the Executing Assembly if present. If this attribute is not found, a default value is returned.
        /// </summary>
        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "JNI Solutions";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        /// <summary>
        /// Obtains the <c>AssemblyTitleAttribute</c> from the Executing Assembly if present.
        /// If this attribute is not found, the name of the executable (without the extension) is returned.
        /// </summary>
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// The name of the Executing Assembly.
        /// </summary>
        public override string Name
        {
            get
            {
                return _Name;
            }
        }

        /// <summary>
        /// Gets a brief, friendly description suitable for display in administrative tools or other user interfaces.
        /// </summary>
        public override string Description
        {
            get
            {
                return base.Description;
            }
        }

        /// <summary>
        /// Overrides the ApplicationName property, returning the solution name.  No need to set anything, we just need to
        /// retrieve information, though the set method still needs to be defined.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return (Assembly.GetExecutingAssembly().GetName().Name);
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// Simply returns the name of the settings file, which is the solution name plus ".config".
        /// </summary>
        /// <returns></returns>
        public virtual string GetSettingsFilename()
        {
#if DEBUG
            return ApplicationName + ".debug.config";
#else
            return ApplicationName + ".exe.config";
#endif
        }

        /// <summary>
        /// Gets current executable path in order to determine where to read and write the config file.
        /// </summary>
        /// <returns>Returns the current executable path.</returns>
        public virtual string GetAppPath()
        {
            string configDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + AssemblyCompany + "\\" + AssemblyTitle;

            try
            {
                if (!Directory.Exists(configDirectoryName))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(configDirectoryName);
                }
            }
            catch (Exception e)
            {
                // Create an informational message for the user if we cannot save the settings.
                // ==== Enable whichever applies to your application type. ====

                // Uncomment the following line to enable a MessageBox for forms-based apps
                MessageBox.Show("The process failed:" + Environment.NewLine + e.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Uncomment the following line to enable a console message for console-based apps
                //Console.Error.WriteLine("The process failed:\n" + e.Message);
            }
            //return new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            return configDirectoryName;
        }

        /// <summary>
        /// Retrieve settings from the configuration file.
        /// </summary>
        /// <param name="sContext">Ignored in this implementation, but must be specified.</param>
        /// <param name="settingsColl">The <c>SettingsPropertyCollection</c> to parse.</param>
        /// <returns>Returns a <c>SettingsPropertyValueCollection</c> containing the settings which are currently defined.</returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sContext, SettingsPropertyCollection settingsColl)
        {
            // Create a collection of values to return
            SettingsPropertyValueCollection retValues = new SettingsPropertyValueCollection();

            // Create a temporary SettingsPropertyValue to reuse
            SettingsPropertyValue setVal;

            // Loop through the list of settings that the application has requested and add them
            // to our collection of return values.
            foreach (SettingsProperty sProp in settingsColl)
            {
                setVal = new SettingsPropertyValue(sProp);
                setVal.IsDirty = false;
                setVal.SerializedValue = GetSetting(sProp);
                retValues.Add(setVal);
            }
            return retValues;
        }

        /// <summary>
        /// Save any of the applications settings that have changed (flagged as "dirty").
        /// </summary>
        /// <param name="sContext">Ignored in this implementation, but must be specified.</param>
        /// <param name="settingsColl">The <c>SettingsPropertyCollection</c> to parse.</param>
        public override void SetPropertyValues(SettingsContext sContext, SettingsPropertyValueCollection settingsColl)
        {
            // Set the values in XML
            foreach (SettingsPropertyValue spVal in settingsColl)
            {
                SetSetting(spVal);
            }

            // Write the XML file to disk
            try
            {
                XMLConfig.Save(Path.Combine(GetAppPath(), GetSettingsFilename()));
            }
            catch (Exception ex)
            {
                // Create an informational message for the user if we cannot save the settings.
                // ==== Enable whichever applies to your application type. ====

                // Uncomment the following line to enable a MessageBox for forms-based apps
                //MessageBox.Show(ex.Message, "Error writting configuration file to disk", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Uncomment the following line to enable a console message for console-based apps
                Console.Error.WriteLine("Error writing configuration file to disk: " + ex.Message);
            }
        }

        private XmlDocument XMLConfig
        {
            get
            {
                // Check if we already have accessed the XML config file. If the xmlDoc object is empty, we have not.
                if (xmlDoc == null)
                {
                    xmlDoc = new XmlDocument();

                    // If we have not loaded the config, try reading the file from disk.
                    try
                    {
                        xmlDoc.Load(Path.Combine(GetAppPath(), GetSettingsFilename()));
                    }

                    // If the file does not exist on disk, catch the exception then create the XML template for the file.
                    catch (Exception)
                    {
                        // XML Declaration
                        // <?xml version="1.0" encoding="utf-8"?>
                        XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                        xmlDoc.AppendChild(dec);

                        // Create root node and append to the document
                        // <configuration>
                        XmlElement rootNode = xmlDoc.CreateElement(XMLROOT);
                        xmlDoc.AppendChild(rootNode);

                        // Create Configuration Sections node and add as the first node under the root
                        // <configSections>
                        XmlElement configNode = xmlDoc.CreateElement(CONFIGNODE);
                        xmlDoc.DocumentElement.PrependChild(configNode);

                        // Create the user settings section group declaration and append to the config node above
                        // <sectionGroup name="userSettings"...>
                        XmlElement groupNode = xmlDoc.CreateElement(GROUPNODE);
                        groupNode.SetAttribute("name", USERNODE);
                        groupNode.SetAttribute("type", "System.Configuration.UserSettingsGroup");
                        configNode.AppendChild(groupNode);

                        // Create the Application section declaration and append to the groupNode above
                        // <section name="AppName.Properties.Settings"...>
                        XmlElement newSection = xmlDoc.CreateElement("section");
                        newSection.SetAttribute("name", APPNODE);
                        newSection.SetAttribute("type", "System.Configuration.ClientSettingsSection");
                        groupNode.AppendChild(newSection);

                        // Create the userSettings node and append to the root node
                        // <userSettings>
                        XmlElement userNode = xmlDoc.CreateElement(USERNODE);
                        xmlDoc.DocumentElement.AppendChild(userNode);

                        // Create the Application settings node and append to the userNode above
                        // <AppName.Properties.Settings>
                        XmlElement appNode = xmlDoc.CreateElement(APPNODE);
                        userNode.AppendChild(appNode);
                    }
                }
                return xmlDoc;
            }
        }

        // Retrieve values from the configuration file, or if the setting does not exist in the file, 
        // retrieve the value from the application's default configuration
        private object GetSetting(SettingsProperty setProp)
        {
            object retVal;
            try
            {
                // Search for the specific settings node we are looking for in the configuration file.
                // If it exists, return the InnerText or InnerXML of its first child node, depending on the setting type.

                // If the setting is serialized as a string, return the text stored in the config
                if (setProp.SerializeAs.ToString() == "String")
                {
                    return XMLConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']").FirstChild.InnerText;
                }

                // If the setting is stored as XML, deserialize it and return the proper object.  This only supports
                // StringCollections at the moment - I will likely add other types as I use them in applications.
                else
                {
                    string settingType = setProp.PropertyType.ToString();
                    string xmlData = XMLConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']").FirstChild.InnerXml;
                    XmlSerializer xs = new XmlSerializer(typeof(string[]));
                    string[] data = (string[])xs.Deserialize(new XmlTextReader(xmlData, XmlNodeType.Element, null));

                    switch (settingType)
                    {
                        case "System.Collections.Specialized.StringCollection":
                            StringCollection sc = new StringCollection();
                            sc.AddRange(data);
                            return sc;
                        default:
                            return "";
                    }
                }
            }
            catch (Exception)
            {
                // Check to see if a default value is defined by the application.
                // If so, return that value, using the same rules for settings stored as Strings and XML as above
                if ((setProp.DefaultValue != null))
                {
                    if (setProp.SerializeAs.ToString() == "String")
                    {
                        retVal = setProp.DefaultValue.ToString();
                    }
                    else
                    {
                        string settingType = setProp.PropertyType.ToString();
                        string xmlData = setProp.DefaultValue.ToString();
                        XmlSerializer xs = new XmlSerializer(typeof(string[]));
                        string[] data = (string[])xs.Deserialize(new XmlTextReader(xmlData, XmlNodeType.Element, null));

                        switch (settingType)
                        {
                            case "System.Collections.Specialized.StringCollection":
                                StringCollection sc = new StringCollection();
                                sc.AddRange(data);
                                return sc;

                            default: return "";
                        }
                    }
                }
                else
                {
                    retVal = "";
                }
            }
            return retVal;
        }

        private void SetSetting(SettingsPropertyValue setProp)
        {
            // Define the XML path under which we want to write our settings if they do not already exist
            XmlNode SettingNode = null;

            try
            {
                // Search for the specific settings node we want to update.
                // If it exists, return its first child node, (the <value>data here</value> node)
                SettingNode = XMLConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']").FirstChild;
            }
            catch (Exception)
            {
                SettingNode = null;
            }

            // If we have a pointer to an actual XML node, update the value stored there
            if ((SettingNode != null))
            {
                if (setProp.Property.SerializeAs.ToString() == "String")
                {
                    SettingNode.InnerText = setProp.SerializedValue.ToString();
                }
                else
                {
                    // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                    // the value, otherwise .Net's configuration system complains about the additional declaration.
                    SettingNode.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                }
            }
            else
            {
                // If the value did not already exist in this settings file, create a new entry for this setting

                // Search for the application settings node (<Appname.Properties.Settings>) and store it.
                XmlNode tmpNode = XMLConfig.SelectSingleNode("//" + APPNODE);

                // Create a new settings node and assign its name as well as how it will be serialized
                XmlElement newSetting = xmlDoc.CreateElement("setting");
                newSetting.SetAttribute("name", setProp.Name);

                if (setProp.Property.SerializeAs.ToString() == "String")
                {
                    newSetting.SetAttribute("serializeAs", "String");
                }
                else
                {
                    newSetting.SetAttribute("serializeAs", "Xml");
                }

                // Append this node to the application settings node (<Appname.Properties.Settings>)
                tmpNode.AppendChild(newSetting);

                // Create an element under our named settings node, and assign it the value we are trying to save
                XmlElement valueElement = xmlDoc.CreateElement("value");
                if (setProp.Property.SerializeAs.ToString() == "String")
                {
                    valueElement.InnerText = setProp.SerializedValue.ToString();
                }
                else
                {
                    // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                    // the value, otherwise .Net's configuration system complains about the additional declaration
                    valueElement.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                }

                //Append this new element under the setting node we created above
                newSetting.AppendChild(valueElement);
            }
        }
    }
}