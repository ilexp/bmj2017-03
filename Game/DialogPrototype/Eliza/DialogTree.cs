using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DialogPrototype
{
	public class DialogTree
	{
		private DialogNode dontGetItNode = null;
		private List<DialogNode> nodes = new List<DialogNode>();


		public DialogNode DontGetItNode
		{
			get { return this.dontGetItNode; }
		}
		public IEnumerable<DialogNode> Nodes
		{
			get { return this.nodes; }
		}

		public List<ScoredDialogNode> Match(DialogContext context, Message input)
		{
			List<ScoredDialogNode> rankedNodes = new List<ScoredDialogNode>();
			foreach (DialogNode node in this.nodes)
			{
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


		public static DialogTree Load(string filePath, VectorDataStore vectorData)
		{
			DialogTree tree = new DialogPrototype.DialogTree();
			Dictionary<string, DialogContext> contextMap = new Dictionary<string, DialogContext>();

			using (Stream stream = File.OpenRead(filePath))
			using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true))
			using (JsonTextReader jsonReader = new JsonTextReader(reader))
			{
				JObject jsonRoot = JObject.Load(jsonReader);

				JObject jsonDontGetItNode = jsonRoot.Value<JObject>("dontGetItNode");
				tree.dontGetItNode = ParseNode(jsonDontGetItNode, vectorData, contextMap);

				JArray jsonNodes = jsonRoot.Value<JArray>("nodes");
				foreach (JObject jsonNode in jsonNodes)
				{
					DialogNode node = ParseNode(jsonNode, vectorData, contextMap);
					tree.Add(node);
				}
			}

			return tree;
		}
		private static DialogNode ParseNode(JObject jsonNode, VectorDataStore vectorData, Dictionary<string, DialogContext> contextMap)
		{
			string contextId = jsonNode.Value<string>("context");
			DialogContext context = ParseContext(contextId, contextMap);

			Statement input = new Statement(context);
			foreach (JObject jsonMessage in jsonNode.Value<JArray>("input"))
			{
				Message message = ParseMessage(jsonMessage, vectorData, contextMap);
				input.Add(message);
			}

			Statement output = new Statement(context);
			foreach (JObject jsonMessage in jsonNode.Value<JArray>("output"))
			{
				Message message = ParseMessage(jsonMessage, vectorData, contextMap);
				output.Add(message);
			}

			DialogNode node = new DialogNode(input, output);
			return node;
		}
		private static Message ParseMessage(JObject jsonMessage, VectorDataStore vectorData, Dictionary<string,DialogContext> contextMap)
		{
			string contextId = jsonMessage.Value<string>("context");
			string text = jsonMessage.Value<string>("text");
			DialogContext context = ParseContext(contextId, contextMap);
			return new Message(context, text, vectorData);
		}
		private static DialogContext ParseContext(string id, Dictionary<string, DialogContext> contextMap)
		{
			if (id == null) return null;

			DialogContext context;
			if (!contextMap.TryGetValue(id, out context))
			{
				context = new DialogContext(id);
				contextMap.Add(id, context);
			}
			return context;
		}
	}
}
