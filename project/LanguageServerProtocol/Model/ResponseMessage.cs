using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Model
{
	class ResponseMessage: IMessage
	{
		public string jsonrpc { get; set; } = "2.0";
		public int id=0;
		public object result=null;
		public ResponseError error=null;		
	}

	class ResponseError
	{
		/**
		 * A number indicating the error type that occurred.
		 */
		public ErrorCodes code;

		/**
		 * A string providing a short description of the error.
		 */
		public string message;

		/**
		 * A primitive or structured value that contains additional
		 * information about the error. Can be omitted.
		 */
		public object data;
	}

	enum ErrorCodes
	{
		// Defined by JSON RPC
		ParseError					= -32700,
		InvalidRequest				= -32600,
		MethodNotFound				= -32601,
		InvalidParams				= -32602,
		InternalError				= -32603,
		serverErrorStart			= -32099,
		serverErrorEnd				= -32000,
		ServerNotInitialized		= -32002,
		UnknownErrorCode			= -32001,

		// Defined by the protocol.
		RequestCancelled			= -32800,
		ContentModified 			= -32801,
	}
	

}
