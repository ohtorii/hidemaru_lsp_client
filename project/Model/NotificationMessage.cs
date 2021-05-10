using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Model
{
	interface INotificationMessage
	{		
		string method { get; set; }
		object @params { get; set; }
	}
	class NotificationMessage : INotificationMessage
	{
		public string method { get; set; }
		public object @params { get; set; }
	}
	enum MessageType
	{
		/**
		 * An error message.
		 */
		Error = 1,
		/**
		 * A warning message.
		 */
		Warning = 2,
		/**
		 * An information message.
		 */
		Info = 3,
		/**
		 * A log message.
		 */
		Log = 4,
	}
	

	/// <summary>
	/// method: ‘window/logMessage’
	/// </summary>
	class LogMessageParams {
		/**
		 * The message type. See {@link MessageType}
		 */
		public MessageType type;

		/**
		* The actual message
		*/
		public string message;
	}

	/// <summary>
	/// method: ‘window/showMessage’
	/// </summary>
	class ShowMessageParams
	{
		/**
		 * The message type. See {@link MessageType}.
		 */
		public MessageType type;

		/**
		 * The actual message.
		 */
		public string message;
	}

}
