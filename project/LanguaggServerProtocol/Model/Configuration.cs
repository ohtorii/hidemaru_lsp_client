using System;
using System.Collections.Generic;
using System.Text;
using DocumentUri =System.String;

namespace LSP.Model
{
	interface IConfigurationItem
	{
		/**
		 * The scope to get the configuration section for.
		 */
		DocumentUri scopeUri { get; set; }

		/**
		 * The configuration section asked for.
		 */
		string section{ get; set; }
	}


	class ConfigurationParams 
	{
		public ConfigurationItem[] items { get; set; }
	}

	class ConfigurationItem : IConfigurationItem
	{
		public DocumentUri scopeUri { get; set; }
		public string section { get; set; }
	}

}
