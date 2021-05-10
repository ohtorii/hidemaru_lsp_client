using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DidChangeConfigurationClientCapabilities
	{
		public bool dynamicRegistration;
	}

	class DidChangeConfigurationParams
	{
		public object settings;
	}
}
