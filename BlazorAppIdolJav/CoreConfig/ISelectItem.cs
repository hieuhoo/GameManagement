namespace GameManagement.CoreConfig
{
    public interface ISelectItem
    {
        string GetKey();
        string GetDisplay();
        string GetCustomDisplay();
        void SetKey(string key);
    }
}
