using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIFabricTest;
//using Microsoft.Windows.SemanticSearch;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace ICSharpCode.AvalonEdit.Search
{
	public class SemanticSearch
	{
		public string docText;
		public string[] docSentences;
		//public EmbeddingVector[] embeddings;
		public int[] indexes;
		public float[] scores;
		public int[] rank;
		public int currentIndex = 0;
		public bool updated = false;
		TextEmbedding embedCreator;
		String currentSearchString;
		public SemanticSearch()
		{ }

		public void SetDocText(string documentText)
		{
			if ((docText == documentText)) { return;}
			docText = documentText;
			updated = false;
		}

		public void GenerateEmbeddings()
		{
			if (embedCreator == null) { embedCreator = new TextEmbedding(); }
			currentIndex = 0;
			//AIFabricTest.Utils.LoadAIFabric();
			docSentences = docText.Split('.');

			if (docSentences[docSentences.Length - 1] == "")
	        { 
				docSentences = docSentences.Take(docSentences.Length -1).ToArray();
			}
			if (docSentences.Length == null)
	        {
				docSentences.Append(docText);
			}
			//embeddings = new EmbeddingVector[docSentences.Length];

		    indexes = new int[docSentences.Length + 1];
			indexes[0] = -1;
			for (int i = 0; i < docSentences.Length; i++) {
				indexes[i + 1] = docSentences[i].Length + indexes[i] + 1;
			}
			//embeddings =
		    embedCreator.CreateTextEmbedding(docSentences);

			updated = true;
		}

		public async void CreateRanking(string searchString)
		{
			if (currentSearchString == searchString) {
				return;
			}

			currentSearchString = searchString;

			if (updated == false) {
				GenerateEmbeddings();
			}

			currentIndex = 0;
			String[] stringArray = new string[1];
			stringArray[0] = searchString;
			AIFabricTest.Utils.LoadAIFabric();
			var inputStringEmbed = embedCreator.CreateTextEmbedding(stringArray);

			rank = embedCreator.CalculateRanking();

			/*for (int i = 0; i < docSentences.Length; i++) {
				scores[i] = AIFabricTest.Utils.CosineSimilarity(inputStringEmbed[0], embeddings[i]);
			}

			var indexedFloats = scores.Select((value, index) => new { Value = value, Index = index })
						  .ToArray();

			// Sort the indexed floats by value in descending order
			Array.Sort(indexedFloats, (a, b) => b.Value.CompareTo(a.Value));

			// Extract the top k indices
			rank = indexedFloats.Select(item => item.Index).ToArray();*/

		}

		public int GetStartIndex()
		{
			int sentenceIndex = rank[currentIndex];
			//currentIndex++;
			return indexes[sentenceIndex] + 1;

		}

		public int GetEndIndex()
		{
			int sentenceIndex = rank[currentIndex];
			currentIndex++;
			return indexes[sentenceIndex + 1] ;
		}

		public float CheckOverflow(double x)
		{
			if (x >= double.MaxValue) {
				throw new OverflowException("operation caused overflow");
			}
			return (float)x;
		}

		public float DotProduct(float[] a, float[] b)
		{
			float result = 0.0f;
			for (int i = 0; i < a.Length; i++) {
				result = CheckOverflow(result + CheckOverflow(a[i] * b[i]));
			}
			return result;
		}

		public float Magnitude(float[] v)
		{
			float result = 0.0f;
			for (int i = 0; i < v.Length; i++) {
				result = CheckOverflow(result + CheckOverflow(v[i] * v[i]));
			}
			return (float)Math.Sqrt(result);
		}
/*
		public float CosineSimilarity(EmbeddingVector v1, EmbeddingVector v2)
		{
			if (v1.Count != v2.Count) {
				throw new ArgumentException("Vectors must have the same length.");
			}

			int size = (int)(v1.Count);

			float[] raw1 = new float[size];
			float[] raw2 = new float[size];

			v1.GetValues(raw1);
			v2.GetValues(raw2);

			float m1 = Magnitude(raw1);
			float m2 = Magnitude(raw2);

			// Vectors should already be normalized.
			if (Math.Abs(m1 - m2) > 0.001f || Math.Abs(m1 - 1.0f) > 0.001f) {
				throw new InvalidOperationException("Vectors are not normalized.");
			}

			return DotProduct(raw1, raw2);
		}*/

	}
}
