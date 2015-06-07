using System;
using System.Collections.Generic;
using System.Text;

namespace RefBox.Terminal
{
	public class CompletionTree
	{
		private CompletionTreeNode root;

		public CompletionTree()
		{
			this.root = new CompletionTreeNode((char)0);
		}

		public void AddWord(string word)
		{
			word = word.Trim();
			CompletionTreeNode node = this.root;
			CompletionTreeNode pNode = null;
			for (int i = 0; i < word.Length; ++i)
			{
				pNode = node;
				node = node.Add(word[i]);
				if (IsSpace(word[i]))
				{
					node.EndOfWord = true;
					if ((pNode.Children.Count == 1) && pNode.EndOfWord)
						pNode.EndOfWord = false;
				}
			}
			node.EndOfWord = true;
		}

		private bool IsSpace(char c)
		{
			switch (c)
			{
				case ' ':
				case '\f':
				case '\n':
				case '\r':
				case '\t':
				case '\v':
					return true;

				default:
					return false;

			}
		}

		public string CompleteWord(string prefix)
		{
			CompletionTreeNode node;
			return CompleteWord(prefix, out node);
		}

		private string CompleteWord(string prefix, out CompletionTreeNode node)
		{
			StringBuilder sb = new StringBuilder(100);
			
			node = this.root;
			if (String.IsNullOrEmpty(prefix))
				return String.Empty;
			// Advance all the prefix on the tree.
			// If a character is not found, the prefix can not be completed so it's returned as is
			for (int i = 0; i < prefix.Length; ++i)
			{
				node = node[prefix[i]];
				if (node == null) return String.Empty;
			}
			// At this point we are done with the prefix. 
			// It must go over the three while the corrent node has only one child
			// and untill a EndOfWord is reached
			while (node != null)
			{
				if ((node.Children.Count != 1) || node.EndOfWord)
					break;
				node = node.First;
				sb.Append(node.Value);
			}
			return sb.ToString();
		}

		public string[] GetAlternatives(string prefix)
		{
			string s;
			string alternative;
			CompletionTreeNode aNode;
			CompletionTreeNode node;
			List<string> alternatives;

			// First, get the first complete word (if any) for the prefix
			prefix += CompleteWord(prefix, out node);
			if ((node == null) || (node.Children.Count < 1))
				return null;
			alternatives = new List<string>(node.Children.Count);
			foreach (CompletionTreeNode child in node.Children.Values)
			{
				s = prefix + child.Value.ToString();
				alternative = s + CompleteWord(s, out aNode);
				if(aNode.EndOfWord)
					alternatives.Add(alternative);
				else
					alternatives.AddRange(GetAlternatives(alternative));
			}
			alternatives.Sort();
			return alternatives.ToArray();
		}
	}
}
