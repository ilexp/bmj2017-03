using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class DialogTree : IEnumerable<DialogNode>
	{
		private List<DialogNode> nodes = new List<DialogNode>();


		public IEnumerable<DialogNode> Nodes
		{
			get { return this.nodes; }
		}

		public List<ScoredDialogNode> Match(DialogNode context, Message input)
		{
			List<ScoredDialogNode> rankedNodes = new List<ScoredDialogNode>();
			foreach (DialogNode node in this.nodes)
			{
				if (node.Context != null && node.Context != context)
					continue;

				rankedNodes.Add(new ScoredDialogNode
				{
					Score = node.Matches(context, input),
					Node = node
				});
			}
			rankedNodes.Sort((a, b) => Comparer<float>.Default.Compare(b.Score, a.Score));
			return rankedNodes;
		}
		public void Add(DialogNode node)
		{
			this.nodes.Add(node);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.nodes.GetEnumerator();
		}
		IEnumerator<DialogNode> IEnumerable<DialogNode>.GetEnumerator()
		{
			return this.nodes.GetEnumerator();
		}
	}
}
