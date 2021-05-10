using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IWorkspaceEdit
	{
//Todo: あとで実装
#if false
		changes?: { [uri: DocumentUri]: TextEdit[]; };

	
		documentChanges?: (
			TextDocumentEdit[] |
			(TextDocumentEdit | CreateFile | RenameFile | DeleteFile)[]
		);


		changeAnnotations?: {
			[id: string /* ChangeAnnotationIdentifier */]: ChangeAnnotation;
		};
#endif
	}

	class  WorkspaceEditClientCapabilities
	{		
		public bool documentChanges;
		public ResourceOperationKind[]  resourceOperations;
		public FailureHandlingKind failureHandling;
		public bool normalizesLineEndings;		
		public class _changeAnnotationSupport {
			public bool groupsOnLabel;
		};
		public _changeAnnotationSupport changeAnnotationSupport;
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum ResourceOperationKind
	{
		Create = 0,
		Rename = 1,
		Delete = 2
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum FailureHandlingKind
	{
		Abort = 0,
		Transactional = 1,
		TextOnlyTransactional = 2,
		Undo = 3
	}

}
