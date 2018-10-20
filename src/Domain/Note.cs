using System;

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
			
			public Builder WithContent(string content)
			{
				_content = content;
				return this;
			}

			public override Note Build()
			{
				var note = new Note();
				note.SetContent(_content);
				note.AddTags(Tags);
				return note;
			}
		}
	}
}