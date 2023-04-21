namespace NodeTest
{
    public class Startup
    {
        public async Task<object> Invoke(string parameter)
        {
            var _strResult = "the input is illegal";
            if (!string.IsNullOrEmpty(parameter) && parameter.Contains(","))
            {
                var a = 0;
                var b = 0;
                if (int.TryParse(parameter.Split(',')[0], out a) && int.TryParse(parameter.Split(',')[1], out b))
                {
                    _strResult = (a + b).ToString();
                }
            }
            return _strResult;
        }
    }
}