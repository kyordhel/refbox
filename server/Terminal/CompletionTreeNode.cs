using System;
using System.Collections.Generic;


namespace RefBox.Terminal
{
	public class CompletionTreeNode
	{
		private char value;
		private SortedDictionary<char, CompletionTreeNode> children;

		public CompletionTreeNode(char value)
		{
			this.value = value;
			this.children = new SortedDictionary<char, CompletionTreeNode>();
		}

		public char Value { get { return value; } }
		public SortedDictionary<char, CompletionTreeNode> Children { get { return this.children; } }
		public bool EndOfWord { get; set; }

		public CompletionTreeNode this[char value]
		{
			get
			{
				if (this.Children.ContainsKey(value))
					return this.Children[value];
				return null;
			}
		}

		public CompletionTreeNode Add(char value)
		{
			if (this.Children.ContainsKey(value))
				return this.Children[value];

			CompletionTreeNode node = new CompletionTreeNode(value);
			this.Children.Add(value, node);
			return node;

		}

		public CompletionTreeNode First
		{
			get
			{
				foreach (KeyValuePair<char, CompletionTreeNode> pair in this.children)
					return pair.Value;
				return null;
			}
		}

		public bool HasChild(char value)
		{
			return this.Children.ContainsKey(value);
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(this.value);
			sb.Append(" => { ");
			foreach (KeyValuePair<char, CompletionTreeNode> pair in this.children)
			{
				sb.Append('\'');
				sb.Append(pair.Key);
				sb.Append("', ");
			}
			if (this.children.Count > 0)
				sb.Length -= 2;
			sb.Append(" }");
			return sb.ToString();
		}
	}
}
