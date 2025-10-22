namespace GameManagement.CoreConfig
{
    public class SelectItem : ISelectItem
    {
        public SelectItem()
        {

        }

        public SelectItem(string value)
        {
            Value = value;
            Text = Value;
        }

        public SelectItem(object value, string text = null)
        {
            Value = value?.ToString();
            if (text == null)
            {
                Text = Value;
            }
            else
            {
                Text = text;
            }
        }

        public string Value { get; set; }
        public string Text { get; set; }
        public string CustomText { get => GetCustomDisplay(); set { } }
        public string GetCustomDisplay() => $"{Value} - {Text}";

        public string GetDisplay() => Text;

        public string GetKey() => Value;

        public void SetKey(string key) => Value = key;
    }
}
