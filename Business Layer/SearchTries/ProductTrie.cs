using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.SearchTries
{
    public class ProductTrie
    {
        public char Value { get; set; }
        public bool isEndPoint { get; set; }
        public Dictionary<char, ProductTrie> Next = new Dictionary<char, ProductTrie>();

        public ProductTrie(char val = ' ')
        {
            Value = val;
        }


        public void AddList(List<string> words)
        {
            Next = new();

            foreach (string word in words)
            {
                Add(this, word.ToLower(), 0);
            }
        }
        private void Add(ProductTrie current, string word, int index)
        {
            if (index == word.Length)
            {
                return;
            }

            char c = word[index];

            if (!current.Next.ContainsKey(c))
            {
                current.Next[c] = new ProductTrie(c);
            }
            if (word.Length - 1 == index)
            {
                current.Next[c].isEndPoint = true;
            }

            Add(current.Next[c], word, index + 1);
        }


        public async Task<List<string>> GetWordsStartWith(string word)
        {
            string lowerSring = word.ToLower();

            if (this.Next.ContainsKey(lowerSring[0]))
            {
                return await GetWordsStartWithHelper(this, lowerSring, new List<string>());
            }
            return [];
        }
        private async Task<List<string>> GetWordsStartWithHelper(ProductTrie current, string word, List<string> list)
        {
            char[] buffer = new char[255];

            for (int i = 0; i < word.Length; i++)
            {
                if (!current.Next.ContainsKey(word[i]))
                {
                    return [];
                }
                buffer[i] = word[i];
                current = current.Next[word[i]];
            }

            await DFS(current, word.Length - 1, buffer, list);

            return list;
        }
        private async Task DFS(ProductTrie current, int index, char[] buffer, List<string> list)
        {
            if (list.Count == 6)
            {
                return;
            }

            buffer[index] = current.Value;

            if (current.isEndPoint)
            {
                list.Add(new string(buffer, 0, index + 1));
            }

            foreach (var node in current.Next.Values)
            {
                await DFS(node, index + 1, buffer, list);
            }

        }

    }
}
