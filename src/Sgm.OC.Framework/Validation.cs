using System.Collections.Generic;
using System.Linq;

namespace Sgm.OC.Framework
{
    public class Validation
    {
        public List<ValidationItem> Items { get; set; } = new List<ValidationItem>();

        public bool IsValid { get { return Items.Count == 0; } }

        public void Add(string message)
        {
            Items.Add(new ValidationItem(message));
        }

        public void AddRange(List<ValidationItem> items)
        {
            Items.AddRange(items);
        }

        public List<string> GetMessages()
        {
            return Items.Select(i => i.Message).ToList();
        }

    }

    public class ValidationItem
    {
        public ValidationItem(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

}
