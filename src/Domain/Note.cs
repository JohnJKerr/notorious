using System;
using System.Collections.Generic;

namespace Domain
{
	public class Note : Entity
	{
		public string Content { get; private set; }

		public void SetContent(string content)
		{
			Content = content;
		}

		public class Builder : Entity.Builder<Note>
		{
			private string _content;

			public Builder()
			{
			}
			
			public Builder WithContent(string content)
			{
				_content = content;
				return this;
			}

			public override Note Build()
			{
				var note = new Note();
				SetBaseProperties(note);
				note.SetContent(_content);
				return note;
			}
		}
	}
}